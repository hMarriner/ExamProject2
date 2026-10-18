using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.HasKey(c => c.ClinicId);

        builder.Property(c => c.Name)
        .HasMaxLength(100)
        .IsRequired();

        builder.Property(c => c.Address)
        .HasMaxLength(254)
        .IsRequired();

        builder.HasIndex(c => new
        {
            c.Name,
            c.Address
        })
       .IsUnique();

        builder.HasMany(c => c.Appointments)
        .WithOne(a => a.Clinic)
        .HasForeignKey(a => a.ClinicId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Doctors)
        .WithOne(d => d.Clinic)
        .HasForeignKey(d => d.ClinicId)
        .OnDelete(DeleteBehavior.Restrict);

    }
}