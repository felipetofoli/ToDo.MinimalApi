using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MinimalApi.Tests;

internal class MinimalApiApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services => { 
            services.AddSingleton<IDatabase, InMemoryDatabaseFake>();
        });

        return base.CreateHost(builder);
    }
}