"use server";

import { cookies } from "next/headers";
import {
  LOGIN_API_URL,
  PATIENT_API_URL,
  REGISTER_API_URL,
} from "@/lib/constants";
import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";

type LoginState = {
  message: string;
};

type RegisterState = {
  message: string;
};

export async function loginUser(
  previousState: LoginState,
  formData: FormData,
): Promise<LoginState> {
  const emailValue = formData.get("email");
  const email = typeof emailValue === "string" ? emailValue : "";
  const passwordValue = formData.get("password");
  const password = typeof passwordValue === "string" ? passwordValue : "";

  if (!email || !password) {
    return {
      ...previousState,
      message: "Email and password are required",
    };
  }

  try {
    const response = await fetch(LOGIN_API_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      throw new Error(`API responded with status ${response.status}`);
    }
    const json = await response.json();
    (await cookies()).set("token", json.token, { httpOnly: true });
  } catch (error) {
    console.log(error);
    return { message: (error as Error).message };
  }
  redirect("/");
}

export async function logoutUser() {
  (await cookies()).delete("token");
  redirect("/login");
}

export async function registerUser(
  previousState: RegisterState,
  formData: FormData,
): Promise<RegisterState> {
  const response = await fetch(REGISTER_API_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      email: formData.get("email"),
      password: formData.get("password"),
      firstName: formData.get("firstName"),
      lastName: formData.get("lastName"),
      driversLicenseNumber: formData.get("driversLicenseNumber"),
      birthDate: formData.get("birthDate"),
      taxNumber: formData.get("taxNumber"),
      socialSecurityNumber: formData.get("socialSecurityNumber"),
      medicalInsuranceNumber: formData.get("medicalInsuranceNumber"),
      religion: formData.get("religion"),
      gender: formData.get("gender"),
    }),
  });

  if (!response.ok) {
    const message = await response.text();
    return { message: message || "Registration failed" };
  }

  redirect("/login");
}

export async function updateUser(
  previousState: RegisterState,
  formData: FormData,
): Promise<RegisterState> {
  const token = (await cookies()).get("token")?.value;
  if (!token) {
    redirect("/login");
  }

  const response = await fetch(PATIENT_API_URL, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      email: formData.get("email"),
      firstName: formData.get("firstName"),
      lastName: formData.get("lastName"),
      driversLicenseNumber: formData.get("driversLicenseNumber"),
      birthDate: formData.get("birthDate"),
      taxNumber: formData.get("taxNumber"),
      socialSecurityNumber: formData.get("socialSecurityNumber"),
      medicalInsuranceNumber: formData.get("medicalInsuranceNumber"),
      religion: formData.get("religion"),
      gender: formData.get("gender"),
    }),
  });
  if (!response.ok) {
    const message = await response.text();
    return { message: message || "Update failed" };
  }
  revalidatePath("/myprofile");
  return { message: "Update successful" };
}
