using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoverMissionPlanner.Application.DTOs;
using RoverMissionPlanner.Application.Services;
using RoverMissionPlanner.Domain;
using RoverMissionPlanner.Domain.Interfaces;
using RoverMissionPlanner.Infrastructure.Repositories;

namespace RoverMissionPlanner.Tests
{
    [TestClass]
    public class RoverTaskOverlapTests
    {
        private IRoverTaskService _roverTaskService;
        private IRoverTaskRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryRoverTaskRepository();
            _roverTaskService = new RoverTaskService(_repository);
        }

        [TestMethod]
        public void RoverTask_OverlapsWith_ShouldReturnTrue_WhenTasksOverlap()
        {
            // Arrange
            var task1 = new RoverTask
            {
                Id = Guid.NewGuid(),
                RoverName = "Rover1",
                StartsAt = DateTime.UtcNow.AddHours(1),
                DurationMinutes = 60
            };

            var task2 = new RoverTask
            {
                Id = Guid.NewGuid(),
                RoverName = "Rover1",
                StartsAt = DateTime.UtcNow.AddMinutes(30),
                DurationMinutes = 60
            };

            // Act
            var overlaps = task1.OverlapsWith(task2);

            // Assert
            Assert.IsTrue(overlaps);
        }

        [TestMethod]
        public void RoverTask_OverlapsWith_ShouldReturnFalse_WhenTasksDontOverlap()
        {
            // Arrange
            var task1 = new RoverTask
            {
                Id = Guid.NewGuid(),
                RoverName = "Rover1",
                StartsAt = DateTime.UtcNow.AddHours(1),
                DurationMinutes = 60
            };

            var task2 = new RoverTask
            {
                Id = Guid.NewGuid(),
                RoverName = "Rover1",
                StartsAt = DateTime.UtcNow.AddHours(3),
                DurationMinutes = 60
            };

            // Act
            var overlaps = task1.OverlapsWith(task2);

            // Assert
            Assert.IsFalse(overlaps);
        }

        [TestMethod]
        public void RoverTask_OverlapsWith_ShouldReturnFalse_WhenDifferentRovers()
        {
            // Arrange
            var task1 = new RoverTask
            {
                Id = Guid.NewGuid(),
                RoverName = "Rover1",
                StartsAt = DateTime.UtcNow.AddHours(1),
                DurationMinutes = 60
            };

            var task2 = new RoverTask
            {
                Id = Guid.NewGuid(),
                RoverName = "Rover2",
                StartsAt = DateTime.UtcNow.AddMinutes(30),
                DurationMinutes = 60
            };

            // Act
            var overlaps = task1.OverlapsWith(task2);

            // Assert
            Assert.IsFalse(overlaps);
        }

        [TestMethod]
        public async Task CreateTaskAsync_ShouldThrowException_WhenTasksOverlap()
        {
            // Arrange
            var rover1 = "Rover1";
            var startTime = DateTime.UtcNow.AddHours(1);

            var firstTask = new CreateRoverTaskDto
            {
                RoverName = rover1,
                TaskType = TaskType.Drill,
                Latitude = -25.0,
                Longitude = -70.0,
                StartsAt = startTime,
                DurationMinutes = 60
            };

            var secondTask = new CreateRoverTaskDto
            {
                RoverName = rover1,
                TaskType = TaskType.Sample,
                Latitude = -25.1,
                Longitude = -70.1,
                StartsAt = startTime.AddMinutes(30), // Overlaps with first task
                DurationMinutes = 60
            };

            // Act
            await _roverTaskService.CreateTaskAsync(rover1, firstTask);

            // Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _roverTaskService.CreateTaskAsync(rover1, secondTask));
        }

        [TestMethod]
        public async Task CreateTaskAsync_ShouldSucceed_WhenNoOverlap()
        {
            // Arrange
            var rover1 = "Rover1";
            var startTime = DateTime.UtcNow.AddHours(1);

            var firstTask = new CreateRoverTaskDto
            {
                RoverName = rover1,
                TaskType = TaskType.Drill,
                Latitude = -25.0,
                Longitude = -70.0,
                StartsAt = startTime,
                DurationMinutes = 60
            };

            var secondTask = new CreateRoverTaskDto
            {
                RoverName = rover1,
                TaskType = TaskType.Sample,
                Latitude = -25.1,
                Longitude = -70.1,
                StartsAt = startTime.AddHours(2), // No overlap
                DurationMinutes = 60
            };

            // Act & Assert
            var result1 = await _roverTaskService.CreateTaskAsync(rover1, firstTask);
            var result2 = await _