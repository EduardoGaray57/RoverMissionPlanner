namespace RoverMissionPlanner.Domain
{
    /// <summary>
    /// Representa la tarea asignada al rover
    /// </summary>
    public class RoverTask
    {
        /// <summary>
        /// Identificador único de la tarea.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del rover al que se le asigna la tarea.
        /// </summary>
        public string RoverName { get; set; } = string.Empty;

        /// <summary>
        /// El tipo de tarea a realizar.
        /// </summary>
        public TaskType TaskType { get; set; }

        /// <summary>
        /// Coordenada de latitud para la tarea.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Coordenada de longitud para la tarea.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Fecha y hora de inicio de la tarea en formato UTC.
        /// </summary>
        public DateTime StartsAt { get; set; }

        /// <summary>
        /// Duración de la tarea en minutos.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Estado actual de la tarea.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Fecha y hora de finalización de la tarea calculada.
        /// </summary>
        public DateTime EndsAt => StartsAt.AddMinutes(DurationMinutes);
    }
}