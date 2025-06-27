using RoverMissionPlanner.Domain;

namespace RoverMissionPlanner.Application.DTOs
{
    public class CreateRoverTaskDto
    {
        public string RoverName { get; set; } = string.Empty;
        public TaskType TaskType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartsAt { get; set; }
        public int DurationInMinutes { get; set; }
    }

    public class CreateRoverTaskDto
    {
        public Guid Id { get; set; }
        public string RoverName { get; set; } = string.Empty;
        public TaskType TaskType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartsAt { get; set; }
        public Status Status { get; set; }
        public DateTime EndsAt { get; set; }
    }

    public class RoverUtilizationDto
    {
        public string RoverId { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public double UtilizationPercentage { get; set; }
        public int TotalPlannedMinutes { get; set; }
        public int TotalAvailableMinutes { get; set; }
    }
}