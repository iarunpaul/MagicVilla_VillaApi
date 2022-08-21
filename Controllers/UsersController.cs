using MagicVilla_VillaApi.Models.Dto;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("api/UserAuth")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        public UsersController(IUserRepository repo)
        {
            _repo = repo;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _repo.Login(model);
            if (loginResponse == null)
            {
                return NotFound();
            }
            return Ok(loginResponse);
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            if (!_repo.IsUserUnique(model.Username))
            {
                return BadRequest("Username exists");
            }
            var user = await _repo.Register(model);
            if (user == null)
            {
                return BadRequest("Error during registration");
            }
            return Ok(user);
        }
    }
}
