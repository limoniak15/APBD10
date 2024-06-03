using Cwiczenia10.Entities;

namespace Cwiczenia10.ModelsDTO;

public class PatientPrescriptionDto
{
    public PatientDto PatientDto { get; set; }

    public ICollection<MedicamentDto> MedicamentsDto { get; set; }

    public DateTime Date { get; set; }

    public DateTime DueDate { get; set; }
}