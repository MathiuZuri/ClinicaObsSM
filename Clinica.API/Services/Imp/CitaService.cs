using Clinica.Domain.DTOs;
using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;

namespace Clinica.API.Services.Imp;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _citaRepository;

    public CitaService(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<IEnumerable<CitaResponseDto>> ObtenerTodasAsync()
    {
        var citas = await _citaRepository.ListarTodosAsync();
        return citas.Select(c => new CitaResponseDto(c.Id, c.PacienteId, c.FechaHoraProgramada, c.Servicio, c.Estado));
    }

    public async Task<CitaResponseDto?> ObtenerPorIdAsync(Guid id)
    {
        var c = await _citaRepository.ObtenerPorIdAsync(id);
        return c == null ? null : new CitaResponseDto(c.Id, c.PacienteId, c.FechaHoraProgramada, c.Servicio, c.Estado);
    }

    public async Task ActualizarAsync(Guid id, CrearCitaDto dto)
    {
        var cita = await _citaRepository.ObtenerPorIdAsync(id) 
                   ?? throw new KeyNotFoundException("Cita no encontrada");
    
        // Convertimos explícitamente a UTC para que Postgres no proteste
        cita.FechaHoraProgramada = DateTime.SpecifyKind(dto.FechaHoraProgramada, DateTimeKind.Utc);
    
        cita.Servicio = dto.Servicio;
        cita.MotivoConsulta = dto.MotivoConsulta;
        cita.PersonalMedicoId = dto.PersonalMedicoId;
        cita.Notas = dto.Notas;
    
        await _citaRepository.ActualizarAsync(cita);
        await _citaRepository.SaveChangesAsync();
    }

    public async Task EliminarAsync(Guid id)
    {
        var cita = await _citaRepository.ObtenerPorIdAsync(id)
                   ?? throw new KeyNotFoundException("La cita que intenta eliminar no existe.");
        
        await _citaRepository.EliminarAsync(cita);
    
        await _citaRepository.SaveChangesAsync();
    }
    
    public async Task<CitaResponseDto> AgendarNuevaCitaAsync(CrearCitaDto dto)
    {
        // Regla de negocio: Validar cruce de horarios (si se asignó un médico)
        if (dto.PersonalMedicoId.HasValue)
        {
            bool hayCruce = await _citaRepository.ExisteInterferenciaDeHorarioAsync(
                dto.PersonalMedicoId.Value, 
                dto.FechaHoraProgramada, 
                30); // Asumimos 30 min por defecto para este ejemplo

            if (hayCruce)
            {
                throw new InvalidOperationException("El médico seleccionado ya tiene una cita en ese horario.");
            }
        }

        // Mapear DTO a la Entidad de Dominio
        var nuevaCita = new Cita(dto.PacienteId, dto.Servicio, dto.MotivoConsulta);
        
        // Ejecutar la acción de negocio de Agendar
        if (dto.PersonalMedicoId.HasValue)
        {
            nuevaCita.Agendar(dto.PersonalMedicoId.Value, dto.FechaHoraProgramada);
        }

        // Guardar en Base de Datos usando el Repositorio
        await _citaRepository.AgregarAsync(nuevaCita);
        await _citaRepository.SaveChangesAsync();

        // Retornar el DTO de respuesta
        return new CitaResponseDto(
            nuevaCita.Id, 
            nuevaCita.PacienteId, 
            nuevaCita.FechaHoraProgramada, 
            nuevaCita.Servicio, 
            nuevaCita.Estado);
    }
}