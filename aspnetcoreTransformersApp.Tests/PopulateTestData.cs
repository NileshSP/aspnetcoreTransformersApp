using aspnetcoreTransformersApp.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcoreTransformerApp.Test
{
    static class PopulateTestData
    {
        /// <summary>
        /// Generate test data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Task SeedTestData(this ITransformerDBContext context)
        {
            return Task.Run(() =>
            {
                // To truncate tables and reseed the identities -- only required during odd cases
                //if (context.Transformers.Any())
                //{
                //    context.Transformers.RemoveRange(context.Transformers);
                //    context.TransformerAllegiances.RemoveRange(context.TransformerAllegiances);
                //    context.SaveChanges();
                //    if (context.Database.IsSqlServer()) // For MSSQL only
                //    {
                //        context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Transformers', RESEED, 0)");
                //        context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('TransformerAllegiances', RESEED, 0)");
                //    }
                //}

                if (!context.TransformerAllegiances.Any())
                {
                    context.TransformerAllegiances.AddRange(TransformersEnums.TransformerAllegiancesList());
                    context.DatabaseContext.SaveChanges();
                }

                if (!context.Transformers.Any())
                {
                    context.Transformers.AddRange(getTransformers(context).GetAwaiter().GetResult());
                    context.DatabaseContext.SaveChanges();
                }
            });
        }

        /// <summary>
        /// returns list of tranformers
        /// </summary>
        /// <param name="context"></param>
        /// <returns><List<Transformer></returns>
        public static Task<List<Transformer>> getTransformers(ITransformerDBContext context) {

            return Task.Run(() => context
                                    .TransformerAllegiances
                                    .ToList<TransformerAllegiance>()
                                    .Select((transformerAllegiance, index) =>  
                                        Enumerable
                                            .Range(1,5)
                                            .Select(item => 
                                                new Transformer {
                                                        AllegianceId = transformerAllegiance.TransformerAllegianceId 
                                                        ,Name = $"{transformerAllegiance.AllegianceName} - {ModelBuilderExtensions.NumberToWords(item)}"
                                                        ,Strength = ModelBuilderExtensions.getRandomNumber(1,10)
                                                        ,Intelligence = ModelBuilderExtensions.getRandomNumber(1,10)
                                                        ,Speed = ModelBuilderExtensions.getRandomNumber(1,10)
                                                        ,Endurance = ModelBuilderExtensions.getRandomNumber(1,10)
                                                        ,Rank = ModelBuilderExtensions.getRandomNumber(1,10)
                                                        ,Courage = ModelBuilderExtensions.getRandomNumber(1,10)
                                                        ,Firepower = ModelBuilderExtensions.getRandomNumber(1,10)
                                                        ,Skill = ModelBuilderExtensions.getRandomNumber(1,10)
                                                    }
                                             )
                                             .ToList<Transformer>()
                                    )
                                    .SelectMany(items => items)
                                    .ToList<Transformer>()
           );
        }

        /// <summary>
        /// Returns configuration object instance for files like appsettings.json
        /// </summary>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public static IConfiguration GetConfiguration(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

    }

    public class TransformerTestCases
    {
        public static IEnumerable TransformerAddCases
        {
            get
            {
                yield return new TestCaseData(new Transformer { AllegianceId = 1, Courage = 1, Endurance = 1, Firepower = 1, Intelligence = 1, Rank = 1, Skill = 1, Speed = 1, Strength = 1, Name = "Autobot - six" }).Returns("Transformer added succesfully").SetName("TransfomerAdd - add new case");
                yield return new TestCaseData(new Transformer { AllegianceId = 1, Courage = 1, Endurance = 1, Firepower = 1, Intelligence = 1, Rank = 1, Skill = 1, Speed = 1, Strength = 1, Name = "Autobot - one" }).Returns("Autobot - one with Autobot already exists !!").SetName("TransfomerAdd - returns appropriate message while adding existing transformer as new");
            }
        }

        public static IEnumerable TransformerRetrieveCases
        {
            get
            {
                yield return new TestCaseData(1).Returns(true).SetName("TransformerRetrieve - returns available transformer");
                yield return new TestCaseData(15).Returns(false).SetName("TransformerRetrieve - transformer not found");
            }
        }

        public static IEnumerable TransformerUpdateCases
        {
            get
            {
                yield return new TestCaseData(new Transformer { AllegianceId = 1, Courage = 1, Endurance = 1, Firepower = 1, Intelligence = 1, Rank = 1, Skill = 1, Speed = 1, Strength = 1, Name = "Autobot - six" }, 1).Returns("Transformer updated successfully").SetName("TransformerUpdate - updates transformer");
            }
        }

        public static IEnumerable TransformerDeleteCases
        {
            get
            {
                yield return new TestCaseData(1).Returns("Transformer with TransformerId=1 was removed/deleted successfully").SetName("TransformerRemove - deletes/removes transformer");
            }
        }

        public static IEnumerable TransformerScoreCases
        {
            get
            {
                yield return new TestCaseData(1).Returns("error occured : Relational-specific methods can only be used when the context is using a relational database provider.").SetName("TransformerScore - returns expected message");
                //yield return new TestCaseData(1).Returns(new { TransformerId = 1, Score = 45 }).SetName("TransformerScore - returns expected score");
                //yield return new TestCaseData(15).Returns("Transformer with TransformerId=15 not found").SetName("TransformerScore - returns expected not found message");
            }
        }
    }
}
