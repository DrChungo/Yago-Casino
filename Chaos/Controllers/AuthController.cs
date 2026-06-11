using Chaos.Api.Interface;
using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Microsoft.AspNetCore.Mvc;


namespace Chaos.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IUserService userService, IAnimalService animalService, IAuthService authService) : ControllerBase
    {

        private readonly IUserService _userService = userService;
        private readonly IAnimalService _animalService = animalService;
        private readonly IAuthService _authService = authService;



        /// <summary>
        /// Checks for users existance and adds a user to the Database when success with custom message
        /// </summary>
        /// <param name="request"></param>
        /// <returns>An informative message wheter the user has been created or not</returns>
        [HttpPost("SignIn")]
        public async Task<ActionResult<UserResponse>> CreateUser(UserRequest request)
        {

            var result = await _userService.CreateUser(request);

            // Creates a message depending on succeeded or failed SignIn
            string message = result == null
                ? "Your mail has been already used"
                : $"User {result.Name} created successfully";

            if(result != null)
                await _animalService.CreateInitialAnimals(result!.Id);
            return Ok(new { result, message });

        }


        /// <summary>
        /// Checks for user existance and compares passwords with the user's hashed one
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The token JWT if succeeded</returns>
        [HttpPost("Login")]
        public IActionResult Login(LoginRequest request)
        {

            var response = _authService.Login(request);

            if (response == null)
                return Unauthorized("Invalid credentials");

            return Ok(response);

        }

    }

}