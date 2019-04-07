using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace aspnetcoreTransformersApp.Models
{
    public interface ITransformerDBContext
    {
        DbSet<Transformer> Transformers { get; set; }
        DbSet<TransformerAllegiance> TransformerAllegiances { get; set; }
        DbContext DatabaseContext { get; }

        void Dispose();
    }

    public interface ITransformerRepository
    {
        Task<Transformer> getTransformer(Func<Transformer, bool> whereClause);
        Task<TransformerAllegiance> getTransformerAllegiance(Func<TransformerAllegiance, bool> whereClause);
        Task<int> TransformerAdd(Transformer transformer);
        Task<Transformer> TransformerRetrieve(int transformerId);
        Task<int> TransformerUpdate(Transformer transformer);
        Task<int> TransformerRemove(int transformerId);
        Task<List<Transformer>> TransformersList(Func<Transformer, bool> whereClause);
        Task<List<TransformerAllegiance>> TransformerAllegianceList(Func<TransformerAllegiance, bool> whereClause);
        Task<int> TransformerScoreDB(List<dynamic> paramList, string spConfigPath);
        Task<int> TransformerScore(Transformer transformer);
    }

    public interface ITransformerAdd
    {
        Task<IActionResult> ExecuteAdd(Transformer transformer);
    }

    public interface ITransformerRetrieve
    {
        Task<IActionResult> ExecuteRetrieve(int transformerId);
    }

    public interface ITransformerUpdate
    {
        Task<IActionResult> ExecuteUpdate(Transformer transformer, int transformerId);
    }

    public interface ITransformerRemove
    {
        Task<IActionResult> ExecuteRemove(int transformerId);
    }

    public interface ITransformerList
    {
        List<Transformer> TransformerListWithAllegianceValue(List<Transformer> sortedList, TransformerAllegiance tAllegiance);
        Task<SortedList<string, Transformer>> TransformersListForAllegiance(TransformerAllegiance transformerAllegiance);
        Task<IActionResult> ExecuteList(string Allegiance, bool sorted);
    }

    public interface ITransformerScore
    {
        Task<IActionResult> ExecuteScore(int transformerId);
    }

    public interface ITransformerWar
    {
        Task<IActionResult> ExecuteWar();
    }
}
