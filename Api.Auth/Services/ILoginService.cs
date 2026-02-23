namespace Api.Auth.Services
{
    public interface ILoginService
    {
        Task<bool> Login(Models.Account userAccount);
        Task<bool> Register(Models.Account userAccount);
    }
}
