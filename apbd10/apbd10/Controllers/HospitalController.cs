using Cwiczenia10.ModelsDTO;
using Cwiczenia10.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia10.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HospitalController : ControllerBase
{
    private IHospitalService _hospitalService;

    public HospitalController(IHospitalService hospitalService)
    {
        _hospitalService = hospitalService;
    }

    [HttpPut("AddPatientWithPrescription")]
    public async Task<IActionResult> AddPatientWithPrescription(PatientPrescriptionDto patientPrescriptionDto,
        CancellationToken cancellationToken)
    {
        var res = await _hospitalService.AddPatientWithPrescription(patientPrescriptionDto, cancellationToken);

        if (res == -1)
        {
            return NotFound("Medicaments cannot be longer than 10!");
        }

        if (res == -2)
        {
            return NotFound("At least one of the Medicaments provided do not exist!");
        }

        if (res == -3)
        {
            return NotFound("DueDate has to be larger or equal to Date!");
        }

        return Ok("Prescription successfully added");
    }

    [HttpGet("GetPatientAllData")]
    public async Task<IActionResult> GetPatientAllData(int idPatient, CancellationToken cancellationToken)
    {
        var res = await _hospitalService.GetPatientAllData(idPatient, cancellationToken);

        if (res.PatientDto.IdPatient == -1)
        {
            return NotFound("Patient with provided Id does not exist!");
        }

        return Ok(res);
    }
}