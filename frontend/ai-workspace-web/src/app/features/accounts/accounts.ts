import { Component } from '@angular/core';

@Component({
  selector: 'app-accounts',
  standalone: true,
  templateUrl: './accounts.html',
})
export class Accounts {
  readonly columns = [
    'Account Number',
    'Account Owner',
    'Currency',
    'Balance',
    'Status',
  ];
}