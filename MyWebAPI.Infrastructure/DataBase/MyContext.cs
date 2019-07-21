using Microsoft.EntityFrameworkCore;
using MyWebAPI.Core.Entities;
using MyWebAPI.Infrastructure.DataBase.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebAPI.Infrastructure.DataBase
{
    public class MyContext:DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PostConfiguration());
        }

        public DbSet<Post> Posts { get; set; }

    }
}
