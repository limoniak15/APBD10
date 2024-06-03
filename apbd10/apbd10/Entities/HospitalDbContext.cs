using Cwiczenia10.Entities.Configs;
using Microsoft.EntityFrameworkCore;

namespace Cwiczenia10.Entities;

public class HospitalDbContext : DbContext
{
    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<Medicament> Medicaments { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

    public HospitalDbContext()
    {
    }

    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
    {
    }

    // WAŻNE !!
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=master;User Id=sa;Password=bazaTestowa1234;TrustServerCertificate=True");
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Aplikuje wszystkie konfigurację z folderu, ale podajemy jedną klasę tylko
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DoctorEfConfig).Assembly);
    }
}