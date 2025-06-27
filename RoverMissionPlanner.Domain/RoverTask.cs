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
        public Guild Id { get; set; }

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
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Duración de la tarea en minutos.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Estado actual de la tarea.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Fecha y hora de finalizacion calculada  
        /// </summary>
        public DateTime EndsAt => StartAt.AddMinutes(DurationMinutes);

        /// <summary>
        /// Verificacion de superposicion con otra tarea
        /// </summary>
        public bool OverlapsWith(RoverTask other)
        {
            if (other == null || RoverName != other.RoverName)
                return false;

            return StartAt < other.EndsAt && EndsAt > other.StartAt;
        }
    }
}