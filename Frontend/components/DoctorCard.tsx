import Card from "./Card";
import type { Doctor } from "../types/doctor";

export default function DoctorCard({ doctor }: { readonly doctor: Doctor }) {
  return (
    <Card
      title={`${doctor.firstName} ${doctor.lastName}`}
      subtitle={`${doctor.specialityName} - ${doctor.clinicName}`}
    />
  );
}
