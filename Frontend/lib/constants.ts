const BASE_API_URL =
  process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5054/api";

export const LOGIN_API_URL = `${BASE_API_URL}/auth/login`;
export const REGISTER_API_URL = `${BASE_API_URL}/auth/register`;
export const PATIENT_API_URL = `${BASE_API_URL}/patient/me`;
export const DOCTOR_API_URL = `${BASE_API_URL}/doctor`;
export const APPOINTMENT_API_URL = `${BASE_API_URL}/appointment`;
export const APPOINTMENT_AVAILABILITY_API_URL = `${BASE_API_URL}/appointment/availability`;
export const GUEST_BOOKING_API_URL = `${BASE_API_URL}/appointment/guest-booking`;
export const SPECIALITY_API_URL = `${BASE_API_URL}/speciality`;
export const APPOINTMENT_CATEGORY_API_URL = `${BASE_API_URL}/appointmentcategory`;
export const CLINIC_API_URL = `${BASE_API_URL}/clinic`;
export const DOCTOR_SEARCH_API_URL = `${BASE_API_URL}/doctor/search`;
