import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  // Angular defaults components to OnPush. As the root component, App is never
  // itself marked dirty, so leaving it OnPush blocks the change-detection tree
  // walk from ever descending into routed pages — even ones that opt into Eager
  // checking, like HomePage. Eager here keeps that path open for the whole tree.
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
})
export class App {}
