using Buttons.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buttons.Models.Question
{
    public class Get
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string QuestionText { get; set; }

        public decimal Answer { get; set; }

        public bool? Correct { get; set; }

        public IEnumerable<UserSummary> UsersCorrectlyAnswered { get; set; }
    }
}