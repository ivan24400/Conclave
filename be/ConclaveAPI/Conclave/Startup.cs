using Conclave.Filters.Authorization;
using Conclave.Middlewares;
using Conclave.Services;
using Conclave.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Reflection;

namespace Conclave
{
    public class Startup
    {
        private static readonly string API_VERSION = "v1";
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings." + env.EnvironmentName + ".json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables().Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<AppSettingsConfig>(Configuration.GetSection("AppConfig"));
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = Int32.MaxValue;
            });
            services.Configure<ApiBehaviorOptions>(a =>
            {
                a.InvalidModelStateResponseFactory = context =>
                {
                    return new BadRequestObjectResult(new CBadRequest(context))
                    {
                        ContentTypes = { "application/json" }
                    };
                };
            });
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton<RedisClient>();
            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new ConclaveAuthorizationFilterFactory());
                    options.UseGeneralRoutePrefix(new RouteAttribute(API_VERSION));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            CLogger._exceptionFilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Configuration["AppConfig:Storage:ExceptionFile"]);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
                RequestPath = "/Uploads"
            });
            app.UseMiddleware(typeof(ExceptionHandler));
            app.UseMvc();
        }


    }
}
