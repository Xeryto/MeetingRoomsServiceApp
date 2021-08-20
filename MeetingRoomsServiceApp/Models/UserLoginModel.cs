using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsServiceApp.Models
{
    public class UserLoginModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool InfoNotValid { get; set; }
    }
}
