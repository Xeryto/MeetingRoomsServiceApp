using BusinessLogic.DAL;
using BusinessLogic.Models;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ReservationService
    {
        protected readonly IGenericRepository<Reservation> _genericRepository;
        protected readonly TimeSpan _maximumReservationTime = new(3, 0, 0);

        public ReservationService (IGenericRepository<Reservation> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<List<Reservation>> GetAll()
        {
            return await _genericRepository.GetAllReservations();
        }

        public async Task<Reservation> GetById(int id)
        {
            return await _genericRepository.GetReservationById(id);
        }

        public async Task<List<Reservation>> GetInInterval(int roomId, DateTime from, DateTime to)
        {
            return await _genericRepository.GetInInterval(roomId, from, to);
        }

        private bool CheckChoosedData(ReservationUpdateModel reserve)
        {
            var query = _genericRepository.Query()
                    .Where(x => x.MeetingRoomId == reserve.MeetingRoomId && x.TimeFrom < reserve.To && x.TimeTo > reserve.From);
            if (reserve.Id != 0)
                query = query.Where(x => x.Id != reserve.Id);
            return reserve.From < DateTime.Now || reserve.To < reserve.From || reserve.To.Subtract(reserve.From) > _maximumReservationTime || query.Any();
        }

        public async Task<Tuple<bool, Reservation>> Add(ReservationUpdateModel reserve, User user, MeetingRoom room)
        {
            if (CheckChoosedData(reserve)) return new Tuple<bool, Reservation>(false, null);
            var reservation = new Reservation
            {
                Id = reserve.Id,
                User = user,
                UserId = user.Id,
                MeetingRoom = room,
                MeetingRoomId = room.Id,
                TimeFrom = reserve.From,
                TimeTo = reserve.To
            };
            
            return new Tuple<bool, Reservation>(true, await _genericRepository.AddAsync(reservation));
        }

        public async Task<Tuple<bool, Reservation>> Update(ReservationUpdateModel reserve, User user, MeetingRoom room)
        {
            if (CheckChoosedData(reserve)) return new Tuple<bool, Reservation>(false, null);
            var reservation = new Reservation
            {
                Id = reserve.Id,
                User = user,
                UserId = user.Id,
                MeetingRoom = room,
                MeetingRoomId = room.Id,
                TimeFrom = reserve.From,
                TimeTo = reserve.To
            };

            return new Tuple<bool, Reservation>(true, await _genericRepository.UpdateAsync(reservation));
        }

        public async Task<Reservation> Delete(int id)
        {
            return await _genericRepository.Delete(id);
        }
    }
}
