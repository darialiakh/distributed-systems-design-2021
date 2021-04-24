using Logic.DTO;
using Logic.QueueListener;
using Logic.QueueSender;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Producer
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
            //services.AddTransient<IQueueSender, PublishSubscribeQueueSender>();

            //services.AddHostedService<PointToPointQueueListener>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
