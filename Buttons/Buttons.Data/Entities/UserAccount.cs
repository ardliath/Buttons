using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buttons.Data.Entities
{
    public class UserAccount : Entity
    {
        public int Score { get; set; }
        public List<QuestionsAttempted> QuestionsAttempted { get; set; }        
    }
}
