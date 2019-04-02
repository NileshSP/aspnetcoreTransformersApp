using aspnetcoreTransformersApp.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcoreTransformersApp.Services
{
    public class TransformerRepository : ITransformerRepository, IDisposable
    {
        private ITransformerDBContext _transformerDBContext;
        private readonly IConfiguration _config;

        public TransformerRepository(ITransformerDBContext context, IConfiguration configuration)
        {
            _transformerDBContext = context;
            _config = configuration;
        }

        /// <summary>
        /// Get Transformer entity from Db by passing dbContext and filter criteria
        /// </summary>
        /// <params name="dbContext">database context</params>
        /// <params name="whereClause" >Filter criteria for transformers to be filtered to</params>
        /// <returns>Transformer</returns>
        public async Task<Transformer> getTransformer(Func<Transformer, bool> whereClause)
        {
            return await _transformerDBContext
                    .Transformers
                    .Where(whereClause)
                    .ToAsyncEnumerable()
                    .FirstOrDefault();
        }

        /// <summary>
        /// Get TransformerAllgiance entity from Db by passing dbContext and filter criteria
        /// </summary>
        /// <params name="dbContext">database context</params>
        /// <params name="whereClause" >Filter criteria for TransformerAllegiance to be filtered to</params>
        /// <returns>TransformerAllegiance</returns>
        public async Task<TransformerAllegiance> getTransformerAllegiance(Func<TransformerAllegiance, bool> whereClause)
        {
            return await _transformerDBContext
                            .TransformerAllegiances
                            .Where(whereClause)
                            .ToAsyncEnumerable()
                            .FirstOrDefault();
        }

        public async Task<int> TransformerAdd(Transformer transformer)
        {
            return await _transformerDBContext.Transformers
                .Add(transformer)
                .Context
                .SaveChangesAsync();
        }

        public async Task<Transformer> TransformerRetrieve(int transformerId)
        {
            return await this.getTransformer(s => s.TransformerId == transformerId);
        }

        public async Task<int> TransformerUpdate(Transformer transformer)
        {
            return await _transformerDBContext
                        .Transformers
                        .Update(transformer)
                        .Context
                        .SaveChangesAsync();
        }

        public async Task<int> TransformerRemove(int transformerId)
        {
            Transformer transformer = await this.getTransformer(s => s.TransformerId == transformerId);
            return await _transformerDBContext
                        .Transformers
                        .Remove(transformer)
                        .Context
                        .SaveChangesAsync();
        }

        public async Task<List<Transformer>> TransformersList(Func<Transformer, bool> whereClause)
        {
            return await _transformerDBContext
                        .Transformers
                        .Where(whereClause)
                        .ToAsyncEnumerable()
                        .ToList<Transformer>();
        }

        public async Task<List<TransformerAllegiance>> TransformerAllegianceList(Func<TransformerAllegiance, bool> whereClause)
        {
            return await _transformerDBContext
                        .TransformerAllegiances
                        .Where(whereClause)
                        .ToAsyncEnumerable()
                        .ToList<TransformerAllegiance>();
        }

        public async Task<int> TransformerScore(int transformerId)
        {
            int Score = 0;
            using (var conn = _transformerDBContext.DatabaseContext.Database.GetDbConnection())
            {
                conn.Open();
                var configPath = "StoredProcedures:Score";
                var spFromConfig = _config.GetSection(configPath).GetChildren().ToList();
                string spName = spFromConfig[0].Value;
                DynamicParameters spParams = new DynamicParameters();
                spFromConfig.Skip(1).ToList().ForEach(item => {
                    if (item.Value.ToString().ToLower().Trim().Contains("transformerid"))
                    {
                        spParams.Add($"@{item.Value}", transformerId);
                    }
                });
                Score = await conn.ExecuteScalarAsync<int>(spName, spParams, commandType: CommandType.StoredProcedure);
            }
            return Score;
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_transformerDBContext != null)
                {
                    _transformerDBContext.Dispose();
                    _transformerDBContext = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
