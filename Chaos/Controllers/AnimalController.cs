using Chaos.Api.Interface;
using Chaos.Api.Models;
using Chaos.Api.RequestEntity;
using Microsoft.AspNetCore.Authorization;
using Chaos.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Chaos.Api.Controllers
{
    /// <summary>
    /// Controller to manage all animal-related operations (CRUD and necessary queries).
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalController(IAnimalService animalService, IUserService userService) : ControllerBase
    {

        private readonly IAnimalService _animalService = animalService;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Retrieves all animals in the list.
        /// </summary>
        /// <returns>A list of animals with their details.</returns>
        /// <remarks>
        /// Example response:
        ///
        ///     {
        ///         "animalList": [
        ///             {
        ///                 "id": "bab68685-8c98-4e22-83cf-7bf4ffede6c6",
        ///                 "name": "232",
        ///                 "typeAnimal": "FLY",
        ///                 "age": 1,
        ///                 "weight": 0,
        ///                 "height": 1,
        ///                 "health": "RECOVERING",
        ///                 "value": 2,
        ///                 "ownerId": "8ba5d184-a49a-4d04-b0fc-d2407ad55809",
        ///                 "IsAvailable": true
        ///             },
        ///             {
        ///                 "id": "c4b195bb-4a08-4e24-8803-4049a6acbe5e",
        ///                 "name": "Prueba",
        ///                 "typeAnimal": "FLY",
        ///                 "age": 1,
        ///                 "weight": 0,
        ///                 "height": 2,
        ///                 "health": "SICK",
        ///                 "value": 4,
        ///                 "ownerId": null,
        ///                 "IsAvailable": true
        ///             }
        ///         ],
        ///         "totalAnimals": 2
        ///     }
        /// </remarks>
        /// <response code="200">Returns the list of animals successfully.</response>

        [HttpGet("All")]
        public async Task<ActionResult<List<AnimalResponse>>> GetAnimals()
        {
            var result = await _animalService.GetAnimals();
            return Ok(new { AnimalList = result, result.Count });
        }

        /// <summary>
        /// Sells an animal by its ID, changing the IsAvailable attribute into false. 
        /// </summary>
        /// <param name="Id">The ID of the animal to sell.</param>
        /// <returns>The value of the sold animal.</returns>


        [HttpPost("Sell")]
        public async Task<ActionResult> SellAnimal([FromBody] Guid Id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _animalService.SellAnimal(Id, userId);

            if (!string.IsNullOrEmpty(result.error))
            {
                return BadRequest(result.error);
            }

            await _userService.AddIntoWallet(userId, (long)result.value!);

            return Ok(result.value);
        }

        /// <summary>
        /// Plays a slot machine game for the specified animal and change the following attributes: IsSold = true; SoldAt = CurrentDate; OwnerId = UserId.
        /// </summary>
        /// <returns>An HTTP 200 response with the slot machine game result if successful; otherwise, an HTTP 404 response if the
        /// animal is not found.</returns>

        [HttpPost("BuyAnimal")]
        public async Task<IActionResult> BuyAnimal([FromBody] BuyAnimalRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            Console.WriteLine($"ANIMAL ID RECIBIDO: {request.AnimalId}");

            var success = await _animalService.BuyAnimal(
                request.AnimalId,
                userId,
                request.Name
            );

            if (!success.success)
                return BadRequest(success.message);

            return Ok(success.message);
        }


        /// <summary>
        /// Gets the average values of all animals in the system
        /// </summary>
        /// <returns>The average values of all animals.</returns>

        [HttpPost("GetValues")]
        public async Task<ActionResult> GetValues()
        {
            var values = await _animalService.GetAvgValues();
            return Ok(values);
        }

        [HttpPost("AnimalRoullete")]
        public async Task<IActionResult> RouletteForRoulette(string Dificultad)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _animalService.RouletteForRoulette(userId, Dificultad);

            return Ok(new
            {
                success = result.success,
                message = result.message,
                totalSpins = result.totalSpins, // ✅ siempre presente
                data = result.data?.Select(r => new
                {
                    success = r.Success,
                    message = r.Message,
                    candidates = r.Candidates.Select(c => new { id = c.Id, name = c.Name }),
                    winner = r.Winner == null
                                    ? null
                                    : (object)new { id = r.Winner.Id, name = r.Winner.Name }
                })
            });
        }

        [HttpGet("GetAnimalByOwnerId")]
        public async Task<IActionResult> GetAnimalsByOwnerId()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _animalService.GetAnimalsByOwnerId(userId);

            if (!result.success)
            {
                return Ok(new
                {
                    success = false,
                    result.message,
                    result.data
                });
            }

            return Ok(new
            {
                success = true,
                result.message,
                result.data
            });
        }

        [HttpGet("GetShopAnimals")]
        public async Task<IActionResult> GetShopAnimals()
        {
            var result = await _animalService.GetShopAnimals();

            if (!result.success)
            {
                return Ok(new
                {
                    success = false,
                    result.message,
                    result.data
                });
            }

            return Ok(new
            {
                success = true,
                result.message,
                result.data
            });
        }
        [HttpGet("GetAnimalsByUser/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAnimalsByUserId(Guid userId)
        {
            var result = await _animalService.GetAnimalsByOwnerId(userId);

            // ✅ Siempre devuelve 200 con la estructura correcta
            // (lista vacía es válida, no es un error HTTP)
            return Ok(new
            {
                success = result.success,
                message = result.message,
                // ✅ Si data es null, devuelve lista vacía para que el front no explote
                data = result.data ?? new List<AnimalResponse>()
            });
        }

        // ── Crear y asignar animal a un usuario (backoffice admin) ──
        [HttpPost("CreateAndAssign/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAndAssignAnimal(Guid userId)
        {
            var animal = await _animalService.CreateRandomAnimal(userId);
            return Ok(new { success = true, message = "Animal creado y asignado", animalId = animal.Id });
        }

        // ── Eliminar animal de un usuario (backoffice admin) ──
        [HttpDelete("Remove/{animalId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveAnimalAdmin(Guid animalId)
        {
            await _animalService.RemoveAnimal(animalId);
            return Ok(new { success = true, message = "Animal eliminado" });
        }
    }
}