import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { UserDto } from '../../models/user.model';
import { PaginatedList } from '../../models/common.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="page">
      <h1>Users</h1>

      <div class="card create-form">
        <form (ngSubmit)="addUser()" class="inline-form">
          <input [(ngModel)]="firstName" name="firstName" placeholder="First Name" required />
          <input [(ngModel)]="lastName" name="lastName" placeholder="Last Name" required />
          <input [(ngModel)]="email" name="email" placeholder="Email" required type="email" />
          <button type="submit" class="btn btn-primary" [disabled]="!firstName || !lastName || !email">Add</button>
        </form>
        @if (error) { <div class="error">{{ error }}</div> }
      </div>

      <div class="card">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
            </tr>
          </thead>
          <tbody>
            @for (user of result?.data; track user.id) {
              <tr>
                <td>{{ user.firstName }} {{ user.lastName }}</td>
                <td>{{ user.email }}</td>
              </tr>
            }
            @if (!result || result.data.length === 0) {
              <tr><td colspan="2" class="empty">No users found.</td></tr>
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
    h1 { font-size: 1.5rem; margin-bottom: 16px; }
    .card {
      background: #fff; border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08); overflow: hidden;
    }
    .create-form { padding: 16px; margin-bottom: 16px; }
    .inline-form { display: flex; gap: 12px; align-items: center; flex-wrap: wrap; }
    .inline-form input {
      padding: 10px 12px; border: 1px solid #ddd; border-radius: 6px;
      font-size: 0.9rem; flex: 1; min-width: 140px;
    }
    .btn { padding: 8px 16px; border: 1px solid #ddd; border-radius: 6px;
           background: #fff; cursor: pointer; font-size: 0.9rem; }
    .btn-primary { background: #1a1a2e; color: #fff; border: none; font-weight: 500; }
    .btn-primary:disabled { opacity: 0.5; cursor: default; }
    .btn-sm { padding: 6px 12px; font-size: 0.85rem; }
    .error { color: #e53935; font-size: 0.85rem; margin-top: 8px; }
    table { width: 100%; border-collapse: collapse; }
    th { text-align: left; padding: 12px 16px; font-size: 0.8rem; color: #888;
         text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 1px solid #eee; }
    td { padding: 12px 16px; border-bottom: 1px solid #f5f5f5; font-size: 0.9rem; }
    .empty { text-align: center; color: #888; padding: 32px 16px; }
    .pagination { display: flex; align-items: center; justify-content: center; gap: 16px; margin-top: 16px; }
    .page-info { font-size: 0.9rem; color: #666; }
  `]
})
export class UserListComponent implements OnInit {
  result: PaginatedList<UserDto> | null = null;
  page = 1;
  firstName = '';
  lastName = '';
  email = '';
  error = '';

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.api.getUsers(this.page).subscribe({
      next: (data) => this.result = data,
      error: (err) => console.error('Failed to load users', err)
    });
  }

  goToPage(p: number): void {
    this.page = p;
    this.loadUsers();
  }

  addUser(): void {
    this.error = '';
    this.api.createUser({
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email
    }).subscribe({
      next: () => {
        this.firstName = '';
        this.lastName = '';
        this.email = '';
        this.loadUsers();
      },
      error: (err) => this.error = err.error?.error || err.error?.title || 'Failed to create user'
    });
  }
}
