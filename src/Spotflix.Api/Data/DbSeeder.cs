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
        await SeedCatalogAsync(db);
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

    private static async Task SeedCatalogAsync(AppDbContext db)
    {
        if (await db.Bands.AnyAsync())
            return;

        var bands = new[]
        {
            new Band { Id = Guid.NewGuid(), Name = "The Beatles", Genre = "Rock", FormedYear = 1960 },
            new Band { Id = Guid.NewGuid(), Name = "Pink Floyd", Genre = "Rock Progressivo", FormedYear = 1965 },
            new Band { Id = Guid.NewGuid(), Name = "Queen", Genre = "Rock", FormedYear = 1970 },
            new Band { Id = Guid.NewGuid(), Name = "David Bowie", Genre = "Rock", FormedYear = 1967 },
            new Band { Id = Guid.NewGuid(), Name = "Led Zeppelin", Genre = "Hard Rock", FormedYear = 1968 },
            new Band { Id = Guid.NewGuid(), Name = "The Rolling Stones", Genre = "Rock", FormedYear = 1962 },
        };

        await db.Bands.AddRangeAsync(bands);
        await db.SaveChangesAsync();

        var bandsFromDb = await db.Bands.ToListAsync();

        var beatles = bandsFromDb.First(b => b.Name == "The Beatles");
        var abeyRoad = new Album { Id = Guid.NewGuid(), BandId = beatles.Id, Title = "Abbey Road", ReleaseYear = 1969 };
        var whiteSongs = new Album { Id = Guid.NewGuid(), BandId = beatles.Id, Title = "The White Album", ReleaseYear = 1968 };

        var pinkFloyd = bandsFromDb.First(b => b.Name == "Pink Floyd");
        var darkSide = new Album { Id = Guid.NewGuid(), BandId = pinkFloyd.Id, Title = "The Dark Side of the Moon", ReleaseYear = 1973 };
        var wish = new Album { Id = Guid.NewGuid(), BandId = pinkFloyd.Id, Title = "Wish You Were Here", ReleaseYear = 1975 };

        var queen = bandsFromDb.First(b => b.Name == "Queen");
        var bohemian = new Album { Id = Guid.NewGuid(), BandId = queen.Id, Title = "A Night at the Opera", ReleaseYear = 1975 };
        var news = new Album { Id = Guid.NewGuid(), BandId = queen.Id, Title = "News of the World", ReleaseYear = 1977 };

        var bowie = bandsFromDb.First(b => b.Name == "David Bowie");
        var ziggy = new Album { Id = Guid.NewGuid(), BandId = bowie.Id, Title = "The Rise and Fall of Ziggy Stardust", ReleaseYear = 1972 };

        var zeppelin = bandsFromDb.First(b => b.Name == "Led Zeppelin");
        var iv = new Album { Id = Guid.NewGuid(), BandId = zeppelin.Id, Title = "Led Zeppelin IV", ReleaseYear = 1971 };

        var stones = bandsFromDb.First(b => b.Name == "The Rolling Stones");
        var satisfaction = new Album { Id = Guid.NewGuid(), BandId = stones.Id, Title = "Satisfaction", ReleaseYear = 1965 };

        var albums = new[] { abeyRoad, whiteSongs, darkSide, wish, bohemian, news, ziggy, iv, satisfaction };
        await db.Albums.AddRangeAsync(albums);
        await db.SaveChangesAsync();

        var albumsFromDb = await db.Albums.ToListAsync();

        var songs = new List<Song>();

        // Abbey Road
        songs.AddRange(new[]
        {
            new Song { Id = Guid.NewGuid(), AlbumId = abeyRoad.Id, Title = "Come Together", TrackNumber = 1, DurationSeconds = 259 },
            new Song { Id = Guid.NewGuid(), AlbumId = abeyRoad.Id, Title = "Something", TrackNumber = 2, DurationSeconds = 183 },
            new Song { Id = Guid.NewGuid(), AlbumId = abeyRoad.Id, Title = "Maxwell's Silver Hammer", TrackNumber = 3, DurationSeconds = 207 },
            new Song { Id = Guid.NewGuid(), AlbumId = abeyRoad.Id, Title = "Oh! Darling", TrackNumber = 4, DurationSeconds = 210 },
        });

        // The White Album
        songs.AddRange(new[]
        {
            new Song { Id = Guid.NewGuid(), AlbumId = whiteSongs.Id, Title = "Back in the U.S.S.R.", TrackNumber = 1, DurationSeconds = 230 },
            new Song { Id = Guid.NewGuid(), AlbumId = whiteSongs.Id, Title = "Dear Prudence", TrackNumber = 2, DurationSeconds = 241 },
            new Song { Id = Guid.NewGuid(), AlbumId = whiteSongs.Id, Title = "Glass Onion", TrackNumber = 3, DurationSeconds = 246 },
            new Song { Id = Guid.NewGuid(), AlbumId = whiteSongs.Id, Title = "Ob-La-Di, Ob-La-Da", TrackNumber = 4, DurationSeconds = 171 },
        });

        // The Dark Side of the Moon
        songs.AddRange(new[]
        {
            new Song { Id = Guid.NewGuid(), AlbumId = darkSide.Id, Title = "Speak to Me", TrackNumber = 1, DurationSeconds = 90 },
            new Song { Id = Guid.NewGuid(), AlbumId = darkSide.Id, Title = "Breathe", TrackNumber = 2, DurationSeconds = 163 },
            new Song { Id = Guid.NewGuid(), AlbumId = darkSide.Id, Title = "On the Run", TrackNumber = 3, DurationSeconds = 345 },
            new Song { Id = Guid.NewGuid(), AlbumId = darkSide.Id, Title = "Time", TrackNumber = 4, DurationSeconds = 408 },
        });

        // Wish You Were Here
        songs.AddRange(new[]
        {
            new Song { Id = Guid.NewGuid(), AlbumId = wish.Id, Title = "Shine On You Crazy Diamond", TrackNumber = 1, DurationSeconds = 745 },
            new Song { Id = Guid.NewGuid(), AlbumId = wish.Id, Title = "Welcome to the Machine", TrackNumber = 2, DurationSeconds = 360 },
            new Song { Id = Guid.NewGuid(), AlbumId = wish.Id, Title = "Have a Cigar", TrackNumber = 3, DurationSeconds = 330 },
            new Song { Id = Guid.NewGuid(), AlbumId = wish.Id, Title = "Wish You Were Here", TrackNumber = 4, DurationSeconds = 295 },
        });

        // A Night at the Opera
        songs.AddRange(new[]
        {
            new Song { Id = Guid.NewGuid(), AlbumId = bohemian.Id, Title = "Death on Two Legs", TrackNumber = 1, DurationSeconds = 356 },
            new Song { Id = Guid.NewGuid(), AlbumId = bohemian.Id, Title = "Lazing on a Sunday Afternoon", TrackNumber = 2, DurationSeconds = 282 },
            new Song { Id = Guid.NewGuid(), AlbumId = bohemian.Id, Title = "I'm in Love with My Car", TrackNumber = 3, DurationSeconds = 199 },
            new Song { Id = Guid.NewGuid(), AlbumId = bohemian.Id, Title = "Bohemian Rhapsody", TrackNumber = 5, DurationSeconds = 354 },
        });

        // News of the World
        songs.AddRange(new[]
        {
            new Song { Id = Guid.NewGuid(), AlbumId = news.Id, Title = "We Will Rock You", TrackNumber = 1, DurationSeconds = 125 },
            new Song { Id = Guid.NewGuid(), AlbumId = news.Id, Title = "Another One Bites the Dust", TrackNumber = 2, DurationSeconds = 215 },
            new Song { Id = Guid.NewGuid(), AlbumId = news.Id, Title = "Sheer Heart Attack", TrackNumber = 3, DurationSeconds = 199 },
            new Song { Id = Guid.NewGuid(), AlbumId = news.Id, Title = "All Dead, All Dead", TrackNumber = 4, DurationSeconds = 227 },
        });

        // The Rise and Fall of Ziggy Stardust
        songs.AddRange(new[]
        {
            new Song { Id = Guid.NewGuid(), AlbumId = ziggy.Id, Title = "Five Years", TrackNumber = 1, DurationSeconds = 300 },
            new Song { Id = Guid.NewGuid(), AlbumId = ziggy.Id, Title = "Soul Love", TrackNumber = 2, DurationSeconds = 310 },
            new Song { Id = Guid.NewGuid(), AlbumId = ziggy.Id, Title = "Moonage Daydream", TrackNumber = 3, DurationSeconds = 215 },
            new Song { Id = Guid.NewGuid(), AlbumId = ziggy.Id, Title = "Starman", TrackNumber = 4, DurationSeconds = 260 },
        });

        // Led Zeppelin IV
        songs.AddRange(new[]
        {
            new Song { Id = Guid.NewGuid(), AlbumId = iv.Id, Title = "Black Dog", TrackNumber = 1, DurationSeconds = 296 },
            new Song { Id = Guid.NewGuid(), AlbumId = iv.Id, Title = "Rock and Roll", TrackNumber = 2, DurationSeconds = 236 },
            new Song { Id = Guid.NewGuid(), AlbumId = iv.Id, Title = "The Battle of Evermore", TrackNumber = 3, DurationSeconds = 367 },
            new Song { Id = Guid.NewGuid(), AlbumId = iv.Id, Title = "Stairway to Heaven", TrackNumber = 4, DurationSeconds = 482 },
        });

        // Satisfaction
        songs.AddRange(new[]
        {
            new Song { Id = Guid.NewGuid(), AlbumId = satisfaction.Id, Title = "Tell Me", TrackNumber = 1, DurationSeconds = 219 },
            new Song { Id = Guid.NewGuid(), AlbumId = satisfaction.Id, Title = "(I Can't Get No) Satisfaction", TrackNumber = 2, DurationSeconds = 264 },
            new Song { Id = Guid.NewGuid(), AlbumId = satisfaction.Id, Title = "Grown Up Wrong", TrackNumber = 3, DurationSeconds = 172 },
            new Song { Id = Guid.NewGuid(), AlbumId = satisfaction.Id, Title = "Down Home Girl", TrackNumber = 4, DurationSeconds = 223 },
        });

        await db.Songs.AddRangeAsync(songs);
        await db.SaveChangesAsync();
    }

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
