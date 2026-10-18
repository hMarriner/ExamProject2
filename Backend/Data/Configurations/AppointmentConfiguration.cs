using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.AppointmentId);

        builder.Property(a => a.Date)
        .IsRequired();

        builder.HasOne(a => a.Clinic)
        .WithMany(c => c.Appointments)
        .HasForeignKey(a => a.ClinicId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Doctor)
        .WithMany(d => d.Appointments)
        .HasForeignKey(a => a.DoctorId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Patient)
        .WithMany(p => p.Appointments)
        .HasForeignKey(a => a.PatientId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AppointmentCategory)
        .WithMany(ac => ac.Appointments)
        .HasForeignKey(a => a.AppointmentCategoryId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}