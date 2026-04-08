import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { AssetDto, AssetCategoryDto } from '../../models/asset.model';
import { PaginatedList } from '../../models/common.model';

@Component({
  selector: 'app-asset-list',
  standalone: true,
  imports: [RouterLink, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Assets</h1>
        <a routerLink="/assets/new" class="btn btn-primary">+ New Asset</a>
      </div>

      <div class="filters">
        <select [(ngModel)]="statusFilter" (ngModelChange)="loadAssets()">
          <option value="">All Statuses</option>
          <option value="Available">Available</option>
          <option value="Loaned">Loaned</option>
          <option value="Reserved">Reserved</option>
        </select>
        <select [(ngModel)]="categoryFilter" (ngModelChange)="loadAssets()">
          <option [ngValue]="0">All Categories</option>
          @for (cat of categories; track cat.id) {
            <option [ngValue]="cat.id">{{ cat.name }}</option>
          }
        </select>
      </div>

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
            @for (asset of result?.data; track asset.id) {
              <tr>
                <td>{{ asset.name }}</td>
                <td>{{ asset.assetCategoryName }}</td>
                <td>{{ asset.serialNumber ?? '-' }}</td>
                <td>
                  <span class="badge" [class]="'badge-' + asset.status.toLowerCase()">
                    {{ asset.status }}
                  </span>
                </td>
                <td><a [routerLink]="['/assets', asset.id]" class="link">Detail</a></td>
              </tr>
            }
            @if (!result || result.data.length === 0) {
              <tr><td colspan="5" class="empty">No assets found.</td></tr>
            }
          </tbody>
        </table>
      </div>

      @if (result && result.totalPages > 1) {
        <div class="pagination">
          <button (click)="goToPage(page - 1)" [disabled]="!result.hasPreviousPage" class="btn btn-sm">Previous</button>
          <span class="page-info">Page {{ result.pageIndex }} of {{ result.totalPages }}</span>
          <button (click)="goToPage(page + 1)" [disabled]="!result.hasNextPage" class="btn btn-sm">Next</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .page { padding: 24px; max-width: 1000px; margin: 0 auto; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
    h1 { font-size: 1.5rem; }
    .filters { display: flex; gap: 12px; margin-bottom: 16px; }
    .filters select {
      padding: 8px 12px; border: 1px solid #ddd; border-radius: 6px;
      font-size: 0.9rem; background: #fff;
    }
    .card {
      background: #fff; border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08); overflow: hidden;
    }
    table { width: 100%; border-collapse: collapse; }
    th { text-align: left; padding: 12px 16px; font-size: 0.8rem; color: #888;
         text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 1px solid #eee; }
    td { padding: 12px 16px; border-bottom: 1px solid #f5f5f5; font-size: 0.9rem; }
    .empty { text-align: center; color: #888; padding: 32px 16px; }
    .badge {
      display: inline-block; padding: 4px 10px; border-radius: 12px;
      font-size: 0.8rem; font-weight: 600;
    }
    .badge-available { background: #e8f5e9; color: #2e7d32; }
    .badge-loaned { background: #fff3e0; color: #e65100; }
    .badge-reserved { background: #e3f2fd; color: #1565c0; }
    .link { color: #1a1a2e; text-decoration: none; font-weight: 500; }
    .link:hover { text-decoration: underline; }
    .btn { padding: 8px 16px; border: 1px solid #ddd; border-radius: 6px;
           background: #fff; cursor: pointer; font-size: 0.9rem; }
    .btn:disabled { opacity: 0.5; cursor: default; }
    .btn-primary { background: #1a1a2e; color: #fff; border: none; text-decoration: none;
                   padding: 8px 16px; border-radius: 6px; font-size: 0.9rem; font-weight: 500; }
    .btn-sm { padding: 6px 12px; font-size: 0.85rem; }
    .pagination { display: flex; align-items: center; justify-content: center; gap: 16px; margin-top: 16px; }
    .page-info { font-size: 0.9rem; color: #666; }
  `]
})
export class AssetListComponent implements OnInit {
  result: PaginatedList<AssetDto> | null = null;
  categories: AssetCategoryDto[] = [];
  page = 1;
  statusFilter = '';
  categoryFilter = 0;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getCategories().subscribe(cats => this.categories = cats);
    this.loadAssets();
  }

  loadAssets(): void {
    this.page = 1;
    this.fetchAssets();
  }

  goToPage(p: number): void {
    this.page = p;
    this.fetchAssets();
  }

  private fetchAssets(): void {
    const cat = this.categoryFilter || undefined;
    const status = this.statusFilter || undefined;
    this.api.getAssets(this.page, 10, status, cat).subscribe({
      next: (data) => this.result = data,
      error: (err) => console.error('Failed to load assets', err)
    });
  }
}
