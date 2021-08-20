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
using System.Web.Helpers;
using MeetingRoomsServiceApp.Models;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Http;
using AutoMapper;

namespace MeetingRoomsServiceApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _service;
        private readonly ReservationService _reservationService;
        private readonly MapperConfiguration editIndexConfig = new(cfg => cfg
        .CreateMap<User, UserEditModel>()
        .ForMember(x => x.Image, opt => opt.Ignore())
        .ForMember(x => x.EditNotValid, opt => opt.Ignore())
        .ForMember(x => x.UserId, opt => opt.Ignore()));
        private readonly MapperConfiguration editConfig = new(cfg => cfg
        .CreateMap<UserEditModel, User>()
        .ForMember(x => x.Image, opt => opt.Ignore()));
        private readonly MapperConfiguration config = new(cfg => cfg
        .CreateMap<UserPostModel, User>()
        .ForMember("Image", opt => opt.Ignore()));

        public UsersController(UserService service, ReservationService reservationService)
        {
            _service = service;
            _reservationService = reservationService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAll());
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Login,Password,Name,Image")] UserPostModel userPost)
        {
            if (ModelState.IsValid)
            {
                var mapper = new Mapper(config);
                var user = mapper.Map<UserPostModel, User>(userPost);
                if (userPost.Image != null)
                {
                    var stream = new MemoryStream();
                    await userPost.Image.CopyToAsync(stream);
                    user.Image = stream.ToArray();
                }
                
                await _service.Add(user);
                return RedirectToAction(nameof(Login));
            }
            return View(userPost);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id, int userId)
        {
            var user = await _service.GetById(id);
            var mapper = new Mapper(editIndexConfig);
            var userPost = mapper.Map<User, UserEditModel>(user);
            //var userPost = new UserPostModel()
            //{
            //    Id = user.Id,
            //    Login = user.Login,
            //    Password = user.Password,
            //    Name = user.Name
            //};
            return View(userPost);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int userId, [Bind("Id,Login,Password,Name,Image")] UserEditModel userEdit)
        {
            if (id != userId) return View(new UserEditModel()
            {
                EditNotValid = true
            });
            var mapper = new Mapper(editConfig);
            var user = mapper.Map<UserEditModel, User>(userEdit);
            if (userEdit.Image != null)
            {
                var stream = new MemoryStream();
                await userEdit.Image.CopyToAsync(stream);
                user.Image = stream.ToArray();
            }
            await _service.Update(user);
            return RedirectToAction("IndexWithDates", "Reservations");
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int id, int userId)
        {
            var user = await _service.GetById(id);
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int userId)
        {
            if (id != userId) return View(new UserDeleteModel()
            {
                DeleteNotValid = true
            });
            await _service.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel user)
        {
            if (await _service.Login(user.Login, user.Password))
            {
                var commonUser = await _service.GetByLogin(user.Login);
                UserCacheModel userCache = new()
                {
                    Id = commonUser.Id,
                    Image = commonUser.Image
                };
                WebCache.Set("LoggedIn", userCache, 60, true);
                return RedirectToAction("IndexWithDates", "Reservations");
            }
            else return View(new UserLoginModel()
            {
                InfoNotValid = true
            });
        }

        public async Task<IActionResult> Logout()
        {
            WebCache.Remove("LoggedIn");
            return RedirectToAction("IndexWithDates", "Reservations");
        }

        public async Task<IActionResult> UserPage()
        {
            UserCacheModel user = WebCache.Get("LoggedIn");
            return View(new UserPageModel()
            {
                Id = user.Id,
                Reservations = _reservationService.GetForUser(user.Id),
                User = await _service.GetById(user.Id)
            });
        }
    }
}
