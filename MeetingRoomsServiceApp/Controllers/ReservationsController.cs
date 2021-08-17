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

namespace MeetingRoomsServiceApp.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ReservationService _service;
        private readonly MeetingRoomService _meetingRoomService;
        private readonly UserService _userService;

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
            var today = DateTime.Today;
            var start = new DateTime(today.Year, today.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            var list = GetDays(start, end);
            var reservations = await _service.GetInInterval(start, end);
            return View("IndexWithDates", new ResultModel
            {
                Start = start,
                End = end,
                Result = list,
                Reservations = reservations
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexWithDates(DateTime start, DateTime end, int? meetingRoomId)
        {
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
        public async Task<IActionResult> Create()
        {
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id");
            ViewData["UserId"] = new SelectList(await _userService.GetAll(), "Id", "Id");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,MeetingRoomId,TimeFrom,TimeTo")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Add(reservation);
                if (result) return RedirectToAction(nameof(Index));
                else return Conflict("Wrong time");
            }
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id", reservation.MeetingRoomId);
            ViewData["UserId"] = new SelectList(await _userService.GetAll(), "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var reservation = await _service.GetById(id);
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id", reservation.MeetingRoomId);
            ViewData["UserId"] = new SelectList(await _userService.GetAll(), "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,MeetingRoomId,TimeFrom,TimeTo")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Update(reservation);
                if (result) return RedirectToAction(nameof(Index));
                else return Conflict("Wrong time");
            }
            ViewData["MeetingRoomId"] = new SelectList(await _meetingRoomService.GetAll(), "Id", "Id", reservation.MeetingRoomId);
            ViewData["UserId"] = new SelectList(await _userService.GetAll(), "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _service.GetById(id);
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.GetById(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetReservationsInInterval(DateTime from,  DateTime to) {
            if (from > to) return Conflict("Use appropriate interval");
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

            return View(res);
        } 
    }
}
