using aspnetcoreTransformersApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcoreTransformersApp.Services
{
    public class TransformerWar : ITransformerWar
    {
        private ITransformerRepository _transformerRepository;
        private ITransformerList _transformerList;

        public TransformerWar(ITransformerRepository respository, ITransformerList transformerList)
        {
            _transformerRepository = respository;
            _transformerList = transformerList;
        }

        /// <summary>
        /// Execute/Simulate war between Transformer teams
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ExecuteWar()
        {
            IActionResult response;
            try
            {
                #region Get both teams
                IDictionary<TransformerAllegiance, SortedList<string, Transformer>> mainTransformersList = new Dictionary<TransformerAllegiance, SortedList<string, Transformer>>();
                List<TransformerAllegiance> transformerAllegiances = await _transformerRepository.TransformerAllegianceList(s => !string.IsNullOrEmpty(s.AllegianceName));
                transformerAllegiances.ForEach(async item =>
                {
                    SortedList<string, Transformer> transformersList = await _transformerList.TransformersListForAllegiance(item);
                    mainTransformersList.Add(item, transformersList);
                });
                #endregion

                #region Proceeding ahead ....
                if (mainTransformersList.Count == 2)
                {
                    #region Sort each team by rank
                    IDictionary<string, Transformer> teamA = await mainTransformersList.First().Value.ToAsyncEnumerable().OrderByDescending(s => s.Value.Rank).ToDictionary(x => x.Key, x => x.Value);
                    IDictionary<string, Transformer> teamB = await mainTransformersList.Last().Value.ToAsyncEnumerable().OrderByDescending(s => s.Value.Rank).ToDictionary(x => x.Key, x => x.Value);
                    #endregion

                    #region If the war is between optimus and predaking
                    if (await IsOptimusTeamVsPreDakingTeam(teamA,teamB))
                    {
                        return response = new OkObjectResult("Optimus and Predaking are both on oppsite sides and hence war is not imaginable!!");
                    }
                    #endregion

                    #region Getting soldiers ready before war starts.... i.e. declaring/defining the variables
                    dynamic finalResult = new JObject();
                    finalResult.WarOutcome = "";
                    finalResult.WarSummary = new JArray();
                    List<Transformer> transformerVictoryList = new List<Transformer>();
                    List<Transformer> transformerSurvivorList = new List<Transformer>();
                    int count = 0, countA = 0, countB = 0;
                    string resultCandidate = "";

                    Func<object, JObject> jsonSerialize = (obj) => JObject.Parse(JsonConvert.SerializeObject(obj));

                    Func<List<Transformer>, JArray> getWarRemainders = (list) =>
                    {
                        var objList = new JArray();
                        string itemsList = JsonConvert.SerializeObject(list);
                        objList.Add(JObject.Parse("{" + $"'List' : {itemsList}" + "}"));
                        return objList;
                    };

                    Func<Transformer, List<TransformerAllegiance>, Transformer> getTransformerWithAllegianceValue = (transformer, tAllegiance) =>
                    {
                        transformer.Allegiance = tAllegiance.Where(e => e.TransformerAllegianceId == transformer.AllegianceId).ToList<TransformerAllegiance>();
                        return transformer;
                    };

                    Action<Transformer, Transformer, string, string, int> appendResult = (transformerA, transformerB, candidate, reason, index) =>
                    {
                        var candidateVal = candidate.ToLower().Trim();

                        transformerA = getTransformerWithAllegianceValue(transformerA, transformerAllegiances);
                        transformerB = getTransformerWithAllegianceValue(transformerB, transformerAllegiances);

                        if (candidateVal != "none")
                        {
                            if (candidateVal == "a")
                            {
                                transformerVictoryList.Add(transformerA);
                                transformerSurvivorList.Add(transformerB);
                            }
                            else
                            {
                                transformerVictoryList.Add(transformerB);
                                transformerSurvivorList.Add(transformerA);
                            }
                            countA = (candidateVal == "a" ? countA + 1 : countA);
                            countB = (candidateVal == "b" ? countB + 1 : countB);
                            var WarDetails = new
                            {
                                TransformerAState = (candidateVal == "a" ? "Victor" : "Survivor"),
                                TransformerA = JObject.Parse(JsonConvert.SerializeObject(transformerA)),
                                TransformerBState = (candidateVal == "b" ? "Victor" : "Survivor"),
                                TransformerB = JObject.Parse(JsonConvert.SerializeObject(transformerB))
                            };
                            var warResult = new
                            {
                                Result = $"War {index} - '{(candidateVal == "a" ? transformerA.Name : transformerB.Name)}' won due to {reason}",
                                WarDetails
                            };
                            JArray item = (JArray)finalResult.WarSummary;
                            item.Add(jsonSerialize(warResult));
                        }
                    };
                    #endregion

                    #region Actual war starts now..
                    foreach ((Transformer transformerA, Transformer transformerB) in teamA.Zip(teamB, (s1,s2) => (s1.Value,s2.Value)))
                    {
                        count++;
                        resultCandidate = "none";

                        // If the war is between optimus and predaking
                        Transformer transformerOptimusOrPredaking = await IsOptimusOrPreDaking(transformerA, transformerB);
                        if (transformerOptimusOrPredaking.TransformerId != 0)
                        {
                            resultCandidate = (transformerOptimusOrPredaking.TransformerId == transformerA.TransformerId ? "A" : "B");
                            appendResult(transformerA, transformerB, resultCandidate, $"being {(resultCandidate == "A" ? transformerA.Name : transformerB.Name )}", count);
                        }

                        if (resultCandidate == "none")
                        {
                            // if the (3x strength teamA candidate > teamB candidate) && (teamB candidate courage < 5) then teamA contendor won
                            resultCandidate = await TransformerAStrengthAndCourageCheck(transformerA, transformerB);
                            appendResult(transformerA, transformerB, resultCandidate, "good strength & courage", count);
                        }

                        if (resultCandidate == "none")
                        {
                            // if the teamA candidate skill is better than teamB candidate
                            resultCandidate = await TransformerASkillCheck(transformerA, transformerB);
                            appendResult(transformerA, transformerB, resultCandidate, "better skill", count);
                        }

                        if (resultCandidate == "none")
                        {
                            // if the (strength + 3 of teamA > strength of teamB) && (teamB courage < 5) then teamA contendor won
                            resultCandidate = await HigherOverAllRating(transformerA, transformerB);
                            appendResult(transformerA, transformerB, resultCandidate, "high overall rating", count);
                        }
                    }
                    #endregion War finishes....

                    #region Preparing output
                    if (countA > countB || countA < countB)
                    {
                        finalResult["WarOutcome"] = $"{(countA > countB ? "TransformerA" : "TransformerB")} team won against {(countA > countB ? "TransformerB" : "TransformerA")} team by {(countA > countB ? countA : countB)} versus {(countA > countB ? countB : countA)} victory count";
                    }
                    else if (countA == countB)
                    {
                        finalResult["WarOutcome"] = $"War tied up between TeamA & TeamB with {countB} victories";
                    }

                    finalResult.Victories = getWarRemainders(transformerVictoryList);
                    finalResult.Survivors = getWarRemainders(transformerSurvivorList);

                    finalResult = jsonSerialize(finalResult);
                    #endregion

                    #region Output ready.....
                    return response = new OkObjectResult(finalResult);
                    #endregion
                }
                else
                {
                    response = new OkObjectResult("There are more than 2 transformer allegiances where war is unimaginable");
                }
                #endregion
            }
            catch (Exception ex)
            {
                response = new ObjectResult(ex);
            }
            return response;
        }

        /// <summary>
        /// Is Optimus team Vs PreDaking team check
        /// </summary>
        /// <param name="teamA">IDictionary<string, Transformer></param>
        /// <param name="teamB">IDictionary<string, Transformer></param>
        /// <returns>bool</returns>
        private Task<bool> IsOptimusTeamVsPreDakingTeam(IDictionary<string, Transformer> teamA, IDictionary<string, Transformer> teamB)
        {
            if ((teamA.Where(s => s.Key.ToString().ToLower().Contains("optimus")).Count() > 0 && teamB.Where(s => s.Key.ToString().ToLower().Contains("predaking")).Count() > 0)
                || (teamB.Where(s => s.Key.ToString().ToLower().Contains("optimus")).Count() > 0 && teamA.Where(s => s.Key.ToString().ToLower().Contains("predaking")).Count() > 0))
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Transformer is Optimus or PreDaking
        /// </summary>
        /// <param name="teamA">Transformer</param>
        /// <param name="teamB">Transformer</param>
        /// <returns>Transformer</returns>
        private Task<Transformer> IsOptimusOrPreDaking(Transformer teamA, Transformer teamB)
        {
            Transformer resultTransformer = new Transformer();

            if (teamA.Name.ToString().ToLower().Trim().Contains("optimus"))
            {
                resultTransformer = teamA;
            }
            else if (teamB.Name.ToString().ToLower().Trim().Contains("optimus"))
            {
                resultTransformer = teamB;
            }
            else if (teamA.Name.ToString().ToLower().Trim().Contains("predaking"))
            {
                resultTransformer = teamA;
            }
            else if (teamB.Name.ToString().ToLower().Trim().Contains("predaking"))
            {
                resultTransformer = teamB;
            }

            return Task.FromResult(resultTransformer);
        }

        /// <summary>
        /// Check TransformerA Strength And Courage case versus TransformerB
        /// </summary>
        /// <param name="teamACandidate">Transformer</param>
        /// <param name="teamBCandidate">Transformer</param>
        /// <returns>string</returns>
        private Task<string> TransformerAStrengthAndCourageCheck(Transformer teamACandidate, Transformer teamBCandidate)
        {
            string result = "none";
            if (teamACandidate.Strength >= teamBCandidate.Strength && teamBCandidate.Courage <= 5)
            {
                result = "A";
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// Check TransformerA Skill case versus TransformerB
        /// </summary>
        /// <param name="teamACandidate">Transformer</param>
        /// <param name="teamBCandidate">Transformer</param>
        /// <returns></returns>
        private Task<string> TransformerASkillCheck(Transformer teamACandidate, Transformer teamBCandidate)
        {
            string result = "none";
            if (teamACandidate.Skill > teamBCandidate.Skill + 5)
            {
                result = "A";
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// Get Transformer candidate with High overall rating case
        /// </summary>
        /// <param name="teamACandidate"></param>
        /// <param name="teamBCandidate"></param>
        /// <returns>string</returns>
        private Task<string> HigherOverAllRating(Transformer teamACandidate, Transformer teamBCandidate)
        {
            Func<Transformer, int> getOverAllScore = (transformer) => (transformer.Courage + transformer.Endurance + transformer.Firepower
                + transformer.Intelligence + transformer.Rank + transformer.Skill + transformer.Speed + transformer.Strength);

            string result = "none";

            int transformerAscore = getOverAllScore(teamACandidate);
            int transformerBscore = getOverAllScore(teamBCandidate);

            if (transformerAscore > transformerBscore)
            {
                result = "A";
            }
            else if(transformerAscore < transformerBscore)
            {
                result = "B";
            }

            return Task.FromResult(result);
        }

    }
}
