using Microsoft.Extensions.Logging;

namespace SeatReservation.Infrastructure.Postgres.Seeding;

public class ReservationSeeder: ISeeder
{
    
    public ReservationSeeder(
        ReservationServiceDbContext dbContext,
        ILogger<ReservationSeeder> logger)
    {
        
    }
    
    public Task SeedAsync()
    {
        
    }
}