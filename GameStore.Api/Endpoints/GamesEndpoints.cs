using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;

namespace GameStore.Api.Endpoints
{
    public static class GamesEndpoints
    {

        private static readonly List<GameSummaryDto> games = new List<GameSummaryDto>
        {
            new GameSummaryDto(1, "The Legend of Zelda: Breath of the Wild", "1", 59.99m, new DateOnly(2017, 3, 3)),
            new GameSummaryDto(2, "Super Mario Odyssey", "2", 59.99m, new DateOnly(2017, 10, 27)),
            new GameSummaryDto(3, "Metroid Dread", "1", 59.99m, new DateOnly(2021, 10, 8)),
            new GameSummaryDto(4, "Splatoon 3", "3", 59.99m, new DateOnly(2022, 9, 9)),
            new GameSummaryDto(5, "Animal Crossing: New Horizons", "4", 59.99m, new DateOnly(2020, 3, 20))
        };

        public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
        {
            var gamesGroup = app.MapGroup("games").WithParameterValidation();

            gamesGroup.MapGet("/", (GameStoreContext dbContext) =>
            {
                var gameSummaries = dbContext.Games
                    .Include(game => game.Genre)
                    .Select(game => game.ToGameSummaryDto())
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

            gamesGroup.MapDelete("/{id:int}", (int id) =>
            {
                var game = games.FirstOrDefault(g => g.Id == id);
                if (game is null) return Results.NotFound();

                games.Remove(game);
                return Results.NoContent();
            });

            return gamesGroup;
        }
    }
}