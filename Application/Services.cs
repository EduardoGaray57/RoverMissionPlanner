using RoverMissionPlanner.Application.DTOs;
using RoverMissionPlanner.Domain;
using RoverMissionPlanner.Domain.Interfaces;

namespace RoverMissionPlanner.Application.Services
{
    public interface IRoverTaskService
    {
        Task<RoverTaskDto> CreateTaskAsync(string roverId, CreateRoverTaskDto createTaskDto);
        Task<IEnumerable<RoverTaskDto>> GetTasksByDateAsync(string roverId, DateOnly date);
        Task<RoverUtilizationDto> GetUtilizationAsync(string roverId, DateOnly date);
    }

    public class RoverTaskService : IRoverTaskService
    {
        private readonly IRoverTaskRepository _repository;

        public RoverTaskService(IRoverTaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<RoverTaskDto> CreateTaskAsync(string roverId, CreateRoverTaskDto createTaskDto)
        {
            var task = new RoverTask
            {
                Id = Guid.NewGuid(),
                RoverName = roverId,
                TaskType = createTaskDto.TaskType,
                Latitude = createTaskDto.Latitude,
                Longitude = createTaskDto.Longitude,
                StartsAt = createTaskDto.StartsAt,
                DurationMinutes = createTaskDto.DurationMinutes,
                Status = Status.Planned
            };

            // Verificar solapamiento
            var hasOverlap = await _repository.HasOverlappingTasksAsync(
                roverId, 
                task.StartsAt, 
                task.EndsAt);

            if (hasOverlap)
            {
                throw new InvalidOperationException("La tarea se superpone con otra tarea existente del mismo rover");
            }

            var createdTask = await _repository.AddTaskAsync(task);

            return MapToDto(createdTask);
        }

        public async Task<IEnumerable<RoverTaskDto>> GetTasksByDateAsync(string roverId, DateOnly date)
        {
            var tasks = await _repository.GetTasksByRoverAndDateAsync(roverId, date);
            return tasks.OrderBy(t => t.StartsAt).Select(MapToDto);
        }

        public async Task<RoverUtilizationDto> GetUtilizationAsync(string roverId, DateOnly date)
        {
            var tasks = await _repository.GetTasksByRoverAndDateAsync(roverId, date);
            
            var totalPlannedMinutes = tasks.Sum(t => t.DurationMinutes);
            var totalAvailableMinutes = 24 * 60; // 1440 minutos en un día
            
            var utilizationPercentage = totalAvailableMinutes > 0 
                ? (double)totalPlannedMinutes / totalAvailableMinutes * 100 
                : 0;

            return new RoverUtilizationDto
            {
                RoverId = roverId,
                Date = date,
                UtilizationPercentage = Math.Round(utilizationPercentage, 2),
                TotalPlannedMinutes = totalPlannedMinutes,
                TotalAvailableMinutes = totalAvailableMinutes
            };
        }

        private static RoverTaskDto MapToDto(RoverTask task)
        {
            return new RoverTaskDto
            {
                Id = task.Id,
                RoverName = task.RoverName,
                TaskType = task.TaskType,
                Latitude = task.Latitude,
                Longitude = task.Longitude,
                StartsAt = task.StartsAt,
                DurationMinutes = task.DurationMinutes,
                Status = task.Status,
                EndsAt = task.EndsAt
            };
        }
    }
}