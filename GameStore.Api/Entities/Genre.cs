using System;

namespace GameStore.Api.Entities;

public class Genre
{
  public int Id { get; set; }
  public required string Name { get; set; }
}


// Notes:
// Database Migration is converting the C# classes [Entity Models] to database tables