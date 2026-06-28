using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Models;
using Spotflix.Api.Models.Billing;
using Spotflix.Api.Models.Catalog;
using Spotflix.Api.Models.Payments;
using Spotflix.Api.Models.Social;

namespace Spotflix.Api.Data;

/// <summary>
/// DbContext da aplicação, integrando o ASP.NET Core Identity (usuário/role com chave Guid).
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Band> Bands => Set<Band>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Song> Songs => Set<Song>();

    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<FavoriteSong> FavoriteSongs => Set<FavoriteSong>();
    public DbSet<FavoriteBand> FavoriteBands => Set<FavoriteBand>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Essencial: configura o schema do Identity (AspNetUsers, AspNetRoles, etc.)
        base.OnModelCreating(builder);

        // Extensão usada pelos índices trigram (busca por substring performática).
        builder.HasPostgresExtension("pg_trgm");

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(256);
            entity.HasIndex(rt => rt.Token).IsUnique();

            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Propriedades calculadas não são mapeadas para colunas.
            entity.Ignore(rt => rt.IsExpired);
            entity.Ignore(rt => rt.IsActive);
        });

        builder.Entity<Band>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Genre).HasMaxLength(100);
            // Índice GIN trigram: acelera ILIKE '%termo%' por nome (requisito de performance).
            entity.HasIndex(b => b.Name).HasMethod("gin").HasOperators("gin_trgm_ops");
        });

        builder.Entity<Album>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Title).IsRequired().HasMaxLength(250);

            entity.HasOne(a => a.Band)
                  .WithMany(b => b.Albums)
                  .HasForeignKey(a => a.BandId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => a.BandId);
        });

        builder.Entity<Song>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title).IsRequired().HasMaxLength(300);
            entity.HasIndex(s => s.Title).HasMethod("gin").HasOperators("gin_trgm_ops");

            entity.HasOne(s => s.Album)
                  .WithMany(a => a.Songs)
                  .HasForeignKey(s => s.AlbumId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => s.AlbumId);
        });

        builder.Entity<Plan>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(120);
            entity.Property(p => p.Price).HasPrecision(10, 2);
        });

        builder.Entity<Subscription>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.HasOne(s => s.User)
                  .WithMany()
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Plan)
                  .WithMany(p => p.Subscriptions)
                  .HasForeignKey(s => s.PlanId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => s.UserId);
        });

        builder.Entity<Card>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.HolderName).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Last4).IsRequired().HasMaxLength(4);
            entity.Property(c => c.Brand).HasMaxLength(40);
            entity.Property(c => c.AvailableLimit).HasPrecision(12, 2);

            entity.HasOne(c => c.User)
                  .WithMany()
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => c.UserId);
        });

        builder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Merchant).IsRequired().HasMaxLength(150);
            entity.Property(t => t.Amount).HasPrecision(12, 2);

            entity.HasOne(t => t.Card)
                  .WithMany(c => c.Transactions)
                  .HasForeignKey(t => t.CardId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Consulta de histórico recente por cartão/tempo (regras de frequência).
            entity.HasIndex(t => new { t.CardId, t.OccurredAt });
        });

        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Recipient).IsRequired().HasMaxLength(200);
            entity.Property(n => n.Channel).IsRequired().HasMaxLength(40);
            entity.Property(n => n.Message).IsRequired().HasMaxLength(500);

            entity.HasOne(n => n.Transaction)
                  .WithMany()
                  .HasForeignKey(n => n.TransactionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FavoriteSong>(entity =>
        {
            entity.HasKey(f => new { f.UserId, f.SongId });

            entity.HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(f => f.Song).WithMany().HasForeignKey(f => f.SongId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FavoriteBand>(entity =>
        {
            entity.HasKey(f => new { f.UserId, f.BandId });

            entity.HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(f => f.Band).WithMany().HasForeignKey(f => f.BandId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
