using AiWorkspace.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AiWorkspace.Infrastructure.Persistence;

public sealed class AuthDbContext
    : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(
        DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .ToTable("Users");
    }
}