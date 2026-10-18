import React from "react";

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  id: string;
}

export default function Select({
  label,
  id,
  className,
  children,
  ...rest
}: SelectProps) {
  return (
    <div className="mb-4">
      {label && (
        <label
          htmlFor={id}
          className="mb-1 block text-base font-semibold text-gray-900"
        >
          {label}
        </label>
      )}
      <select
        id={id}
        className={`mt-2 block w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring ${className ?? ""}`}
        {...rest}
      >
        {children}
      </select>
    </div>
  );
}
