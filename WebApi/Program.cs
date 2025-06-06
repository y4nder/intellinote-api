using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Serilog;
using WebApi.Extensions;
using WebApi.Middlewares;
using WebApi.Services.Agent;
using WebApi.Services.External;
using WebApi.Services.Hubs;

var builder = WebApplication.CreateBuilder(args);

// logging with Serilog
builder.Host.AddSerilog();

// Adding Endpoints and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Setup Database Context
builder.Services.SetupCors();

builder.Services.AddApplicationServices(builder.Configuration);

// Adding Aufy
builder.Services.SetupAufy(builder.Configuration);
builder.Services.Configure<IdentityOptions>(options => { options.SignIn.RequireConfirmedEmail = false; });

var assembly = typeof(Program).Assembly;

// Mediatr
builder.Services.AddMediatR(config
    => config.RegisterServicesFromAssembly(assembly));

// Adding Carter
builder.Services.AddCarter();

// Adding Fluent Validation
builder.Services.AddValidatorsFromAssembly(assembly);

// manual registration of email service
builder.Services.SetupEmailService(builder.Configuration);

// for http context accessor
builder.Services.AddHttpContextAccessor();

// adding quartz
builder.Services.SetupQuartz(builder.Configuration);

// adding signalr
builder.Services.AddSignalR();

// adding external api service
builder.Services.AddGeneratedResponseService(builder.Configuration);

// adding agents
builder.Services.AddChatAgentsWithTools();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapCarter();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowLocalDev");

app.UseAuthentication();
app.UseAuthorization();

app.UseAufyEndpoints();


app.MapFallbackToFile("index.html");

app.MapHub<NoteHub>("/note-hub");
//use result pattern exception handler
app.UseResultExceptionHandler();
try
{
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "An error occurred while starting the application");
}