using Chaos.Api.ResponseEntity.Config.LoginResponse;
using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;


namespace Chaos.Api.Interface.Config
{
    public interface IAuthServiceBackOffice
    {
        Task<AuthBackOfficeResponse> RegisterAsync(RegisterBackOfficeRequest request);
        Task<AuthBackOfficeResponse> LoginAsync(LoginBackOfficeRequest request);
        Task<AuthBackOfficeResponse> MakeAdminAsync(Guid userId);
        Task<AuthBackOfficeResponse> RevokeAdminAsync(Guid userId);
    }
}
