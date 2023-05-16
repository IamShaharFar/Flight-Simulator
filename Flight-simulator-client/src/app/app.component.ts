import { Component } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'Flight-simulator-client';
  private connection: signalR.HubConnection;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7209/flighthub')
      .build();

    this.connection
      .start()
      .then(() => console.log('SignalR Connected'))
      .catch((error) => console.error('SignalR Connection Error:', error));

    this.connection.on(
      'SendNextLegAndFlightId',
      (nextLegId: number, flightId: number) => {
        // Handle the received next leg ID and flight ID
        if (flightId == 1) {
          console.log('Received Next Leg ID:', nextLegId);
          console.log('Received Flight ID:', flightId);
        }
      }
    );
  }
}
