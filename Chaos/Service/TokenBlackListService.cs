using Chaos.Api.Interface;

namespace Chaos.Api.Service
{
    public class TokenBlackListService : ITokenBlackListService
    {
        // Guardamos los tokens revocados (puede haber más de uno), en una lista de HashSet (ya que en realidad un token es un hash)
        private readonly HashSet<string> _revokedTokens = [];

        // Lock permite acceder a los procesos de una función asíncrona
        private readonly Lock _lock = new();

        public void RevokeToken(string token)
        {
            // Hacemos lock de cualquier función y añadimos el token a la lista de tokens revocados
            lock (_lock)
            {
                _revokedTokens.Add(token);
            }
        }

        public bool IsTokenRevoked(string token)
        {
            // Devuelve true o false si el token esta, o no, revocado, es decir, está en la lista de tokens revocados
            lock (_lock)
            {
                return _revokedTokens.Contains(token);
            }
        }
    }
}
