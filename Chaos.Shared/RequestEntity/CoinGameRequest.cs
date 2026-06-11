using System;

namespace Chaos.Shared.RequestEntity
{
    public class CoinGameRequest
    {
        public string ConfigName { get; set; } = string.Empty;// Nombre de la configuración
        public string GameName { get; set; } = string.Empty;// Nombre del juego
        public bool IsActive { get; set; } = true;// Estado activo/inactivo
    }
}