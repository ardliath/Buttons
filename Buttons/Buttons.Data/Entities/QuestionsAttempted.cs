using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buttons.Data.Entities
{
    public class QuestionsAttempted
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public bool Correct { get; set; }

        public DateTime AttemptedDate { get; set; }
    }
}
