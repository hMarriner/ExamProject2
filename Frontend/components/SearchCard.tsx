import type { DoctorSearchResult } from "../types/doctor";
import Card from "./Card";

export default function SearchCard({ result }: { readonly result: DoctorSearchResult }) {
  const title = result.doctorFullName;
  const subtitle = `Clinic: ${result.clinicName} Speciality: ${result.specialityName}`;

  return <Card title={title} subtitle={subtitle} />;
}
