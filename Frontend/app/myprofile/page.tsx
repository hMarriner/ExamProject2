import { redirect } from "next/navigation";
import { cookies } from "next/headers";
import { PATIENT_API_URL } from "@/lib/constants";
import RegisterForm from "@/components/RegisterForm";
import { Patient } from "@/types/patient";

export default async function MyProfilePage() {
  const token = (await cookies()).get("token")?.value;
  if (!token) redirect("/login");

  const response = await fetch(PATIENT_API_URL, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    redirect("/login");
  }
  const patient = (await response.json()) as Patient;
  return <RegisterForm mode="update" patient={patient} />;
}
