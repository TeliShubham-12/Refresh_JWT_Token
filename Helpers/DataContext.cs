namespace WebApi.Helpers;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using WebApi.Entities;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DataContext(DbContextOptions options)
        : base(options)
    {
        Users = Set<User>();
    }
}
