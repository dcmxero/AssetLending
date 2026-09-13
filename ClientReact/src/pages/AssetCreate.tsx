import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQuery } from '@tanstack/react-query';
import { api } from '../api/client';
import { createErrorMessage } from '../lib/errors';
import styles from './AssetCreate.module.css';

export function AssetCreate() {
  const navigate = useNavigate();
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [serialNumber, setSerialNumber] = useState('');
  const [categoryId, setCategoryId] = useState(0);
  const [error, setError] = useState('');

  const { data: categories = [] } = useQuery({
    queryKey: ['categories'],
    queryFn: api.getCategories,
  });

  const createAsset = useMutation({
    mutationFn: api.createAsset,
    onSuccess: () => navigate('/assets'),
    onError: (err) => setError(createErrorMessage(err, 'Failed to create asset')),
  });

  const submit = (event: FormEvent) => {
    event.preventDefault();
    setError('');
    createAsset.mutate({
      name,
      description: description || null,
      serialNumber: serialNumber || null,
      assetCategoryId: categoryId,
    });
  };

  return (
    <div className={styles.page}>
      <h1>Create Asset</h1>
      <div className={styles.card}>
        <form onSubmit={submit}>
          <div className={styles.formGroup}>
            <label>Name</label>
            <input
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              placeholder="Asset name"
            />
          </div>
          <div className={styles.formGroup}>
            <label>Description</label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
              placeholder="Optional description"
            />
          </div>
          <div className={styles.formGroup}>
            <label>Serial Number</label>
            <input
              value={serialNumber}
              onChange={(e) => setSerialNumber(e.target.value)}
              placeholder="Optional serial number"
            />
          </div>
          <div className={styles.formGroup}>
            <label>Category</label>
            <select
              value={categoryId}
              onChange={(e) => setCategoryId(Number(e.target.value))}
              required
            >
              <option value={0} disabled>
                Select a category
              </option>
              {categories.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {cat.name}
                </option>
              ))}
            </select>
          </div>
          {error && <div className={styles.error}>{error}</div>}
          <div className={styles.formActions}>
            <button type="button" className={styles.btn} onClick={() => navigate('/assets')}>
              Cancel
            </button>
            <button
              type="submit"
              className={`${styles.btn} ${styles.btnPrimary}`}
              disabled={!name || !categoryId}
            >
              Create
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
