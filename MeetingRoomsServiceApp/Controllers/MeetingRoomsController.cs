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

namespace MeetingRoomsServiceApp.Controllers
{
    public class MeetingRoomsController : Controller
    {
        private readonly MeetingRoomService _service;

        public MeetingRoomsController(MeetingRoomService service)
        {
            _service = service;
        }

        // GET: MeetingRooms
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAll());
        }

        // GET: MeetingRooms/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return View(await _service.GetById(id));
        }

        // GET: MeetingRooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MeetingRooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Color")] MeetingRoom meetingRoom)
        {
            if (ModelState.IsValid)
            {
                await _service.Add(meetingRoom);
                return RedirectToAction(nameof(Index));
            }
            return View(meetingRoom);
        }

        // GET: MeetingRooms/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _service.GetById(id));
        }

        // POST: MeetingRooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Color")] MeetingRoom meetingRoom)
        {
            if (ModelState.IsValid)
            {
                _service.Update(meetingRoom);
                return RedirectToAction(nameof(Index));
            }
            return View(meetingRoom);
        }

        // GET: MeetingRooms/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            return View(await _service.GetById(id));
        }

        // POST: MeetingRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
