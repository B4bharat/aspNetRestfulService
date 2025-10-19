using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;

namespace GameStore.Api.Endpoints
{
    public static class GamesEndpoints
    {

        public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
        {
            var gamesGroup = app.MapGroup("games").WithParameterValidation();

            gamesGroup.MapGet("/", async (GameStoreContext dbContext) =>
            {
                var gameSummaries = await dbContext.Games
                    .Include(game => game.Genre)
                    .Select(game => game.ToGameSummaryDto())
                    .AsNoTracking()
                    .ToListAsync();
                return Results.Ok(gameSummaries);
            });

            gamesGroup.MapGet("/{id:int}", async (int id, GameStoreContext dbContext) =>
            {
                var game = await dbContext.Games.FindAsync(id);
                return game is not null ? Results.Ok(game.ToGameDetailsDto()) : Results.NotFound();
            });

            gamesGroup.MapPost("/", async (CreateGameDto createGameDto, GameStoreContext dbContext) =>
            {
                Game game = createGameDto.ToEntity();

                dbContext.Games.Add(game);
                await dbContext.SaveChangesAsync();

                var gameDto = game.ToGameDetailsDto();

                return Results.Created($"/games/{game.Id}", gameDto);
            });

            gamesGroup.MapPut("/{id:int}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                var existingGame = await dbContext.Games.FindAsync(id);
                if (existingGame is null) return Results.NotFound();

                dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));
                await dbContext.SaveChangesAsync();

                return Results.Ok();
            });

            gamesGroup.MapDelete("/{id:int}", async (int id, GameStoreContext dbContext) =>
            {
                await dbContext.Games.Where(games => games.Id == id).ExecuteDeleteAsync();

                return Results.NoContent();
            });

            return gamesGroup;
        }
    }
}