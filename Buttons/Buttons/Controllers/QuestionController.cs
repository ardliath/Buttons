using Buttons.Data;
using Buttons.Data.Entities;
using Buttons.Filters;
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
    public class QuestionController : ControllerBase
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

        [UserAuthenticationFilter]
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
        [UserAuthenticationFilter]
        public async Task<ActionResult> Get(Get get)
        {
            var loadedQuestion = await _dependency.GetQuestion(get.Id);
            var correct = await _dependency.AnswerQuestionAsync(loadedQuestion, get.Answer, get.Username);

            get.Title = loadedQuestion.Title;
            get.QuestionText = loadedQuestion.Text;
            get.Correct = correct;

            return View(get);
        }

        [UserAuthenticationFilter]
        public async Task<ActionResult> Upsert(string id)
        {
            Models.Question.Upsert model;
            if (id != null)
            {
                var loadedQuestion = await _dependency.GetQuestion(id);
                model = new Models.Question.Upsert
                {
                    Id = loadedQuestion?.ID,
                    Title = loadedQuestion?.Title,
                    Text = loadedQuestion?.Text,
                    Answer = loadedQuestion?.Answer ?? 0
                };
            }
            else
            {
                model = new Models.Question.Upsert
                {
                    Title = "New Question"
                };
            }


            return View(model);
        }

        [HttpPost]
        [UserAuthenticationFilter]
        public async Task<RedirectToRouteResult>Upsert(Upsert model)
        {
            var question = new Question
            {
                ID = model.Id,
                Title = model.Title,
                Text = model.Text,
                Answer = model.Answer,
                EntityType = EntityType.Question,
                UserId = "Adam"
            };
            question = await _dependency.UpsertQuestionAsync(question);

            return RedirectToAction("Get", "Question", new { id = question.ID });
        }

        [HttpGet]
        [UserAuthenticationFilter]
        public async Task<ActionResult> Delete(string id)
        {
            if (this.IsLoggedIn)
            {
                await _dependency.DeleteQuestion(id);
            }

            return RedirectToAction("Index", "Question");
        }
    }
}