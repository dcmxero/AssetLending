export interface PaginatedList<T> {
  data: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface StatisticsDto {
  totalAssets: number;
  totalUsers: number;
  activeLoans: number;
  overdueLoans: number;
  activeReservations: number;
}
