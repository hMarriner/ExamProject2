using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class AppointmentCategoryConfiguration : IEntityTypeConfiguration<AppointmentCategory>
{
    public void Configure(EntityTypeBuilder<AppointmentCategory> builder)
    {
        builder.HasKey(ac => ac.AppointmentCategoryId);

        builder.Property(ac => ac.Name)
        .IsRequired()
        .HasMaxLength(100);

        builder.Property(ac => ac.Description)
        .HasMaxLength(500);

        builder.HasIndex(ac => ac.Name)
        .IsUnique();

        builder.HasMany(ac => ac.Appointments)
        .WithOne(a => a.AppointmentCategory)
        .HasForeignKey(a => a.AppointmentCategoryId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}