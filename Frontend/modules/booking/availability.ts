import { APPOINTMENT_AVAILABILITY_API_URL } from "@/lib/constants";

export async function getAvailableSlots(
  doctorId: string,
  date: string,
  durationInMinutes: string,
): Promise<string[]> {
  const params = new URLSearchParams({
    doctorId,
    date,
    durationInMinutes,
  });

  const response = await fetch(
    `${APPOINTMENT_AVAILABILITY_API_URL}?${params.toString()}`,
  );
  if (!response.ok) {
    throw new Error("Failed to fetch available slots");
  }

  return response.json();
}
