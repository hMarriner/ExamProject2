using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.DoctorId);

        builder.Property(d => d.FirstName)
        .IsRequired()
        .HasMaxLength(100);

        builder.Property(d => d.LastName)
        .IsRequired()
        .HasMaxLength(100);

        builder.HasOne(d => d.Speciality)
        .WithMany(s => s.Doctors)
        .HasForeignKey(d => d.SpecialityId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new
        {
            d.FirstName,
            d.LastName,
            d.SpecialityId,
            d.ClinicId
        })
        .IsUnique();
    }
}