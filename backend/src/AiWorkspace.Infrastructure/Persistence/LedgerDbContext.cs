
using System.Globalization;
using AiWorkspace.Domain.Ledger;
using Microsoft.EntityFrameworkCore;

namespace AiWorkspace.Infrastructure.Persistence;

public sealed class LedgerDbContext : DbContext
{
    public LedgerDbContext(
        DbContextOptions<LedgerDbContext> options)
        : base(options)
    {
    }

    public DbSet<LedgerEntry> LedgerEntries =>
        Set<LedgerEntry>();

    public DbSet<LedgerTransaction> LedgerTransactions =>
        Set<LedgerTransaction>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LedgerEntry>(entity =>
        {
            entity.ToTable("LedgerEntries");

            entity.HasKey(entry => entry.Id);

            entity.Property(entry => entry.TransactionId)
                .IsRequired();

            entity.Property(entry => entry.AccountId)
                .IsRequired();

            entity.Property(entry => entry.Amount)
                .HasConversion(
                    amount => amount.ToString(
                        CultureInfo.InvariantCulture),
                    value => decimal.Parse(
                        value,
                        CultureInfo.InvariantCulture))
                .HasColumnType("TEXT")
                .IsRequired();

            entity.Property(entry => entry.Currency)
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(entry => entry.EntryType)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(entry => entry.CreatedAt)
                .IsRequired();

            entity.HasIndex(entry => entry.TransactionId);

            entity.HasIndex(entry => new
            {
                entry.AccountId,
                entry.CreatedAt
            });

            entity.HasIndex(entry => new
            {
                entry.TransactionId,
                entry.AccountId,
                entry.EntryType
            }).IsUnique();
        });

        modelBuilder.Entity<LedgerTransaction>(entity =>
        {
            entity.ToTable("LedgerTransactions");

            entity.HasKey(transaction => transaction.Id);

            entity.Property(transaction => transaction.IdempotencyKey)
                .HasMaxLength(128)
                .IsRequired();

            entity.HasIndex(transaction => transaction.IdempotencyKey)
                .IsUnique();

            entity.Property(transaction => transaction.SourceAccountId)
                .IsRequired();

            entity.Property(transaction => transaction.DestinationAccountId)
                .IsRequired();

            entity.Property(transaction => transaction.Amount)
                .HasConversion(
                    amount => amount.ToString(
                        CultureInfo.InvariantCulture),
                    value => decimal.Parse(
                        value,
                        CultureInfo.InvariantCulture))
                .HasColumnType("TEXT")
                .IsRequired();

            entity.Property(transaction => transaction.Currency)
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(transaction => transaction.CreatedAt)
                .IsRequired();
        });
    }
}
