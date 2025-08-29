using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservationDomain.Event;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Users;
using SeatReservationDomain.Venue;

namespace SeatReservation.Infrastructure.Postgres.Seeding;

public class ReservationSeeder : ISeeder
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<ReservationSeeder> _logger;
    private readonly Random _random = new();

    // Константы для количества данных
    private const int USERS_COUNT = 500;
    private const int VENUES_COUNT = 500;
    private const int SEATS_PER_VENUE_MIN = 50;
    private const int SEATS_PER_VENUE_MAX = 500;
    private const int EVENTS_COUNT = 5000;

    public ReservationSeeder(ReservationServiceDbContext dbContext, ILogger<ReservationSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting seeding reservations data...");

        try
        {
            await SeedData();
            _logger.LogInformation("Finished seeding reservations data.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding reservations data.");
            throw;
        }
    }

    private async Task SeedData()
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            // Очистка базы данных
            await ClearDatabase();

            // Сидирование данных батчами
            await SeedUsersBatched();
            await SeedVenuesAndSeatsBatched();
            await SeedEventsBatched();
            await SeedReservationsBatched();

            await transaction.CommitAsync();
            _logger.LogInformation("Seeding completed successfully.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task ClearDatabase()
    {
        _logger.LogInformation("Clearing database...");

        // Удаляем в правильном порядке из-за внешних ключей
        _dbContext.ReservationSeats.RemoveRange(_dbContext.ReservationSeats);
        _dbContext.Reservations.RemoveRange(_dbContext.Reservations);
        _dbContext.Events.RemoveRange(_dbContext.Events);
        _dbContext.Seats.RemoveRange(_dbContext.Seats);
        _dbContext.Venues.RemoveRange(_dbContext.Venues);
        _dbContext.Set<User>().RemoveRange(_dbContext.Set<User>());

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Database cleared.");
    }

    private async Task SeedUsersBatched()
    {
        _logger.LogInformation("Seeding users in batches...");

        const int batchSize = 1000;
        var firstNames = new[]
        {
            "Александр", "Елена", "Дмитрий", "Анна", "Михаил", "Ольга", "Сергей", "Наталья", "Владимир", "Татьяна"
        };
        var lastNames = new[]
        {
            "Иванов", "Петров", "Сидоров", "Козлов", "Новиков", "Морозов", "Петухов", "Обухов", "Калинин", "Лебедев"
        };

        // Отключаем отслеживание изменений для ускорения
        _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        var users = new List<User>();
        for (var i = 0; i < USERS_COUNT; i++)
        {
            var firstName = firstNames[_random.Next(firstNames.Length)];
            var lastName = lastNames[_random.Next(lastNames.Length)];

            users.Add(new User
            {
                Id = Guid.NewGuid(),
                Details = new Details
                {
                    FIO = $"{firstName} {lastName}",
                    Description = $"Пользователь {firstName} {lastName}",
                    Socials = GenerateRandomSocials()
                }
            });

            if (users.Count < batchSize)
                continue;

            _dbContext.Set<User>().AddRange(users);
            await _dbContext.SaveChangesAsync();
            users.Clear();
        }

        if (users.Count != 0)
        {
            _dbContext.Set<User>().AddRange(users);
            await _dbContext.SaveChangesAsync();
        }

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        _logger.LogInformation("Seeded {UsersCount} users.", USERS_COUNT);
    }

    private async Task SeedVenuesAndSeatsBatched()
    {
        _logger.LogInformation("Seeding venues and seats in batches...");

        var venuePrefixes = new[]
        {
            "MSK", "SPB", "EKB", "NSK", "KZN", "NN", "CHE", "SMR", "UFA", "RND"
        };
        var venueNames = new[]
        {
            "Центральный зал", "Концертный холл", "Конференц-зал", "Театральный зал", "Большой зал", "Малый зал", "Летняя площадка", "Крытый павильон",
            "Амфитеатр", "Арена"
        };

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        // Генерируем все площадки
        var venues = new List<Venue>();
        for (var i = 0; i < VENUES_COUNT; i++)
        {
            var prefix = venuePrefixes[_random.Next(venuePrefixes.Length)];
            var name = venueNames[_random.Next(venueNames.Length)];
            var seatsLimit = _random.Next(SEATS_PER_VENUE_MIN, SEATS_PER_VENUE_MAX + 1);

            var venueResult = Venue.Create(prefix, name, seatsLimit);
            if (venueResult.IsSuccess)
            {
                venues.Add(venueResult.Value);
            }
        }

        _dbContext.Venues.AddRange(venues);
        await _dbContext.SaveChangesAsync();

        // Генерируем все места для всех площадок сразу
        var allSeats = new List<Seat>();
        const int seatBatchSize = 5000;

        foreach (var venue in venues)
        {
            var seatsCount = _random.Next(SEATS_PER_VENUE_MIN, Math.Min(venue.MaxSeatsCount, SEATS_PER_VENUE_MAX) + 1);
            var rows = Math.Max(1, seatsCount / 20);
            var seatsPerRow = Math.Max(1, seatsCount / rows);

            for (var row = 1; row <= rows; row++)
            {
                var seatsInThisRow = row == rows ? seatsCount - ((rows - 1) * seatsPerRow) : seatsPerRow;

                for (var seatNum = 1; seatNum <= seatsInThisRow; seatNum++)
                {
                    var seatResult = Seat.Create(venue.Id, row, seatNum);
                    if (!seatResult.IsSuccess)
                        continue;

                    allSeats.Add(seatResult.Value);

                    if (allSeats.Count < seatBatchSize)
                        continue;

                    _dbContext.Seats.AddRange(allSeats);
                    await _dbContext.SaveChangesAsync();
                    allSeats.Clear();
                }
            }
        }

        if (allSeats.Count != 0)
        {
            _dbContext.Seats.AddRange(allSeats);
            await _dbContext.SaveChangesAsync();
        }

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        _logger.LogInformation($"Seeded {VENUES_COUNT} venues with seats.");
    }

    private async Task SeedEventsBatched()
    {
        _logger.LogInformation("Seeding events in batches...");

        const int batchSize = 2000;
        var concertPerformers = new[]
        {
            "Би-2", "Сплин", "Мумий Тролль", "Земфира", "Ленинград", "Каста", "Noize MC", "Баста"
        };
        var conferenceTopics = new[]
        {
            "Технологии будущего", "ИИ в бизнесе", "Экология и устойчивое развитие", "Цифровая трансформация"
        };
        var conferenceSpeakers = new[]
        {
            "Иван Петров", "Мария Сидорова", "Алексей Козлов", "Екатерина Новикова"
        };
        var eventNames = new[]
        {
            "Большой концерт", "Весенний фестиваль", "Техническая конференция", "Онлайн-митап"
        };

        // Получаем ID площадок из базы
        var venueIds = await _dbContext.Venues.Select(v => new
        {
            v.Id,
            v.MaxSeatsCount
        }).ToListAsync();

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        var events = new List<Event>();
        for (var i = 0; i < EVENTS_COUNT; i++)
        {
            var venue = venueIds[_random.Next(venueIds.Count)];
            var eventType = (EventType)_random.Next(0, 3);
            var eventDate = DateTime.UtcNow.AddDays(_random.Next(1, 365));
            var startDate = eventDate.AddHours(_random.Next(-2, 3));
            var endDate = startDate.AddHours(_random.Next(1, 8));
            var capacity = _random.Next(50, venue.MaxSeatsCount);
            var description = $"Описание события {eventNames[_random.Next(eventNames.Length)]}";
            var name = eventNames[_random.Next(eventNames.Length)];

            var eventResult = eventType switch
            {
                EventType.Concert => Event.CreateConcert(
                    venue.Id, name, eventDate, startDate, endDate, capacity, description,
                    concertPerformers[_random.Next(concertPerformers.Length)]),

                EventType.Conference => Event.CreateConference(
                    venue.Id, name, eventDate, startDate, endDate, capacity, description,
                    conferenceSpeakers[_random.Next(conferenceSpeakers.Length)],
                    conferenceTopics[_random.Next(conferenceTopics.Length)]),

                EventType.Online => Event.CreateOnline(
                    venue.Id, name, eventDate, startDate, endDate, capacity, description,
                    $"https://meet.example.com/room{_random.Next(1000, 9999)}"),

                _ => throw new ArgumentOutOfRangeException()
            };

            if (!eventResult.IsSuccess)
                continue;

            events.Add(eventResult.Value);

            if (events.Count < batchSize)
                continue;

            _dbContext.Events.AddRange(events);
            await _dbContext.SaveChangesAsync();
            events.Clear();
        }

        if (events.Count != 0)
        {
            _dbContext.Events.AddRange(events);
            await _dbContext.SaveChangesAsync();
        }

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        _logger.LogInformation("Seeded {EventsCount} events.", EVENTS_COUNT);
    }

    private async Task SeedReservationsBatched()
    {
        _logger.LogInformation("Seeding reservations in batches...");

        // Получаем все данные одним запросом
        var eventVenueData = await _dbContext.Events.Select(e => new
        {
            e.Id,
            e.VenueId
        }).ToListAsync();
        var userIds = await _dbContext.Set<User>().Select(u => u.Id).ToListAsync();
        var seatsByVenue = await _dbContext.Seats
            .GroupBy(s => s.VenueId)
            .Select(g => new
            {
                VenueId = g.Key,
                SeatIds = g.Select(s => s.Id.Value).ToList()
            })
            .ToListAsync();

        var venueSeatsDict = seatsByVenue.ToDictionary(x => x.VenueId, x => x.SeatIds);

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        const int batchSize = 1000;
        var reservations = new List<Reservation>();

        foreach (var eventInfo in eventVenueData)
        {
            if (!venueSeatsDict.TryGetValue(eventInfo.VenueId, out var seatIds)) continue;

            var seatsToBook = seatIds.OrderBy(_ => _random.Next()).Take(seatIds.Count / 2).ToArray();

            for (var i = 0; i < seatsToBook.Length;)
            {
                var seatsInReservation = Math.Min(_random.Next(1, 5), seatsToBook.Length - i);
                var reservationSeats = seatsToBook.Skip(i).Take(seatsInReservation).ToList();

                var reservation = Reservation.Create(
                    eventInfo.Id, 
                    userIds[_random.Next(userIds.Count)], 
                    reservationSeats);
                
                if (reservation.IsSuccess)
                {
                    reservations.Add(reservation.Value);

                    if (reservations.Count >= batchSize)
                    {
                        _dbContext.Reservations.AddRange(reservations);
                        await _dbContext.SaveChangesAsync();
                        reservations.Clear();
                    }
                }

                i += seatsInReservation;
            }
        }

        if (reservations.Any())
        {
            _dbContext.Reservations.AddRange(reservations);
            await _dbContext.SaveChangesAsync();
        }

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        _logger.LogInformation("Completed seeding reservations.");
    }

    private List<SocialNetwork> GenerateRandomSocials()
    {
        var socialNetworks = new[]
        {
            "VK", "Telegram", "Instagram", "Facebook", "Twitter"
        };
        var count = _random.Next(0, 3);
        var socials = new List<SocialNetwork>();

        for (var i = 0; i < count; i++)
        {
            var network = socialNetworks[_random.Next(socialNetworks.Length)];
            socials.Add(new SocialNetwork
            {
                Name = network,
                Link = $"https://{network.ToLower()}.com/user{_random.Next(1000, 9999)}"
            });
        }

        return socials;
    }
}