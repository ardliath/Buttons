using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buttons.Data.Entities
{
    public class Question : Entity
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public decimal Answer { get; set; }

        public int NumberOfTimesQuestionHasBeenAttempted { get; set; }

        public int NumberOfTimesQuestionHasBeenAnsweredCorrectly { get; set; }
        public int NumberOfTimesQuestionHasBeenAnsweredIncorrectly { get; set; }
    }
}
