namespace Chaos.Api.Interface
{
    // Interfaz para implementar el servicio de blacklist tokens
    public interface ITokenBlackListService
    {
        public void RevokeToken(string token);
        public bool IsTokenRevoked(string token);
    }
}
