import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './dashboard.html',
})
export class Dashboard {
  readonly capabilities = [
    {
      title: 'Onboarding Agent',
      description: 'Customer onboarding and account opening guidance.',
      icon: '◈',
    },
    {
      title: 'Account Agent',
      description: 'Bank account operations and account information.',
      icon: '▤',
    },
    {
      title: 'Fraud Agent',
      description: 'Fraud assessment and transaction risk evaluation.',
      icon: '◇',
    },
    {
      title: 'Compliance Agent',
      description: 'Compliance checks and regulatory workflows.',
      icon: '✓',
    },
    {
      title: 'Ledger Agent',
      description: 'Transaction records and ledger operations.',
      icon: '⇄',
    },
  ];
}