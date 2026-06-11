using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;

namespace Chaos.BlazorCasinoApp.IApiService
{
    public interface IAuthService
    {
        Task<LoginResponse?> Login(LoginRequest request);
        Task Logout();
        Task<string?> GetToken();
    }
}
