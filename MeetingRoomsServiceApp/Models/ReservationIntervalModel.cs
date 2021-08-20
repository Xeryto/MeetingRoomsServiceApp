using BusinessLogic.Models;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsServiceApp.Models
{
    public class ReservationIntervalModel
    {
        public List<ReservationsListModel> Reservations { get; set; }
        public bool TimeNotValid { get; set; }
    }
}
