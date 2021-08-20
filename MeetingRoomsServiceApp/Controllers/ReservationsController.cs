using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Models;
using BusinessLogic.Services;
using BusinessLogic.Models;
using MeetingRoomsServiceApp.Models;
using AutoMapper;

namespace MeetingRoomsServiceApp.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ReservationService _service;
        private readonly MeetingRoomService _meetingRoomService;
        private readonly UserService _userService;
        private readonly MapperConfiguration config = new(cfg => cfg
        .CreateMap<Reservation, ReservationValidModel>()
        .ForMember(x => x.NotValid, opt => opt.Ignore()));
        private readonly MapperConfiguration editConfig = new(cfg => cfg
        .CreateMap<ReservationEditModel, Reservation>());
        private readonly MapperConfiguration editIndexConfig = new(cfg => cfg
        .CreateMap<Reservation, ReservationEditModel>()
        .ForMember(x => x.TimeNotValid, opt => opt.Ignore())
        .ForMember(x => x.UserNotValid, opt => opt.Ignore()));
        private readonly MapperConfiguration validConfig = new(cfg => cfg
        .CreateMap<ReservationValidModel, Reservation>());

        public ReservationsController(ReservationService service, UserService userService, MeetingRoomService meetingRoomService)
        {
            _service = service;
            _userService = userService;
            _meetingRoomService = meetingRoomService;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAll());
        }

        public async Task<IActionResult> IndexWithDates()
        {
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id");
            var today = DateTime.Today.AddDays(1- (int)DateTime.Today.DayOfWeek);
            var end = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            var list = GetDays(today, end);
            var reservations = await _service.GetInInterval(today, end);
            return View(new ResultModel
            {
                Start = today,
                End = end,
                Result = list,
                Reservations = reservations
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexWithDates(DateTime start, DateTime end, int? meetingRoomId)
        {
            start = start.AddDays(1 - (int)start.DayOfWeek);
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id", meetingRoomId);
            var list = GetDays(start, end);
            if (meetingRoomId == null)
            {
                var reservations = await _service.GetInInterval(start, end);

                return View(new ResultModel
                {
                    Start = start,
                    End = end,
                    Result = list,
                    Reservations = reservations
                });
            }
            else
            {
                var reservations = await _service.GetInInterval(start, end, meetingRoomId);

                return View(new ResultModel
                {
                    Start = start,
                    End = end,
                    MeetingRoomId = (int)meetingRoomId,
                    Result = list,
                    Reservations = reservations
                });
            }
            
        }

        private static List<string> GetDays(DateTime start, DateTime end)
        {
            return Enumerable.Range(0, (end - start).Days).Select(x => start.AddDays(x).ToShortDateString()).ToList();
        }


        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return View(await _service.GetById(id));
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create(int userId)
        {
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int userId, [Bind("Id,UserId,MeetingRoomId,TimeFrom,TimeTo")] ReservationValidModel reservationValid)
        {
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id", reservationValid.MeetingRoomId);
            if (ModelState.IsValid)
            {
                var mapper = new Mapper(validConfig);
                var reservation = mapper.Map<ReservationValidModel, Reservation>(reservationValid);
                var result = await _service.Add(reservation);
                if (result) return RedirectToAction(nameof(IndexWithDates));
                else return View(new ReservationValidModel()
                {
                    NotValid = true
                });
            }
            return View(reservationValid);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int id, int userId)
        {
            var reservation = await _service.GetById(id);
            var mapper = new Mapper(editIndexConfig);
            var reservationEdit = mapper.Map<Reservation, ReservationEditModel>(reservation);
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id", reservationEdit.MeetingRoomId);
            return View(reservationEdit);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int userId, [Bind("Id,UserId,MeetingRoomId,TimeFrom,TimeTo")] ReservationEditModel reservationEdit)
        {
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id", reservationEdit.MeetingRoomId);
            if (ModelState.IsValid)
            {
                reservationEdit.UserId = (await _service.GetById(id)).UserId;
                reservationEdit.TimeNotValid = false;
                reservationEdit.UserNotValid = false;
                if (reservationEdit.UserId == userId)
                {
                    var mapper = new Mapper(editConfig);
                    var reservation = mapper.Map<ReservationEditModel, Reservation>(reservationEdit);
                    var result = await _service.Update(reservation);
                    if (result) return RedirectToAction(nameof(IndexWithDates));
                    else
                    {
                        reservationEdit.TimeNotValid = true;
                        return View(reservationEdit);
                    }
                }
                else
                {
                    reservationEdit.UserNotValid = true;
                    return View(reservationEdit);
                }
                
            }
            return View(reservationEdit);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int id, int userId)
        {
            var reservation = await _service.GetById(id);
            var mapper = new Mapper(config);
            var deleteModel = mapper.Map<Reservation, ReservationValidModel>(reservation);
            return View(deleteModel);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int userId)
        {
            var reservation = await _service.GetById(id);
            if (reservation.UserId != userId)
            {
                var mapper = new Mapper(config);
                var deleteModel = mapper.Map<Reservation, ReservationValidModel>(reservation);
                deleteModel.NotValid = true;
                return View(deleteModel);
            }
            await _service.Delete(id);
            return RedirectToAction(nameof(IndexWithDates));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetReservationsInInterval(DateTime from,  DateTime to) {
            if (from > to) return View(new ReservationIntervalModel()
            {
                TimeNotValid = true
            });
            var rooms = await _meetingRoomService.GetAll();
            List<ReservationsListModel> res = new();
            foreach (MeetingRoom room in rooms)
            {
                ReservationsListModel reservationsListModel = new()
                {
                    MeetingRoomId = room.Id,
                    Reservations = await _service.GetInInterval(from, to, room.Id)
                };
                res.Add(reservationsListModel);
            }

            return View(new ReservationIntervalModel()
            {
                Reservations = res
            });
        } 
    }
}
