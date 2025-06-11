using Nonuso.Api.Common;
using Nonuso.Api.Exceptions;
using Nonuso.Api.Extensions;
using Nonuso.Api.Hubs;
using Nonuso.Application;
using Nonuso.Domain;
using Nonuso.Infrastructure.Auth;
using Nonuso.Infrastructure.Notification;
using Nonuso.Infrastructure.Persistence;
using Nonuso.Infrastructure.Redis;
using Nonuso.Infrastructure.Secret;
using Nonuso.Infrastructure.Storage;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();
builder.Services.AddInfrastructureSecret();

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var sp = builder.Services.BuildServiceProvider();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var secretManager = sp.GetRequiredService<ISecretManager>();

builder.Services.AddSwagger(secretManager, builder.Configuration);

builder.Services.AddInfrastructurePersistence(secretManager);
builder.Services.AddInfrastructureAuth(secretManager, builder.Configuration);
builder.Services.AddInfrastructureS3Storage(secretManager);
builder.Services.AddInfrastructureRedis(secretManager);
builder.Services.AddInfrastructureNotification();
builder.Services.AddApplication();
builder.Services.AddValidators();

var app = builder.Build();

app.MapHub<ChatHub>("/chatHub").RequireAuthorization();
app.MapHub<PresenceHub>("/presenceHub").RequireAuthorization();

app.SetupSwagger();

app.UseMiddleware<ApiExceptionHandler>();

//app.UseHttpsRedirection();

app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
