using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RoverMissionPlanner.Application.DTOs;
using RoverMissionPlanner.Application.Services;

namespace RoverMissionPlanner.Api.Controller
{
    [ApiController]
    [Route("api/rovers")]
    public class RoversController : ControllerBase
    {
        private readonly IRoverTaskService _roverTaskService;
        private readonly IValidator<CreateRoverTaskDto> _validator;

        public RoversController(IRoverTaskService roverTaskService, IValidator<CreateRoverTaskDto> validator)
        {
            _roverTaskService = roverTaskService;
            _validator = validator;
        }

        /// <summary>
        /// Crea una nueva tarea para el rover especificado
        /// </summary>
        /// <param name="id">ID del rover</param>
        /// <param name="createTaskDto">Datos de la tarea a crear</param>
        /// <returns>La tarea creada</returns>
        [HttpPost("{id}/tasks")]
        public async Task<ActionResult<RoverTaskDto>> CreateTask(string id, [FromBody] CreateRoverTaskDto createTaskDto)
        {
            var validationResult = await _validator.ValidateAsync(createTaskDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            try
            {
                var task = await _roverTaskService.CreateTaskAsync(id, createTaskDto);
                return CreatedAtAction(nameof(GetTasks), new { id, date = DateOnly.FromDateTime(task.StartsAt) }, task);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las tareas de un rover para una fecha específica
        /// </summary>
        /// <param name="id">ID del rover</param>
        /// <param name="date">Fecha en formato YYYY-MM-DD</param>
        /// <returns>Lista de tareas ordenadas cronológicamente</returns>
        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<RoverTaskDto>>> GetTasks(string id, [FromQuery] string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                return BadRequest(new { message = "Formato de fecha inválido. Use YYYY-MM-DD" });
            }

            var tasks = await _roverTaskService.GetTasksByDateAsync(id, parsedDate);
            return Ok(tasks);
        }

        /// <summary>
        /// Obtiene el porcentaje de utilización de un rover para una fecha específica
        /// </summary>
        /// <param name="id">ID del rover</param>
        /// <param name="date">Fecha en formato YYYY-MM-DD</param>
        /// <returns>Información de utilización del rover</returns>
        [HttpGet("{id}/utilization")]
        public async Task<ActionResult<RoverUtilizationDto>> GetUtilization(string id, [FromQuery] string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                return BadRequest(new { message = "Formato de fecha inválido. Use YYYY-MM-DD" });
            }

            var utilization = await _roverTaskService.GetUtilizationAsync(id, parsedDate);
            return Ok(utilization);
        }
    }
}