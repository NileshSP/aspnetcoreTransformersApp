using aspnetcoreTransformersApp.Controllers;
using aspnetcoreTransformersApp.Models;
using aspnetcoreTransformersApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using aspnetcoreTransformerApp.Test;

namespace aspnetcoreTransformersAppTests
{
    public class TransformersTests
    {
        private TransformersController _transformersController;
        private ITransformerDBContext _transformerDBContext;

        [SetUp]
        public async void Setup()
        {
            var services = new ServiceCollection();
            services.AddDbContext<TransformerDBContext>(options => options.UseInMemoryDatabase());
            services.AddTransient<ITransformerDBContext, TransformerDBContext>();
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
            await _transformerDBContext.SeedTestData();
            _transformersController = serviceProvider.GetService<TransformersController>();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}