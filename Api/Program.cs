using Nonuso.Api.Common;
using Nonuso.Api.Exceptions;
using Nonuso.Api.Extensions;
using Nonuso.Application;
using Nonuso.Infrastructure.Auth;
using Nonuso.Infrastructure.Persistence;

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
builder.Services.AddApplication();


var app = builder.Build();

app.SetupSwagger();

app.UseMiddleware<ApiExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
