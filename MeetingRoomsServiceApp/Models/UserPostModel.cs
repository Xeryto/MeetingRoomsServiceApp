using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsServiceApp.Models
{
    public class UserPostModel
    {
        public int Id { get; set; }
        public string Login { get; set; } 
        public string Password { get; set; }
        public string Name { get; set; }
        public IFormFile Image { get; set; }
    }
}
