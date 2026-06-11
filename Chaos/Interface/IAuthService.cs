using Chaos.Api.ResponseEntity;

namespace Chaos.Api.Interface
{
    public interface IAuthService
    {
        LoginResponse? Login(LoginRequest request);
    }
}