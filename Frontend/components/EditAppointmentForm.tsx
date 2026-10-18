"use client";

import { useState, useEffect, useActionState } from "react";
import type { Appointment } from "@/types/appointment";
import type {
  Doctor,
  Clinic,
  AppointmentCategory,
} from "@/modules/booking/actions";
import { getAvailableSlots } from "@/modules/booking/availability";
import { updateAppointment } from "@/modules/booking/actions";
import Button from "./Button";
import Input from "./Input";
import Select from "./Select";

type EditAppointmentFormProps = {
  readonly appointment: Appointment;
  readonly doctors: Doctor[];
  readonly clinics: Clinic[];
  readonly appointmentCategories: AppointmentCategory[];
};

export default function EditAppointmentForm({
  appointment,
  doctors,
  clinics,
  appointmentCategories,
}: EditAppointmentFormProps) {
  const [doctorId, setDoctorId] = useState(String(appointment.doctorId));
  const [clinicId, setClinicId] = useState(String(appointment.clinicId));
  const [appointmentCategoryId, setAppointmentCategoryId] = useState(
    String(appointment.appointmentCategoryId),
  );
  const [appointmentDay, setAppointmentDay] = useState(
    appointment.appointmentDate.slice(0, 10),
  );
  const [durationInMinutes, setDurationInMinutes] = useState(
    String(appointment.durationInMinutes),
  );
  const [selectedSlot, setSelectedSlot] = useState(appointment.appointmentDate);
  const [availableSlots, setAvailableSlots] = useState<string[]>([]);
  const [loadingSlots, setLoadingSlots] = useState(false);
  const [availabilityError, setAvailabilityError] = useState("");
  const [state, dispatch] = useActionState(updateAppointment, { message: "" });
  const clinicDoctors = doctors.filter((d) => String(d.clinicId) === clinicId);

  useEffect(() => {
    if (!doctorId || !appointmentDay || !durationInMinutes) {
      setAvailableSlots([]);
      return;
    }

    let cancelled = false;

    async function loadSlots() {
      setLoadingSlots(true);
      setAvailabilityError("");

      try {
        const slots = await getAvailableSlots(
          doctorId,
          appointmentDay,
          durationInMinutes,
        );

        if (!cancelled) {
          const isOriginalAppointment =
            doctorId === String(appointment.doctorId) &&
            appointmentDay === appointment.appointmentDate.slice(0, 10) &&
            durationInMinutes === String(appointment.durationInMinutes);

          setAvailableSlots(
            isOriginalAppointment
              ? Array.from(new Set([...slots, appointment.appointmentDate]))
              : slots,
          );
          if (!isOriginalAppointment) {
            setSelectedSlot("");
          }
        }
      } catch {
        if (!cancelled) {
          setAvailableSlots([]);
          setAvailabilityError("Failed to load available slots.");
        }
      } finally {
        if (!cancelled) {
          setLoadingSlots(false);
        }
      }
    }
    void loadSlots();

    return () => {
      cancelled = true;
    };
  }, [
    doctorId,
    appointmentDay,
    durationInMinutes,
    appointment.doctorId,
    appointment.appointmentDate,
    appointment.durationInMinutes,
  ]);

  return (
    <form action={dispatch} className="mx-auto max-w-md space-y-4">
      {state.message && (
        <div
          role="status"
          aria-live="polite"
          className="mt-4 rounded-md border border-blue-300 bg-blue-50 px-4 py-3 font-semibold text-blue-900"
        >
          {state.message}
        </div>
      )}
      <input
        type="hidden"
        name="appointmentId"
        value={appointment.appointmentId}
      />

      <Select
        id="clinicId"
        name="clinicId"
        label="Clinic"
        value={clinicId}
        onChange={(event) => {
          setClinicId(event.target.value);
          setDoctorId("");
          setAvailableSlots([]);
          setSelectedSlot("");
          setAvailabilityError("");
        }}
        required
      >
        <option value="">Select a clinic</option>
        {clinics.map((c) => (
          <option key={c.clinicId} value={c.clinicId}>
            {c.name}
          </option>
        ))}
      </Select>

      <Select
        id="doctorId"
        name="doctorId"
        label="Doctor(speciality)"
        value={doctorId}
        onChange={(event) => {
          setDoctorId(event.target.value);
        }}
        required
      >
        <option value="">Select a doctor</option>
        {clinicDoctors.map((d) => (
          <option key={d.doctorId} value={d.doctorId}>
            {d.firstName} {d.lastName} - {d.specialityName}
          </option>
        ))}
      </Select>

      <Select
        id="appointmentCategoryId"
        name="appointmentCategoryId"
        label="Appointment Category"
        value={appointmentCategoryId}
        onChange={(event) => setAppointmentCategoryId(event.target.value)}
        required
      >
        <option value="">Select a category</option>
        {appointmentCategories.map((category) => (
          <option
            key={category.appointmentCategoryId}
            value={category.appointmentCategoryId}
          >
            {category.name}
          </option>
        ))}
      </Select>

      <Input
        id="appointmentDay"
        name="appointmentDay"
        label="Date"
        type="date"
        value={appointmentDay}
        onChange={(event) => setAppointmentDay(event.target.value)}
        required
      />

      <Input
        id="durationInMinutes"
        name="durationInMinutes"
        label="Duration (minutes)"
        type="number"
        min={10}
        max={30}
        value={durationInMinutes}
        onChange={(event) => setDurationInMinutes(event.target.value)}
        required
      />

      <div>
        <p className="mb-2 font-semibold">Available Times</p>

        {loadingSlots && <p>Loading available times...</p>}

        {availabilityError && (
          <p role="alert" className="text-red-600">
            {availabilityError}
          </p>
        )}
        <div className="grid grid-cols-2 gap-2">
          {availableSlots.map((slot) => (
            <button
              key={slot}
              type="button"
              onClick={() => setSelectedSlot(slot)}
              className={
                selectedSlot === slot
                  ? "rounded-md bg-blue-600 px-4 py-2 text-white"
                  : "rounded-md bg-gray-300 px-4 py-2"
              }
            >
              {new Date(slot).toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
              })}
            </button>
          ))}
        </div>
      </div>

      <input type="hidden" name="appointmentDate" value={selectedSlot} />
      <input type="hidden" name="clinicId" value={clinicId} />

      <Button type="submit" disabled={!selectedSlot || loadingSlots}>
        Update Appointment
      </Button>
    </form>
  );
}
