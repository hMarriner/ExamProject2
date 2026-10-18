import Link from "next/link";

type CardProps = Readonly<{
  title: string;
  subtitle?: string;
  href?: string;
  children?: React.ReactNode;
  actions?: React.ReactNode;
}>;

export default function Card({
  title,
  subtitle,
  href,
  children,
  actions,
}: CardProps) {
  const content = (
    <>
      <h3 className="font-semibold">{title}</h3>
      {subtitle && <p className="text-sm text-gray-600">{subtitle}</p>}
      {children}
    </>
  );

  return (
    <article className="rounded border p-4 shadow-sm">
      {href ? <Link href={href}>{content}</Link> : content}
      {actions && <div className="mt-2 flex gap-2">{actions}</div>}
    </article>
  );
}
