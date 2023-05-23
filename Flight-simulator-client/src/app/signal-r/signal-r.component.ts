import { Component, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import Flight from 'src/models/flight.model';

@Component({
  selector: 'app-signal-r',
  templateUrl: './signal-r.component.html',
  styleUrls: ['./signal-r.component.css'],
})
export class SignalRComponent implements OnInit {
  private connection: signalR.HubConnection;
  private orderedFlightsList: HTMLElement[] = [];
  private flights: Flight[] = [];

  ngOnInit() {
    this.findAndSortFlightElements();
  }

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
        let flight: Flight;
        if (!this.flights.some((flight) => flight.id == flightId)) {
          const newFlight: Flight = {
            id: flightId,
            legs: [nextLegId],
          };
          flight = newFlight;
          this.flights.push(newFlight);
        } else {
          const updatedflight = this.flights.find(
            (flight) => flight.id === flightId
          );
          if (nextLegId < 1 || nextLegId > 9) {
            const endedFlight = updatedflight as Flight;
            console.log('flight-' + endedFlight.id + ' has ended');
            console.log('road-' + endedFlight.legs);
          }
          if (updatedflight) {
            const parentElement =
              this.orderedFlightsList[
                updatedflight.legs[updatedflight.legs.length - 1] - 1
              ];
            const childElements = Array.from(parentElement.children);
            const classToRemove = 'flight-' + updatedflight.id;
            childElements.forEach((child) => {
              if (child.classList.contains(classToRemove)) {
                parentElement.removeChild(child);
              }
            });
            updatedflight.legs.push(nextLegId);
          }
          flight = updatedflight as Flight;
        }
        const newSpan = document.createElement('span');
        newSpan.classList.add('flight-' + flight.id);
        newSpan.textContent = 'flight id ' + flight.id;
        this.orderedFlightsList[
          flight.legs[flight.legs.length - 1] - 1
        ].appendChild(newSpan);
      }
    );
  }

  findAndSortFlightElements() {
    const elements = Array.from(
      document.querySelectorAll('.flights')
    ) as HTMLElement[];

    const flightElements = elements.filter((element) => {
      const classNames = element.className.split(' ');
      return classNames.some((className) => className.includes('flights-'));
    });

    this.orderedFlightsList = flightElements.sort((a, b) => {
      const aNumber = +a.className.split('flights-')[1];
      const bNumber = +b.className.split('flights-')[1];
      return aNumber - bNumber;
    });

    console.log('legs', this.orderedFlightsList);
  }
}
