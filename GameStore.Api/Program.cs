using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStore.Api.Data.GameStoreContext>(connString);

var app = builder.Build();

// Helps mapping endpoints
app.MapGamesEndpoints();

// Migrate Database
app.MigrateDb();

app.Run();
