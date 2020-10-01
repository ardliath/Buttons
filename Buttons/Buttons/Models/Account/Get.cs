using Buttons.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buttons.Models.Account
{
    public class Get
    {
        public string Username { get; set; }

        public IEnumerable<QuestionSummary> QuestionsIHaveAnswered { get; set; }
    }
}