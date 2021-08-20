using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsServiceApp.Models
{
    public class ReservationEditModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MeetingRoomId { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }

        public MeetingRoom MeetingRoom { get; set; }
        public User User { get; set; }

        public override bool Equals(object o)
        {
            return o is ReservationValidModel reservation && reservation.Id == this.Id && reservation.UserId == this.UserId && reservation.MeetingRoomId == this.MeetingRoomId && reservation.TimeFrom == this.TimeFrom && reservation.TimeTo == this.TimeTo && reservation.User == this.User && reservation.MeetingRoom == this.MeetingRoom;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public bool TimeNotValid { get; set; }
        public bool UserNotValid { get; set; }
    }
}
