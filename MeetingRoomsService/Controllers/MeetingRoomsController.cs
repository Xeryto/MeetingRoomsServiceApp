using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;
using Swashbuckle.AspNetCore.Annotations;
using BusinessLogic.Services;

namespace MeetingRoomsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingRoomsController : ControllerBase
    {
        private readonly MeetingRoomService _service;

        public MeetingRoomsController(MeetingRoomService service)
        {
            _service = service;
        }

        // GET: api/MeetingRooms
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<MeetingRoom>))]
        public async Task<IEnumerable<MeetingRoom>> GetMeetingRooms()
        {
            return await _service.GetAll();
        }

        // GET: api/MeetingRooms/5
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(MeetingRoom))]
        public async Task<ActionResult<MeetingRoom>> GetMeetingRoom(int id)
        {
            return await _service.GetById(id);
        }

        // POST: api/MeetingRooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> PostMeetingRoom(MeetingRoom meetingRoom)
        {
            return Ok((await _service.Add(meetingRoom)).Id);
        }

        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id, string name)
        {
            if (name == null)
            {
                await DeleteMeetingRoomById(id);
            }
            else if (id == 0)
            {
                await DeleteMeetingRoomByName(name);
            }
            return Ok();
        }

        // DELETE: api/MeetingRooms/5
        [HttpDelete("byId/{id}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMeetingRoomById(int id)
        {
            await _service.Delete(id);

            return Ok();
        }

        [HttpDelete("byName/{id}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        private async Task<IActionResult> DeleteMeetingRoomByName(string name)
        {
            await _service.Delete((await _service.GetByName(name)).Id);

            return Ok();
        }

        [HttpPatch]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> UpdateAsync(MeetingRoom meetingRoom)
        {
            return Ok((await _service.Update(meetingRoom)).Id);
        }
    }
}
