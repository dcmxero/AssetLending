import type { PaginatedList, StatisticsDto } from '../models/common';
import type { AssetCategoryDto, AssetDto, CreateAssetDto } from '../models/asset';
import type { CreateLoanDto, LoanDto } from '../models/loan';
import type { CreateReservationDto, ReservationDto } from '../models/reservation';
import type { CreateUserDto, UserDto } from '../models/user';

const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7197/api';

/**
 * Error carrying the parsed problem-details body, so callers can pull out the
 * `error`, `errors` or `title` field the API returned.
 */
export class ApiError extends Error {
  constructor(
    readonly status: number,
    readonly body: unknown
  ) {
    super(`Request failed with status ${status}`);
    this.name = 'ApiError';
  }
}

type QueryValue = string | number | undefined;

function buildUrl(path: string, params?: Record<string, QueryValue>): string {
  const url = new URL(`${baseUrl}${path}`, window.location.origin);
  for (const [key, value] of Object.entries(params ?? {})) {
    if (value !== undefined) {
      url.searchParams.set(key, String(value));
    }
  }
  return url.toString();
}

async function request<T>(
  path: string,
  init: RequestInit & { params?: Record<string, QueryValue> } = {}
): Promise<T> {
  const { params, ...rest } = init;
  const response = await fetch(buildUrl(path, params), {
    ...rest,
    headers: rest.body ? { 'Content-Type': 'application/json', ...rest.headers } : rest.headers,
  });

  if (!response.ok) {
    throw new ApiError(response.status, await response.json().catch(() => null));
  }

  if (response.status === 204) {
    return undefined as T;
  }
  return (await response.json()) as T;
}

const post = <T>(path: string, body: unknown) =>
  request<T>(path, { method: 'POST', body: JSON.stringify(body) });

const put = <T>(path: string, body: unknown) =>
  request<T>(path, { method: 'PUT', body: JSON.stringify(body) });

export const api = {
  getUsers: (page = 1, pageSize = 10) =>
    request<PaginatedList<UserDto>>('/users', { params: { page, pageSize } }),

  createUser: (dto: CreateUserDto) => post<UserDto>('/users', dto),

  getCategories: () => request<AssetCategoryDto[]>('/assetcategories'),

  getAssets: (page = 1, pageSize = 10, status?: string, categoryId?: number) =>
    request<PaginatedList<AssetDto>>('/assets', {
      params: { page, pageSize, status, categoryId },
    }),

  getAsset: (id: number) => request<AssetDto>(`/assets/${id}`),

  createAsset: (dto: CreateAssetDto) => post<AssetDto>('/assets', dto),

  getAssetLoans: (assetId: number, page = 1, pageSize = 10) =>
    request<PaginatedList<LoanDto>>(`/assets/${assetId}/loans`, { params: { page, pageSize } }),

  getActiveLoans: () => request<LoanDto[]>('/loans/active'),

  getOverdueLoans: () => request<LoanDto[]>('/loans/overdue'),

  createLoan: (dto: CreateLoanDto) => post<LoanDto>('/loans', dto),

  returnLoan: (loanId: number) => put<LoanDto>(`/loans/${loanId}/return`, {}),

  createReservation: (dto: CreateReservationDto) => post<ReservationDto>('/reservations', dto),

  cancelReservation: (reservationId: number) =>
    put<ReservationDto>(`/reservations/${reservationId}/cancel`, {}),

  getStatistics: () => request<StatisticsDto>('/statistics'),
};
