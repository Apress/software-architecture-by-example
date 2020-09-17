using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicketSales.Api.Configuration;
using TicketSales.Common;
using TicketSales.ServiceBusHelper;

namespace TicketSales.Api
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
            services.AddControllers();

            services.AddSingleton<ServiceBusConfiguration>((s) => new ServiceBusConfiguration()
            {
                ConnectionString = Configuration.GetValue<string>("ServiceBus:ConnectionString"),
                QueueName = Configuration.GetValue<string>("ServiceBus:QueueName")
            });

            services.AddSingleton<ExternalBookingConfiguration>((s) => new ExternalBookingConfiguration()
            {
                ExternalBookingEndpoint = Configuration.GetValue<string>("ExternalTicketBookingEndpoint")
            });

            services.AddScoped<IQueueHelper, QueueHelper>();
            services.AddScoped<ILogger, ConsoleLogger>();

            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
