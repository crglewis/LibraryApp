import 'zone.js';
// Angular's HttpClient uses the Fetch backend by default, but zone.js's core
// bundle only patches XHR. Without this plugin zone.js never sees HTTP requests
// settle, so no change detection runs and views never show loaded data.
import 'zone.js/plugins/zone-patch-fetch';
import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { appConfig } from './app/app.config';

bootstrapApplication(App, appConfig).catch((err) => console.error(err));
