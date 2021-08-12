using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DAL
{
    public static class Extensions
    {
        public static Task<List<Reservation>> GetAllReservations(this IGenericRepository<Reservation> repository)
        {
            try
            {
                return repository.Query().Include(x => x.MeetingRoom).Include(x => x.User).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }
        public static async Task<Reservation> GetReservationById(this IGenericRepository<Reservation> repository, int id)
        {
            var reservation = await repository.Query().Include(x => x.MeetingRoom).Include(x => x.User)
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            if (reservation == null) throw new Exception($"Entity with id {id} doesn't exist");

            return reservation;
        }

        public static Task<List<Reservation>> GetInInterval(this IGenericRepository<Reservation> repository, int roomId, DateTime from, DateTime to)
        {
            return repository.Query().Where(x => x.MeetingRoomId == roomId && x.TimeFrom < to && x.TimeTo > from)
                .ToListAsync();
        }
    }
}
