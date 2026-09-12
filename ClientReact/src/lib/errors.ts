import { ApiError } from '../api/client';

interface ProblemBody {
  error?: string;
  title?: string;
  errors?: Record<string, string[]>;
}

function problemBody(err: unknown): ProblemBody {
  return err instanceof ApiError && err.body !== null && typeof err.body === 'object'
    ? (err.body as ProblemBody)
    : {};
}

/** Message shown by the create forms: server message, then problem title, then a fallback. */
export function createErrorMessage(err: unknown, fallback: string): string {
  const body = problemBody(err);
  return body.error || body.title || fallback;
}

/** Message shown by the asset actions, which also surface per-field validation messages. */
export function actionErrorMessage(err: unknown): string {
  const body = problemBody(err);
  if (body.error) {
    return body.error;
  }
  if (body.errors) {
    return Object.values(body.errors).flat().join(' ');
  }
  if (body.title) {
    return body.title;
  }
  return 'An unexpected error occurred.';
}
