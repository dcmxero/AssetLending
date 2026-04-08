import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { AssetCategoryDto } from '../../models/asset.model';

@Component({
  selector: 'app-asset-create',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="page">
      <h1>Create Asset</h1>
      <div class="card">
        <form (ngSubmit)="submit()">
          <div class="form-group">
            <label>Name</label>
            <input [(ngModel)]="name" name="name" required placeholder="Asset name" />
          </div>
          <div class="form-group">
            <label>Description</label>
            <textarea [(ngModel)]="description" name="description" rows="3" placeholder="Optional description"></textarea>
          </div>
          <div class="form-group">
            <label>Serial Number</label>
            <input [(ngModel)]="serialNumber" name="serialNumber" placeholder="Optional serial number" />
          </div>
          <div class="form-group">
            <label>Category</label>
            <select [(ngModel)]="categoryId" name="categoryId" required>
              <option [ngValue]="0" disabled>Select a category</option>
              @for (cat of categories; track cat.id) {
                <option [ngValue]="cat.id">{{ cat.name }}</option>
              }
            </select>
          </div>
          @if (error) {
            <div class="error">{{ error }}</div>
          }
          <div class="form-actions">
            <button type="button" class="btn" (click)="cancel()">Cancel</button>
            <button type="submit" class="btn btn-primary" [disabled]="!name || !categoryId">Create</button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .page { padding: 24px; max-width: 600px; margin: 0 auto; }
    h1 { font-size: 1.5rem; margin-bottom: 16px; }
    .card {
      background: #fff; border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08); padding: 24px;
    }
    .form-group { margin-bottom: 16px; }
    .form-group label { display: block; font-size: 0.85rem; font-weight: 600; color: #555; margin-bottom: 6px; }
    .form-group input, .form-group textarea, .form-group select {
      width: 100%; padding: 10px 12px; border: 1px solid #ddd; border-radius: 6px;
      font-size: 0.9rem; font-family: inherit;
    }
    .form-actions { display: flex; gap: 12px; justify-content: flex-end; margin-top: 8px; }
    .btn { padding: 10px 20px; border: 1px solid #ddd; border-radius: 6px;
           background: #fff; cursor: pointer; font-size: 0.9rem; }
    .btn-primary { background: #1a1a2e; color: #fff; border: none; font-weight: 500; }
    .btn-primary:disabled { opacity: 0.5; cursor: default; }
    .error { color: #e53935; font-size: 0.85rem; margin-bottom: 12px; }
  `]
})
export class AssetCreateComponent implements OnInit {
  categories: AssetCategoryDto[] = [];
  name = '';
  description = '';
  serialNumber = '';
  categoryId = 0;
  error = '';

  constructor(private api: ApiService, private router: Router) {}

  ngOnInit(): void {
    this.api.getCategories().subscribe(cats => this.categories = cats);
  }

  submit(): void {
    this.error = '';
    this.api.createAsset({
      name: this.name,
      description: this.description || null,
      serialNumber: this.serialNumber || null,
      assetCategoryId: this.categoryId
    }).subscribe({
      next: () => this.router.navigate(['/assets']),
      error: (err) => this.error = err.error?.error || err.error?.title || 'Failed to create asset'
    });
  }

  cancel(): void {
    this.router.navigate(['/assets']);
  }
}
