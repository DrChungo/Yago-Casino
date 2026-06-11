using Chaos.Api.Interface;
using Chaos.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chaos.Api.Controllers
{

    [Authorize]
    [Route("api/User")]
    public class UserController(IUserService userService) : ControllerBase
    {

        /// <summary>
        /// Gets all users in the Database taking the UserResponse format
        /// </summary>
        /// <returns>All users in the Database</returns>
        [HttpGet("AllUsers")]
        public ActionResult GetAllUsers()
        {

            var result = userService.GetAllUsers();

            return Ok(new { Users = result, Total = result.Count });

        }



        /// <summary>
        /// Shows "N" fake users given the number, following the UserResponse format
        /// </summary>
        /// <param name="numUsers"></param>
        /// <returns>A list of fake number given its length by parameter</returns>
        [HttpPost("CreateNUsers")]
        public async Task<IActionResult> CreateNUsers(int numUsers)
        {

            var result = await userService.CreateNUsers(numUsers);

            return Ok(result);

        }
        // ── Añadir balance a un usuario (backoffice admin) ──
        [HttpPatch("{userId}/balance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBalance(Guid userId, [FromQuery] long amount)
        {
            await userService.AddIntoWallet(userId, amount);
            return Ok(new { success = true, message = $"Balance actualizado: +{amount}" });
        }


        [HttpGet("GetUserById")]
        public IActionResult GetUserById(Guid userId)
        {
            try
            {
                var result = userService.GetUserById(userId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Usuario no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }
        [HttpGet("GetMyUser")]
        public IActionResult GetMyUser()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = userService.GetUserById(userId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Usuario no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }


    }

}
