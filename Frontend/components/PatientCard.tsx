import type { Patient } from "../types/patient";
import Card from "./Card";

export default function PatientCard({ patient }: { patient: Patient }) {
  const birthDateLabel = patient.birthDate
    ? new Date(patient.birthDate).toLocaleString()
    : "N/A";

  return (
    <Card
      title={`${patient.firstName} ${patient.lastName}`}
      subtitle={`Email: ${patient.email} Date of Birth: ${birthDateLabel}`}
    />
  );
}
