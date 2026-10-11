using Api;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();
app.UseApi();
app.Run();

public partial class Program;
