using MeetingRoomsService.DAL;
using MeetingRoomsService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsService.Controllers
{
    public class ReservationService
    {
        private readonly IGenericRepository<Reservation> _genericRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<MeetingRoom> _meetingRoomRepository;
        private ReservationsController controller;

        public ReservationService(IGenericRepository<Reservation> genericRepository, IGenericRepository<User> userRepository, IGenericRepository<MeetingRoom> meetingRoomRepository)
        {
            _genericRepository = genericRepository;
            _userRepository = userRepository;
            _meetingRoomRepository = meetingRoomRepository;
        }

        public async Task<ActionResult<Reservation>> AddReservation(ReservationContractModel reserve)
        {
            var user = await _userRepository.GetByIdAsync(reserve.UserId);
            var room = await _meetingRoomRepository.GetByIdAsync(reserve.MeetingRoomId);
            var reservation = new Reservation
            {
                User = user,
                MeetingRoom = room,
                TimeFrom = reserve.From,
                TimeTo = reserve.To
            };

            return await controller.PostReservation(reservation);
        }

        public async Task<ActionResult<Reservation>> EditReservation(ReservationContractModel reserve)
        {
            
        }
    }
}
