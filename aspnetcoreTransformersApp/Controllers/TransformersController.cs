using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using aspnetcoreTransformersApp.Models;
using aspnetcoreTransformersApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcoreTransformersApp.Controllers
{
    [Route("api/[controller]")]
    public class TransformersController : Controller
    {
        private ITransformerAdd _transfromerAdd;
        private ITransformerRetrieve _transformerRetrieve;
        private ITransformerUpdate _transformerUpdate;
        private ITransformerRemove _transformerRemove;
        private ITransformerList _transformerList;
        private ITransformerScore _transformerScore;
        private ITransformerWar _transformerWar;

        public TransformersController(ITransformerAdd transformerAdd, ITransformerRetrieve transformerRetrieve, ITransformerUpdate transformerUpdate
                , ITransformerRemove transformerRemove, ITransformerList transformerList, ITransformerScore transformerScore, ITransformerWar transformerWar)
        {
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
            return await _transformerRemove.ExecuteRemove(transformerId);
        }

        /// <summary>
        /// Action faciliates for result of autobot sorted list of transformers
        /// </summary>
        /// <returns>List<Transformer></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> AutobotSortedList()
        {
            return await _transformerList.ExecuteList(TransformersEnums.TransformerAllegiances.Autobot.ToString(), true);
        }

        /// <summary>
        /// Action faciliates for result of decpticon sorted list of transformers
        /// </summary>
        /// <returns>List<Transformer></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> DecepticonSortedList()
        {
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
            return await _transformerScore.ExecuteScore(transformerId);
        }

        /// <summary>
        /// Action faciliates execution of war between autobot & decepticon for returing output with results
        /// </summary>
        /// <returns>object</returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> War()
        {
            return await _transformerWar.ExecuteWar();
        }
    }
}