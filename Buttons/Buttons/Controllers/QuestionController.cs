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
            var currentUser = this.IsLoggedIn
                ? await _dependency.GetUserAsync(this.CurrentUsername)
                : null;

            var correctlyAnsweredQuestions = currentUser?.QuestionsAttempted
                ?.Where(q => q.Correct)
                ?.Select(q => q.ID);

            var model = new Models.Question.Index
            {
                Questions = questions.Select(q => new QuestionSummary
                {
                    Id = q.ID,
                    Title = q.Title,
                    Text = q.Text,
                    Answered = correctlyAnsweredQuestions?.Contains(q.ID) ?? false
                })
            };

            return View(model);
        }


        public async Task<ActionResult> Get(string id)
        {
            var question = await _dependency.GetQuestion(id);
            var model = new Get
            {
                Id = id,
                Title = question.Title,
                QuestionText = question.Text,
                UsersCorrectlyAnswered = question.UserAnsweredQuestion?.Where(q => q.Successful)?.OrderBy(q => q.DateAttempted)?.Select(q => new UserSummary
                {
                    Id = q.UserId,
                    Username = q.Username,
                    Date = q.DateAttempted
                }) ?? new UserSummary[] { }
            };
            
            return View(model);
        }

        [HttpPost]
        [UserAuthenticationFilter]
        public async Task<ActionResult> Get(Get get)
        {
            if (this.IsLoggedIn)
            {
                var loadedQuestion = await _dependency.GetQuestion(get.Id);
                var correct = await _dependency.AnswerQuestionAsync(loadedQuestion, get.Answer, this.CurrentUsername);

                get.Title = loadedQuestion.Title;
                get.QuestionText = loadedQuestion.Text;
                get.Correct = correct;
                get.UsersCorrectlyAnswered = loadedQuestion.UserAnsweredQuestion?.Where(q => q.Successful)?.OrderBy(q => q.DateAttempted)?.Select(q => new UserSummary
                {
                    Id = q.UserId,
                    Username = q.Username,
                    Date = q.DateAttempted
                }) ?? new UserSummary[] { };
            }

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
                UserId = this.CurrentUsername
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