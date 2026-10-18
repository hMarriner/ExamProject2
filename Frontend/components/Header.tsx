import Link from "next/link";
import Image from "next/image";
import { cookies } from "next/headers";
import MainNav from "./MainNav";

const Header = async () => {
  const cookieStore = await cookies();
  const token = cookieStore.get("token");

  return (
    <header className="container h-full mx-auto flex justify-between items-center">
      <Link href="/" aria-label="Home">
        <Image
          src="/ClinicLogo.png"
          alt="Clinic Appointment Booking App"
          width={120}
          height={40}
          className="h-20 w-auto object-contain"
          priority
        />
      </Link>
      <MainNav isLoggedIn={Boolean(token)} />
    </header>
  );
};

export default Header;
