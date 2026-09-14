import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from '../api/client';
import { actionErrorMessage } from '../lib/errors';
import { formatMediumDate } from '../lib/format';
import styles from './AssetDetail.module.css';

type ActionType = 'loan' | 'reserve' | '';

export function AssetDetail() {
  const assetId = Number(useParams().id);
  const queryClient = useQueryClient();

  const [actionType, setActionType] = useState<ActionType>('');
  const [selectedUserId, setSelectedUserId] = useState(0);
  const [selectedDate, setSelectedDate] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const { data: asset } = useQuery({
    queryKey: ['asset', assetId],
    queryFn: () => api.getAsset(assetId),
  });
  const { data: loanHistory } = useQuery({
    queryKey: ['asset', assetId, 'loans'],
    queryFn: () => api.getAssetLoans(assetId),
  });
  const { data: users } = useQuery({
    queryKey: ['users', 1, 100],
    queryFn: () => api.getUsers(1, 100),
  });

  const checkout = useMutation({
    mutationFn: () =>
      api.createLoan({ assetId, borrowedById: selectedUserId, dueDate: selectedDate }),
    onSuccess: () => {
      setSuccess('Asset checked out successfully.');
      void queryClient.invalidateQueries({ queryKey: ['asset', assetId] });
    },
    onError: (err) => setError(actionErrorMessage(err)),
  });

  const reserve = useMutation({
    mutationFn: () =>
      api.createReservation({ assetId, reservedById: selectedUserId, reservedUntil: selectedDate }),
    onSuccess: () => {
      setSuccess('Asset reserved successfully.');
      void queryClient.invalidateQueries({ queryKey: ['asset', assetId] });
    },
    onError: (err) => setError(actionErrorMessage(err)),
  });

  const changeActionType = (value: ActionType) => {
    setActionType(value);
    setSelectedUserId(0);
    setSelectedDate('');
    setError('');
    setSuccess('');
  };

  const submitAction = () => {
    setError('');
    setSuccess('');
    if (actionType === 'loan') {
      checkout.mutate();
    } else if (actionType === 'reserve') {
      reserve.mutate();
    }
  };

  if (!asset) {
    return (
      <div className={styles.page}>
        <p className={styles.loading}>Loading asset...</p>
      </div>
    );
  }

  const messages = (
    <>
      {error && <div className={`${styles.msg} ${styles.error}`}>{error}</div>}
      {success && <div className={`${styles.msg} ${styles.success}`}>{success}</div>}
    </>
  );

  return (
    <div className={styles.page}>
      <div className={styles.backLink}>
        <Link to="/dashboard">&larr; Back to Dashboard</Link>
      </div>
      <h1>{asset.name}</h1>

      <div className={styles.topRow}>
        <div className={`${styles.card} ${styles.infoCard}`}>
          <h3>Details</h3>
          <div className={styles.infoRow}>
            <span className={styles.label}>Category</span>
            <span>{asset.assetCategoryName}</span>
          </div>
          <div className={styles.infoRow}>
            <span className={styles.label}>Serial Number</span>
            <span>{asset.serialNumber ?? '-'}</span>
          </div>
          <div className={styles.infoRow}>
            <span className={styles.label}>Status</span>
            <span className={`${styles.badge} ${styles[`badge${asset.status}`]}`}>
              {asset.status}
            </span>
          </div>
          <div className={styles.infoRow}>
            <span className={styles.label}>Active</span>
            <span>{asset.isActive ? 'Yes' : 'No'}</span>
          </div>
          {asset.description && (
            <div className={styles.infoRow}>
              <span className={styles.label}>Description</span>
              <span>{asset.description}</span>
            </div>
          )}
        </div>

        <div className={`${styles.card} ${styles.actionCard}`}>
          {asset.status === 'Available' ? (
            <>
              <h3>Action</h3>
              <div className={styles.formGroup}>
                <label>Type</label>
                <select
                  value={actionType}
                  onChange={(e) => changeActionType(e.target.value as ActionType)}
                >
                  <option value="" disabled>
                    Select action
                  </option>
                  <option value="loan">Loan (Checkout)</option>
                  <option value="reserve">Reserve</option>
                </select>
              </div>

              {actionType && (
                <>
                  <div className={styles.formGroup}>
                    <label>User</label>
                    <select
                      value={selectedUserId}
                      onChange={(e) => setSelectedUserId(Number(e.target.value))}
                    >
                      <option value={0} disabled>
                        Select user
                      </option>
                      {users?.data.map((u) => (
                        <option key={u.id} value={u.id}>
                          {u.firstName} {u.lastName}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className={styles.formGroup}>
                    <label>{actionType === 'loan' ? 'Due Date' : 'Reserve Until'}</label>
                    <input
                      type="date"
                      value={selectedDate}
                      onChange={(e) => setSelectedDate(e.target.value)}
                    />
                  </div>

                  {messages}

                  <button
                    className={styles.btnPrimary}
                    onClick={submitAction}
                    disabled={!selectedUserId || !selectedDate}
                  >
                    {actionType === 'loan' ? 'Checkout' : 'Reserve'}
                  </button>
                </>
              )}
            </>
          ) : (
            <>
              <h3>Status</h3>
              <p className={styles.statusInfo}>
                This asset is currently <strong>{asset.status}</strong> and cannot be loaned or
                reserved.
              </p>
              {messages}
            </>
          )}
        </div>
      </div>

      <h2>Loan History</h2>
      <div className={styles.card}>
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
            {loanHistory?.data.map((loan) => (
              <tr key={loan.id}>
                <td>{loan.borrowedByName}</td>
                <td>{formatMediumDate(loan.borrowedAt)}</td>
                <td>{formatMediumDate(loan.dueDate)}</td>
                <td>{loan.returnedAt ? formatMediumDate(loan.returnedAt) : '-'}</td>
                <td>
                  <span
                    className={`${styles.badge} ${
                      loan.status === 'Active' ? styles.badgeLoaned : styles.badgeAvailable
                    }`}
                  >
                    {loan.status}
                  </span>
                </td>
              </tr>
            ))}
            {(!loanHistory || loanHistory.data.length === 0) && (
              <tr>
                <td colSpan={5} className={styles.empty}>
                  No loan history.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
