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
            var user = await GetUserAsync(username);
            if(user == null)
            {
                user = new UserAccount
                {
                    EntityType = EntityType.UserAccount,
                    UserId = username
                };                
            }
            await UpserttUserAsync(user);

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

        public async Task<UserAccount> GetUserAsync(string username)
        {
            using (var client = CreateDocumentClient())
            {
                var user = client.CreateDocumentQuery<UserAccount>(CreateDocumentCollectionUri(), new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = false })
                    .Where(u => u.UserId == username)
                    .Where(s => s.EntityType == EntityType.UserAccount)
                    .AsEnumerable()
                    .FirstOrDefault();

                return await Task.FromResult(user);
            }
        }

        public async Task<UserAccount> UpserttUserAsync(UserAccount user)
        {
            using (var client = CreateDocumentClient())
            {
                var uri = CreateDocumentCollectionUri();
                await client.UpsertDocumentAsync(uri, user);

                return await Task.FromResult(user);
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
}
