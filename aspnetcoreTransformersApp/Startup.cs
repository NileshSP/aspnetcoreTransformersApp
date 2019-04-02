using aspnetcoreTransformersApp.Models;
using aspnetcoreTransformersApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag.AspNetCore;

namespace aspnetcoreTransformersApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TransformerDBContext>(options =>
                                //options.UseInMemoryDatabase()                                                     // For In memory
                                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))      //For MSSql
                            );
            services.AddTransient<ITransformerDBContext, TransformerDBContext>();
            services.AddTransient<ITransformerRepository, TransformerRepository>();
            services.AddTransient<ITransformerAdd, TransformerCommands>();
            services.AddTransient<ITransformerRetrieve, TransformerCommands>();
            services.AddTransient<ITransformerUpdate, TransformerCommands>();
            services.AddTransient<ITransformerRemove, TransformerCommands>();
            services.AddTransient<ITransformerList, TransformerCommands>();
            services.AddTransient<ITransformerScore, TransformerCommands>();
            services.AddTransient<ITransformerWar, TransformerWar>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // For API documentation UI
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Transformers API";
                    document.Info.Description = "A simple ASP.NET Core web API";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "Nilesh Patel",
                        Email = "emailnileshsp@gmail.com",
                        Url = "https://github.com/NileshSP"
                    };
                    document.Info.License = new NSwag.SwaggerLicense
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    };
                };
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddResponseCaching();
            services.AddResponseCompression();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, TransformerDBContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            // For API documentation UI
            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            //For MSSql
            if (context.Database.IsSqlServer()) context.Database.Migrate();

            //Populate initial data
            context.SeedData().GetAwaiter().GetResult();
        }
    }
}
