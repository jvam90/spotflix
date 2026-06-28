using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Authorization;
using Spotflix.Api.Models;
using Spotflix.Api.Models.Billing;
using Spotflix.Api.Models.Catalog;
using Spotflix.Api.Models.Payments;
using Spotflix.Api.Models.Social;

namespace Spotflix.Api.Data;

/// <summary>
/// Aplica migrations, semeia papéis, usuários, planos, catálogo e dados de teste.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        await SeedRolesAsync(sp);
        await SeedAdminUserAsync(sp, db);
        await SeedTestUsersAsync(sp, db);
        await SeedPlansAsync(db);
        await SeedCatalogFromMp3sAsync(sp, db);
        await SeedCardsAsync(db);
        await SeedTransactionsAsync(db);
        await SeedSubscriptionsAsync(db);
        await SeedFavoritesAsync(db);
    }

    private static async Task SeedRolesAsync(IServiceProvider sp)
    {
        var roleManager = sp.GetRequiredService<RoleManager<ApplicationRole>>();
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole(role));
        }
    }

    private static async Task SeedAdminUserAsync(IServiceProvider sp, AppDbContext db)
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var adminEmail = config["Seed:AdminEmail"];
        var adminPassword = config["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            return;

        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrador",
            };
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }

    private static async Task SeedTestUsersAsync(IServiceProvider sp, AppDbContext db)
    {
        if (await db.Users.AnyAsync(u => u.Email != null && u.Email.StartsWith("user")))
            return;

        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var testUsers = new[]
        {
            new { Email = "user1@spotflix.com", FullName = "João Silva", Password = "Senha@123" },
            new { Email = "user2@spotflix.com", FullName = "Maria Santos", Password = "Senha@123" },
            new { Email = "user3@spotflix.com", FullName = "Pedro Oliveira", Password = "Senha@123" },
        };

        foreach (var testUser in testUsers)
        {
            var user = await userManager.FindByEmailAsync(testUser.Email);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = testUser.Email,
                    Email = testUser.Email,
                    EmailConfirmed = true,
                    FullName = testUser.FullName,
                };
                var result = await userManager.CreateAsync(user, testUser.Password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, Roles.User);
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedPlansAsync(AppDbContext db)
    {
        if (await db.Plans.AnyAsync())
            return;

        var plans = new[]
        {
            new Plan
            {
                Id = Guid.NewGuid(),
                Name = "Gratuito",
                Price = 0m,
                Period = BillingPeriod.Monthly,
                Description = "Acesso limitado com anúncios",
                IsActive = true,
            },
            new Plan
            {
                Id = Guid.NewGuid(),
                Name = "Premium",
                Price = 19.90m,
                Period = BillingPeriod.Monthly,
                Description = "Sem anúncios, qualidade alta, sem limite de streams",
                IsActive = true,
            },
            new Plan
            {
                Id = Guid.NewGuid(),
                Name = "Family",
                Price = 34.90m,
                Period = BillingPeriod.Monthly,
                Description = "Até 6 perfis, Premium completo para toda família",
                IsActive = true,
            },
        };

        await db.Plans.AddRangeAsync(plans);
        await db.SaveChangesAsync();
    }

    private static async Task SeedCatalogFromMp3sAsync(IServiceProvider sp, AppDbContext db)
    {
        if (await db.Bands.AnyAsync())
        {
            // Catálogo antigo (sem áudio) já populado — limpa e ressemeia.
            if (await db.Songs.AnyAsync(s => s.AudioData != null))
                return; // Já semeado com áudio, nada a fazer.

            db.FavoriteSongs.RemoveRange(db.FavoriteSongs);
            db.FavoriteBands.RemoveRange(db.FavoriteBands);
            db.Songs.RemoveRange(db.Songs);
            db.Albums.RemoveRange(db.Albums);
            db.Bands.RemoveRange(db.Bands);
            await db.SaveChangesAsync();
        }

        var env = sp.GetRequiredService<IWebHostEnvironment>();
        var mp3Dir = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "..", "Mp3"));
        if (!Directory.Exists(mp3Dir)) return;

        var files = Directory.GetFiles(mp3Dir, "*.mp3");
        if (files.Length == 0) return;

        var bandDict  = new Dictionary<string, Band>(StringComparer.OrdinalIgnoreCase);
        var albumDict = new Dictionary<(string, string), Album>();
        var songs     = new List<Song>();
        int fallbackTrack = 1;

        foreach (var filePath in files)
        {
            using var tagFile = TagLib.File.Create(filePath);
            var artist    = NullIfEmpty(tagFile.Tag.FirstPerformer?.Trim()) ?? "Unknown Artist";
            var albumName = NullIfEmpty(tagFile.Tag.Album?.Trim()) ?? "Unknown Album";
            var title     = NullIfEmpty(tagFile.Tag.Title?.Trim())
                            ?? Path.GetFileNameWithoutExtension(filePath);
            var track     = (int)tagFile.Tag.Track;
            var duration  = (int)tagFile.Properties.Duration.TotalSeconds;
            var genre     = NullIfEmpty(tagFile.Tag.FirstGenre?.Trim()) ?? "Unknown";
            var year      = tagFile.Tag.Year > 0 ? (int)tagFile.Tag.Year : DateTime.UtcNow.Year;
            var bytes     = await File.ReadAllBytesAsync(filePath);

            if (!bandDict.TryGetValue(artist, out var band))
            {
                band = new Band { Id = Guid.NewGuid(), Name = artist, Genre = genre, FormedYear = year };
                bandDict[artist] = band;
            }

            var key = (artist, albumName);
            if (!albumDict.TryGetValue(key, out var alb))
            {
                alb = new Album { Id = Guid.NewGuid(), BandId = band.Id, Title = albumName, ReleaseYear = year };
                albumDict[key] = alb;
            }

            songs.Add(new Song
            {
                Id              = Guid.NewGuid(),
                AlbumId         = alb.Id,
                Title           = title,
                TrackNumber     = track > 0 ? track : fallbackTrack++,
                DurationSeconds = duration,
                AudioData       = bytes,
                ContentType     = "audio/mpeg",
            });
        }

        await db.Bands.AddRangeAsync(bandDict.Values);
        await db.Albums.AddRangeAsync(albumDict.Values);
        await db.Songs.AddRangeAsync(songs);
        await db.SaveChangesAsync();
    }

    private static string? NullIfEmpty(string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s;

    private static async Task SeedCardsAsync(AppDbContext db)
    {
        if (await db.Cards.AnyAsync())
            return;

        var users = await db.Users.Where(u => u.Email != null && u.Email.StartsWith("user")).ToListAsync();

        var cards = new List<Card>();
        foreach (var user in users)
        {
            cards.Add(new Card
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                HolderName = user.FullName ?? user.UserName ?? "Card User",
                Last4 = "1234",
                Brand = "Visa",
                Active = true,
                AvailableLimit = 5000m,
            });
            cards.Add(new Card
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                HolderName = user.FullName ?? user.UserName ?? "Card User",
                Last4 = "5678",
                Brand = "Mastercard",
                Active = true,
                AvailableLimit = 3000m,
            });
        }

        await db.Cards.AddRangeAsync(cards);
        await db.SaveChangesAsync();
    }

    private static async Task SeedTransactionsAsync(AppDbContext db)
    {
        if (await db.Transactions.AnyAsync())
            return;

        var cards = await db.Cards.ToListAsync();
        var merchants = new[] { "Spotify", "Apple Music", "YouTube Music", "Deezer", "Tidal", "Amazon Music" };
        var transactions = new List<Transaction>();
        var now = DateTime.UtcNow;

        foreach (var card in cards.Take(2))
        {
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(new Transaction
                {
                    Id = Guid.NewGuid(),
                    CardId = card.Id,
                    Merchant = merchants[i % merchants.Length],
                    Amount = decimal.Round((decimal)(new Random().NextDouble() * 100 + 10), 2),
                    OccurredAt = now.AddDays(-i),
                    Status = TransactionStatus.Authorized,
                    Violations = new List<string>(),
                });
            }
        }

        await db.Transactions.AddRangeAsync(transactions);
        await db.SaveChangesAsync();
    }

    private static async Task SeedSubscriptionsAsync(AppDbContext db)
    {
        if (await db.Subscriptions.AnyAsync())
            return;

        var users = await db.Users.Where(u => u.Email != null && u.Email.StartsWith("user")).ToListAsync();
        var plans = await db.Plans.ToListAsync();
        var premiumPlan = plans.First(p => p.Name == "Premium");
        var familyPlan = plans.First(p => p.Name == "Family");

        var subscriptions = new List<Subscription>();
        var now = DateTime.UtcNow;

        // user1 premium
        subscriptions.Add(new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = users[0].Id,
            PlanId = premiumPlan.Id,
            StartedAt = now.AddMonths(-1),
            CurrentPeriodEnd = now.AddMonths(1),
            Status = SubscriptionStatus.Active,
        });

        // user2 family
        subscriptions.Add(new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = users[1].Id,
            PlanId = familyPlan.Id,
            StartedAt = now.AddMonths(-2),
            CurrentPeriodEnd = now.AddMonths(1),
            Status = SubscriptionStatus.Active,
        });

        // user3 premium (expired)
        subscriptions.Add(new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = users[2].Id,
            PlanId = premiumPlan.Id,
            StartedAt = now.AddMonths(-1),
            CurrentPeriodEnd = now.AddDays(-1),
            CanceledAt = now.AddDays(-1),
            Status = SubscriptionStatus.Expired,
        });

        await db.Subscriptions.AddRangeAsync(subscriptions);
        await db.SaveChangesAsync();
    }

    private static async Task SeedFavoritesAsync(AppDbContext db)
    {
        if (await db.FavoriteSongs.AnyAsync() || await db.FavoriteBands.AnyAsync())
            return;

        var users = await db.Users.Where(u => u.Email != null && u.Email.StartsWith("user")).ToListAsync();
        var songs = await db.Songs.Take(10).ToListAsync();
        var bands = await db.Bands.Take(3).ToListAsync();

        if (users.Count < 3 || songs.Count < 7 || bands.Count < 3)
            return;

        var favoriteSongs = new List<FavoriteSong>();
        var favoriteBands = new List<FavoriteBand>();

        // user1 favorites
        favoriteSongs.Add(new FavoriteSong { UserId = users[0].Id, SongId = songs[0].Id });
        favoriteSongs.Add(new FavoriteSong { UserId = users[0].Id, SongId = songs[1].Id });
        favoriteSongs.Add(new FavoriteSong { UserId = users[0].Id, SongId = songs[2].Id });
        favoriteBands.Add(new FavoriteBand { UserId = users[0].Id, BandId = bands[0].Id });

        // user2 favorites
        favoriteSongs.Add(new FavoriteSong { UserId = users[1].Id, SongId = songs[3].Id });
        favoriteSongs.Add(new FavoriteSong { UserId = users[1].Id, SongId = songs[4].Id });
        favoriteBands.Add(new FavoriteBand { UserId = users[1].Id, BandId = bands[1].Id });
        favoriteBands.Add(new FavoriteBand { UserId = users[1].Id, BandId = bands[2].Id });

        // user3 favorites
        favoriteSongs.Add(new FavoriteSong { UserId = users[2].Id, SongId = songs[5].Id });
        favoriteSongs.Add(new FavoriteSong { UserId = users[2].Id, SongId = songs[6].Id });

        await db.FavoriteSongs.AddRangeAsync(favoriteSongs);
        await db.FavoriteBands.AddRangeAsync(favoriteBands);
        await db.SaveChangesAsync();
    }
}
