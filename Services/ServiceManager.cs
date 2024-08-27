using AutoMapper;
using BookLib.Contracts;
using BookLib.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookLib.Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthenticationService> _authenticationService;

        public ServiceManager(ILoggerManager logger, IMapper mapper, UserManager<User> userManager, IConfiguration configuration) 
        {
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, mapper, userManager, configuration));
        }
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
    }
}
