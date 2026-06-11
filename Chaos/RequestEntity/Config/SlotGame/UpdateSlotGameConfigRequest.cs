using System.ComponentModel.DataAnnotations;

namespace Chaos.Api.RequestEntity.Config.SlotGame
{
    //request con validaciones para actualizar una configuración de juego de tragamonedas
    public class UpdateSlotGameConfigRequest
    {

        [Required(ErrorMessage = "MachineName is required")]
        [MaxLength(100, ErrorMessage = "MachineName cannot exceed 100 characters")]
        public string MachineName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Multiplier is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Multiplier must be greater than 0")]
        public decimal Multiplier { get; set; }

        [Required(ErrorMessage = "NumberOfReels is required")]
        [Range(1, 10, ErrorMessage = "NumberOfReels must be between 1 and 10")]
        public int NumberOfReels { get; set; }

        [Required(ErrorMessage = "NumberOfRows is required")]
        [Range(1, 10, ErrorMessage = "NumberOfRows must be between 1 and 10")]
        public int NumberOfRows { get; set; }

        [Required(ErrorMessage = "PayLines is required")]
        [Range(1, 100, ErrorMessage = "PayLines must be between 1 and 100")]
        public int PayLines { get; set; }

        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; }
    }
}
