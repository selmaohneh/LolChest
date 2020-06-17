using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace LolChest
{
    public class CloudTables
    {
        private readonly CloudTableClient _tableClient;

        public CloudTables()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Returns the <see cref="CloudTable"/> instance for the
        /// table with the given name.
        /// </summary>
        public async Task<CloudTable> Get(string tableName)
        {
            var cloudTable = _tableClient.GetTableReference(tableName);
            await cloudTable.CreateIfNotExistsAsync();

            return cloudTable;
        }
    }
}