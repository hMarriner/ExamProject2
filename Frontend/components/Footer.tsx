const Footer = () => {
  const currentYear = new Date().getFullYear();
  return (
    <footer className="container h-full mx-auto flex justify-between items-center text-center">
      <p>Copyright © {currentYear}</p>
    </footer>
  );
};
export default Footer;
