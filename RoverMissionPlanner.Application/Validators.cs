using FluentValidation;
using RoverMissionPlanner.Application.DTOs;
using RoverMissionPlanner.Domain;

namespace RoverMissionPlanner.Application.Validators
{
    public class CreateRoverTaskValidator : AbstractValidator<CreateRoverTaskDto>
    {
        public CreateRoverTaskValidator()
        {
            RuleFor(x => x.RoverName)
                .NotEmpty()
                .WithMessage("RoverName es requerido")
                .MaximumLength(100)
                .WithMessage("RoverName no puede exceder 100 caracteres");

            RuleFor(x => x.TaskType)
                .IsInEnum()
                .WithMessage("TaskType debe ser un valor válido");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude debe estar entre -90 y 90 grados");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude debe estar entre -180 y 180 grados");

            RuleFor(x => x.StartsAt)
                .NotEmpty()
                .WithMessage("StartsAt es requerido")
                .Must(BeInFuture)
                .WithMessage("StartsAt debe ser en el futuro");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0)
                .WithMessage("DurationMinutes debe ser mayor a 0")
                .LessThanOrEqualTo(1440) // 24 horas
                .WithMessage("DurationMinutes no puede exceder 1440 minutos (24 horas)");
        }

        private bool BeInFuture(DateTime startsAt)
        {
            return startsAt > DateTime.UtcNow;
        }
    }
}