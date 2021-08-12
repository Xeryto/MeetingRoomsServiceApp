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
    }
}
