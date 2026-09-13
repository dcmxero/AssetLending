import { Navigate, Route, Routes } from 'react-router-dom';
import { Navbar } from './components/layout/Navbar';
import { Dashboard } from './pages/Dashboard';
import { AssetCreate } from './pages/AssetCreate';
import { AssetDetail } from './pages/AssetDetail';

export function App() {
  return (
    <>
      <Navbar />
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/assets" element={<Navigate to="/dashboard" replace />} />
        <Route path="/assets/new" element={<AssetCreate />} />
        <Route path="/assets/:id" element={<AssetDetail />} />
      </Routes>
    </>
  );
}
