using System.Threading.Tasks;
using aspnetcoreTransformersApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspnetcoreTransformersApp.Controllers
{
    [Route("api/[controller]")]
    public class TransformersController : Controller
    {
        private readonly ILogger<Controller> _logger;
        private ITransformerAdd _transfromerAdd;
        private ITransformerRetrieve _transformerRetrieve;
        private ITransformerUpdate _transformerUpdate;
        private ITransformerRemove _transformerRemove;
        private ITransformerList _transformerList;
        private ITransformerScore _transformerScore;
        private ITransformerWar _transformerWar;

        public TransformersController(ILogger<Controller> logger, ITransformerAdd transformerAdd, ITransformerRetrieve transformerRetrieve, ITransformerUpdate transformerUpdate
                , ITransformerRemove transformerRemove, ITransformerList transformerList, ITransformerScore transformerScore, ITransformerWar transformerWar)
        {
            _logger = logger;
            _transfromerAdd = transformerAdd;
            _transformerRetrieve = transformerRetrieve;
            _transformerUpdate = transformerUpdate;
            _transformerRemove = transformerRemove;
            _transformerList = transformerList;
            _transformerScore = transformerScore;
            _transformerWar = transformerWar;
        }

        /// <summary>
        /// Action faciliates addition of new Transformer entity
        /// </summary>
        /// <param name="transformer">Transformer</param>
        /// <returns>string</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Add([FromBody] Transformer transformer)
        {
            _logger?.LogInformation("Add action called to add transformer");
            return await _transfromerAdd.ExecuteAdd(transformer);
        }

        /// <summary>
        /// Action faciliates retrieval of Tansformer entity
        /// </summary>
        /// <param name="transformerId">int</param>
        /// <returns>Transformer</returns>
        [HttpGet("[action]/{transformerId}")]
        public async Task<IActionResult> Retrieve(int transformerId)
        {
            _logger?.LogInformation("Retrive action called to retrieve transformer");
            return await _transformerRetrieve.ExecuteRetrieve(transformerId);
        }

        /// <summary>
        /// Action faciliates update to existing Transformer 
        /// </summary>
        /// <param name="transformer">Transformer</param>
        /// <param name="transformerId">int</param>
        /// <returns>string</returns>
        [HttpPut("[action]/{transformerId}")]
        public async Task<IActionResult> Update([FromBody] Transformer transformer, int transformerId)
        {
            _logger?.LogInformation("Update action called to update transformer");
            return await _transformerUpdate.ExecuteUpdate(transformer, transformerId);
        }

        /// <summary>
        /// Action faciliates removal/deletion of Transformer
        /// </summary>
        /// <param name="transformerId">int</param>
        /// <returns>string</returns>
        [HttpDelete("[action]/{transformerId}")]
        public async Task<IActionResult> Remove(int transformerId)
        {
            _logger?.LogInformation("Remove action called to remove transformer");
            return await _transformerRemove.ExecuteRemove(transformerId);
        }

        /// <summary>
        /// Action faciliates for result of autobot sorted list of transformers
        /// </summary>
        /// <returns>List<Transformer></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> AutobotSortedList()
        {
            _logger?.LogInformation("AutobotSortedList action called to retrieve sorted list of autobots");
            return await _transformerList.ExecuteList(TransformersEnums.TransformerAllegiances.Autobot.ToString(), true);
        }

        /// <summary>
        /// Action faciliates for result of decpticon sorted list of transformers
        /// </summary>
        /// <returns>List<Transformer></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> DecepticonSortedList()
        {
            _logger?.LogInformation("DecepticonSortedList action called to retrieve sorted list of decepticons");
            return await _transformerList.ExecuteList(TransformersEnums.TransformerAllegiances.Decepticon.ToString(), true);
        }

        /// <summary>
        /// Action faciliates for getting score of a particular Transformer
        /// </summary>
        /// <param name="transformerId">int</param>
        /// <returns>object</returns>
        [HttpGet("[action]/{transformerId}")]
        public async Task<IActionResult> Score(int transformerId)
        {
            _logger?.LogInformation("Score action called to retrieve score of a transformer");
            return await _transformerScore.ExecuteScore(transformerId);
        }

        /// <summary>
        /// Action faciliates execution of war between autobot & decepticon for returing output with results
        /// </summary>
        /// <returns>object</returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> War()
        {
            _logger?.LogInformation("War action called to simulate war between aubot and decepticon");
            return await _transformerWar.ExecuteWar();
        }
    }
}