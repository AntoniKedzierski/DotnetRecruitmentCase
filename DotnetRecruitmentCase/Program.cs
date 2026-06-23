using AutoMapper;
using InvoicesApi.Mappers;
using InvoicesDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Serialization;

public partial class Program {

    private static async Task Main(string[] args) {

        var builder = WebApplication.CreateBuilder(args);
        builder.Services
            .AddControllers()
            .AddNewtonsoftJson(opt => {
                opt.SerializerSettings.ContractResolver = new DefaultContractResolver {
                    NamingStrategy = new CamelCaseNamingStrategy {
                        OverrideSpecifiedNames = true
                    }
                };
            });

        builder.Services.AddDbContext<InvoicesDbContext>(opt => opt.UseInMemoryDatabase("InvoicesDb"));

        builder.Services.AddSingleton<IMapper>(
            new MapperConfiguration(
                cfg => cfg.AddProfile<InvoicesProfile>(),
                NullLoggerFactory.Instance
            )
            .CreateMapper()
        );

        var app = builder.Build();

        using (var scope = app.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<InvoicesDbContext>();
            DatabaseSeeder.Seed(db);
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync();
    }

}