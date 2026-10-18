import { notFound, redirect } from "next/navigation";
import EditAppointmentForm from "@/components/EditAppointmentForm";
import {
  getAppointmentCategories,
  getClinics,
  getDoctors,
  getPatientAppointment,
} from "@/modules/booking/actions";
import { cookies } from "next/headers";

type EditAppointmentProps = {
  readonly params: Promise<{ id: string }>;
};

export default async function EditAppointment({
  params,
}: EditAppointmentProps) {
  const token = (await cookies()).get("token")?.value;

  if (!token) {
    redirect("/login");
  }

  const { id } = await params;
  const appointmentId = Number(id);

  if (!Number.isSafeInteger(appointmentId) || appointmentId <= 0) {
    notFound();
  }
  const [appointment, doctors, clinics, appointmentCategories] =
    await Promise.all([
      getPatientAppointment(appointmentId),
      getDoctors(),
      getClinics(),
      getAppointmentCategories(),
    ]);

  if (!appointment) {
    notFound();
  }

  if (!doctors || !clinics || !appointmentCategories) {
    return (
      <main>
        <p role="alert">Unable to load the appointment editing form.</p>
      </main>
    );
  }
  return (
    <EditAppointmentForm
      appointment={appointment}
      doctors={doctors}
      clinics={clinics}
      appointmentCategories={appointmentCategories}
    />
  );
}
