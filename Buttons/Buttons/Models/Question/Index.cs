using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Buttons.Models.Shared;

namespace Buttons.Models.Question
{
    public class Index
    {
        public IEnumerable<QuestionSummary> Questions { get; set; }
    }
}