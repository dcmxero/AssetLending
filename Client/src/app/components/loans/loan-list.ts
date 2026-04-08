import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { LoanDto } from '../../models/loan.model';

@Component({
  selector: 'app-loan-list',
  standalone: true,
  imports: [DatePipe],
  template: `
    <div class="page">
      <h1>Loans</h1>

      <div class="tabs">
        <button class="tab" [class.active]="tab === 'active'" (click)="switchTab('active')">Active</button>
        <button class="tab" [class.active]="tab === 'overdue'" (click)="switchTab('overdue')">Overdue</button>
      </div>

      <div class="card">
        <table>
          <thead>
            <tr>
              <th>Asset</th>
              <th>Borrowed By</th>
              <th>Borrowed At</th>
              <th>Due Date</th>
              <th>Status</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            @for (loan of loans; track loan.id) {
              <tr>
                <td>{{ loan.assetName }}</td>
                <td>{{ loan.borrowedByName }}</td>
                <td>{{ loan.borrowedAt | date:'mediumDate' }}</td>
                <td>{{ loan.dueDate | date:'mediumDate' }}</td>
                <td>
                  <span class="badge" [class]="loan.status === 'Overdue' ? 'badge-overdue' : 'badge-loaned'">
                    {{ loan.status }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-primary" (click)="returnLoan(loan.id)">Return</button>
                </td>
              </tr>
            }
            @if (loans.length === 0) {
              <tr><td colspan="6" class="empty">No {{ tab }} loans found.</td></tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .page { padding: 24px; max-width: 1000px; margin: 0 auto; }
    h1 { font-size: 1.5rem; margin-bottom: 16px; }
    .tabs { display: flex; gap: 4px; margin-bottom: 16px; }
    .tab {
      padding: 8px 20px; border: 1px solid #ddd; border-radius: 6px;
      background: #fff; cursor: pointer; font-size: 0.9rem; font-weight: 500;
    }
    .tab.active { background: #1a1a2e; color: #fff; border-color: #1a1a2e; }
    .card {
      background: #fff; border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08); overflow: hidden;
    }
    table { width: 100%; border-collapse: collapse; }
    th { text-align: left; padding: 12px 16px; font-size: 0.8rem; color: #888;
         text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 1px solid #eee; }
    td { padding: 12px 16px; border-bottom: 1px solid #f5f5f5; font-size: 0.9rem; }
    .empty { text-align: center; color: #888; padding: 32px 16px; }
    .badge { display: inline-block; padding: 4px 10px; border-radius: 12px; font-size: 0.8rem; font-weight: 600; }
    .badge-loaned { background: #fff3e0; color: #e65100; }
    .badge-overdue { background: #ffebee; color: #c62828; }
    .btn { padding: 8px 16px; border: 1px solid #ddd; border-radius: 6px; background: #fff; cursor: pointer; font-size: 0.9rem; }
    .btn-primary { background: #1a1a2e; color: #fff; border: none; font-weight: 500; }
    .btn-sm { padding: 6px 12px; font-size: 0.85rem; }
  `]
})
export class LoanListComponent implements OnInit {
  tab: 'active' | 'overdue' = 'active';
  loans: LoanDto[] = [];

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadLoans();
  }

  switchTab(tab: 'active' | 'overdue'): void {
    this.tab = tab;
    this.loadLoans();
  }

  loadLoans(): void {
    const obs = this.tab === 'active' ? this.api.getActiveLoans() : this.api.getOverdueLoans();
    obs.subscribe({
      next: (data) => this.loans = data,
      error: (err) => console.error('Failed to load loans', err)
    });
  }

  returnLoan(loanId: number): void {
    this.api.returnLoan(loanId).subscribe({
      next: () => this.loadLoans(),
      error: (err) => console.error('Failed to return loan', err)
    });
  }
}
