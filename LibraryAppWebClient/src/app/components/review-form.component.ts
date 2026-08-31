import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ApiService } from '../services/api.service';
import { Book } from '../models';

@Component({
  selector: 'app-review-form',
  standalone: true,
  // Angular 22 defaults components to OnPush. This component keeps state in plain
  // mutable fields updated from RxJS subscribe callbacks, so it needs eager checking.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, FormsModule],
  templateUrl: './review-form.component.html',
  styleUrl: './review-form.component.css',
})
export class ReviewFormComponent {
  @Input() book: Book | null = null;
  @Output() reviewSubmitted = new EventEmitter<void>();

  rating = 5;
  message = '';
  submitting = false;

  constructor(private apiService: ApiService) {}

  submitReview(): void {
    if (!this.book || this.message.trim().length < 5) return;

    this.submitting = true;
    this.apiService.submitReview(this.book.id, this.rating, this.message.trim()).subscribe({
      next: () => {
        this.submitting = false;
        this.message = '';
        this.rating = 5;
        this.reviewSubmitted.emit();
      },
      error: (err) => {
        this.submitting = false;
        alert(err?.error?.message || err?.error || 'Failed to submit review.');
      },
    });
  }
}
