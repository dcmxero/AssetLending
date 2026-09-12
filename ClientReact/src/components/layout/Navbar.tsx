import { NavLink } from 'react-router-dom';
import styles from './Navbar.module.css';

const linkClass = ({ isActive }: { isActive: boolean }) => (isActive ? styles.active : undefined);

export function Navbar() {
  return (
    <>
      <header className={styles.header}>
        <div className={styles.headerInner}>
          <div className={styles.navbarBrand}>Asset Lending</div>
        </div>
      </header>
      <nav className={styles.navbar}>
        <div className={styles.navbarInner}>
          <div className={styles.navbarLinks}>
            <NavLink to="/dashboard" className={linkClass}>
              Dashboard
            </NavLink>
            <NavLink to="/loans" className={linkClass}>
              Loans
            </NavLink>
            <NavLink to="/users" className={linkClass}>
              Users
            </NavLink>
          </div>
        </div>
      </nav>
    </>
  );
}
