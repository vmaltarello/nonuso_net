using Nonuso.Api.Common;
using Nonuso.Api.Exceptions;
using Nonuso.Api.Extensions;
using Nonuso.Api.Hubs;
using Nonuso.Application;
using Nonuso.Domain;
using Nonuso.Infrastructure.Auth;
using Nonuso.Infrastructure.Notification;
using Nonuso.Infrastructure.Persistence;
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
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();

builder.Services.AddInfrastructurePersistence(builder.Configuration);
builder.Services.AddInfrastructureAuth(builder.Configuration);
builder.Services.AddInfrastructureS3Storage(builder.Configuration);
builder.Services.AddInfrastructureNotification();
builder.Services.AddApplication();
builder.Services.AddValidators();

var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");

app.SetupSwagger();

app.UseMiddleware<ApiExceptionHandler>();

app.UseHttpsRedirection();

app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
