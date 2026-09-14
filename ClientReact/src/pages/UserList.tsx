import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from '../api/client';
import { createErrorMessage } from '../lib/errors';
import styles from './UserList.module.css';

export function UserList() {
  const [page, setPage] = useState(1);
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');

  const queryClient = useQueryClient();
  const { data: result } = useQuery({
    queryKey: ['users', page],
    queryFn: () => api.getUsers(page),
  });

  const addUser = useMutation({
    mutationFn: api.createUser,
    onSuccess: () => {
      setFirstName('');
      setLastName('');
      setEmail('');
      void queryClient.invalidateQueries({ queryKey: ['users'] });
    },
    onError: (err) => setError(createErrorMessage(err, 'Failed to create user')),
  });

  const submit = (event: FormEvent) => {
    event.preventDefault();
    setError('');
    addUser.mutate({ firstName, lastName, email });
  };

  return (
    <div className={styles.page}>
      <h1>Users</h1>

      <div className={`${styles.card} ${styles.createForm}`}>
        <form onSubmit={submit} className={styles.inlineForm}>
          <input
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            placeholder="First Name"
            required
          />
          <input
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            placeholder="Last Name"
            required
          />
          <input
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Email"
            required
            type="email"
          />
          <button
            type="submit"
            className={`${styles.btn} ${styles.btnPrimary}`}
            disabled={!firstName || !lastName || !email}
          >
            Add
          </button>
        </form>
        {error && <div className={styles.error}>{error}</div>}
      </div>

      <div className={styles.card}>
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
            </tr>
          </thead>
          <tbody>
            {result?.data.map((user) => (
              <tr key={user.id}>
                <td>
                  {user.firstName} {user.lastName}
                </td>
                <td>{user.email}</td>
              </tr>
            ))}
            {(!result || result.data.length === 0) && (
              <tr>
                <td colSpan={2} className={styles.empty}>
                  No users found.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {result && result.totalPages > 1 && (
        <div className={styles.pagination}>
          <button
            onClick={() => setPage(page - 1)}
            disabled={!result.hasPreviousPage}
            className={`${styles.btn} ${styles.btnSm}`}
          >
            Previous
          </button>
          <span className={styles.pageInfo}>
            Page {result.pageIndex} of {result.totalPages}
          </span>
          <button
            onClick={() => setPage(page + 1)}
            disabled={!result.hasNextPage}
            className={`${styles.btn} ${styles.btnSm}`}
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
}
