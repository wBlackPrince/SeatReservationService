using Microsoft.EntityFrameworkCore;
using SeatReservation.Infrastructure.Postgres;
using SeatReservation.Infrastructure.Postgres.Database;
using SeatReservation.Infrastructure.Postgres.Repositories;
using SeatReservationDomain;
using SeatReservationDomain.Event;
using SeatReservationDomain.Venue;
using SeatReservationService;
using SeatReservationService.Application;
using SeatReservationService.Application.Database;
using SeatReservationService.Application.Venues;
using EventId = SeatReservationDomain.Event.EventId;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory >();

builder.Services.AddScoped<ReservationServiceDbContext>(_ => 
    new ReservationServiceDbContext(
        builder.Configuration.GetConnectionString("ReservationServiceDb")!));
builder.Services.AddScoped<IReservationServiceDbContext, ReservationServiceDbContext>(_ => 
    new ReservationServiceDbContext(
        builder.Configuration.GetConnectionString("ReservationServiceDb")!));

//builder.Services.AddScoped<IVenuesRepository, NpgsqlVenuesRepository>();
builder.Services.AddScoped<IVenuesRepository, EfCoreVenuesRepository>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<CreateVenueHandler>();
builder.Services.AddScoped<UpdateVenueNameHandler>();
builder.Services.AddScoped<UpdateVenueNameByPrefixHandler>();
builder.Services.AddScoped<UpdateVenueHandler>();
builder.Services.AddScoped<UpdateVenueSeatsHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DevQuestions v1"));  
}

app.MapControllers();

app.Run();