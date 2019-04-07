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

        /// <summary>
        /// Add Transformer object to database
        /// </summary>
        /// <param name="transformer">Transformer</param>
        /// <returns>int</returns>
        public async Task<int> TransformerAdd(Transformer transformer)
        {
            return await _transformerDBContext.Transformers
                .Add(transformer)
                .Context
                .SaveChangesAsync();
        }

        /// <summary>
        /// Returns Transformer entity for specified Transformer Id from database
        /// </summary>
        /// <param name="transformerId">TransformerId</param>
        /// <returns>Transformer</returns>
        public async Task<Transformer> TransformerRetrieve(int transformerId)
        {
            return await this.getTransformer(s => s.TransformerId == transformerId);
        }

        /// <summary>
        /// Updates Transformer entity in database
        /// </summary>
        /// <param name="transformer">Transformer</param>
        /// <returns>int</returns>
        public async Task<int> TransformerUpdate(Transformer transformer)
        {
            return await _transformerDBContext
                        .Transformers
                        .Update(transformer)
                        .Context
                        .SaveChangesAsync();
        }

        /// <summary>
        /// Removes Transformer entity from database for specified Transformer Id
        /// </summary>
        /// <param name="transformerId">TransformerId</param>
        /// <returns>int</returns>
        public async Task<int> TransformerRemove(int transformerId)
        {
            Transformer transformer = await this.getTransformer(s => s.TransformerId == transformerId);
            return await _transformerDBContext
                        .Transformers
                        .Remove(transformer)
                        .Context
                        .SaveChangesAsync();
        }

        /// <summary>
        /// Returns Transformer entity list from database for specified filter clause
        /// </summary>
        /// <param name="whereClause">Func<Transformer, bool></param>
        /// <returns>List<Transformer></returns>
        public async Task<List<Transformer>> TransformersList(Func<Transformer, bool> whereClause)
        {
            return await _transformerDBContext
                        .Transformers
                        .Where(whereClause)
                        .ToAsyncEnumerable()
                        .ToList<Transformer>();
        }

        /// <summary>
        /// Returns TransormerAllegiance entity list from database for specified filter clause
        /// </summary>
        /// <param name="whereClause">Func<TransformerAllegiance, bool></param>
        /// <returns>List<TransformerAllegiance></returns>
        public async Task<List<TransformerAllegiance>> TransformerAllegianceList(Func<TransformerAllegiance, bool> whereClause)
        {
            return await _transformerDBContext
                        .TransformerAllegiances
                        .Where(whereClause)
                        .ToAsyncEnumerable()
                        .ToList<TransformerAllegiance>();
        }

        /// <summary>
        /// Returns Tranformer score from database using stored procedure using Dapper(ORM) package
        /// </summary>
        /// <param name="transformerIdParamObject">dynamic</param>
        /// <param name="spConfigPath">string</param>
        /// <returns>int</returns>
        public async Task<int> TransformerScoreDB(List<dynamic> spParamList, string spConfigPath)
        {
            int Score = 0;
            using (var conn = _transformerDBContext.DatabaseContext.Database.GetDbConnection())
            {
                conn.Open();
                var spFromConfig = _config.GetSection(spConfigPath).GetChildren().ToList();
                if (spFromConfig.Count > 0)
                {
                    string spName = spFromConfig[0]?.Value;
                    if (!string.IsNullOrEmpty(spName))
                    {
                        DynamicParameters spParams = new DynamicParameters();
                        spFromConfig.Skip(1).ToList().ForEach(item =>
                        {
                            spParamList.ForEach(param => {
                                if (item.Value.ToString().ToLower().Trim().Contains(param.Name))
                                {
                                    spParams.Add($"@{item.Value}", param.Value);
                                }
                            });
                        });
                        Score = await conn.ExecuteScalarAsync<int>(spName, spParams, commandType: CommandType.StoredProcedure);
                    }
                }
            }
            return Score;
        }

        /// <summary>
        /// Returns Tranformer score from aggregated properties of Transformer entity
        /// </summary>
        /// <param name="transformer">Transformer</param>
        /// <returns>Task<int></returns>
        public async Task<int> TransformerScore(Transformer transformer) 
            => await Task.FromResult<int>(transformer.Courage + transformer.Endurance + transformer.Firepower + transformer.Intelligence + transformer.Rank + transformer.Skill + transformer.Speed + transformer.Strength);

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
