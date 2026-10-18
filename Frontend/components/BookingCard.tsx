import { cookies } from "next/headers";
import BookingForm from "@/components/BookingForm";
import {
  getAppointmentCategories,
  getClinics,
  getDoctors,
} from "@/modules/booking/actions";

export default async function Home() {
  const [doctors, clinics, categories] = await Promise.all([
    getDoctors(),
    getClinics(),
    getAppointmentCategories(),
  ]);

  if (doctors === null || clinics === null || categories === null) {
    return (
      <main>
        <p role="alert">
          Unable to load booking options. Please try again later.
        </p>
      </main>
    );
  }

  const isLoggedIn = Boolean((await cookies()).get("token"));

  return (
    <main className="flex flex-col flex-1 items-center justify-center bg-secondary-light px-6 py-16 text-black">
      <section className="mx-auto w-full max-w-md rounded-lg border border-gray-200 bg-white p-6 shadow-sm">
        <div className="w-full max-w-3xl space-y-8 text-center">
          <h1 className="text-3xl font-bold">
            Welcome to the Appointment Booking System
          </h1>
          <h2 className="text-xl font-bold text-gray-600">
            Choose a clinic, doctor, appointment category, and time.
          </h2>
        </div>

        <BookingForm
          clinics={clinics}
          doctors={doctors}
          categories={categories}
          isLoggedIn={isLoggedIn}
        />
      </section>
    </main>
  );
}
