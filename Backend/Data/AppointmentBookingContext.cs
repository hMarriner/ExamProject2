using Microsoft.EntityFrameworkCore;

using Backend.Models;

namespace Backend.Data;

public class AppointmentBookingContext : DbContext
{
    public AppointmentBookingContext(
        DbContextOptions<AppointmentBookingContext> options)
        : base(options)
    {
    }

    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Speciality> Specialities { get; set; }
    public DbSet<Appointment> Appointments { get; set; } = null!;

    public DbSet<Patient> Patients { get; set; }

    public DbSet<AppointmentCategory> AppointmentCategories { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppointmentBookingContext).Assembly);
    }
}