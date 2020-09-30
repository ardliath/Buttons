using Buttons.Data;
using Buttons.Models.Question;
using Buttons.Models.Shared;
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
            var model = new Models.Question.Index
            {
                Questions = questions.Select(q => new QuestionSummary
                {
                    Id = q.ID,
                    Title = q.Title,
                    Text = q.Text
                })
            };

            return View(model);
        }


        public async Task<ActionResult> Create()
        {
            var questions = await _dependency.CreateSampleQuestions();
            return View();
        }


        public async Task<ActionResult> Get(string id)
        {
            var question = await _dependency.GetQuestion(id);
            var model = new Get
            {
                Id = id,
                Title = question.Title,
                QuestionText = question.Text
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Get(Get get)
        {
            var loadedQuestion = await _dependency.GetQuestion(get.Id);

            get.Title = loadedQuestion.Title;
            get.QuestionText = loadedQuestion.Text;
            get.Correct = get.Answer == loadedQuestion.Answer;

            return View(get);
        }
    }
}