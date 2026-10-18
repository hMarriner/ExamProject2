"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { logoutUser } from "@/modules/auth/actions";

const MainNav = ({
  isLoggedIn,
}: Readonly<{
  isLoggedIn: boolean;
}>) => {
  const pathname = usePathname();

  return (
    <nav className="flex items-center gap-4 text-white">
      {isLoggedIn ? (
        <form action={logoutUser}>
          <button type="submit">Logout</button>
        </form>
      ) : (
        <Link
          href="/login"
          className={`hover:text-background px-2 transition-colors duration-300 
                    ${pathname === "/login" ? "underline text-background-dark" : ""}`}
        >
          Login
        </Link>
      )}
      <Link
        href="/register"
        className={`hover:text-background px-2 transition-colors duration-300 
                    ${pathname === "/register" ? "underline text-background-dark" : ""}`}
      >
        Register
      </Link>

      <Link
        href="/search"
        className={`hover:text-background px-2 transition-colors duration-300 
                    ${pathname === "/search" ? "underline text-background-dark" : ""}`}
      >
        Search
      </Link>

      <Link
        href="/book"
        className={`hover:text-background px-2 transition-colors duration-300 
                    ${pathname === "/book" ? "underline text-background-dark" : ""}`}
      >
        Book
      </Link>

      <Link
        href="/appointments"
        className={`hover:text-background px-2 transition-colors duration-300 
                    ${pathname === "/appointments" ? "underline text-background-dark" : ""}`}
      >
        My Appointments
      </Link>

      <Link
        href="/myprofile"
        className={`hover:text-background px-2 transition-colors duration-300 
                    ${pathname === "/myprofile" ? "underline text-background-dark" : ""}`}
      >
        My Profile
      </Link>
    </nav>
  );
};

export default MainNav;
