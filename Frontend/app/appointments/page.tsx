import AppointmentTable from "@/components/AppointmentTable";
import getPatientAppointments from "@/modules/booking/actions";

type PageProps = {
  readonly searchParams: Promise<{
    updated?: string;
    cancelled?: string;
  }>;
};

export default async function MyAppointmentsPage({ searchParams }: PageProps) {
  const { updated, cancelled } = await searchParams;
  const appointments = await getPatientAppointments();

  if (appointments === null) {
    return (
      <main>
        <h1 className="mb-6 text-3xl font-bold items-center text-center">
          My Appointments
        </h1>
        <p role="alert">Unable to load your appointments.</p>
      </main>
    );
  }

  let notificationMessage = "";
  if (updated === "1") {
    notificationMessage = "Appointment updated successfully.";
  } else if (cancelled === "1") {
    notificationMessage = "Appointment cancelled successfully.";
  }

  return (
    <main className="max-w-4xl mx-auto">
      {notificationMessage && (
        <div
          role="status"
          aria-live="polite"
          className="mt-4 rounded-md border border-gray-300 bg-blue-50 px-4 py-3 font-semibold text-center text-blue-900"
        >
          {notificationMessage}
        </div>
      )}

      <h1 className="mb-6 text-3xl font-bold items-center text-center">
        My Appointments
      </h1>

      {appointments.length === 0 ? (
        <p>You have no upcoming appointments.</p>
      ) : (
        <AppointmentTable appointments={appointments} />
      )}
    </main>
  );
}
