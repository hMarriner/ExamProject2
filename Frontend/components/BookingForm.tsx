"use client";

import { useActionState, useState, useEffect } from "react";
import { bookAppointment } from "@/modules/booking/actions";
import type {
  Doctor,
  AppointmentCategory,
  Clinic,
} from "@/modules/booking/actions";
import { getAvailableSlots } from "@/modules/booking/availability";
import Button from "./Button";
import Input from "./Input";
import Select from "./Select";

const initialState = {
  message: "",
};

type BookingFormProps = {
  readonly clinics: Clinic[];
  readonly doctors: Doctor[];
  readonly categories: AppointmentCategory[];
  readonly isLoggedIn: boolean;
};

export default function BookingForm({
  clinics,
  doctors,
  categories,
  isLoggedIn,
}: BookingFormProps) {
  const [state, dispatch] = useActionState(bookAppointment, { message: "" });

  const [clinicId, setClinicId] = useState("");
  const [doctorId, setDoctorId] = useState("");
  const [appointmentDay, setAppointmentDay] = useState("");
  const [durationInMinutes, setDurationInMinutes] = useState("10");
  const [availableSlots, setAvailableSlots] = useState<string[]>([]);
  const [loadingSlots, setLoadingSlots] = useState(false);
  const [availabilityError, setAvailabilityError] = useState("");
  const [selectedSlot, setSelectedSlot] = useState("");
  const clinicDoctors = doctors.filter((d) => String(d.clinicId) === clinicId);

  useEffect(() => {
    if (state.success) {
      setClinicId("");
      setDoctorId("");
      setAppointmentDay("");
      setDurationInMinutes("10");
      setAvailableSlots([]);
      setSelectedSlot("");
      setAvailabilityError("");
    }
  }, [state]);

  useEffect(() => {
    if (!doctorId || !appointmentDay || !durationInMinutes) {
      setAvailableSlots([]);
      setSelectedSlot("");
      return;
    }

    let cancelled = false;

    async function loadAvailableSlots() {
      setLoadingSlots(true);
      setAvailabilityError("");
      setSelectedSlot("");

      try {
        const slots = await getAvailableSlots(
          doctorId,
          appointmentDay,
          durationInMinutes,
        );

        if (!cancelled) {
          setAvailableSlots(slots);
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

    void loadAvailableSlots();

    return () => {
      cancelled = true;
    };
  }, [doctorId, appointmentDay, durationInMinutes]);

  return (
    <form action={dispatch} className="mx-auto max-w-md space-y-4">
      {state.message && (
        <div
          role={state.success ? "status" : "alert"}
          aria-live="polite"
          className={
            state.success
              ? "mt-4 rounded-md border border-blue-300 bg-blue-50 px-4 py-3 font-semibold text-blue-900"
              : "mt-4 rounded-md border border-red-300 bg-red-50 px-4 py-3 font-semibold text-red-900"
          }
        >
          {state.message}
        </div>
      )}
      {!isLoggedIn && (
        <>
          <Input id="firstName" name="firstName" label="First name" required />

          <Input id="lastName" name="lastName" label="Last name" required />

          <Input id="email" name="email" label="Email" type="email" required />

          <Input
            id="birthDate"
            name="birthDate"
            label="Date of birth"
            type="date"
            required
          />
        </>
      )}

      <Select
        id="clinicId"
        name="clinicId"
        label="Clinic"
        required
        value={clinicId}
        onChange={(event) => {
          setClinicId(event.target.value);
          setDoctorId("");
          setAvailableSlots([]);
          setSelectedSlot("");
          setAvailabilityError("");
        }}
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
        required
        value={doctorId}
        disabled={!clinicId}
        onChange={(event) => setDoctorId(event.target.value)}
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
        required
      >
        <option value="">Select a category</option>
        {categories.map((ac) => (
          <option
            key={ac.appointmentCategoryId}
            value={ac.appointmentCategoryId}
          >
            {ac.name}
          </option>
        ))}
      </Select>

      <Input
        id="appointmentDay"
        name="appointmentDay"
        label="Date"
        type="date"
        required
        value={appointmentDay}
        onChange={(event) => setAppointmentDay(event.target.value)}
      />
      <Input
        id="durationInMinutes"
        name="durationInMinutes"
        label="Duration (minutes)"
        type="number"
        min={10}
        max={30}
        required
        value={durationInMinutes}
        onChange={(event) => setDurationInMinutes(event.target.value)}
      />

      <div className="mb-4">
        <p className="mb-2 block text-base font-semibold text-gray-900">
          Available Times
        </p>

        {loadingSlots && <p>Loading available times...</p>}

        {availabilityError && (
          <p role="alert" className="text-red-600">
            {availabilityError}
          </p>
        )}

        {!loadingSlots &&
          !availabilityError &&
          doctorId &&
          appointmentDay &&
          availableSlots.length === 0 && (
            <p>No available times for the selected date.</p>
          )}

        <div className="grid grid-cols-2 gap-2">
          {availableSlots.map((slot) => {
            const time = new Date(slot).toLocaleTimeString([], {
              hour: "2-digit",
              minute: "2-digit",
            });

            const isSelected = selectedSlot === slot;

            return (
              <button
                key={slot}
                type="button"
                onClick={() => setSelectedSlot(slot)}
                className={
                  isSelected
                    ? "rounded-md bg-blue-600 px-4 py-2 text-white"
                    : "rounded-md border border-gray-300 px-4 py-2"
                }
              >
                {time}
              </button>
            );
          })}
        </div>
      </div>
      <input
        type="hidden"
        name="appointmentDate"
        value={selectedSlot}
        required
      />
      <Button type="submit" disabled={!selectedSlot}>
        Book Appointment
      </Button>
    </form>
  );
}
