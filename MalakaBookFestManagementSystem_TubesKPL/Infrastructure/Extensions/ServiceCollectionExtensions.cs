using MalakaBookFest.Application.Services;
using MalakaBookFest.Application.Tables;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Repositories;
using MalakaBookFest.Core.Interfaces.Services;
using MalakaBookFest.Infrastructure.Configuration;
using MalakaBookFest.Infrastructure.Persistence;
using MalakaBookFest.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DefaultConnection"));
        dataSourceBuilder.EnableUnmappedTypes();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(dataSource));

        services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
        services.Configure<TicketConfig>(configuration.GetSection("TicketConfig"));
        services.Configure<TalkshowConfig>(configuration.GetSection("TalkshowConfig"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBoothRepository, BoothRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ITalkshowRepository, TalkshowRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITalkshowRegistrationRepository, TalkshowRegistrationRepository>();
        services.AddScoped<IRepository<AuditLog>, Repository<AuditLog>>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBoothService, BoothService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ITalkshowService, TalkshowService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        services.AddSingleton<TicketPriceTable>();

        return services;
    }
}
