import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-transfers',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './transfers.html',
})
export class Transfers {
  readonly sourceAccountId = signal('');
  readonly destinationAccountId = signal('');
  readonly amount = signal<number | null>(null);
  readonly currency = signal('USD');

  readonly idempotencyKey = crypto.randomUUID();

  readonly currencies = ['USD', 'EUR', 'BRL'];

  resetForm(): void {
    this.sourceAccountId.set('');
    this.destinationAccountId.set('');
    this.amount.set(null);
  }
}