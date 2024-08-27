using AutoMapper;
using BookLib.Contracts;
using BookLib.DTOs;
using BookLib.Entities;
using dotenv.net;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookLib.Services
{
    // Only accessible within an assembly and can't be inherited
    internal sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        private User _user;

        public AuthenticationService(ILoggerManager logger, IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterUser(UserForRegistrationDTO userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);

            var result = await _userManager.CreateAsync(user, userForRegistration.Password);

            if (result.Succeeded)
                await _userManager.AddToRolesAsync(user, userForRegistration.Roles);

            return result;
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDTO userForAuthenticationDTO)
        {
            _user = await _userManager.FindByNameAsync(userForAuthenticationDTO.UserName);
            var result = (_user is not null && await _userManager.CheckPasswordAsync(_user, userForAuthenticationDTO.Password));
            if (!result)
            {
                _logger.LogWarn($"{nameof(ValidateUser)} Authentication failed. Username or password are wrong");
            }
            return result;
        }

        public async Task<TokenDTO> CreateToken(bool extendRefreshToken)
        {
            /*
             * 1. Get jwt secret
             * 2. Create a symmetric key from it
             * 3. Generate a signature using HMAC with SHA
             * 4. Get user claims and roles
             * 5. Generate config for jwt token in the form of options
             * 6. Create a refresh token
             * 7. 
             */
            var jwtSecret = DotEnv.Read(options: new DotEnvOptions(ignoreExceptions: false))["JWT_SECRET"];
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            SigningCredentials _signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

            var _claims = new List<Claim> { new Claim(ClaimTypes.Name, _user.UserName) };
            var roles = await _userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                _claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken
            (
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: _claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: _signingCredentials
            );

            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;

            if (extendRefreshToken)
            {
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            }

            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDTO(accessToken, refreshToken);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var prng = RandomNumberGenerator.Create())
            {
                prng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DotEnv.Read(options: new DotEnvOptions(ignoreExceptions: false))["JWT_SECRET"])),
                ValidateLifetime = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<TokenDTO> RefreshToken(TokenDTO tokenDTO)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDTO.AccessToken);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            if (user is null || user.RefreshToken != tokenDTO.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new Exception("Invalid client request. TokenDTO has invalid values");
            }

            _user = user;

            return await CreateToken(extendRefreshToken: false);
        }
    }
}
