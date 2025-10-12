using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GameMapping
{
  // Extension method to map CreateGameDto to Game entity
  public static Game ToEntity(this CreateGameDto dto)
  {
      return new Game
      {
          Name = dto.Name,
          GenreId = dto.GenreId,
          Price = dto.Price,
          ReleaseDate = dto.ReleaseDate
      };
  }
}