import {
  Component,
  EventEmitter,
  Output,
} from '@angular/core';

import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.html',
})
export class Header {
  @Output() menuClick = new EventEmitter<void>();

  readonly applicationName = environment.application.name;
  readonly environmentName = environment.production
    ? 'Production'
    : 'Development';
}