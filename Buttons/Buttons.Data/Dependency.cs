using Buttons.Data.Entities;
using Microsoft.Azure.Documents;
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
            var user = await GetUserAsync(username);
            if(user != null)
            {
                var correct = question.Answer == answer;
                bool hasUserPreviouslyCorrectlyAnsweredQuestion = user.QuestionsAttempted?.Any(q => q.ID == question.ID && q.Correct) ?? false;

                if (!hasUserPreviouslyCorrectlyAnsweredQuestion)
                {
                    question.NumberOfTimesQuestionHasBeenAttempted++;
                    if (correct)
                    {
                        question.NumberOfTimesQuestionHasBeenAnsweredCorrectly++;
                    }
                    else
                    {
                        question.NumberOfTimesQuestionHasBeenAnsweredIncorrectly++;
                    }

                    var now = DateTime.Now;
                    if (user.QuestionsAttempted == null) user.QuestionsAttempted = new List<QuestionsAttempted>();
                    user.QuestionsAttempted.Add(new QuestionsAttempted
                    {
                        ID = question.ID,
                        Correct = correct,
                        Title = question.Title,
                        AttemptedDate = now
                    });

                    if (question.UserAnsweredQuestion == null) question.UserAnsweredQuestion = new List<UserAnsweredQuestion>();
                    question.UserAnsweredQuestion.Add(new UserAnsweredQuestion
                    {
                        UserId = user.ID,
                        Username = user.UserId,
                        DateAttempted = now,
                        Successful = correct
                    });
                }



                await UpserttUserAsync(user);
                await UpsertQuestionAsync(question);
                return await Task.FromResult(correct);
            }

            return false;
        }

        public async Task<bool> DeleteQuestion(string id)
        {
            using (var client = CreateDocumentClient())
            {
                var question = await GetQuestion(client, id);
                if (question != null)
                {
                    await DeleteQuestion(client, id, question.UserId);
                }
            }

            return true;
        }

        public async Task<bool> DeleteQuestion(DocumentClient client, string id, string creator)
        {
            var docUri = UriFactory.CreateDocumentUri("Buttons", "Entities", id);
            await client.DeleteDocumentAsync(docUri, new RequestOptions { PartitionKey = new Microsoft.Azure.Documents.PartitionKey(creator) });

            return true;
        }

        public Task<Question> GetQuestion(DocumentClient client, string id)
        {
                Question question = client
                        .CreateDocumentQuery<Question>(CreateDocumentCollectionUri(), new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                        .Where(s => s.EntityType == EntityType.Question)
                        .Where(q => q.ID == id)
                        .AsEnumerable()
                        .FirstOrDefault();

                return Task.FromResult(question);            
        }

        public Task<Question> GetQuestion(string id)
        {
            using (var client = CreateDocumentClient())
            {
                return GetQuestion(client, id);
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

        public async Task<Question> UpsertQuestionAsync(Question question)
        {
            using (var client = CreateDocumentClient())
            {
                var uri = CreateDocumentCollectionUri();
                Document doc = await client.UpsertDocumentAsync(uri, question);
                question = await GetQuestion(client, doc.Id);

                return await Task.FromResult(question);
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
