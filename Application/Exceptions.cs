namespace RoverMissionPlanner.Application.Exceptions
{
    public class TaskOverlapException : Exception
    {
        public TaskOverlapException(string message) : base(message) { }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}
