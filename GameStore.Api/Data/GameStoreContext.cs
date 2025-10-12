using System;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext : DbContext
{
  public GameStoreContext(DbContextOptions<GameStoreContext> options)
      : base(options)
  {
  }

  public DbSet<Game> Games => Set<Game>();
  public DbSet<Genre> Genres => Set<Genre>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      modelBuilder.Entity<Game>()
          .HasOne(g => g.Genre)
          .WithMany()
          .HasForeignKey(g => g.GenreId);
  }
}
