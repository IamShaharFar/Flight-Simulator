import { Time } from "@angular/common";

export default interface Flight {
    id: number;
    legs: number[];
    hasEnded: Boolean;
    isLanding: Boolean;
    contactTime: Date;
  }