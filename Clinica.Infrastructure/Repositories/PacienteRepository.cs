using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Repositories;

public class PacienteRepository : GenericRepository<Paciente>, IPacienteRepository
{
    
}