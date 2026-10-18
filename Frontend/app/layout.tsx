import type { Metadata } from "next";
import Header from "../components/Header";
import Footer from "../components/Footer";
import "./globals.css";

export const metadata: Metadata = {
  title: "Clinic Appointment Booking",
  description: "Book and manage your clinic appointments.",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html lang="en" className="h-full">
      <body className="min-h-screen flex flex-col">
        <div className="h-20 border-b border-black bg-primary text-white">
          <div className="container h-full mx-auto flex justify-between items-center p-4">
            <Header />
          </div>
        </div>
        <main className="flex-1 bg-secondary-light text-black">{children}</main>

        <footer className="h-32 border-t border-black bg-primary text-white">
          <div className="container h-full mx-auto flex justify-between items-center p-4">
            <Footer />
          </div>
        </footer>
      </body>
    </html>
  );
}
