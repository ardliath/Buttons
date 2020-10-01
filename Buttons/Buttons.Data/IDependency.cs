﻿using Buttons.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buttons.Data
{
    public interface IDependency
    {
        Task<TestEntity> GetTestEntityAsync();
        Task<IEnumerable<Question>> ListQuestionsAsync();
        Task<IEnumerable<Question>> CreateSampleQuestions();
        Task<Question> GetQuestion(string id);
        Task<bool> AnswerQuestionAsync(Question question, decimal answer, string username);
        Task<Question> UpsertQuestionAsync(Question question);
    }
}
