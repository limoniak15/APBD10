namespace Cwiczenia10.ModelsDTO;

public class PatientDto
{
    public int IdPatient { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public DateTime Birthdate { get; set; }
}