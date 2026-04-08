import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { AssetDto } from '../../models/asset.model';
import { LoanDto } from '../../models/loan.model';
import { UserDto } from '../../models/user.model';
import { PaginatedList } from '../../models/common.model';

@Component({
  selector: 'app-asset-detail',
  standalone: true,
  imports: [FormsModule, DatePipe, RouterLink],
  template: `
    <div class="page">
      @if (asset) {
        <div class="back-link">
          <a routerLink="/dashboard">&larr; Back to Dashboard</a>
        </div>
        <h1>{{ asset.name }}</h1>

        <div class="top-row">
          <div class="card info-card">
            <h3>Details</h3>
            <div class="info-row">
              <span class="label">Category</span>
              <span>{{ asset.assetCategoryName }}</span>
            </div>
            <div class="info-row">
              <span class="label">Serial Number</span>
              <span>{{ asset.serialNumber ?? '-' }}</span>
            </div>
            <div class="info-row">
              <span class="label">Status</span>
              <span class="badge" [class]="'badge-' + asset.status.toLowerCase()">{{ asset.status }}</span>
            </div>
            <div class="info-row">
              <span class="label">Active</span>
              <span>{{ asset.isActive ? 'Yes' : 'No' }}</span>
            </div>
            @if (asset.description) {
              <div class="info-row">
                <span class="label">Description</span>
                <span>{{ asset.description }}</span>
              </div>
            }
          </div>

          <div class="card action-card">
            @if (asset.status === 'Available') {
              <h3>Action</h3>
              <div class="form-group">
                <label>Type</label>
                <select [(ngModel)]="actionType" name="actionType" (ngModelChange)="resetForm()">
                  <option value="" disabled>Select action</option>
                  <option value="loan">Loan (Checkout)</option>
                  <option value="reserve">Reserve</option>
                </select>
              </div>

              @if (actionType) {
                <div class="form-group">
                  <label>User</label>
                  <select [(ngModel)]="selectedUserId" name="selectedUser">
                    <option [ngValue]="0" disabled>Select user</option>
                    @for (u of users; track u.id) {
                      <option [ngValue]="u.id">{{ u.firstName }} {{ u.lastName }}</option>
                    }
                  </select>
                </div>
                <div class="form-group">
                  <label>{{ actionType === 'loan' ? 'Due Date' : 'Reserve Until' }}</label>
                  <input type="date" [(ngModel)]="selectedDate" name="selectedDate" />
                </div>

                @if (error) { <div class="msg error">{{ error }}</div> }
                @if (success) { <div class="msg success">{{ success }}</div> }

                <button
                  class="btn btn-primary"
                  (click)="submitAction()"
                  [disabled]="!selectedUserId || !selectedDate">
                  {{ actionType === 'loan' ? 'Checkout' : 'Reserve' }}
                </button>
              }
            } @else {
              <h3>Status</h3>
              <p class="status-info">
                This asset is currently <strong>{{ asset.status }}</strong> and cannot be loaned or reserved.
              </p>
              @if (error) { <div class="msg error">{{ error }}</div> }
              @if (success) { <div class="msg success">{{ success }}</div> }
            }
          </div>
        </div>

        <h2>Loan History</h2>
        <div class="card">
          <table>
            <thead>
              <tr>
                <th>Borrowed By</th>
                <th>Borrowed At</th>
                <th>Due Date</th>
                <th>Returned At</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              @for (loan of loanHistory?.data; track loan.id) {
                <tr>
                  <td>{{ loan.borrowedByName }}</td>
                  <td>{{ loan.borrowedAt | date:'mediumDate' }}</td>
                  <td>{{ loan.dueDate | date:'mediumDate' }}</td>
                  <td>{{ loan.returnedAt ? (loan.returnedAt | date:'mediumDate') : '-' }}</td>
                  <td>
                    <span class="badge" [class]="loan.status === 'Active' ? 'badge-loaned' : 'badge-available'">
                      {{ loan.status }}
                    </span>
                  </td>
                </tr>
              }
              @if (!loanHistory || loanHistory.data.length === 0) {
                <tr><td colspan="5" class="empty">No loan history.</td></tr>
              }
            </tbody>
          </table>
        </div>
      } @else {
        <p class="loading">Loading asset...</p>
      }
    </div>
  `,
  styles: [`
    .page { padding: 24px; max-width: 1000px; margin: 0 auto; }
    .back-link { margin-bottom: 12px; }
    .back-link a { color: #1a1a2e; text-decoration: none; font-size: 0.9rem; }
    .back-link a:hover { text-decoration: underline; }
    h1 { font-size: 1.5rem; margin-bottom: 16px; }
    h2 { font-size: 1.15rem; margin: 24px 0 12px; }
    h3 { font-size: 1rem; margin-bottom: 14px; color: #1a1a2e; }
    .loading { color: #888; }

    .top-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      margin-bottom: 8px;
    }
    .top-row > .card {
      min-height: 280px;
    }
    @media (max-width: 700px) {
      .top-row { grid-template-columns: 1fr; }
    }

    .card {
      background: #fff; border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08); overflow: hidden;
    }
    .info-card { padding: 20px; }
    .info-row { display: flex; gap: 12px; padding: 6px 0; font-size: 0.9rem; }
    .info-row .label { font-weight: 600; color: #888; min-width: 120px; }
    .badge { display: inline-block; padding: 4px 10px; border-radius: 12px; font-size: 0.8rem; font-weight: 600; }
    .badge-available { background: #e8f5e9; color: #2e7d32; }
    .badge-loaned { background: #fff3e0; color: #e65100; }
    .badge-reserved { background: #e3f2fd; color: #1565c0; }

    .action-card { padding: 20px; }
    .status-info { color: #666; font-size: 0.9rem; line-height: 1.5; }
    .form-group { margin-bottom: 12px; }
    .form-group label { display: block; font-size: 0.8rem; font-weight: 600; color: #555; margin-bottom: 4px; }
    .form-group input, .form-group select {
      width: 100%; padding: 8px 10px; border: 1px solid #ddd; border-radius: 6px; font-size: 0.9rem;
    }
    .btn-primary { background: #1a1a2e; color: #fff; border: none; padding: 8px 16px; border-radius: 6px; cursor: pointer; font-size: 0.9rem; font-weight: 500; }
    .btn-primary:disabled { opacity: 0.5; cursor: default; }
    .msg { font-size: 0.85rem; margin-bottom: 8px; padding: 8px; border-radius: 6px; }
    .error { color: #e53935; background: #ffeaea; }
    .success { color: #2e7d32; background: #e8f5e9; }

    table { width: 100%; border-collapse: collapse; }
    th { text-align: left; padding: 12px 16px; font-size: 0.8rem; color: #888;
         text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 1px solid #eee; }
    td { padding: 12px 16px; border-bottom: 1px solid #f5f5f5; font-size: 0.9rem; }
    .empty { text-align: center; color: #888; padding: 32px 16px; }
  `]
})
export class AssetDetailComponent implements OnInit {
  asset: AssetDto | null = null;
  loanHistory: PaginatedList<LoanDto> | null = null;
  users: UserDto[] = [];

  actionType: 'loan' | 'reserve' | '' = '';
  selectedUserId = 0;
  selectedDate = '';
  error = '';
  success = '';

  private assetId = 0;

  constructor(private api: ApiService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.assetId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadAsset();
    this.loadLoans();
    this.api.getUsers(1, 100).subscribe(res => this.users = res.data);
  }

  resetForm(): void {
    this.selectedUserId = 0;
    this.selectedDate = '';
    this.error = '';
    this.success = '';
  }

  submitAction(): void {
    if (this.actionType === 'loan') {
      this.checkout();
    } else if (this.actionType === 'reserve') {
      this.reserve();
    }
  }

  private loadAsset(): void {
    this.api.getAsset(this.assetId).subscribe({
      next: (data) => this.asset = data,
      error: (err) => console.error('Failed to load asset', err)
    });
  }

  private loadLoans(): void {
    this.api.getAssetLoans(this.assetId).subscribe({
      next: (data) => this.loanHistory = data,
      error: (err) => console.error('Failed to load loan history', err)
    });
  }

  private checkout(): void {
    this.error = '';
    this.success = '';
    this.api.createLoan({
      assetId: this.assetId,
      borrowedById: this.selectedUserId,
      dueDate: this.selectedDate
    }).subscribe({
      next: () => {
        this.success = 'Asset checked out successfully.';
        this.loadAsset();
        this.loadLoans();
      },
      error: (err) => this.error = this.extractError(err)
    });
  }

  private reserve(): void {
    this.error = '';
    this.success = '';
    this.api.createReservation({
      assetId: this.assetId,
      reservedById: this.selectedUserId,
      reservedUntil: this.selectedDate
    }).subscribe({
      next: () => {
        this.success = 'Asset reserved successfully.';
        this.loadAsset();
      },
      error: (err) => this.error = this.extractError(err)
    });
  }

  private extractError(err: any): string {
    if (err.error?.error) {
      return err.error.error;
    }
    if (err.error?.errors) {
      const messages = Object.values(err.error.errors).flat();
      return messages.join(' ');
    }
    if (err.error?.title) {
      return err.error.title;
    }
    return 'An unexpected error occurred.';
  }
}
