import SearchCard from "@/components/SearchCard";
import SearchForm from "@/components/SearchForm";
import { DOCTOR_SEARCH_API_URL } from "@/lib/constants";
import type { DoctorSearchResult } from "@/types/doctor";

type SearchPageProps = {
  searchParams: Promise<{ query?: string }>;
};

export default async function SearchPage({ searchParams }: SearchPageProps) {
  const { query = "" } = await searchParams;

  let searchResults: DoctorSearchResult[] = [];
  let errorMessage = "";

  if (query.trim()) {
    const url = new URL(DOCTOR_SEARCH_API_URL);
    url.searchParams.set("query", query.trim());

    const response = await fetch(url, { cache: "no-store" });

    if (response.ok) {
      searchResults = await response.json();
    } else {
      errorMessage = await response.text();
    }
  }

  return (
    <div>
      <SearchForm query={query} />

      {searchResults.map((result) => (
        <SearchCard
          key={`${result.doctorFullName}-${result.clinicName}`}
          result={result}
        />
      ))}
      {errorMessage && <p className="text-red-500">{errorMessage}</p>}
    </div>
  );
}
