using Cwiczenia10.Entities;
using Cwiczenia10.ModelsDTO;
using Microsoft.EntityFrameworkCore;

namespace Cwiczenia10.Repositories;

public class HospitalRepository : IHospitalRepository
{
    private readonly HospitalDbContext _context = new();

    public async Task<int> AddPatientWithPrescription(PatientPrescriptionDto patientPrescriptionDto,
        CancellationToken cancellationToken)
    {
        if (await DoesPatientAlreadyExist(patientPrescriptionDto.PatientDto, cancellationToken) == false)
        {
            // Wytworzenie pacjenta, jeżeli nie istnieje w bazie.
            var res = await _context
                .Patients
                .AddAsync(new Patient
                {
                    IdPatient = patientPrescriptionDto.PatientDto.IdPatient,
                    Birthdate = patientPrescriptionDto.PatientDto.Birthdate,
                    FirstName = patientPrescriptionDto.PatientDto.FirstName,
                    LastName = patientPrescriptionDto.PatientDto.LastName
                }, cancellationToken);
        }

        if (DoProvidedMedicamentsExceedSetLimit(patientPrescriptionDto.MedicamentsDto) == true)
        {
            return -1;
        }

        if (await DoAllMedicamentsProvidedExist(patientPrescriptionDto.MedicamentsDto, cancellationToken) == false)
        {
            return -2;
        }

        if (IsDueDateLargerThanDate(patientPrescriptionDto.DueDate, patientPrescriptionDto.Date) == false)
        {
            return -3;
        }

        // Dodajemy nowe Prescription i wyciągamy jego ID z obiektu.
        var prescription = new Prescription
        {
            Date = patientPrescriptionDto.Date,
            DueDate = patientPrescriptionDto.DueDate,
            IdPatient = patientPrescriptionDto.PatientDto.IdPatient,
            IdDoctor = 1
        };

        await _context
            .Prescriptions
            .AddAsync(prescription, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var idPrescriptionGenerated = prescription.IdPrescription;

        // Dodajemy nowe Prescription_Medicament, korzystając z wyciągniętego Id.
        foreach (var medicament in patientPrescriptionDto.MedicamentsDto)
        {
            await _context
                .PrescriptionMedicaments
                .AddAsync(new PrescriptionMedicament
                {
                    IdMedicament = medicament.IdMedicament,
                    IdPrescription = idPrescriptionGenerated,
                    Details = medicament.Description
                }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        return 1;
    }

    private async Task<bool> DoesPatientAlreadyExist(PatientDto patientDto, CancellationToken cancellationToken)
    {
        var res =
            await
                _context
                    .Patients
                    .Where(p =>
                        p.IdPatient == patientDto.IdPatient &&
                        p.FirstName == patientDto.FirstName &&
                        p.LastName == patientDto.LastName &&
                        p.Birthdate == patientDto.Birthdate
                    )
                    .FirstOrDefaultAsync(cancellationToken);

        return res != null;
    }

    private async Task<bool> DoAllMedicamentsProvidedExist(ICollection<MedicamentDto> medicamentsDtos,
        CancellationToken cancellationToken)
    {
        var medsIds = medicamentsDtos.Select(m => m.IdMedicament).ToList();

        var countAsync = await
            _context
                .Medicaments
                .Where(m => medsIds.Contains(m.IdMedicament))
                .CountAsync(cancellationToken);

        return countAsync == medsIds.Count;
    }

    private bool DoProvidedMedicamentsExceedSetLimit(ICollection<MedicamentDto> medicamentsDtos)
    {
        return medicamentsDtos.Count > 10;
    }

    private bool IsDueDateLargerThanDate(DateTime dueDate, DateTime date)
    {
        return dueDate > date;
    }

    public async Task<PatientAllDataDto> GetPatientAllData(int idPatient, CancellationToken cancellationToken)
    {
        if (await DoesPatientExist(idPatient, cancellationToken) == false)
        {
            return new PatientAllDataDto
            {
                PatientDto = new PatientDto
                {
                    IdPatient = -1
                }
            };
        }

        var patientDto = await _context
            .Patients
            .Where(p => p.IdPatient == idPatient)
            .Select(p => new PatientDto()
            {
                IdPatient = p.IdPatient,
                Birthdate = p.Birthdate,
                FirstName = p.FirstName,
                LastName = p.LastName
            })
            .FirstOrDefaultAsync(cancellationToken);

        var res = await _context
            .Prescriptions
            .Where(p => p.IdPatient == idPatient)
            .Select(
                allDtoData => new PatientAllDataDto
                {
                    PatientDto = patientDto!,
                    PrescriptionAllDataDtos =
                        _context
                            .Prescriptions
                            .Include(pr => pr.PrescriptionMedicaments)
                            .ThenInclude(prMed => prMed.Medicament)
                            .Include(pr => pr.Doctor)
                            .Where(p => p.IdPatient == idPatient)
                            .Select(
                                prDtoData => new PrescriptionAllDataDto
                                {
                                    IdPrescription = prDtoData.IdPrescription,
                                    MedicamentDtos = prDtoData.PrescriptionMedicaments
                                        .Select(prMed => new MedicamentDto
                                        {
                                            IdMedicament = prMed.Medicament.IdMedicament,
                                            Name = prMed.Medicament.Name,
                                            Description = prMed.Medicament.Description,
                                            Type = prMed.Medicament.Type
                                        })
                                        .ToList(),
                                    DoctorDto = new DoctorDto
                                    {
                                        IdDoctor = prDtoData.Doctor.IdDoctor,
                                        FirstName = prDtoData.Doctor.FirstName,
                                        LastName = prDtoData.Doctor.LastName,
                                        Email = prDtoData.Doctor.Email
                                    }
                                }
                            )
                            .ToList()
                }
            )
            .FirstOrDefaultAsync(cancellationToken);

        return res!;
    }

    private async Task<bool> DoesPatientExist(int idPatient, CancellationToken cancellationToken)
    {
        var res = await _context
            .Patients
            .Where(p => p.IdPatient == idPatient)
            .FirstOrDefaultAsync(cancellationToken);

        return res != null;
    }
}