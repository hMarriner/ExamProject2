using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
{
    public void Configure(EntityTypeBuilder<Speciality> builder)
    {
        builder.HasKey(s => s.SpecialityId);

        builder.Property(s => s.Name)
        .HasMaxLength(100)
        .IsRequired();

        builder.HasIndex(s => s.Name)
       .IsUnique();

        builder.HasMany(s => s.Doctors)
        .WithOne(d => d.Speciality)
        .HasForeignKey(d => d.SpecialityId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}