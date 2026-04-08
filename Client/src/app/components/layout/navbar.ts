import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <header class="header">
      <div class="header-inner">
        <div class="navbar-brand">Asset Lending</div>
      </div>
    </header>
    <nav class="navbar">
      <div class="navbar-inner">
        <div class="navbar-links">
          <a routerLink="/dashboard" routerLinkActive="active">Dashboard</a>
          <a routerLink="/loans" routerLinkActive="active">Loans</a>
          <a routerLink="/users" routerLinkActive="active">Users</a>
        </div>
      </div>
    </nav>
  `,
  styles: [`
    .header {
      background: #1a1a2e;
      border-bottom: 1px solid rgba(255,255,255,0.1);
    }
    .header-inner {
      max-width: 1000px;
      margin: 0 auto;
      padding: 16px 24px;
      text-align: center;
    }
    .navbar {
      background: #fff;
      border-bottom: 1px solid #eee;
    }
    .navbar-inner {
      max-width: 1000px;
      margin: 0 auto;
      padding: 0 24px;
      display: flex;
      height: 44px;
      align-items: center;
    }
    .navbar-brand {
      color: #fff;
      font-size: 1.25rem;
      font-weight: 700;
      letter-spacing: 0.5px;
    }
    .navbar-links {
      display: flex;
      gap: 8px;
    }
    .navbar-links a {
      color: #fff;
      text-decoration: none;
      padding: 6px 16px;
      border-radius: 6px;
      font-size: 0.85rem;
      font-weight: 500;
      background: #1a1a2e;
      transition: opacity 0.2s;
    }
    .navbar-links a:hover {
      opacity: 0.85;
    }
    .navbar-links a.active {
      opacity: 1;
      box-shadow: 0 2px 6px rgba(26,26,46,0.3);
    }
  `]
})
export class NavbarComponent {}
