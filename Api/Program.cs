using FluentValidation;
using RoverMissionPlanner.Api.Middleware;
using RoverMissionPlanner.Application.DTOs;
using RoverMissionPlanner.Application.Services;
using RoverMissionPlanner.Application.Validators;
using RoverMissionPlanner.Domain.Interfaces;
using RoverMissionPlanner.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Rover Mission Planner API", Version = "v1" });
    c.EnableAnnotations();
});

// Register application services
builder.Services.AddScoped<IRoverTaskService, RoverTaskService>();
builder.Services.AddSingleton<IRoverTaskRepository, InMemoryRoverTaskRepository>();

// Register validators
builder.Services.AddScoped<IValidator<CreateRoverTaskDto>, CreateRoverTaskValidator>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();