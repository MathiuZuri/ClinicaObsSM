using Clinica.Domain.DTOs;
using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;

namespace Clinica.API.Services.Imp;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _pacienteRepository;

    public PacienteService(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<IReadOnlyList<PacienteResponseDto>> ObtenerTodosAsync()
    {
        var pacientes = await _pacienteRepository.ListarTodosAsync();
        return pacientes.Select(p => new PacienteResponseDto(
            p.Id, p.DNI, p.NumeroHC, p.Nombres, p.Apellidos, p.FechaNacimiento, p.Celular
        )).ToList();
    }

    public async Task<PacienteResponseDto?> ObtenerPorIdAsync(Guid id)
    {
        var p = await _pacienteRepository.ObtenerPorIdAsync(id);
        if (p == null) return null;
        
        return new PacienteResponseDto(p.Id, p.DNI, p.NumeroHC, p.Nombres, p.Apellidos, p.FechaNacimiento, p.Celular);
    }

    public async Task<PacienteResponseDto?> ObtenerPorDniAsync(string dni)
    {
        var p = await _pacienteRepository.ObtenerPorDniAsync(dni);
        if (p == null) return null;
        
        return new PacienteResponseDto(p.Id, p.DNI, p.NumeroHC, p.Nombres, p.Apellidos, p.FechaNacimiento, p.Celular);
    }

    public async Task<Guid> CrearAsync(CrearPacienteDto dto)
    {
        // Validar duplicados
        if (await _pacienteRepository.ExisteDniAsync(dto.DNI))
            throw new InvalidOperationException($"El DNI {dto.DNI} ya está registrado.");
            
        if (await _pacienteRepository.ExisteNumeroHcAsync(dto.NumeroHC))
            throw new InvalidOperationException($"La Historia Clínica {dto.NumeroHC} ya existe.");
        
        var paciente = new Paciente(
            dto.UsuarioId, dto.DNI, dto.NumeroHC, dto.Nombres, dto.Apellidos, 
            DateTime.SpecifyKind(dto.FechaNacimiento, DateTimeKind.Utc), // Evitar error de Postgres
            dto.Sexo
        );
        
        paciente.ActualizarContacto(dto.Celular, dto.CorreoSecundario, dto.Direccion);
        
        await _pacienteRepository.AgregarAsync(paciente);
        await _pacienteRepository.SaveChangesAsync();

        return paciente.Id;
    }

    public async Task ActualizarContactoAsync(Guid id, ActualizarContactoPacienteDto dto)
    {
        var paciente = await _pacienteRepository.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Paciente no encontrado.");

        paciente.ActualizarContacto(dto.Celular, dto.CorreoSecundario, dto.Direccion);

        await _pacienteRepository.ActualizarAsync(paciente);
        await _pacienteRepository.SaveChangesAsync();
    }

    private async Task<string> GenerarHcMixtoAsync(string dni)
    {
        int anioActual = DateTime.Now.Year;
        
        int correlativo = await _pacienteRepository.ContarPacientesPorAnioAsync(anioActual) + 1;

        return $"{anioActual}-{correlativo:D4}-{dni}";
    }
    
}