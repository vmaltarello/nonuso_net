using Nonuso.Api.Common;
using Nonuso.Api.Exceptions;
using Nonuso.Api.Extensions;
using Nonuso.Application;
using Nonuso.Domain;
using Nonuso.Infrastructure.Auth;
using Nonuso.Infrastructure.Notification;
using Nonuso.Infrastructure.Persistence;
using Nonuso.Infrastructure.Realtime;
using Nonuso.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();

builder.Services.AddInfrastructurePersistence(builder.Configuration);
builder.Services.AddInfrastructureAuth(builder.Configuration);
builder.Services.AddInfrastructureS3Storage(builder.Configuration);
builder.Services.AddInfrastructureNotification();
builder.Services.AddInfrastructureRealtime();
builder.Services.AddApplication();
builder.Services.AddValidators();

var app = builder.Build();

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
