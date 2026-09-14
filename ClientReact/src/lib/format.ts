/**
 * Matches Angular's `date:'mediumDate'`, which renders 'MMM d, y' — e.g. "Sep 16, 2026".
 */
const mediumDate = new Intl.DateTimeFormat('en-US', {
  year: 'numeric',
  month: 'short',
  day: 'numeric',
});

export function formatMediumDate(value: string): string {
  return mediumDate.format(new Date(value));
}
