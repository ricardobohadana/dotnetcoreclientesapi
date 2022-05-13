using DotNetEnv;
using GestaoDeClientesApi.Domain.Interfaces.Repositories;
using GestaoDeClientesApi.Domain.Interfaces.Services;
using GestaoDeClientesApi.Domain.Services;
using GestaoDeClientesApi.Infra.Data.Contexts;
using GestaoDeClientesApi.Infra.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeClientesApi.Tests
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private string _connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _connectionString = EnvironmentVariables.connectionString;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddApplicationPart(Assembly.Load("GestaoDeClientesApi.Services")).AddControllersAsServices();

            //string connectionString = Configuration.GetConnectionString("BDGestaoClientesAPI");

            services.AddDbContext<SqlServerContext>(c => c.UseSqlServer(_connectionString));
            services.AddTransient<IClienteRepository, ClienteRepository>();
            services.AddTransient<IClienteService, ClienteService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
