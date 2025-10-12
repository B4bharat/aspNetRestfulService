using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;

namespace GameStore.Api.Endpoints
{
    public static class GamesEndpoints
    {

        private static readonly List<GameDto> games = new List<GameDto>
        {
            new GameDto(1, "The Legend of Zelda: Breath of the Wild", "Action-Adventure", 59.99m, new DateOnly(2017, 3, 3)),
            new GameDto(2, "Super Mario Odyssey", "Platformer", 59.99m, new DateOnly(2017, 10, 27)),
            new GameDto(3, "Metroid Dread", "Action-Adventure", 59.99m, new DateOnly(2021, 10, 8)),
            new GameDto(4, "Splatoon 3", "Shooter", 59.99m, new DateOnly(2022, 9, 9)),
            new GameDto(5, "Animal Crossing: New Horizons", "Simulation", 59.99m, new DateOnly(2020, 3, 20))
        };

        public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
        {
            var gamesGroup = app.MapGroup("games").WithParameterValidation();

            gamesGroup.MapGet("/", () => games);

            gamesGroup.MapGet("/{id:int}", (int id) =>
            {
                var game = games.FirstOrDefault(g => g.Id == id);
                return game is not null ? Results.Ok(game) : Results.NotFound();
            });

            gamesGroup.MapPost("/", (CreateGameDto createGameDto, GameStoreContext dbContext) =>
            {
                Game game = createGameDto.ToEntity();
                game.Genre = dbContext.Genres.Find(createGameDto.GenreId);

                dbContext.Games.Add(game);
                dbContext.SaveChanges();

                var gameDto = game.ToDto();

                return Results.Created($"/games/{game.Id}", gameDto);
            });

            gamesGroup.MapPut("/{id:int}", (int id, UpdateGameDto updateGameDto) =>
            {
                var game = games.FirstOrDefault(g => g.Id == id);
                if (game is null) return Results.NotFound();

                var updatedGame = new GameDto(
                    Id: game.Id,
                    Name: updateGameDto.Name,
                    Genre: updateGameDto.Genre,
                    Price: updateGameDto.Price,
                    ReleaseDate: updateGameDto.ReleaseDate
                );

                games.Remove(game);
                games.Add(updatedGame);

                return Results.Ok(updatedGame);
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