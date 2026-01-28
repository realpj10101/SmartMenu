using System.Text.Json.Serialization;
using api.Extensions;
using api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddSignalR();

builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddRepositoryServices();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();