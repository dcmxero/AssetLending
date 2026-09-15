import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedList, StatisticsDto } from '../models/common.model';
import { AssetDto, CreateAssetDto, AssetCategoryDto } from '../models/asset.model';
import { LoanDto, CreateLoanDto } from '../models/loan.model';
import { ReservationDto, CreateReservationDto } from '../models/reservation.model';
import { UserDto, CreateUserDto } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = 'https://localhost:7197/api';

  constructor(private http: HttpClient) {}

  getUsers(page = 1, pageSize = 10): Observable<PaginatedList<UserDto>> {
    return this.http.get<PaginatedList<UserDto>>(`${this.baseUrl}/users`, {
      params: new HttpParams().set('page', page).set('pageSize', pageSize)
    });
  }

  createUser(dto: CreateUserDto): Observable<UserDto> {
    return this.http.post<UserDto>(`${this.baseUrl}/users`, dto);
  }

  getCategories(): Observable<AssetCategoryDto[]> {
    return this.http.get<AssetCategoryDto[]>(`${this.baseUrl}/assetcategories`);
  }

  getAssets(page = 1, pageSize = 10, status?: string, categoryId?: number): Observable<PaginatedList<AssetDto>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (status) { params = params.set('status', status); }
    if (categoryId) { params = params.set('categoryId', categoryId); }
    return this.http.get<PaginatedList<AssetDto>>(`${this.baseUrl}/assets`, { params });
  }

  getAsset(id: number): Observable<AssetDto> {
    return this.http.get<AssetDto>(`${this.baseUrl}/assets/${id}`);
  }

  createAsset(dto: CreateAssetDto): Observable<AssetDto> {
    return this.http.post<AssetDto>(`${this.baseUrl}/assets`, dto);
  }

  getAssetLoans(assetId: number, page = 1, pageSize = 10): Observable<PaginatedList<LoanDto>> {
    return this.http.get<PaginatedList<LoanDto>>(`${this.baseUrl}/assets/${assetId}/loans`, {
      params: new HttpParams().set('page', page).set('pageSize', pageSize)
    });
  }

  getActiveLoans(): Observable<LoanDto[]> {
    return this.http.get<LoanDto[]>(`${this.baseUrl}/loans/active`);
  }

  getOverdueLoans(): Observable<LoanDto[]> {
    return this.http.get<LoanDto[]>(`${this.baseUrl}/loans/overdue`);
  }

  createLoan(dto: CreateLoanDto): Observable<LoanDto> {
    return this.http.post<LoanDto>(`${this.baseUrl}/loans`, dto);
  }

  returnLoan(loanId: number): Observable<LoanDto> {
    return this.http.put<LoanDto>(`${this.baseUrl}/loans/${loanId}/return`, {});
  }

  createReservation(dto: CreateReservationDto): Observable<ReservationDto> {
    return this.http.post<ReservationDto>(`${this.baseUrl}/reservations`, dto);
  }

  cancelReservation(reservationId: number): Observable<ReservationDto> {
    return this.http.put<ReservationDto>(`${this.baseUrl}/reservations/${reservationId}/cancel`, {});
  }

  getStatistics(): Observable<StatisticsDto> {
    return this.http.get<StatisticsDto>(`${this.baseUrl}/statistics`);
  }
}
