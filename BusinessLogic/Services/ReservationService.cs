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

        private bool CheckChoosedData(Reservation reservation)
        {
            var query = _genericRepository.Query()
                    .Where(x => x.MeetingRoomId == reservation.MeetingRoomId && x.TimeFrom < reservation.TimeTo && x.TimeTo > reservation.TimeFrom);
            if (reservation.Id != 0)
                query = query.Where(x => x.Id != reservation.Id);
            Console.WriteLine($"ddd, {reservation.TimeFrom}");
            return reservation.TimeFrom < DateTime.Now || reservation.TimeTo < reservation.TimeFrom || reservation.TimeTo.Subtract(reservation.TimeFrom) > _maximumReservationTime || query.Any();
        }

        public async Task<bool> Add(Reservation reservation)
        {
            if (CheckChoosedData(reservation)) return false;
            await _genericRepository.AddAsync(reservation);
            return true;
        }

        public async Task<bool> Update(Reservation reservation)
        {
            if (CheckChoosedData(reservation)) return false;
            await _genericRepository.UpdateAsync(reservation);
            return true;
        }

        public async Task<Reservation> Delete(int id)
        {
            return await _genericRepository.Delete(id);
        }
    }
}
