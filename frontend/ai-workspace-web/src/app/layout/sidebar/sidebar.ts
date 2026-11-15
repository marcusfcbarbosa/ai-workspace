import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

interface NavigationItem {
  label: string;
  path: string;
  icon: string;
  exact: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
})
export class Sidebar {
  @Input() mobileOpen = false;
  @Output() navigate = new EventEmitter<void>();

  readonly navigation: NavigationItem[] = [
    { label: 'Dashboard', path: '/', icon: '▦', exact: true },
    { label: 'AI Assistant', path: '/assistant', icon: '✦', exact: false },
    { label: 'Accounts', path: '/accounts', icon: '▤', exact: false },
    { label: 'Transfers', path: '/transfers', icon: '⇄', exact: false },
    { label: 'AI Agents', path: '/agents', icon: '⬡', exact: false },
    { label: 'MCP Tools', path: '/mcp-tools', icon: '⚙', exact: false },
  ];
}