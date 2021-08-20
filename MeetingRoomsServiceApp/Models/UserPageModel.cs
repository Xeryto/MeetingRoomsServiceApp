using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsServiceApp.Models
{
    public class UserPageModel
    { 
        public int Id { get; set; }
        public IEnumerable<Reservation> Reservations { get; set; }
        public User User { get; set; }
    }
}
