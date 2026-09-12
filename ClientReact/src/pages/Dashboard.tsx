import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { api } from '../api/client';
import styles from './Dashboard.module.css';

export function Dashboard() {
  const [statusFilter, setStatusFilter] = useState('');
  const [categoryFilter, setCategoryFilter] = useState<number | null>(null);
  const [page, setPage] = useState(1);

  const { data: stats } = useQuery({ queryKey: ['statistics'], queryFn: api.getStatistics });
  const { data: categories = [] } = useQuery({
    queryKey: ['categories'],
    queryFn: api.getCategories,
  });
  const { data: assets } = useQuery({
    queryKey: ['assets', page, statusFilter, categoryFilter],
    queryFn: () =>
      api.getAssets(page, 10, statusFilter || undefined, categoryFilter || undefined),
  });

  return (
    <div className={styles.page}>
      {stats && (
        <div className={styles.statsGrid}>
          <div className={styles.statCard}>
            <div className={styles.statValue}>{stats.totalAssets}</div>
            <div className={styles.statLabel}>Total Assets</div>
          </div>
          <div className={styles.statCard}>
            <div className={styles.statValue}>{stats.totalUsers}</div>
            <div className={styles.statLabel}>Users</div>
          </div>
          <div className={styles.statCard}>
            <div className={styles.statValue}>{stats.activeLoans}</div>
            <div className={styles.statLabel}>Active Loans</div>
          </div>
          <div className={`${styles.statCard} ${styles.overdue}`}>
            <div className={styles.statValue}>{stats.overdueLoans}</div>
            <div className={styles.statLabel}>Overdue</div>
          </div>
          <div className={styles.statCard}>
            <div className={styles.statValue}>{stats.activeReservations}</div>
            <div className={styles.statLabel}>Reservations</div>
          </div>
        </div>
      )}

      <h2>Assets</h2>
      <div className={styles.toolbar}>
        <div className={styles.filters}>
          <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
            <option value="">All Statuses</option>
            <option value="Available">Available</option>
            <option value="Loaned">Loaned</option>
            <option value="Reserved">Reserved</option>
          </select>
          <select
            value={categoryFilter ?? ''}
            onChange={(e) => setCategoryFilter(e.target.value ? Number(e.target.value) : null)}
          >
            <option value="">All Categories</option>
            {categories.map((cat) => (
              <option key={cat.id} value={cat.id}>
                {cat.name}
              </option>
            ))}
          </select>
        </div>
        <Link to="/assets/new" className={`${styles.btn} ${styles.btnPrimary}`}>
          + New Asset
        </Link>
      </div>

      {assets && (
        <>
          <div className={styles.card}>
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
                {assets.data.map((asset) => (
                  <tr key={asset.id}>
                    <td>{asset.name}</td>
                    <td>{asset.assetCategoryName}</td>
                    <td>{asset.serialNumber || '-'}</td>
                    <td>
                      <span className={`${styles.badge} ${styles[`badge${asset.status}`]}`}>
                        {asset.status}
                      </span>
                    </td>
                    <td>
                      <Link to={`/assets/${asset.id}`} className={`${styles.btn} ${styles.btnSm}`}>
                        Detail
                      </Link>
                    </td>
                  </tr>
                ))}
                {assets.data.length === 0 && (
                  <tr>
                    <td colSpan={5} className={styles.empty}>
                      No assets found.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
          {assets.totalPages > 1 && (
            <div className={styles.pagination}>
              <button onClick={() => setPage(page - 1)} disabled={!assets.hasPreviousPage}>
                Previous
              </button>
              <span>
                Page {assets.pageIndex} of {assets.totalPages}
              </span>
              <button onClick={() => setPage(page + 1)} disabled={!assets.hasNextPage}>
                Next
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
}
