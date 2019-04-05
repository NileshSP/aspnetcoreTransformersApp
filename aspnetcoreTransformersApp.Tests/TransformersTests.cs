using aspnetcoreTransformersApp.Controllers;
using aspnetcoreTransformersApp.Models;
using aspnetcoreTransformersApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using aspnetcoreTransformerApp.Test;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace aspnetcoreTransformersTests
{
    public class TransformersTests
    {
        private TransformersController _transformersController;
        private ITransformerDBContext _transformerDBContext;
        private ITransformerRepository _transformerRepository;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>(provider => PopulateTestData.GetConfiguration(TestContext.CurrentContext.TestDirectory));
            services.AddDbContext<TransformerDBContext>(options => options.UseInMemoryDatabase());
            services.AddTransient<ITransformerDBContext,TransformerDBContext>();
            services.AddTransient<ITransformerRepository, TransformerRepository>();
            services.AddTransient<ITransformerAdd, TransformerCommands>();
            services.AddTransient<ITransformerRetrieve, TransformerCommands>();
            services.AddTransient<ITransformerUpdate, TransformerCommands>();
            services.AddTransient<ITransformerRemove, TransformerCommands>();
            services.AddTransient<ITransformerList, TransformerCommands>();
            services.AddTransient<ITransformerScore, TransformerCommands>();
            services.AddTransient<ITransformerWar, TransformerWar>();
            services.AddTransient<TransformersController>();
            var serviceProvider = services.BuildServiceProvider();
            _transformerDBContext = serviceProvider.GetService<ITransformerDBContext>();
            _transformerDBContext.SeedTestData().GetAwaiter().GetResult();
            _transformersController = serviceProvider.GetService<TransformersController>();
            _transformerRepository = serviceProvider.GetService<ITransformerRepository>();
        }

        public TransformersTests()
        {
        }

        [Test, Order(1)]
        [TestCaseSource(typeof(TransformerTestCases), "TransformerAddCases")]
        public async Task<string> TransformerAdd(Transformer transformer)
        {
            var result = await _transformersController.Add(transformer);
            var okObjectResult = result as OkObjectResult;
            var objectResult = result as ObjectResult;
            string resultText = "";
            if (okObjectResult != null)
            {
                resultText = okObjectResult.Value as string;
            }
            else if (objectResult != null)
            {
                resultText = objectResult.Value as string;
            }
            Assert.NotNull(resultText);
            return resultText; 
        }

        [Test, Order(2)]
        [TestCaseSource(typeof(TransformerTestCases), "TransformerRetrieveCases")]
        public async Task<object> TransformerRetrieve(int transformerId)
        {
            var result = await _transformersController.Retrieve(transformerId);
            var okObjectResult = result as OkObjectResult;
            var objectResult = result as ObjectResult;
            if (okObjectResult != null)
            {
                Transformer resultText = (Transformer)okObjectResult.Value;
                Assert.NotNull(resultText);
                return true;
            }
            else 
            {
                string resultText = objectResult.Value as string;
                Assert.NotNull(resultText);
                return false;
            }

        }

        [Test, Order(3)]
        [TestCaseSource(typeof(TransformerTestCases), "TransformerUpdateCases")]
        public async Task<object> TransformerUpdate(Transformer transformer, int transformerId)
        {
            var result = await _transformersController.Update(transformer, transformerId);
            var okObjectResult = result as OkObjectResult;
            var objectResult = result as ObjectResult;
            if (okObjectResult != null)
            {
                string resultText = okObjectResult.Value as string;
                Assert.NotNull(resultText);
                return resultText;
            }
            else
            {
                string resultText = objectResult.Value as string;
                Assert.NotNull(resultText);
                return resultText;
            }
        }

        [Test, Order(4)]
        [TestCaseSource(typeof(TransformerTestCases), "TransformerDeleteCases")]
        public async Task<object> TransformerRemove(int transformerId)
        {
            var result = await _transformersController.Remove(transformerId);
            var okObjectResult = result as OkObjectResult;
            var objectResult = result as ObjectResult;
            if (okObjectResult != null)
            {
                string resultText = okObjectResult.Value as string;
                Assert.NotNull(resultText);
                return resultText;
            }
            else
            {
                string resultText = objectResult.Value as string;
                Assert.NotNull(resultText);
                return resultText;
            }
        }

        [Test, Order(5)]
        public void AutoBotSortedList()
        {
            var result = _transformersController.AutobotSortedList().GetAwaiter().GetResult();
            var okObjectResult = result as OkObjectResult;
            var objectResult = result as ObjectResult;
            if (okObjectResult != null)
            {
                List<Transformer> resultData = okObjectResult.Value as List<Transformer>;
                Assert.IsTrue(resultData.FirstOrDefault().Name.Trim() == "Autobot - five");
            }
            else
            {
                string resultText = objectResult.Value as string;
                Assert.NotNull(resultText);
            }
        }

        [Test, Order(6)]
        public void DecepticonSortedList()
        {
            var result = _transformersController.DecepticonSortedList().GetAwaiter().GetResult();
            var okObjectResult = result as OkObjectResult;
            var objectResult = result as ObjectResult;
            if (okObjectResult != null)
            {
                List<Transformer> resultData = okObjectResult.Value as List<Transformer>;
                Assert.IsTrue(resultData.FirstOrDefault().Name.Trim() == "Decepticon - five");
            }
            else
            {
                string resultText = objectResult.Value as string;
                Assert.NotNull(resultText);
            }
        }

        [Test, Order(7)]
        [TestCaseSource(typeof(TransformerTestCases), "TransformerScoreCases")]
        public async Task<object> TransformerScore(int transformerId)
        {
            Transformer transformer;
            transformer = await _transformerRepository.getTransformer(s => s.TransformerId == transformerId);
            int sampleScore = (transformer != null ? transformer.Courage + transformer.Endurance + transformer.Firepower + transformer.Intelligence + transformer.Rank + transformer.Skill + transformer.Speed + transformer.Strength : 0);
            var result = _transformersController.Score(transformer.TransformerId).GetAwaiter().GetResult();
            var okObjectResult = result as OkObjectResult;
            var objectResult = result as ObjectResult;
            if (okObjectResult != null)
            {
                dynamic resultValue = okObjectResult.Value as JObject;
                Assert.NotNull(resultValue, "Result output is null or not a json");
                Assert.IsTrue(resultValue.Score == sampleScore, "Scores doesn't match");
                return resultValue;
            }
            else
            {
                string resultText = objectResult.Value as string;
                Assert.NotNull(resultText, "message is empty");
                return resultText;
            }
        }

        [Test, Order(8)]
        public void TransformerWar()
        {
            var result = _transformersController.War().GetAwaiter().GetResult();
            var okObjectResult = result as OkObjectResult;
            var objectResult = result as ObjectResult;
            if (okObjectResult != null)
            {
                var resultValue = okObjectResult.Value as JObject;
                Assert.NotNull(resultValue,"Result is not Json output");
                Assert.IsTrue(resultValue.ContainsKey("Victors"), "Result doesn't have victors list");
                Assert.IsTrue(resultValue.ContainsKey("Survivors"), "Result doesn't have survivors list");
                Assert.NotNull(resultValue, "Result is not Json output");
            }
            else
            {
                string resultText = objectResult.Value as string;
                Assert.NotNull(resultText);
                Assert.IsTrue(!resultText.Contains("error"));
            }
        }
    }
}