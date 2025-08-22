using FluentValidation;
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
using SeatReservationService.Application.Reservations;
using SeatReservationService.Application.Venues;
using SeatReservationService.Contract;
using EventId = SeatReservationDomain.Event.EventId;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory >();

builder.Services.AddScoped<ReservationServiceDbContext>(_ => 
    new ReservationServiceDbContext(
        builder.Configuration.GetConnectionString("ReservationServiceDb")!));
builder.Services.AddScoped<ITransactionManager, TransactionManager>();

builder.Services.AddScoped<IVenuesRepository, VenuesRepository>();
builder.Services.AddScoped<IReservationsRepository, ReservationsRepository>();
builder.Services.AddScoped<ISeatsRepository, SeatsRepository>();
builder.Services.AddScoped<IEventsRepository, EventsRepository>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<CreateVenueHandler>();
builder.Services.AddScoped<UpdateVenueNameHandler>();
builder.Services.AddScoped<UpdateVenueNameByPrefixHandler>();
builder.Services.AddScoped<UpdateVenueHandler>();
builder.Services.AddScoped<UpdateVenueSeatsHandler>();
builder.Services.AddScoped<ReserveHandler>();
builder.Services.AddScoped<CreateConcertHandler>();

builder.Services.AddScoped<IValidator<ReserveRequest>, ReserveRequestValidator>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DevQuestions v1"));  
}

app.MapControllers();

app.Run();