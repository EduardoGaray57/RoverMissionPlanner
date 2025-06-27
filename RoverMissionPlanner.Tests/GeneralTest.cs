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
            StartAt = DateTime.UtcNow,
            DurationMinutes = 120,
            Status = Status.Planned
        };

        // Assert
        task.Should().NotBeNull();
        task.RoverName.Should().Be("Mars-Rover-01");
        task.TaskType.Should().Be(TaskType.Drill);
        task.Status.Should().Be(Status.Planned);
        task.DurationMinutes.Should().Be(120);
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
            StartAt = baseTime,
            DurationMinutes = 60
        };

        var task2 = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover-01",
            StartAt = baseTime.AddMinutes(30), // Overlap with task1
            DurationMinutes = 60
        };

        var task3 = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover-01",
            StartAt = baseTime.AddMinutes(120), // No overlap
            DurationMinutes = 60
        };

        // Act & Assert
        var task1End = task1.StartAt.AddMinutes(task1.DurationMinutes);
        var task2Start = task2.StartAt;
        var task3Start = task3.StartAt;

        // Task1 and Task2 should overlap
        (task2Start < task1End).Should().BeTrue("Task2 starts before Task1 ends");
        
        // Task1 and Task3 should not overlap
        (task3Start >= task1End).Should().BeTrue("Task3 starts after Task1 ends");
    }
}