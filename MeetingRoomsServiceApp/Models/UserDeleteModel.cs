using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsServiceApp.Models
{
    public class UserDeleteModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public int UserId { get; set; }
        public bool DeleteNotValid { get; set; }
    }
}
