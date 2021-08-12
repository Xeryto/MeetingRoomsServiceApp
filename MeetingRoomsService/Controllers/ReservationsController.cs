using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AutoMapper;
using BusinessLogic.Models;
using BusinessLogic.Services;
using DataAccessLayer.Models;

namespace MeetingRoomsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationService _service;
        private readonly UserService _userService;
        private readonly MeetingRoomService _meetingRoomService;

        public ReservationsController(ReservationService service, UserService userService, MeetingRoomService meetingRoomService)
        {
            _service = service;
            _userService = userService;
            _meetingRoomService = meetingRoomService;
        }

        // GET: api/Reservations
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Reservation>))]
        public async Task<IEnumerable<Reservation>> GetReservations()
        {
            return await _service.GetAll();
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Reservation))]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            return await _service.GetById(id);
        }


        // POST: api/Reservations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        public async Task<IActionResult> PostReservation(ReservationPostModel reserve)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ReservationPostModel, ReservationUpdateModel>().ForMember(m => m.Id, opt => opt.NullSubstitute(0)));
            var mapper = new Mapper(config);
            var reserveMapped = mapper.Map<ReservationPostModel, ReservationUpdateModel>(reserve);
            var user = await _userService.GetById(reserve.UserId);
            var room = await _meetingRoomService.GetById(reserve.MeetingRoomId);
            var result = await _service.Add(reserveMapped, user, room);
            if (result.Item1) return Ok(result.Item2.Id);
            else return Conflict("Wrong time");
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            await _service.Delete(id);

            return Ok();
        }

        [HttpPatch]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        public async Task<IActionResult> UpdateAsync(ReservationUpdateModel reserve)
        {
            if (reserve.Id == 0) return Conflict("Reservation with id 0 doesn't exist");
            var user = await _userService.GetById(reserve.UserId);
            var room = await _meetingRoomService.GetById(reserve.MeetingRoomId);
            var result = await _service.Update(reserve, user, room);
            if (result.Item1) return Ok(result.Item2.Id);
            else return Conflict("Wrong time");
        }

        [HttpPost("{from}, {to}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<ReservationsListModel>))]
        public async Task<ActionResult<List<ReservationsListModel>>> GetReservationsInInterval(DateTime from, DateTime to)
        {
            if (from > to) return Conflict("Use appropriate interval");
            var rooms = await _meetingRoomService.GetAll();
            List<ReservationsListModel> res = new();
            foreach (MeetingRoom room in rooms)
            {
                ReservationsListModel reservationsListModel = new()
                {
                    MeetingRoomId = room.Id,
                    Reservations = await _service.GetInInterval(room.Id, from, to)
                };
                res.Add(reservationsListModel);
            }

            return res;
        }
    }
}
