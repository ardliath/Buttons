using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buttons.Data.Entities
{
    public class Question : Entity
    {
        public string QuestionText { get; set; }

        public decimal Answer { get; set; }
    }
}
