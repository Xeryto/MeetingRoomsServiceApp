using System;

namespace BusinessLogic.Models
{
    public class ReservationPostModel
    {
        public int UserId { get; set; }
        public int MeetingRoomId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
