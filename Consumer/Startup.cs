using Logic.DTO;
using Logic.QueueListener;
using Logic.QueueSender;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var serviceClientSettingsConfig = Configuration.GetSection("RabbitMq");
            services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig);

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<PointToPointQueueListener>>();
            services.AddSingleton(typeof(ILogger), logger);


            services.AddTransient<IQueueSender, PointToPointQueueSender>();

            services.AddHostedService<PointToPointQueueListener>();
            //services.AddHostedService<PublishSubscribeQueueListener>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
            
        }
    }
}
