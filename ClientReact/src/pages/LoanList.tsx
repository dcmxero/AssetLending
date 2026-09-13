import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from '../api/client';
import { formatMediumDate } from '../lib/format';
import styles from './LoanList.module.css';

type Tab = 'active' | 'overdue';

export function LoanList() {
  const [tab, setTab] = useState<Tab>('active');
  const queryClient = useQueryClient();

  const { data: loans = [] } = useQuery({
    queryKey: ['loans', tab],
    queryFn: () => (tab === 'active' ? api.getActiveLoans() : api.getOverdueLoans()),
  });

  const returnLoan = useMutation({
    mutationFn: api.returnLoan,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['loans', tab] }),
    onError: (err) => console.error('Failed to return loan', err),
  });

  return (
    <div className={styles.page}>
      <h1>Loans</h1>

      <div className={styles.tabs}>
        <button
          className={`${styles.tab} ${tab === 'active' ? styles.active : ''}`}
          onClick={() => setTab('active')}
        >
          Active
        </button>
        <button
          className={`${styles.tab} ${tab === 'overdue' ? styles.active : ''}`}
          onClick={() => setTab('overdue')}
        >
          Overdue
        </button>
      </div>

      <div className={styles.card}>
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
            {loans.map((loan) => (
              <tr key={loan.id}>
                <td>{loan.assetName}</td>
                <td>{loan.borrowedByName}</td>
                <td>{formatMediumDate(loan.borrowedAt)}</td>
                <td>{formatMediumDate(loan.dueDate)}</td>
                <td>
                  <span
                    className={`${styles.badge} ${
                      loan.status === 'Overdue' ? styles.badgeOverdue : styles.badgeLoaned
                    }`}
                  >
                    {loan.status}
                  </span>
                </td>
                <td>
                  <button
                    className={`${styles.btn} ${styles.btnSm} ${styles.btnPrimary}`}
                    onClick={() => returnLoan.mutate(loan.id)}
                  >
                    Return
                  </button>
                </td>
              </tr>
            ))}
            {loans.length === 0 && (
              <tr>
                <td colSpan={6} className={styles.empty}>
                  No {tab} loans found.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
