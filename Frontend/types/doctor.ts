export type Doctor = {
  doctorId: number;
  firstName: string;
  lastName: string;
  specialityId: number;
  specialityName: string;
  clinicId: number;
  clinicName: string;
};

export type DoctorSearchResult = {
  doctorFullName: string;
  clinicName: string;
  specialityName: string;
};
