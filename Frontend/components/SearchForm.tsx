import Input from "./Input";
import Button from "./Button";

export default function SearchForm({ query }: { readonly query: string }) {
  return (
    <form method="GET" className="mx-auto max-w-md space-y-2">
      <Input
        id="query"
        name="query"
        defaultValue={query}
        placeholder="Enter doctors first or last name"
        className="flex-1"
      />

      <Button type="submit">Search</Button>
    </form>
  );
}
