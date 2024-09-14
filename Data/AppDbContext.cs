﻿using Microsoft.EntityFrameworkCore;
using MinhaApi.Estudantes;

namespace MinhaApi.Data;

public class AppDbContext : DbContext
{
   public DbSet<Estudante> Estudantes { get; set; }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      optionsBuilder.UseSqlite(connectionString:"Data Source=Banco.sqlite");
      optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
      optionsBuilder.EnableSensitiveDataLogging();
      
      base.OnConfiguring(optionsBuilder);
   }
} 