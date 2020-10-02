using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buttons.Data.Entities
{
    public class UserAnsweredQuestion
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public bool Successful { get; set; }

        public DateTime DateAttempted { get; set; }
    }
}
