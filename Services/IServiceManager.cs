namespace BookLib.Services
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
    }
}
