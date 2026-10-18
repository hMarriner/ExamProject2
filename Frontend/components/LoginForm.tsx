"use client";

import { useActionState } from "react";
import { loginUser } from "@/modules/auth/actions";
import Button from "./Button";
import Input from "./Input";

const initialState = {
  message: "",
};

function LoginForm() {
  const [state, dispatch] = useActionState(loginUser, initialState);
  return (
    <form action={dispatch} className="max-w-md mx-auto">
      <Input
        id="email"
        name="email"
        label="Email"
        placeholder="Your email"
        type="text"
        autoComplete="email"
        required
      />
      <Input
        id="password"
        name="password"
        label="Password"
        type="password"
        placeholder="Your password"
        autoComplete="current-password"
        required
      />
      <Button type="submit">Login</Button>
      <div>{state?.message}</div>
    </form>
  );
}

export default LoginForm;
