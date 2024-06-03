using Cwiczenia10.ModelsDTO;
using Cwiczenia10.Repositories;

namespace Cwiczenia10.Services;

public class HospitalService : IHospitalService
{
    private IHospitalRepository _hospitalRepository;

    public HospitalService(IHospitalRepository hospitalRepository)
    {
        _hospitalRepository = hospitalRepository;
    }

    public async Task<int> AddPatientWithPrescription(PatientPrescriptionDto patientPrescriptionDto,
        CancellationToken cancellationToken)
    {
        var res = await _hospitalRepository.AddPatientWithPrescription(patientPrescriptionDto, cancellationToken);

        return res;
    }

    public async Task<PatientAllDataDto> GetPatientAllData(int idPatient, CancellationToken cancellationToken)
    {
        var res = await _hospitalRepository.GetPatientAllData(idPatient, cancellationToken);
        
        return res;
    }
}