using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
// This is scoped to the lifetime of the request, may not be directly apparent
// but it is the same instance throughout the request
// Transient would be a new instance every time it is requested
builder.Services.AddSqlite<GameStore.Api.Data.GameStoreContext>(connString);

var app = builder.Build();

// Helps mapping endpoints
app.MapGamesEndpoints();

// Migrate Database
app.MigrateDb();

app.Run();
