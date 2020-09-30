using Buttons.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Buttons.Controllers
{
    public class QuestionController : Controller
    {
        public IDependency _dependency;

        public QuestionController(IDependency dependency)
        {
            _dependency = dependency;
        }

        
        public async Task<ActionResult> Index()
        {
            var questions = await _dependency.ListQuestionsAsync();
            return Json(questions);
        }


        public async Task<ActionResult> Create()
        {
            var questions = await _dependency.CreateSampleQuestions();
            return Json(questions);
        }
    }
}