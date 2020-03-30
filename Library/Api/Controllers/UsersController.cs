using System;
using System.Threading.Tasks;
using Library.Models.DTO;
using Library.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollbar;

namespace Library.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var res = await _userRepository.GetUsers();
                return Ok(res);
            }
            catch (Exception ex)
            {
                var error = "Could not get student list, message = " + ex;
                RollbarLocator.RollbarInstance.Error(error);
                _logger.LogError(error);
                throw;
            }
        }

        [HttpPost(Name = nameof(AddUser))]
        public async Task<IActionResult> AddUser([FromBody] UserDto user)
        {
            try
            {
                var res = await _userRepository.AddUser(user);
                return CreatedAtRoute(nameof(AddUser), res);
            }
            catch (Exception ex)
            {
                var error = "Could not add student, message = " + ex;
                RollbarLocator.RollbarInstance.Error(error);
                _logger.LogError(error);
                throw;
            }
            
        }

        [HttpGet("{idUser:int}")]
        public async Task<IActionResult> GetUser([FromRoute] int idUser)
        {
            try
            {
                var res = await _userRepository.GetUser(idUser);
                return Ok(res);
            }
            catch (Exception ex)
            {
                var error = "Could not get student list, message = " + ex;
                RollbarLocator.RollbarInstance.Error(error);
                _logger.LogError(error);
                throw;
            }
        }
    }
}