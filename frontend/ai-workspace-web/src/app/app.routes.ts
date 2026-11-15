import { Routes } from '@angular/router';
import { MainLayout } from './layout/main-layout/main-layout';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/dashboard/dashboard').then(
            (m) => m.Dashboard
          ),
        title: 'Dashboard | AI Banking Workspace',
      },
      {
        path: 'assistant',
        loadComponent: () =>
          import('./features/ai-assistant/ai-assistant').then(
            (m) => m.AiAssistant
          ),
        title: 'AI Assistant | AI Banking Workspace',
      },
      {
        path: 'accounts',
        loadComponent: () =>
          import('./features/accounts/accounts').then(
            (m) => m.Accounts
          ),
        title: 'Accounts | AI Banking Workspace',
      },
      {
        path: 'transfers',
        loadComponent: () =>
          import('./features/transfers/transfers').then(
            (m) => m.Transfers
          ),
        title: 'Transfers | AI Banking Workspace',
      },
      {
        path: 'agents',
        loadComponent: () =>
          import('./features/agents/agents').then(
            (m) => m.Agents
          ),
        title: 'AI Agents | AI Banking Workspace',
      },
      {
        path: 'mcp-tools',
        loadComponent: () =>
          import('./features/mcp-tools/mcp-tools').then(
            (m) => m.McpTools
          ),
        title: 'MCP Tools | AI Banking Workspace',
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];