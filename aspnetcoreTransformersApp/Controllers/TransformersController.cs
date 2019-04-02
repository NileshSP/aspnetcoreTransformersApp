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
        /// <param name="transformer">Transformer enity to add in request body as json format</param>
        /// <returns>string</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Add([FromBody] Transformer transformer)
        {
            return await _transfromerAdd.ExecuteAdd(transformer);
        }

        [HttpGet("[action]/{transformerId}")]
        public async Task<IActionResult> Retrieve(int transformerId)
        {
            return await _transformerRetrieve.ExecuteRetrieve(transformerId);
        }

        [HttpPut("[action]/{transformerId}")]
        public async Task<IActionResult> Update([FromBody] Transformer transformer, int transformerId)
        {
            return await _transformerUpdate.ExecuteUpdate(transformer, transformerId);
        }

        [HttpDelete("[action]/{transformerId}")]
        public async Task<IActionResult> Remove(int transformerId)
        {
            return await _transformerRemove.ExecuteRemove(transformerId);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> AutobotSortedList()
        {
            return await _transformerList.ExecuteList(TransformersEnums.TransformerAllegiances.Autobot.ToString(), true);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DecepticonSortedList()
        {
            return await _transformerList.ExecuteList(TransformersEnums.TransformerAllegiances.Decepticon.ToString(), true);
        }

        [HttpGet("[action]/{transformerId}")]
        public async Task<IActionResult> Score(int transformerId)
        {
            return await _transformerScore.ExecuteScore(transformerId);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> War()
        {
            return await _transformerWar.ExecuteWar();
        }
    }
}