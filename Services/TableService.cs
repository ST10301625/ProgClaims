using Azure;
using Azure.Data.Tables;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

        // Method to retrieve all claims
        public async Task<List<ClaimEntity>> GetAllClaimsAsync()
        {
            var claims = new List<ClaimEntity>();
            await foreach (var entity in _tableClient.QueryAsync<ClaimEntity>())
            {
                claims.Add(entity);
            }
            return claims;
        }

        // Method to retrieve all pending claims
        public async Task<List<ClaimEntity>> GetPendingClaimsAsync()
        {
            var claims = new List<ClaimEntity>();
            var query = _tableClient.QueryAsync<ClaimEntity>(claim => claim.Status == "Pending");

            await foreach (var entity in query)
            {
                claims.Add(entity);
            }

            return claims;
        }

        // Method to get a claim by PartitionKey and RowKey
        public async Task<ClaimEntity> GetClaimAsync(string partitionKey, string rowKey)
        {
            try
            {
                return await _tableClient.GetEntityAsync<ClaimEntity>(partitionKey, rowKey);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null; // Return null if the claim is not found
            }
        }

        // Method to update a claim
        public async Task UpdateClaimAsync(ClaimEntity claim)
        {
            await _tableClient.UpdateEntityAsync(claim, claim.ETag); // Use claim.ETag to ensure correct entity update
        }
    }
}
