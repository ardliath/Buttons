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
        public async Task<Entity> GetTestEntityAsync()
        {
            using (var client = CreateDocumentClient())
            {
                var uri = UriFactory.CreateDocumentCollectionUri("Buttons", "Entities");

                Entity entity = client.CreateDocumentQuery<Entity>(uri, new FeedOptions { MaxItemCount = 1 })
                    //.Where(s => s.CommandCode == commandCode)
                    .AsEnumerable()
                    .FirstOrDefault();

                entity.EntityType = EntityType.TestEntity;

                await client.UpsertDocumentAsync(uri, entity);
                return await Task.FromResult(entity);
            }
        }



        private DocumentClient CreateDocumentClient()
        {
            var url = ConfigurationManager.AppSettings["DatabaseEndpoint"];
            var key = ConfigurationManager.AppSettings["DatabaseKey"];
            return new DocumentClient(new Uri(url), key);
        }

        public string GetText()
        {
            return "Injected ";
        }
    }

    public interface IDependency
    {
        string GetText();
        Task<Entity> GetTestEntityAsync();
    }
}
