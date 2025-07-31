using Microsoft.EntityFrameworkCore;
using SeatReservation.Infrastructure.Postgres;
using SeatReservationDomain;
using SeatReservationDomain.Event;
using SeatReservationDomain.Venue;
using EventId = SeatReservationDomain.Event.EventId;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddScoped<ReservationServiceDbContext>(_ => 
    new ReservationServiceDbContext(builder.Configuration.GetConnectionString("ReservationServiceDb")!));
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DevQuestions v1"));  
}

app.MapPost("/users", (ReservationServiceDbContext context) =>
{
    var socialTelegram = new SocialNetwork()
    {
        Name = "telegram",
        Link = "telegram.org"
    };
    
    var socialVk = new SocialNetwork()
    {
        Name = "Vk",
        Link = "vk.com"
    };

    var details = new Details()
    {
        Description = "Hello, i'm a developer !",
        FIO = "Nikitin Eduard",
        Socials = [socialVk, socialTelegram]
    };

    context.Add(new User(details));
    context.SaveChanges();
});


app.MapGet("/users", (ReservationServiceDbContext context) =>
{
    return context.Users.Where(u => u.Details.Socials.Any(s => s.Link == "vk.com")).ToListAsync();
});


app.MapPost("/events", (ReservationServiceDbContext context) =>
{
    
    context.Add( new Event(
        new EventId(Guid.NewGuid()),
        new EventDetails(10, "summit"),
        new VenueId(Guid.Parse("d4e02941-b650-479a-ad58-7c42af3cecab")),
        "Summit",
        DateTime.Today,
        new ConferenceInfo("Eduard", "Microservices")
    ));
    
    context.Add( new Event(
        new EventId(Guid.NewGuid()),
        new EventDetails(15, "meeting"),
        new VenueId(Guid.Parse("d4e02941-b650-479a-ad58-7c42af3cecab")),
        "Meeting",
        DateTime.Today,
        new ConferenceInfo("Eduard", "Courtinus")
    ));
    
    
    
    context.SaveChanges();
});


app.MapGet("/events", (ReservationServiceDbContext context) =>
{
    return context.Events.ToListAsync();
});

app.Run();