using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class ReservationsListModel
    {
        public int MeetingRoomId { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}
