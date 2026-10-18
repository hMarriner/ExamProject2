"use client";
import { Patient } from "@/types/patient";
import { useActionState } from "react";
import { registerUser, updateUser } from "@/modules/auth/actions";

import Input from "./Input";
import Button from "./Button";

const initialState = {
  message: "",
};

type RegisterFormProps = {
  readonly mode: "register" | "update";
  readonly patient?: Patient;
};

function RegisterForm({ mode, patient }: RegisterFormProps) {
  const action = mode === "register" ? registerUser : updateUser;
  const [state, dispatch] = useActionState(action, initialState);
  return (
    <main className="flex flex-col flex-1 items-center justify-center bg-secondary-light px-6 py-16 text-black">
      <section className="mx-auto w-full max-w-md rounded-lg border border-gray-200 bg-white p-6 shadow-sm">
        <div className="w-full max-w-3xl space-y-8 text-center">
          <h1 className="text-3xl font-bold text-center">
            {mode === "register" ? "Registration Form" : "Update Your Profile"}
          </h1>
        </div>
    <form action={dispatch} className="max-w-md mx-auto">
      {state?.message && (
        <div
          role="status"
          aria-live="polite"
          className="mt-4 rounded-md border border-blue-300 bg-blue-50 px-4 py-3 font-semibold text-center text-blue-900"
        >
          {state?.message}
        </div>
      )}
      <Input
        id="email"
        name="email"
        label="Email"
        defaultValue={mode === "update" ? (patient?.email ?? "") : ""}
        placeholder="Your email"
        type="text"
        autoComplete="email"
        required
      />

      {mode === "register" && (
        <Input
          id="password"
          name="password"
          label="Password"
          type="password"
          placeholder="Your password"
          autoComplete="new-password"
          required
        />
      )}
      <Input
        id="firstName"
        name="firstName"
        label="First Name"
        type="text"
        defaultValue={mode === "update" ? (patient?.firstName ?? "") : ""}
        placeholder="Your first name"
        autoComplete="given-name"
        required
      />
      <Input
        id="lastName"
        name="lastName"
        label="Last Name"
        type="text"
        defaultValue={mode === "update" ? (patient?.lastName ?? "") : ""}
        placeholder="Your last name"
        autoComplete="surname"
        required
      />

      <Input
        id="birthDate"
        name="birthDate"
        label="Date of Birth"
        type="date"
        defaultValue={
          mode === "update" ? (patient?.birthDate?.slice(0, 10) ?? "") : ""
        }
        required
      />

      <Input
        id="gender"
        name="gender"
        label="Gender"
        type="text"
        defaultValue={mode === "update" ? (patient?.gender ?? "") : ""}
        required={mode === "register"}
      />

      <Input
        id="socialSecurityNumber"
        name="socialSecurityNumber"
        label="Social Security Number"
        maxLength={11}
        type="text"
        required={mode === "register"}
      />

      <Input
        id="religion"
        name="religion"
        label="Religion"
        type="text"
        required={mode === "register"}
      />

      <Input
        id="taxNumber"
        name="taxNumber"
        label="Tax Number"
        type="text"
        required={mode === "register"}
      />

      <Input
        id="driversLicenseNumber"
        name="driversLicenseNumber"
        label="Drivers License Number"
        type="text"
        required={mode === "register"}
      />

      <Input
        id="medicalInsuranceNumber"
        name="medicalInsuranceNumber"
        label="Medical Insurance Number"
        type="text"
        required={mode === "register"}
      />
      <Button type="submit">
        {mode === "register" ? "Register" : "Update"}
      </Button>
    </form>
  </section>
</main>
  );
}

export default RegisterForm;
