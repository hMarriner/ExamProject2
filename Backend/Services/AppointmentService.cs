using Microsoft.EntityFrameworkCore;
using Backend.Data;

namespace Backend.Services
{

    public class AppointmentService
    {
        private readonly AppointmentBookingContext _context;
        public const int MinimumAppointmentDurationInMinutes = 10;
        public const int MaximumAppointmentDurationInMinutes = 30;

        public AppointmentService(AppointmentBookingContext context)
        {
            _context = context;
        }

        public async Task<bool> HasConflictAsync(
            int? patientId,
            int doctorId,
            DateTime appointmentDate,
            int durationInMinutes,
            int? appointmentId = null)
        {



            var appointmentEnd = appointmentDate.AddMinutes(durationInMinutes);

            return await _context.Appointments.AnyAsync(a =>
                a.AppointmentId != appointmentId &&
                a.Date < appointmentEnd &&
                appointmentDate < a.Date.AddMinutes(a.DurationInMinutes) &&
                (a.DoctorId == doctorId ||
                (patientId.HasValue && a.PatientId == patientId.Value)));
        }

        public async Task<List<DateTime>> GetAvailableSlotsAsync(
            int doctorId,
            DateTime date,
            int durationInMinutes)
        {
            if (durationInMinutes < MinimumAppointmentDurationInMinutes ||
            durationInMinutes > MaximumAppointmentDurationInMinutes)
            {
                throw new ArgumentOutOfRangeException(
                nameof(durationInMinutes),
                $"Duration must be between {MinimumAppointmentDurationInMinutes} and {MaximumAppointmentDurationInMinutes} minutes.");
            }

            var openingTime = date.Date.AddHours(9);
            var closingTime = date.Date.AddHours(17);
            var now = DateTime.Now;

            var appointments = await _context.Appointments
            .Where(a => a.DoctorId == doctorId &&
            a.Date < closingTime &&
            a.Date.AddMinutes(a.DurationInMinutes) > openingTime)
            .ToListAsync();

            var slots = new List<DateTime>();

            for (
                var slot = openingTime;
                slot.AddMinutes(durationInMinutes) <= closingTime;
                slot = slot.AddMinutes(10))
            {
                var slotEnd = slot.AddMinutes(durationInMinutes);

                var overlaps = appointments.Any(a =>
                a.Date < slotEnd &&
                slot < a.Date.AddMinutes(a.DurationInMinutes));

                if (slot >= now && !overlaps)
                {
                    slots.Add(slot);
                }
            }
            return slots;
        }

        public static bool IsAppointmentTimeValid(DateTime appointmentDate, int durationInMinutes)
        {
            if (appointmentDate == default ||
                durationInMinutes < MinimumAppointmentDurationInMinutes ||
                durationInMinutes > MaximumAppointmentDurationInMinutes)
            {
                return false;
            }

            var openingTime = appointmentDate.Date.AddHours(9);
            var closingTime = appointmentDate.Date.AddHours(17);
            var appointmentEnd = appointmentDate.AddMinutes(durationInMinutes);
            var slotIntervalTicks = TimeSpan.FromMinutes(10).Ticks;

            return appointmentDate >= DateTime.Now &&
            appointmentDate >= openingTime &&
            appointmentEnd <= closingTime &&
            (appointmentDate - openingTime).Ticks % slotIntervalTicks == 0;

        }


    }

}

