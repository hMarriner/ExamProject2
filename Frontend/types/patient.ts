export type Patient = {
  patientId: number;
  firstName: string;
  lastName: string;
  email: string;
  birthDate?: string;
  gender?: string;
  isRegistered: boolean;
};

export type RegisterPatientInput = {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  birthDate?: string;
  gender?: string;
  socialSecurityNumber?: string;
  taxNumber?: string;
  religion?: string;
  driversLicenseNumber?: string;
  medicalInsuranceNumber?: string;
};
