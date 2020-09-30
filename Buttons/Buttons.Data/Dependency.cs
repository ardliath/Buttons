using Buttons.Data.Entities;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buttons.Data
{
    public class Dependency : IDependency
    {
        public async Task<bool> AnswerQuestionAsync(Question question, decimal answer, string username)
        {
            var correct = question.Answer == answer;
            return await Task.FromResult(correct);
        }

        public async Task<IEnumerable<Question>> CreateSampleQuestions()
        {
            using (var client = CreateDocumentClient())
            {
                var existingQuestions = await ListQuestionsAsync(client);

                foreach (var question in existingQuestions)
                {
                    var docUri = UriFactory.CreateDocumentUri("Buttons", "Entities", question.ID);
                    await client.DeleteDocumentAsync(docUri, new RequestOptions { PartitionKey = new Microsoft.Azure.Documents.PartitionKey(question.UserId) });
                }

                var uri = CreateDocumentCollectionUri();
                for (int i = 1; i < 11; i++)
                {
                    var newQuestion = new Question
                    {
                        Title = $"Question {i}",
                        Text = $"What is {i} + {i * 2}?",
                        Answer = i * 3,
                        EntityType = EntityType.Question,
                        UserId = "Adam"
                    };
                    await client.UpsertDocumentAsync(uri, newQuestion);
                }

                return await ListQuestionsAsync(client);
            }
        }

        public Task<Question> GetQuestion(string id)
        {
            using (var client = CreateDocumentClient())
            {
                Question question = client
                        .CreateDocumentQuery<Question>(CreateDocumentCollectionUri(), new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                        .Where(s => s.EntityType == EntityType.Question)
                        .Where(q => q.ID == id)
                        .AsEnumerable()
                        .FirstOrDefault();

                return Task.FromResult(question);
            }
        }

        public async Task<TestEntity> GetTestEntityAsync()
        {
            using (var client = CreateDocumentClient())
            {
                TestEntity entity = client.CreateDocumentQuery<TestEntity>(CreateDocumentCollectionUri(), new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                    .Where(s => s.EntityType == EntityType.TestEntity)
                    .AsEnumerable()
                    .FirstOrDefault();

                //entity.EntityType = EntityType.TestEntity;

                //await client.UpsertDocumentAsync(uri, entity);
                return await Task.FromResult(entity);
            }
        }

        public async Task<IEnumerable<Question>> ListQuestionsAsync()
        {            
            using (var client = CreateDocumentClient())
            {
                return await ListQuestionsAsync(client);
            }
        }

        public async Task<IEnumerable<Question>> ListQuestionsAsync(DocumentClient client)
        {               
                var questions = client.CreateDocumentQuery<Question>(CreateDocumentCollectionUri(), new FeedOptions { EnableCrossPartitionQuery = true })
                    .Where(s => s.EntityType == EntityType.Question)
                    .AsEnumerable()
                    .ToArray();

                return await Task.FromResult(questions);            
        }

        private DocumentClient CreateDocumentClient()
        {
            var url = ConfigurationManager.AppSettings["DatabaseEndpoint"];
            var key = ConfigurationManager.AppSettings["DatabaseKey"];
            return new DocumentClient(new Uri(url), key);
        }

        private Uri CreateDocumentCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri("Buttons", "Entities");
        }
    }

    public interface IDependency
    {
        Task<TestEntity> GetTestEntityAsync();
        Task<IEnumerable<Question>> ListQuestionsAsync();
        Task<IEnumerable<Question>> CreateSampleQuestions();
        Task<Question> GetQuestion(string id);
        Task<bool> AnswerQuestionAsync(Question question, decimal answer, string username);
    }
}
