using Azure.Data.Tables;
using Azure;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProgClaims.Models;

namespace ProgClaims.Services
{
    public class TableService
    {
        private readonly TableClient _tableClient;

        public TableService(string connectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        // Method to add a new claim to the table
        public async Task AddClaimAsync(ClaimEntity claim)
        {
            await _tableClient.AddEntityAsync(claim);
        }


    }
}
// 