using DataAccessLayer.Models;
using System;
using System.Collections.Generic;

namespace MeetingRoomsServiceApp.Models
{
    public class ResultModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int MeetingRoomId { get; set; }
        public List<string> Result { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}