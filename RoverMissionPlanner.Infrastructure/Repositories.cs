using RoverMissionPlanner.Domain;
using RoverMissionPlanner.Domain.Interfaces;
using System.Collections.Concurrent;

namespace RoverMissionPlanner.Infrastructure.Repositories
{
    public class InMemoryRoverTaskRepository : IRoverTaskRepository
    {
        private readonly ConcurrentDictionary<Guid, RoverTask> _tasks = new();

        public Task<RoverTask> AddTaskAsync(RoverTask task)
        {
            _tasks.TryAdd(task.Id, task);
            return Task.FromResult(task);
        }

        public Task<RoverTask?> GetTaskByIdAsync(Guid taskId)
        {
            _tasks.TryGetValue(taskId, out var task);
            return Task.FromResult(task);
        }

        public Task<IEnumerable<RoverTask>> GetTasksByRoverAndDateAsync(string roverId, DateOnly date)
        {
            var startOfDay = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var endOfDay = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            var tasks = _tasks.Values
                .Where(t => t.RoverName.Equals(roverId, StringComparison.OrdinalIgnoreCase))
                .Where(t => t.StartsAt.Date == date.ToDateTime(TimeOnly.MinValue).Date)
                .AsEnumerable();

            return Task.FromResult(tasks);
        }

        public Task<bool> HasOverlappingTasksAsync(string roverId, DateTime startsAt, DateTime endsAt, Guid? excludeTaskId = null)
        {
            var hasOverlap = _tasks.Values
                .Where(t => t.RoverName.Equals(roverId, StringComparison.OrdinalIgnoreCase))
                .Where(t => excludeTaskId == null || t.Id != excludeTaskId)
                .Any(t => startsAt < t.EndsAt && endsAt > t.StartsAt);

            return Task.FromResult(hasOverlap);
        }
    }
}