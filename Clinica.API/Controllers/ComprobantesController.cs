// Clinica.API/Controllers/ComprobantesController.cs

using Clinica.Domain.DTOs.Comprobantes;
using Clinica.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComprobantesController : ControllerBase
{
    private readonly IComprobanteService _comprobanteService;

    public ComprobantesController(IComprobanteService comprobanteService)
    {
        _comprobanteService = comprobanteService;
    }

    // ==========================================================
    // PREVIEW - BOLETA DE PAGO
    // ==========================================================
    // GET: api/comprobantes/preview/boleta-pago/{pagoId}

    [HttpGet("preview/boleta-pago/{pagoId:guid}")]
    public async Task<ActionResult<ComprobantePagoPreviewDto>> PreviewBoletaPago(Guid pagoId)
    {
        var preview = await _comprobanteService.PreviewBoletaPagoAsync(pagoId);
        return Ok(preview);
    }

    // ==========================================================
    // EMITIR - BOLETA DE PAGO
    // ==========================================================
    // POST: api/comprobantes/emitir/boleta-pago

    [HttpPost("emitir/boleta-pago")]
    public async Task<ActionResult<object>> EmitirBoletaPago([FromBody] EmitirComprobantePagoDto dto)
    {
        var comprobanteId = await _comprobanteService.EmitirBoletaPagoAsync(dto);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = comprobanteId },
            new
            {
                Mensaje = "Boleta de pago emitida correctamente.",
                ComprobanteId = comprobanteId
            }
        );
    }

    // ==========================================================
    // PDF - BOLETA DE PAGO
    // ==========================================================
    // GET: api/comprobantes/{comprobanteId}/pdf/boleta-pago

    [HttpGet("{comprobanteId:guid}/pdf/boleta-pago")]
    public async Task<IActionResult> GenerarPdfBoletaPago(Guid comprobanteId)
    {
        var documento = await _comprobanteService.GenerarPdfBoletaPagoAsync(comprobanteId);

        return File(
            documento.Archivo,
            documento.ContentType,
            documento.NombreArchivo
        );
    }

    // ==========================================================
    // CONSULTA POR ID
    // ==========================================================
    // GET: api/comprobantes/{id}

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ComprobanteDto>> ObtenerPorId(Guid id)
    {
        var comprobante = await _comprobanteService.ObtenerPorIdAsync(id);
        return Ok(comprobante);
    }

    // ==========================================================
    // CONSULTA POR PACIENTE
    // ==========================================================
    // GET: api/comprobantes/paciente/{pacienteId}

    [HttpGet("paciente/{pacienteId:guid}")]
    public async Task<ActionResult<IEnumerable<ComprobanteDto>>> ObtenerPorPaciente(Guid pacienteId)
    {
        var comprobantes = await _comprobanteService.ObtenerPorPacienteAsync(pacienteId);
        return Ok(comprobantes);
    }

    // ==========================================================
    // CONSULTA POR PAGO
    // ==========================================================
    // GET: api/comprobantes/pago/{pagoId}

    [HttpGet("pago/{pagoId:guid}")]
    public async Task<ActionResult<IEnumerable<ComprobanteDto>>> ObtenerPorPago(Guid pagoId)
    {
        var comprobantes = await _comprobanteService.ObtenerPorPagoAsync(pagoId);
        return Ok(comprobantes);
    }

    // ==========================================================
    // CONSULTA POR ATENCIÓN
    // ==========================================================
    // GET: api/comprobantes/atencion/{atencionId}

    [HttpGet("atencion/{atencionId:guid}")]
    public async Task<ActionResult<IEnumerable<ComprobanteDto>>> ObtenerPorAtencion(Guid atencionId)
    {
        var comprobantes = await _comprobanteService.ObtenerPorAtencionAsync(atencionId);
        return Ok(comprobantes);
    }

    // ==========================================================
    // ANULAR COMPROBANTE
    // ==========================================================
    // PATCH: api/comprobantes/{comprobanteId}/anular

    [HttpPatch("{comprobanteId:guid}/anular")]
    public async Task<IActionResult> AnularComprobante(
        Guid comprobanteId,
        [FromBody] AnularComprobanteDto dto)
    {
        await _comprobanteService.AnularComprobanteAsync(comprobanteId, dto.Motivo);

        return Ok(new
        {
            Mensaje = "Comprobante anulado correctamente.",
            ComprobanteId = comprobanteId
        });
    }
}