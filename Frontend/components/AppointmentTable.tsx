import type { Appointment } from "../types/appointment";
import Link from "next/link";
import { cancelAppointment } from "../modules/booking/actions";

export default function AppointmentTable({
  appointments,
}: {
  readonly appointments: Appointment[];
}) {
  return (
    <div className="mx-auto w-full max-w-6xl overflow-x-auto rounded-md border border-black-300 bg-secondary shadow-sm">
      <table className="w-full border-collapse text-left">
        <thead className="bg-primary-light border-b border-gray-400">
          <tr>
            <th scope="col" className="border border-gray-400 ">
              Doctor
            </th>
            <th scope="col" className="border border-gray-400 ">
              Clinic
            </th>
            <th scope="col" className="border border-gray-400 ">
              Appointment Category
            </th>
            <th scope="col" className="border border-gray-400 ">
              Date
            </th>
            <th scope="col" className="border border-gray-400 ">
              Duration
            </th>
            <th scope="col" className="border border-gray-400 ">
              Actions
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200">
          {appointments.map((appointment) => (
            <tr key={appointment.appointmentId}>
              <td className="border border-gray-400 ">
                {appointment.doctorName}
              </td>
              <td className="border border-gray-400 ">
                {appointment.clinicName}
              </td>
              <td className="border border-gray-400 ">
                {appointment.appointmentCategoryName}
              </td>
              <td className="border border-gray-400 ">
                {new Date(appointment.appointmentDate).toLocaleString()}
              </td>
              <td className="border border-gray-400 ">
                {appointment.durationInMinutes} minutes
              </td>
              <td className="border border-gray-400">
                <div className="flex items-center gap-2 whitespace-nowrap">
                  <Link
                    href={`/appointments/edit/${appointment.appointmentId}`}
                    className="inline-flex rounded border bg-gray-500 px-3 py-1 text-white"
                  >
                    Edit
                  </Link>{" "}
                  <form action={cancelAppointment} className="m-0">
                    <input
                      type="hidden"
                      name="appointmentId"
                      value={appointment.appointmentId}
                    />
                    <button
                      type="submit"
                      className="inline-flex rounded border bg-red-500 px-3 py-1 text-white"
                    >
                      Cancel
                    </button>
                  </form>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
