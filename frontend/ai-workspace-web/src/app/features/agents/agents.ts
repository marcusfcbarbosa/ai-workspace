import { Component } from '@angular/core';

interface Agent {
  name: string;
  role: string;
  description: string;
  responsibilities: string[];
  icon: string;
  color: string;
}

@Component({
  selector: 'app-agents',
  standalone: true,
  templateUrl: './agents.html',
})
export class Agents {
  readonly agents: Agent[] = [
    {
      name: 'Onboarding Agent',
      role: 'Customer Onboarding',
      description: 'Handles customer onboarding and account opening guidance.',
      responsibilities: [
        'Customer onboarding',
        'Account opening requirements',
        'Customer information workflows',
      ],
      icon: '◈',
      color: 'blue',
    },
    {
      name: 'Account Agent',
      role: 'Account Management',
      description: 'Handles banking account operations and account information.',
      responsibilities: [
        'Account information',
        'Account ownership',
        'Account validation',
      ],
      icon: '▤',
      color: 'indigo',
    },
    {
      name: 'Fraud Agent',
      role: 'Fraud Detection',
      description: 'Evaluates transaction risk and potential fraudulent activity.',
      responsibilities: [
        'Fraud risk assessment',
        'Transaction risk evaluation',
        'Risk indicators',
      ],
      icon: '◇',
      color: 'rose',
    },
    {
      name: 'Compliance Agent',
      role: 'Regulatory Compliance',
      description: 'Supports compliance checks and regulatory workflows.',
      responsibilities: [
        'Compliance validation',
        'Regulatory requirements',
        'Transaction review',
      ],
      icon: '✓',
      color: 'emerald',
    },
    {
      name: 'Ledger Agent',
      role: 'Financial Ledger',
      description: 'Handles ledger-related transaction operations.',
      responsibilities: [
        'Ledger entries',
        'Transaction records',
        'Financial consistency',
      ],
      icon: '⇄',
      color: 'violet',
    },
  ];
}