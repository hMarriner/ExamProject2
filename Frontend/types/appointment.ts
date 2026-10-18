export type Appointment = {
  appointmentId: number;
  clinicId: number;
  clinicName: string;
  doctorId: number;
  doctorName: string;
  appointmentDate: string;
  durationInMinutes: number;
  patientId: number;
  patientName: string;
  appointmentCategoryId: number;
  appointmentCategoryName: string;
};

export type CreateAppointmentInput = {
  clinicId: number;
  doctorId: number;
  appointmentCategoryId: number;
  appointmentDate: string;
  durationInMinutes: number;
};

export type GuestAppointmentInput = CreateAppointmentInput & {
  firstName: string;
  lastName: string;
  email: string;
  birthDate?: string;
  gender?: string;
};
