using aspnetcoreTransformersApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcoreTransformersApp.Services
{
    public class TransformerCommands : ITransformerAdd, ITransformerRetrieve, ITransformerUpdate, ITransformerRemove,
                                        ITransformerList, ITransformerScore
    {
        private ITransformerRepository _transformerRepository;

        public TransformerCommands(ITransformerRepository repository)
        {
            _transformerRepository = repository;
        }

        /// <summary>
        /// Adds transformer to db
        /// </summary>
        /// <param name="transformer">Transformer model entity to be added</param>
        /// <returns>string</returns>
        public async Task<IActionResult> ExecuteAdd(Transformer transformer)
        {
            IActionResult response;
            try
            {
                if (transformer.Name.ToLower().Trim() == "string")
                {
                    response = new ObjectResult($"Transformer name cannot be {transformer.Name}");
                }
                else if (transformer.Name.ToLower().Trim().Contains("optimus")
                    && transformer.Name.ToLower().Trim().Contains("prime")
                        && transformer.AllegianceId == (_transformerRepository
                                                            .getTransformerAllegiance(s => s.AllegianceName.ToLower().Trim().Contains("decepticon")))
                                                            .GetAwaiter().GetResult().TransformerAllegianceId) // Is decepticon ?
                {
                    response = new ObjectResult("Optimus prime cannot be decepticon");
                }
                else
                {
                    Transformer transformerExistCheck = await _transformerRepository
                                                                   .getTransformer(s => (s.Name.ToLower().Trim() == transformer.Name.ToLower().Trim()
                                                                                             && s.AllegianceId == transformer.AllegianceId
                                                                                        )
                                                                                   );
                    if (transformerExistCheck != null)
                    {
                        TransformerAllegiance transformerAllegianceType = await _transformerRepository.getTransformerAllegiance(s => s.TransformerAllegianceId == transformer.AllegianceId);
                        response = new ObjectResult($"{transformer.Name} with {transformerAllegianceType.AllegianceName} already exists !!");
                    }
                    else
                    {
                        int returnVal = await _transformerRepository.TransformerAdd(transformer);
                        response = (returnVal > 0 ? new OkObjectResult("Transformer added succesfully") : new ObjectResult("Transformer was not added"));
                    }
                }
            }
            catch (Exception ex)
            {
                response = new ObjectResult(ex);
            }
            return response;
        }

        /// <summary>
        /// Returns transformer model entity based on provided transformerId
        /// </summary>
        /// <param name="transformerId">Transformer Id</param>
        /// <returns>Transformer model entity</returns>
        public async Task<IActionResult> ExecuteRetrieve(int transformerId)
        {
            IActionResult response;
            try
            {
                Transformer transformer = await _transformerRepository.getTransformer(s => s.TransformerId == transformerId);
                if (transformer != null)
                {
                    response = new OkObjectResult(transformer);
                }
                else
                {
                    response = new ObjectResult($"Transformer with TransformerId={transformerId} not found");
                }
            }
            catch (Exception ex)
            {
                response = new ObjectResult(ex);
            }
            return response;
        }

        /// <summary>
        /// Updates existing transformer entity based on provided transformerId with updated transformer model entity
        /// </summary>
        /// <param name="transformer">Transformer model entity</param>
        /// <param name="transformerId">Transformer Id</param>
        /// <returns></returns>
        public async Task<IActionResult> ExecuteUpdate(Transformer transformer, int transformerId)
        {
            IActionResult response;
            try
            {
                TransformerAllegiance transformerAllegiance = await _transformerRepository.getTransformerAllegiance(s => s.TransformerAllegianceId == transformer.AllegianceId);
                if (transformerAllegiance == null)
                {
                    response = new NotFoundObjectResult("Transformer allegiance not found!!");
                }
                else
                {
                    Transformer transformerExistCheck = await _transformerRepository.getTransformer(s => (s.Name.ToLower().Trim() == transformer.Name.ToLower().Trim()
                                                                                                            && s.AllegianceId == transformer.AllegianceId
                                                                                                          )
                                                                                                   );
                    if (transformerExistCheck != null)
                    {
                        TransformerAllegiance transformerAllegianceType = await _transformerRepository.getTransformerAllegiance(s => s.TransformerAllegianceId == transformer.AllegianceId);
                        response = new ObjectResult($"{transformer.Name} with {transformerAllegianceType.AllegianceName} already exists and hence cannot be updated !!");
                    }
                    else
                    {
                        Transformer botToUpdate = await _transformerRepository.getTransformer(s => s.TransformerId == transformerId);
                        if (botToUpdate != null)
                        {
                            botToUpdate.AllegianceId = transformer.AllegianceId;
                            botToUpdate.Name = transformer.Name;
                            botToUpdate.Strength = transformer.Strength;
                            botToUpdate.Intelligence = transformer.Intelligence;
                            botToUpdate.Speed = transformer.Speed;
                            botToUpdate.Endurance = transformer.Endurance;
                            botToUpdate.Rank = transformer.Rank;
                            botToUpdate.Courage = transformer.Courage;
                            botToUpdate.Firepower = transformer.Firepower;
                            botToUpdate.Skill = transformer.Skill;

                            int returnVal = await _transformerRepository.TransformerUpdate(botToUpdate);
                            response = (returnVal > 0 ? new OkObjectResult("Transformer updated successfully") : new ObjectResult("Transformer was not updated"));
                        }
                        else
                        {
                            response = new NotFoundObjectResult($"Transformer with TransformerId = {transformerId} not found!!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = new ObjectResult(ex);
            }
            return response;
        }

        /// <summary>
        /// Remove/Delete particular transformer entity
        /// </summary>
        /// <param name="transformerId">Transformer Id to be removed/deleted</param>
        /// <returns>string</returns>
        public async Task<IActionResult> ExecuteRemove(int transformerId)
        {
            IActionResult response;
            try
            {
                int resultVal = await _transformerRepository.TransformerRemove(transformerId);                   
                response = (resultVal > 0 
                                ? new OkObjectResult($"Transformer with TransformerId={transformerId} was removed/deleted successfully") 
                                : new ObjectResult($"Transformer with TransformerId={transformerId} was not removed/deleted")
                            );
            }
            catch (Exception ex)
            {
                response = new ObjectResult(ex);
            }
            return response;
        }

        /// <summary>
        /// Get transformers sorted list filtered by a particular Allegiance
        /// </summary>
        /// <param name="transformerAllegiance">TransformerAllegiance</param>
        /// <returns>SortedList<string, Transformer></returns>
        public async Task<SortedList<string, Transformer>> TransformersListForAllegiance(TransformerAllegiance transformerAllegiance)
        {
            SortedList<string, Transformer> sortedTransformers = new SortedList<string, Transformer>();
            if (transformerAllegiance != null)
            {
                IDictionary<string, Transformer> transformers = (await _transformerRepository
                                                                    .TransformersList(s => s.AllegianceId == transformerAllegiance.TransformerAllegianceId))
                                                                    .ToDictionary(x => x.Name, x => x);
                if (transformers.Count > 0)
                {
                    sortedTransformers = new SortedList<string, Transformer>(transformers);
                }
            }
            return sortedTransformers;
        }

        /// <summary>
        /// Updates the Transformer entity with TransformerAllegiance object
        /// </summary>
        /// <param name="sortedList">List<Transformer></param>
        /// <param name="tAllegiance">TransformerAllegiance</param>
        /// <returns></returns>
        public List<Transformer> TransformerListWithAllegianceValue(List<Transformer> sortedList, TransformerAllegiance tAllegiance)
        {
            return sortedList.Select(s => { s.Allegiance = new List<TransformerAllegiance>() { tAllegiance }; return s; }).ToList();
        }

        /// <summary>
        /// Returns Transformers list for particular allegiance in specified (Asc/Desc)ordered list
        /// </summary>
        /// <param name="AllegianceId">Transformer Allegiance Id</param>
        /// <param name="sorted">boolean value as true for ascending or false for descending</param>
        /// <returns>List</returns>
        public async Task<IActionResult> ExecuteList(string Allegiance, bool sorted)
        {
            IActionResult response;
            try
            {
                TransformerAllegiance transformerAllegiance = await _transformerRepository.getTransformerAllegiance(s => s.AllegianceName.ToLower().Trim().Contains(Allegiance.ToLower().Trim()));
                SortedList<string, Transformer> sortedTransformers = await TransformersListForAllegiance(transformerAllegiance);
                if (sorted)
                {
                    response = new OkObjectResult(TransformerListWithAllegianceValue(sortedTransformers.Select(s => s.Value).ToList<Transformer>(), transformerAllegiance));
                }
                else
                {
                    response = new OkObjectResult(TransformerListWithAllegianceValue(sortedTransformers.Reverse().Select(s => s.Value).ToList<Transformer>(), transformerAllegiance));
                }

            }
            catch (Exception ex)
            {
                response = new ObjectResult(ex);
            }
            return response;
        }

        /// <summary>
        /// Returns score of the transformer
        /// </summary>
        /// <param name="transformerId">Transformer Id to get score for</param>
        /// <returns>int</returns>
        public async Task<IActionResult> ExecuteScore(int transformerId)
        {
            IActionResult response;
            try
            {
                Transformer transformer = await _transformerRepository.getTransformer(s => s.TransformerId == transformerId);
                if (transformer != null)
                {
                    dynamic paramObject = new
                    {
                        Name = "transformerid",
                        Value = transformerId
                    };
                    var score = await _transformerRepository.TransformerScore(paramObject, "StoredProcedures:Score");
                    response = new OkObjectResult(new { TransformerId = transformerId, Score = score });
                }
                else
                {
                    response = new ObjectResult($"Transformer with TransformerId={transformerId} not found");
                }
            }
            catch (Exception ex)
            {
                response = new ObjectResult($"error occured : {ex.Message}" );
            }
            return response;
        }

    }
}
