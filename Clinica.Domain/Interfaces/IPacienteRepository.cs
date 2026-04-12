using Clinica.Domain.Entities;

namespace Clinica.Domain.Interfaces;

public interface IPacienteRepository : IGenericRepository<Paciente>
{
    // Buscamos al paciente por su DNI
    Task<Paciente?> ObtenerPorDniAsync(string dni);

    // Buscamos por Número de Historia Clínica
    Task<Paciente?> ObtenerPorNumeroHcAsync(string numeroHc);

    // Validaciones rápidas para evitar duplicados antes de guardar
    Task<bool> ExisteDniAsync(string dni);
    Task<bool> ExisteNumeroHcAsync(string numeroHc);
    Task<int> ContarPacientesPorAnioAsync(int anio);
}