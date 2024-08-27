using AutoMapper;
using BookLib.Contracts;
using BookLib.DTOs;
using BookLib.Entities;
using dotenv.net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public async Task<string> CreateToken()
        {
            /*
             * 1. Get jwt secret
             * 2. Create a symmetric key from it
             * 3. Generate a signature using HMAC with SHA
             * 4. Get user claims and roles
             * 5. Generate config for jwt token in the form of options
             * 6. Create a token with a signature and claims
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

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
