using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buttons.Models.Question
{
    public class Upsert
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public decimal Answer { get; set; }
    }
}