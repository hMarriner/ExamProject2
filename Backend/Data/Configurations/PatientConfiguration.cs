using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.PatientId);

        builder.Property(p => p.FirstName)
        .IsRequired()
        .HasMaxLength(100);

        builder.Property(p => p.LastName)
        .IsRequired()
        .HasMaxLength(100);

        builder.Property(p => p.Email)
        .IsRequired()
        .HasMaxLength(254);

        builder.Property(p => p.PasswordHash)
        .HasMaxLength(255);

        builder.Property(p => p.Gender)
        .HasMaxLength(10);

        builder.Property(p => p.SocialSecurityNumber)
        .HasMaxLength(11);

        builder.Property(p => p.TaxNumber)
        .HasMaxLength(50);

        builder.Property(p => p.Religion)
        .HasMaxLength(50);

        builder.Property(p => p.DriversLicenseNumber)
        .HasMaxLength(50);

        builder.Property(p => p.MedicalInsuranceNumber)
        .HasMaxLength(50);

        builder.Property(p => p.IsRegistered)
        .IsRequired();

    }
}