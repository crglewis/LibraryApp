import { Injectable, NgZone } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

export interface BookAvailabilityChangedEvent {
  bookId: number;
  isAvailable: boolean;
}

/**
 * Thin wrapper around a single SignalR hub connection to /hubs/books. The hub only
 * broadcasts (see BookHub.cs) — checkout/return still go through the REST API, this
 * just lets every connected client hear about the resulting availability change.
 */
@Injectable({ providedIn: 'root' })
export class SignalrService {
  private connection: signalR.HubConnection | null = null;
  private bookAvailabilityChangedSubject = new Subject<BookAvailabilityChangedEvent>();

  readonly bookAvailabilityChanged$ = this.bookAvailabilityChangedSubject.asObservable();

  constructor(private ngZone: NgZone) {}

  /** Idempotent: safe to call from every page that wants live updates. */
  connect(): void {
    if (this.connection) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/books', { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    // The underlying WebSocket's message event runs outside Angular's zone, so emitting
    // straight from here would update component state without ever scheduling change
    // detection. Re-entering the zone here means every subscriber gets a render for free.
    this.connection.on('BookAvailabilityChanged', (event: BookAvailabilityChangedEvent) => {
      this.ngZone.run(() => this.bookAvailabilityChangedSubject.next(event));
    });

    this.connection.start().catch((err) => console.error('SignalR connection failed:', err));
  }

  disconnect(): void {
    this.connection?.stop();
    this.connection = null;
  }
}
