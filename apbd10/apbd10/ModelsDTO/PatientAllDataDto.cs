namespace Cwiczenia10.ModelsDTO;

public class PatientAllDataDto
{
    public PatientDto PatientDto { get; set; }

    public ICollection<PrescriptionAllDataDto> PrescriptionAllDataDtos { get; set; }
}