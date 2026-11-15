import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';

import { ApiService } from '../../core/services/api.service';
import { environment } from '../../../environments/environment';

interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
  agent?: string;
  timestamp: Date;
}

interface ChatResponse {
  agent: string;
  response: string;
}

@Component({
  selector: 'app-ai-assistant',
  standalone: true,
  imports: [FormsModule, DatePipe],
  templateUrl: './ai-assistant.html',
})
export class AiAssistant {
  private readonly apiService = inject(ApiService);

  readonly messages = signal<ChatMessage[]>([
    {
      role: 'assistant',
      content:
        'Hello! I am your AI Banking Assistant. I can help you explore banking operations, account onboarding, transfers, and available banking capabilities.',
      timestamp: new Date(),
    },
  ]);

  readonly input = signal('');
  readonly loading = signal(false);
  readonly error = signal('');

  sendMessage(): void {
    const message = this.input().trim();

    if (!message || this.loading()) {
      return;
    }

    this.messages.update((current) => [
      ...current,
      {
        role: 'user',
        content: message,
        timestamp: new Date(),
      },
    ]);

    this.input.set('');
    this.error.set('');
    this.loading.set(true);

    this.apiService
      .post<ChatResponse>(
        environment.api.chat,
        { message },
      )
      .subscribe({
        next: (result) => {
          this.messages.update((current) => [
            ...current,
            {
              role: 'assistant',
              content: result.response,
              agent: result.agent,
              timestamp: new Date(),
            },
          ]);

          this.loading.set(false);
        },

        error: (err) => {
          this.error.set(
            err.error?.message ??
              'Unable to communicate with the Banking API. Please verify that the backend is running.',
          );

          this.loading.set(false);
        },
      });
  }

  useSuggestion(message: string): void {
    this.input.set(message);
    this.sendMessage();
  }
}