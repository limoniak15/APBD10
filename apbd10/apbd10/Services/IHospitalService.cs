using Cwiczenia10.ModelsDTO;

namespace Cwiczenia10.Services;

public interface IHospitalService
{
    public Task<int> AddPatientWithPrescription(PatientPrescriptionDto patientPrescriptionDto,
        CancellationToken cancellationToken);

    public Task<PatientAllDataDto> GetPatientAllData(int idPatient, CancellationToken cancellationToken);
}