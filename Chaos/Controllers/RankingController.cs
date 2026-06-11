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
    public class RankingController(IRankingService rankingService, IUserService userService) : ControllerBase
    {

        private readonly IRankingService _rankingService = rankingService;

        [HttpGet("GetRankingAnimals")]
        public async Task<IActionResult> GetRakingAnimals()
        {
            var result = await _rankingService.BestThreeAnimals();

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

        [HttpGet("GetUsersRankingByWallet")]
        public async Task<IActionResult> GetUsersRankingByWallet()
        {
            var result = await _rankingService.BestThreeWallets();

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

        [HttpGet("GetUsersRankingByAnimalValues")]
        public async Task<IActionResult> GetUsersRankingByAnimalValues()
        {
            var result = await _rankingService.BestThreeTotalAnimalValues();

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
    }
}