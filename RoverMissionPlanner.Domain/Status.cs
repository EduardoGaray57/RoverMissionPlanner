namespace RoverMissionPlanner.Domain
{
    /// <summary>
    /// Define el estado de la ejecucion de tarea
    /// </summary>
    public enum Status
    {
        Planned,     //La tarea esta planificada
        InProgress,  //La tarea se esta ejecutando
        Completed,   //La tarea se completo
        Aborted      // La tarea fue cancelada
    }
}