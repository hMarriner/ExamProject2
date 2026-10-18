"use server";

import {
  APPOINTMENT_API_URL,
  GUEST_BOOKING_API_URL,
  DOCTOR_API_URL,
  CLINIC_API_URL,
  APPOINTMENT_CATEGORY_API_URL,
} from "@/lib/constants";
import { redirect } from "next/navigation";
import { cookies } from "next/headers";
import type { Appointment } from "@/types/appointment";
import { revalidatePath } from "next/cache";

export type BookingState = {
  message: string;
  success?: boolean;
};

export type Doctor = {
  doctorId: number;
  firstName: string;
  lastName: string;
  specialityId: number;
  specialityName: string;
  clinicId: number;
  clinicName: string;
};

export type Clinic = {
  clinicId: number;
  name: string;
};

export type AppointmentCategory = {
  appointmentCategoryId: number;
  name: string;
  durationInMinutes: number;
};

async function getList<T>(url: string): Promise<T[] | null> {
  try {
    const response = await fetch(url, { cache: "no-store" });

    if (!response.ok) {
      return null;
    }

    return (await response.json()) as T[];
  } catch {
    return null;
  }
}

export async function getDoctors(): Promise<Doctor[] | null> {
  return getList<Doctor>(DOCTOR_API_URL);
}

export async function getClinics(): Promise<Clinic[] | null> {
  return getList<Clinic>(CLINIC_API_URL);
}

export async function getAppointmentCategories(): Promise<
  AppointmentCategory[] | null
> {
  return getList<AppointmentCategory>(APPOINTMENT_CATEGORY_API_URL);
}

export async function bookAppointment(
  previousState: BookingState,
  formData: FormData,
): Promise<BookingState> {
  const token = (await cookies()).get("token")?.value;

  const payload = {
    clinicId: Number(formData.get("clinicId")),
    doctorId: Number(formData.get("doctorId")),
    appointmentCategoryId: Number(formData.get("appointmentCategoryId")),
    appointmentDate: formData.get("appointmentDate"),
    durationInMinutes: Number(formData.get("durationInMinutes")),
  };

  const isGuest = !token;

  const body = isGuest
    ? {
        ...payload,
        firstName: formData.get("firstName"),
        lastName: formData.get("lastName"),
        email: formData.get("email"),
        birthDate: formData.get("birthDate") || null,
        gender: formData.get("gender") || null,
      }
    : payload;

  const url = isGuest ? GUEST_BOOKING_API_URL : APPOINTMENT_API_URL;

  const response = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    const errorMessage = await response.text();
    return {
      message: errorMessage || `Booking failed (status ${response.status})`,
    };
  }
  revalidatePath("/appointments");
  if (isGuest) {
    return { message: "Appointment booked successfully.", success: true };
  }
  redirect("/appointments?booked=1");
}

export async function getPatientAppointment(
  appointmentId: number,
): Promise<Appointment | null> {
  const token = (await cookies()).get("token")?.value;
  if (!token) {
    redirect("/login");
  }

  const response = await fetch(`${APPOINTMENT_API_URL}/${appointmentId}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  });

  if (!response.ok) {
    return null;
  }
  return (await response.json()) as Appointment;
}

export default async function getPatientAppointments(): Promise<
  Appointment[] | null
> {
  const token = (await cookies()).get("token")?.value;

  if (!token) {
    redirect("/login");
  }

  const response = await fetch(APPOINTMENT_API_URL, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  });

  if (!response.ok) {
    return null;
  }

  return (await response.json()) as Appointment[];
}

export async function cancelAppointment(formData: FormData): Promise<void> {
  const token = (await cookies()).get("token")?.value;
  if (!token) {
    redirect("/login");
  }

  const appointmentId = Number(formData.get("appointmentId"));

  if (!Number.isSafeInteger(appointmentId) || appointmentId <= 0) {
    throw new Error("Invalid appointment ID.");
  }

  const response = await fetch(`${APPOINTMENT_API_URL}/${appointmentId}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || "Failed to cancel appointment.");
  }

  revalidatePath("/appointments");
  redirect("/appointments?cancelled=1");
}

export async function updateAppointment(
  previousState: BookingState,
  formData: FormData,
): Promise<{
  message: string;
}> {
  const token = (await cookies()).get("token")?.value;
  if (!token) {
    redirect("/login");
  }

  const appointmentId = Number(formData.get("appointmentId"));
  const clinicId = Number(formData.get("clinicId"));
  const doctorId = Number(formData.get("doctorId"));
  const appointmentCategoryId = Number(formData.get("appointmentCategoryId"));
  const durationInMinutes = Number(formData.get("durationInMinutes"));
  const appointmentDay = formData.get("appointmentDate");

  if (
    !Number.isSafeInteger(appointmentId) ||
    appointmentId <= 0 ||
    !Number.isSafeInteger(clinicId) ||
    clinicId <= 0 ||
    !Number.isSafeInteger(doctorId) ||
    doctorId <= 0 ||
    !Number.isSafeInteger(appointmentCategoryId) ||
    appointmentCategoryId <= 0 ||
    !Number.isSafeInteger(durationInMinutes) ||
    typeof appointmentDay !== "string" ||
    !appointmentDay
  ) {
    throw new Error("Invalid appointment data.");
  }

  const response = await fetch(`${APPOINTMENT_API_URL}/${appointmentId}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      clinicId,
      doctorId,
      appointmentCategoryId,
      appointmentDate: appointmentDay,
      durationInMinutes,
    }),
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || "Failed to update appointment.");
  }
  revalidatePath("/appointments");
  redirect("/appointments?updated=1");
}
