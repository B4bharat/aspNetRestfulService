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

            gamesGroup.MapGet("/", (GameStoreContext dbContext) =>
            {
                var gameSummaries = dbContext.Games
                    .Include(game => game.Genre)
                    .Select(game => game.ToGameSummaryDto())
                    .AsNoTracking()
                    .ToList();
                return Results.Ok(gameSummaries);
            });

            gamesGroup.MapGet("/{id:int}", (int id, GameStoreContext dbContext) =>
            {
                var game = dbContext.Games.Find(id);
                return game is not null ? Results.Ok(game.ToGameDetailsDto()) : Results.NotFound();
            });

            gamesGroup.MapPost("/", (CreateGameDto createGameDto, GameStoreContext dbContext) =>
            {
                Game game = createGameDto.ToEntity();

                dbContext.Games.Add(game);
                dbContext.SaveChanges();

                var gameDto = game.ToGameDetailsDto();

                return Results.Created($"/games/{game.Id}", gameDto);
            });

            gamesGroup.MapPut("/{id:int}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                var existingGame = dbContext.Games.Find(id);
                if (existingGame is null) return Results.NotFound();

                dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));
                dbContext.SaveChanges();

                return Results.Ok();
            });

            gamesGroup.MapDelete("/{id:int}", (int id, GameStoreContext dbContext) =>
            {
                dbContext.Games.Where(games => games.Id == id).ExecuteDelete();

                return Results.NoContent();
            });

            return gamesGroup;
        }
    }
}