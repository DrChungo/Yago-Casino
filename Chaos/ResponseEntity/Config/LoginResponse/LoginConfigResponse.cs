namespace Chaos.Api.ResponseEntity.Config.LoginResponse
{
    public class LoginConfigResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email{ get; set; } = string.Empty;
    
        public bool IsAdmin { get; set; }
    }
}
