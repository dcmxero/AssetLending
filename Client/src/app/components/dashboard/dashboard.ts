import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { StatisticsDto, PaginatedList } from '../../models/common.model';
import { AssetDto, AssetCategoryDto } from '../../models/asset.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <div class="page">
      @if (stats) {
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-value">{{ stats.totalAssets }}</div>
            <div class="stat-label">Total Assets</div>
          </div>
          <div class="stat-card">
            <div class="stat-value">{{ stats.totalUsers }}</div>
            <div class="stat-label">Users</div>
          </div>
          <div class="stat-card">
            <div class="stat-value">{{ stats.activeLoans }}</div>
            <div class="stat-label">Active Loans</div>
          </div>
          <div class="stat-card overdue">
            <div class="stat-value">{{ stats.overdueLoans }}</div>
            <div class="stat-label">Overdue</div>
          </div>
          <div class="stat-card">
            <div class="stat-value">{{ stats.activeReservations }}</div>
            <div class="stat-label">Reservations</div>
          </div>
        </div>
      }

      <h2>Assets</h2>
      <div class="toolbar">
        <div class="filters">
          <select [(ngModel)]="statusFilter" (ngModelChange)="loadAssets()">
            <option value="">All Statuses</option>
            <option value="Available">Available</option>
            <option value="Loaned">Loaned</option>
            <option value="Reserved">Reserved</option>
          </select>
          <select [(ngModel)]="categoryFilter" (ngModelChange)="loadAssets()">
            <option [ngValue]="null">All Categories</option>
            @for (cat of categories; track cat.id) {
              <option [ngValue]="cat.id">{{ cat.name }}</option>
            }
          </select>
        </div>
        <a routerLink="/assets/new" class="btn btn-primary">+ New Asset</a>
      </div>

      @if (assets) {
        <div class="card">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Category</th>
                <th>Serial Number</th>
                <th>Status</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (asset of assets.data; track asset.id) {
                <tr>
                  <td>{{ asset.name }}</td>
                  <td>{{ asset.assetCategoryName }}</td>
                  <td>{{ asset.serialNumber || '-' }}</td>
                  <td><span class="badge" [class]="'badge-' + asset.status.toLowerCase()">{{ asset.status }}</span></td>
                  <td><a [routerLink]="['/assets', asset.id]" class="btn btn-sm">Detail</a></td>
                </tr>
              }
              @if (assets.data.length === 0) {
                <tr><td colspan="5" class="empty">No assets found.</td></tr>
              }
            </tbody>
          </table>
        </div>
        @if (assets.totalPages > 1) {
          <div class="pagination">
            <button (click)="page = page - 1; loadAssets()" [disabled]="!assets.hasPreviousPage">Previous</button>
            <span>Page {{ assets.pageIndex }} of {{ assets.totalPages }}</span>
            <button (click)="page = page + 1; loadAssets()" [disabled]="!assets.hasNextPage">Next</button>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .page { padding: 24px; max-width: 1000px; margin: 0 auto; }
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
      gap: 12px;
      margin-bottom: 24px;
    }
    .stat-card {
      background: #fff; border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      padding: 16px; text-align: center;
    }
    .stat-card.overdue { border-left: 4px solid #e53935; }
    .stat-value { font-size: 1.75rem; font-weight: 700; color: #1a1a2e; }
    .stat-label { font-size: 0.8rem; color: #888; margin-top: 4px; }
    .stat-card.overdue .stat-value { color: #e53935; }

    h2 { font-size: 1.15rem; color: #1a1a2e; margin-bottom: 12px; }
    .toolbar { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
    .filters { display: flex; gap: 10px; }
    .filters select { padding: 7px 10px; border: 1px solid #ddd; border-radius: 6px; font-size: 0.85rem; }

    .card { background: #fff; border-radius: 10px; box-shadow: 0 2px 8px rgba(0,0,0,0.08); overflow: hidden; }
    table { width: 100%; border-collapse: collapse; }
    th { text-align: left; padding: 10px 16px; font-size: 0.8rem; color: #888; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 1px solid #eee; }
    td { padding: 10px 16px; border-bottom: 1px solid #f5f5f5; font-size: 0.9rem; }
    .empty { text-align: center; color: #888; padding: 24px; }

    .badge { display: inline-block; padding: 3px 10px; border-radius: 12px; font-size: 0.8rem; font-weight: 600; }
    .badge-available { background: #e8f5e9; color: #2e7d32; }
    .badge-loaned { background: #fff3e0; color: #e65100; }
    .badge-reserved { background: #e3f2fd; color: #1565c0; }

    .btn { padding: 6px 14px; border: none; border-radius: 6px; cursor: pointer; text-decoration: none; font-size: 0.85rem; }
    .btn-primary { background: #1a1a2e; color: white; }
    .btn-sm { background: #f0f0f0; color: #333; }

    .pagination { display: flex; justify-content: center; align-items: center; gap: 16px; margin-top: 12px; }
    .pagination button { padding: 6px 14px; border: 1px solid #ddd; border-radius: 6px; background: white; cursor: pointer; font-size: 0.85rem; }
    .pagination button:disabled { opacity: 0.5; cursor: not-allowed; }
  `]
})
export class DashboardComponent implements OnInit {
  stats: StatisticsDto | null = null;
  assets: PaginatedList<AssetDto> | null = null;
  categories: AssetCategoryDto[] = [];
  statusFilter = '';
  categoryFilter: number | null = null;
  page = 1;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getStatistics().subscribe({
      next: (data) => this.stats = data,
      error: (err) => console.error('Failed to load statistics', err)
    });
    this.api.getCategories().subscribe(c => this.categories = c);
    this.loadAssets();
  }

  loadAssets(): void {
    this.api.getAssets(this.page, 10, this.statusFilter || undefined, this.categoryFilter || undefined)
      .subscribe(r => this.assets = r);
  }
}
