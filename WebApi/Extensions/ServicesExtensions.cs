using AspNet.Security.OAuth.Discord;
using AspNet.Security.OAuth.GitHub;
using Aufy.Core;
using Aufy.Core.AuthSchemes;
using Aufy.Core.Endpoints;
using Aufy.EntityFrameworkCore;
using Aufy.FluentEmail;
using FluentEmail.MailKitSmtp;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl.AdoJobStore;
using Serilog;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Features.Auth;
using WebApi.Features.Keywords.Jobs;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Extensions;

public static class ServicesExtensions
{
    public static IHostBuilder AddSerilog(this IHostBuilder builder)
    {
        builder.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console());
        return builder;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // modify database provider
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString, 
            o => o.UseVector()));
        services.AddDatabaseDeveloperPageExceptionFilter();
        // services.AddMongoDbExtensions(configuration);
        
        // //adding repository scope
        services.AddRepositories();
        services.AddWebServices();
        services.AddMapperService();
        return services;
    }


    public static IServiceCollection SetupAufy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAufy<User>(configuration)
            .AddProvider(GitHubAuthenticationDefaults.AuthenticationScheme,
                (auth, options) =>
                {
                    auth.AddGitHub(o => o.Configure(GitHubAuthenticationDefaults.AuthenticationScheme, options));
                })
            .AddProvider(DiscordAuthenticationDefaults.AuthenticationScheme,
                (auth, options) =>
                {
                    auth.AddDiscord(o => o.Configure(DiscordAuthenticationDefaults.AuthenticationScheme, options));
                })
            .AddProvider(GoogleDefaults.AuthenticationScheme,
                (auth, options) =>
                {
                    auth.AddGoogle(o => { o.Configure(GoogleDefaults.AuthenticationScheme, options); });
                })
            .AddDefaultCorsPolicy()
            .AddEntityFrameworkStore<ApplicationDbContext, User>()
            .AddFluentEmail();
            // .UseAufyCustomSignup();


        return services;
    }

    public static IServiceCollection SetupEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddFluentEmail("aufy@example.com")
            .AddMailKitSender(new SmtpClientOptions
            {
                Server = configuration["FluentEmail:SmtpHost"],
                Port = configuration.GetValue<int>("FluentEmail:SmtpPort"),
                User = configuration["FluentEmail:SmtpUsername"],
                Password = configuration["FluentEmail:SmtpPassword"]
            });
        return services;
    }

    // adding quartz
    public static IServiceCollection SetupQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            q.UsePersistentStore(c =>
            {
                c.UsePostgres(options =>
                {
                    options.UseDriverDelegate<PostgreSQLDelegate>();
                    options.ConnectionString = configuration.GetConnectionString("DefaultConnection") 
                                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                    options.TablePrefix = "quartz.qrtz_";
                });
                c.UseProperties = true;
                c.UseNewtonsoftJsonSerializer();
                c.PerformSchemaValidation = false;
            });
            
            q.AddJob<BatchInsertNewKeywords>(j => j
                .StoreDurably()
                .WithIdentity(BatchInsertNewKeywords.Name));
            
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}

public static class AufyBuilderExtensions
{
    /// <summary>
    ///     Use this method to add custom signup models
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AufyServiceBuilder<User> UseAufyCustomSignup(this AufyServiceBuilder<User> builder)
    {
        // builder
        //     .UseSignUpModel<MySignUpRequest>()
        //     .UseExternalSignUpModel<MySignUpExternalRequest>();
        //
        //
        // builder.Services
        //     .AddScoped<ISignUpExternalEndpointEvents<MyUser, MySignUpExternalRequest>, SignUpExternalExtension>();
        // builder.Services.AddScoped<ISignUpEndpointEvents<MyUser, MySignUpRequest>, SignUpExtension>();
        
        return builder;
    }

    public static void UseAufyEndpoints(this WebApplication app)
    {
        app.MapAufyEndpoints(c =>
        {
            c.ConfigureRoute<SignUpEndpoint<User, SignUpRequest>>(handlerBuilder =>
            {
                handlerBuilder.UseCreateUserDataAction();
            });
        });

    }
}