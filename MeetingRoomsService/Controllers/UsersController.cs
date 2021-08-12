using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BusinessLogic.Models;
using BusinessLogic.Services;
using DataAccessLayer.Models;

namespace MeetingRoomsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }
        // GET: api/Users
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _service.GetAll();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(User))]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            return await _service.GetById(id);
        }
        

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        public async Task<IActionResult> PostUser(User user)
        {
            if (_service.CheckExists(user.Login)) return Conflict("User with this login already exists");
            return Ok((await _service.Add(user)).Id);
        }

        // DELETE: api/Users/5
        [HttpDelete("byId/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _service.Delete(id);

            return Ok();
        }

        [HttpDelete("byName/{name}")]
        public async Task<IActionResult> DeleteUserByName(string name)
        {
            var userId = (await _service.GetByName(name)).Id;
            if (userId == 0) return Conflict("No such user");
            await _service.Delete(userId);

            return Ok();
        }

        [HttpDelete("byLogin/{login}")]
        public async Task<IActionResult> DeleteUserByLogin(string login)
        {
            var userId = (await _service.GetByLogin(login)).Id;
            if (userId == 0) return Conflict("No such user");
            await _service.Delete(userId);

            return Ok();
        }

        [HttpPatch]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> UpdateUser(User user)
        {
            return Ok((await _service.Update(user)).Id);
        }

        [HttpGet("{login}, {password}")]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> LoginUser(string login, string password)
        {
            
            if (await _service.Login(login, password)) return Ok("Successful");
            else return Conflict("Wrong login or password");
        }
    }
}
