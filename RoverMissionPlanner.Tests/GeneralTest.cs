using Xunit;
using FluentAssertions;
using RoverMissionPlanner.Domain;


namespace RoverMissionPlanner.Tests;

public class GeneralTests
{
    [Fact]
    public void RoverTask_Creation_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var task = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Mars-Rover-01",
            TaskType = TaskType.Drill,
            Latitude = -23.5505,
            Longitude = -46.6333,
            StartsAt = DateTime.UtcNow,
            DurationMinutes = 120,
            Status = Status.Planned
        };

        // Assert
        task.Should().NotBeNull();
        task.RoverName.Should().Be("Mars-Rover-01");
        task.TaskType.Should().Be(TaskType.Drill);
        task.Status.Should().Be(Status.Planned);
        task.DurationMinutes.Should().Be(120);
        task.EndsAt.Should().Be(task.StartsAt.AddMinutes(120));
    }

    [Theory]
    [InlineData(TaskType.Drill)]
    [InlineData(TaskType.Sample)]
    [InlineData(TaskType.Photo)]
    [InlineData(TaskType.Charge)]
    public void TaskType_AllValues_ShouldBeValid(TaskType taskType)
    {
        // Arrange & Act
        var task = new RoverTask { TaskType = taskType };

        // Assert
        task.TaskType.Should().Be(taskType);
        Enum.IsDefined(typeof(TaskType), taskType).Should().BeTrue();
    }

    [Theory]
    [InlineData(Status.Planned)]
    [InlineData(Status.InProgress)]
    [InlineData(Status.Completed)]
    [InlineData(Status.Aborted)]
    public void Status_AllValues_ShouldBeValid(Status status)
    {
        // Arrange & Act
        var task = new RoverTask { Status = status };

        // Assert
        task.Status.Should().Be(status);
        Enum.IsDefined(typeof(Status), status).Should().BeTrue();
    }

    [Fact]
    public void RoverTask_TaskOverlap_BasicLogic_ShouldWork()
    {
        // Arrange
        var baseTime = DateTime.UtcNow;

        var task1 = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover-01",
            StartsAt = baseTime,
            DurationMinutes = 60
        };

        var task2 = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover-01",
            StartsAt = baseTime.AddMinutes(30), // Overlap with task1
            DurationMinutes = 60
        };

        var task3 = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover-01",
            StartsAt = baseTime.AddMinutes(120), // No overlap
            DurationMinutes = 60
        };

        // Act & Assert
        var task1End = task1.EndsAt;
        var task2Start = task2.StartsAt;
        var task3Start = task3.StartsAt;

        // Task1 and Task2 should overlap
        (task2Start < task1End).Should().BeTrue("Task2 starts before Task1 ends");

        // Task1 and Task3 should not overlap
        (task3Start >= task1End).Should().BeTrue("Task3 starts after Task1 ends");
    }

    [Fact]
    public void RoverTask_EndsAt_ShouldBeCalculatedCorrectly()
    {
        // Arrange
        var startsAt = DateTime.UtcNow;
        var durationMinutes = 90;

        var task = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Test-Rover",
            StartsAt = startsAt,
            DurationMinutes = durationMinutes
        };

        // Act & Assert
        task.EndsAt.Should().Be(startsAt.AddMinutes(durationMinutes));
    }

    [Fact]
    public void RoverTask_OverlapDetection_ShouldWorkCorrectly()
    {
        // Arrange
        var baseTime = DateTime.UtcNow;

        var existingTask = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover-01",
            StartsAt = baseTime,
            DurationMinutes = 60 // Ends at baseTime + 60 minutes
        };

        // Test cases for overlap detection
        var testCases = new[]
        {
            // Overlapping cases
            new { StartsAt = baseTime.AddMinutes(-30), Duration = 60, ShouldOverlap = true, Description = "Task starts before and ends during existing task" },
            new { StartsAt = baseTime.AddMinutes(30), Duration = 60, ShouldOverlap = true, Description = "Task starts during existing task" },
            new { StartsAt = baseTime.AddMinutes(-30), Duration = 120, ShouldOverlap = true, Description = "Task completely contains existing task" },
            new { StartsAt = baseTime.AddMinutes(10), Duration = 30, ShouldOverlap = true, Description = "Task is completely contained within existing task" },
            
            // Non-overlapping cases
            new { StartsAt = baseTime.AddMinutes(-90), Duration = 30, ShouldOverlap = false, Description = "Task ends before existing task starts" },
            new { StartsAt = baseTime.AddMinutes(60), Duration = 30, ShouldOverlap = false, Description = "Task starts when existing task ends" },
            new { StartsAt = baseTime.AddMinutes(90), Duration = 30, ShouldOverlap = false, Description = "Task starts after existing task ends" }
        };

        foreach (var testCase in testCases)
        {
            // Arrange
            var newTask = new RoverTask
            {
                Id = Guid.NewGuid(),
                RoverName = "Rover-01",
                StartsAt = testCase.StartsAt,
                DurationMinutes = testCase.Duration
            };

            // Act - Check if tasks overlap using the same logic as in the repository
            var hasOverlap = newTask.StartsAt < existingTask.EndsAt && newTask.EndsAt > existingTask.StartsAt;

            // Assert
            hasOverlap.Should().Be(testCase.ShouldOverlap, testCase.Description);
        }
    }
}