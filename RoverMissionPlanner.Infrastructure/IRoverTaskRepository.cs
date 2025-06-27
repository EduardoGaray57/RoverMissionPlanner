namespace RoverMissionPlanner.Domain.Interfaces
{
    public interface IRoverTaskRepository
    {
        Task<IEnumerable<RoverTask>> GetTasksByRoverAndDateAsync(string roverId, DateOnly date);
        Task<RoverTask> AddTaskAsync(RoverTask task);
        Task<RoverTask?> GetTaskByIdAsync(Guid taskId);
        Task<bool> HasOverlappingTasksAsync(string roverId, DateTime endsAt, Guid? excludeTaskId = null);
    }
}