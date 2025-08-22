using FluentValidation;
using SeatReservationService.Contract;

namespace SeatReservationService.Application.Reservations;

public class ReserveRequestValidator: AbstractValidator<ReserveRequest>
{
    public ReserveRequestValidator()
    {
        this.RuleFor(r => r.Seats)
            .NotNull().WithMessage("No seats found")
            .NotEmpty().WithMessage("No seats found");
    }
}