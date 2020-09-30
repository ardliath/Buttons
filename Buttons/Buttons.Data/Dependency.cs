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
        public async Task<IEnumerable<Question>> CreateSampleQuestions()
        {
            using (var client = CreateDocumentClient())
            {
                var existingQuestions = await ListQuestionsAsync(client);

                foreach (var question in existingQuestions)
                {
                    var docUri = UriFactory.CreateDocumentUri("Buttons", "Entities", question.ID);
                    await client.DeleteDocumentAsync(docUri);
                }

                var uri = UriFactory.CreateDocumentCollectionUri("Buttons", "Entities");
                for (int i = 1; i < 4; i++)
                {
                    var newQuestion = new Question
                    {
                        QuestionText = $"What is {i} + {i * 2}?",
                        Answer = i * 3,
                        EntityType = EntityType.Question
                    };
                    await client.UpsertDocumentAsync(uri, newQuestion);
                }

                return await ListQuestionsAsync(client);
            }
        }

        public async Task<TestEntity> GetTestEntityAsync()
        {
            using (var client = CreateDocumentClient())
            {
                var uri = UriFactory.CreateDocumentCollectionUri("Buttons", "Entities");

                TestEntity entity = client.CreateDocumentQuery<TestEntity>(uri, new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
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
                var uri = UriFactory.CreateDocumentCollectionUri("Buttons", "Entities");

                var questions = client.CreateDocumentQuery<Question>(uri, new FeedOptions { EnableCrossPartitionQuery = true })
                    .Where(s => s.EntityType == EntityType.Question)
                    .AsEnumerable();

                return await Task.FromResult(questions);            
        }

        private DocumentClient CreateDocumentClient()
        {
            var url = ConfigurationManager.AppSettings["DatabaseEndpoint"];
            var key = ConfigurationManager.AppSettings["DatabaseKey"];
            return new DocumentClient(new Uri(url), key);
        }
    }

    public interface IDependency
    {
        Task<TestEntity> GetTestEntityAsync();
        Task<IEnumerable<Question>> ListQuestionsAsync();
        Task<IEnumerable<Question>> CreateSampleQuestions();        
    }
}
