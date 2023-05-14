using Microsoft.CodeAnalysis.Elfie.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAirportService
    {
        Task<List<Flight>> StartSim();
        Flight AddFlight();
        LegStation GetNextPoint(Flight flight);

    }
    public class AirportService : IAirportService
    {
        private IAirport _airport;
        public AirportService(IAirport airport)
        {
            _airport = airport;
        }
        private Flight CreateRandomFlight()
        {
            // List of airplane models
            List<string> airplanesModels = new List<string> {
        "Boeing 737",
        "Airbus A320",
        "Cessna Citation XLS",
        "Embraer E195",
        "Bombardier CRJ900",
        "Boeing 777",
        "Airbus A350",
        "Gulfstream G550",
        "Dassault Falcon 7X",
        "Bombardier Global 6000"
    };

            // Create a new random flight
            Random rand = new Random();
            var flight = new Flight
            {
                // Generate a unique flight ID
                Id = _airport.Flights.Count == 0 ? 1 : _airport.Flights.Max(f => f.Id) + 1,

                // Select a random airplane model from the list
                Airplane = airplanesModels[rand.Next(airplanesModels.Count)],

                // The road on which the flight will operate (null initially)
                Road = null,

                // The flight's takeoff time (set to current time)
                TakeOff = DateTime.Now,

                // Determine if the flight is for landing or takeoff randomly
                IsLanding = rand.Next(2) == 0,

                // The current leg of the flight (null initially)
                CurrentLeg = null
            };

            // Set the road based on whether the flight is for landing or takeoff
            flight.Road = flight.IsLanding ? _airport.LandingRoad : _airport.TakeoffRoad;

            return flight;
        }
        public Flight AddFlight()
        {
            // Create a new random flight
            Flight flight = CreateRandomFlight();

            // Add the flight to the airport's flights collection
            if (_airport.Flights == null || _airport.Flights.Count == 0)
            {
                // If the flights collection is empty, create a new list and add the flight to it
                _airport.Flights = new Collection<Flight> { flight };
            }
            else
            {
                // If the flights collection already exists, simply add the flight to it
                _airport.Flights.Add(flight);
            }

            // Print information about the newly added flight
            Console.WriteLine($"New flight | Flight number: {flight.Id} | {(flight.IsLanding ? "Landing flight" : "Takeoff flight")}");

            return flight;
        }
        public LegStation GetNextPoint(Flight flight)
        {
            if (flight.CurrentLeg == null)
            {
                // If the flight doesn't have a current leg, determine the next leg to move to

                var leg = (Leg)flight.Road.First();
                if (leg is IMultyLeg)
                {
                    // If the leg is a multi-leg, find the next sub-leg within it
                    var multyLeg = (IMultyLeg)flight.Road.First();
                    var nextLeg = multyLeg.Stations.First();
                    int minWaiting = int.MaxValue;

                    // Iterate through all sub-legs of the multi-leg to find the one with the minimum waiting list count
                    foreach (var s in multyLeg.Stations)
                    {
                        if (s.WaitingList.Count < minWaiting)
                        {
                            nextLeg = (Leg)s;
                            minWaiting = s.WaitingList.Count;
                        }
                    }
                    return new LegStation { MultyLeg = (Leg)multyLeg, SubLeg = (Leg)nextLeg };
                }
                else
                {
                    // If the leg is not a multi-leg, return the same leg as the next leg
                    return new LegStation { MultyLeg = leg, SubLeg = leg };
                }
            }
            else
            {
                // If the flight has a current leg, determine the next leg based on its position in the road
                var currentIndex = flight.Road.ToList().IndexOf(flight.CurrentLeg.MultyLeg);

                // If the current leg is the last leg in the road, there is no next leg
                if (currentIndex >= flight.Road.Count - 1) return null;
                var nextLeg = flight.Road.ToList()[currentIndex + 1];
                if(nextLeg is IMultyLeg)
                {
                    // If the next leg is a multi-leg, find the next sub-leg within it
                    var multyLeg = (IMultyLeg)nextLeg;
                    var nextSubLeg = multyLeg.Stations.First();
                    int minWaiting = int.MaxValue;

                    // Iterate through all sub-legs of the multi-leg to find the one with the minimum waiting list count
                    foreach (var s in multyLeg.Stations)
                    {
                        if (s.WaitingList.Count < minWaiting)
                        {
                            nextSubLeg = (Leg)s;
                            minWaiting = s.WaitingList.Count;
                        }
                    }
                    
                    return new LegStation { MultyLeg = (Leg)multyLeg, SubLeg = (Leg)nextSubLeg };
                }
                else
                {
                    // If the next leg is not a multi-leg, return the same leg as the next leg
                    return new LegStation { MultyLeg = (Leg)nextLeg, SubLeg = (Leg)nextLeg };
                }
            }
        }
        public async Task<List<Flight>> StartSim()
        {
            _airport = new Airport();
            Console.WriteLine("starting simulator");
            AddFlight();
            AddFlight();
            AddFlight();
            AddFlight();
            AddFlight();
            AddFlight();

            var tasks = _airport.Flights.Select(flight => Task.Run(() => MakeFlight(flight)));
            await Task.WhenAll(tasks);


            Console.WriteLine("all flights ended");
            return _airport.Flights.ToList();
            //todo: make a loop that each time make another flight with AddFlight()
        }
        public async Task MakeFlight(Flight flight)
        {
            // Determine the next leg for the flight
            var nextLeg = GetNextPoint(flight);
            // If there is no next leg available, return
            if (nextLeg == null) return;
            else
            {
                do
                {
                    // Move the flight to the next leg
                    MoveFlightToNextLeg(flight);
                    #region old
                    //if (flight.IsLanding)
                    //{                
                    //    //if (flight.CurrentLeg == null)
                    //    //{
                    //    //    flight.CurrentLeg = 
                    //    //    flight.Road = new List<Leg> { _airport.GetLeg(1) };
                    //    //    _airport.GetLeg(1).Airplanes.Add(flight);
                    //    //    Console.WriteLine($"Landing Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                    //    //    nextLeg = GetNextPoint(flight);
                    //    //}

                    //    //switch (nextLeg.Id)
                    //    //{
                    //    //    case 2:
                    //    //    case 3:
                    //    //    case 5:
                    //    //        flight.Road.Last().Airplanes.Remove(flight);
                    //    //        flight.Road.Add(nextLeg);
                    //    //        nextLeg.Airplanes.Add(flight);
                    //    //        Console.WriteLine($"Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                    //    //        break;
                    //    //    case 4:
                    //    //        leg4Queue.Add(flight);
                    //    //        await Task.Run(() => WaitForLeg4(flight));
                    //    //        lock (_airport.GetLeg(4))
                    //    //        {
                    //    //            Console.WriteLine($"leg 4 is ocupied by flight {flight.Id}");
                    //    //            Thread.Sleep(2000);
                    //    //            flight.Road.Last().Airplanes.Remove(flight);
                    //    //            flight.Road.Add(nextLeg);
                    //    //            nextLeg.Airplanes.Add(flight);
                    //    //            Console.WriteLine($"Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                    //    //            Console.WriteLine($"leg 4 is free by flight {flight.Id}");
                    //    //        }
                    //    //        break;
                    //    //    case 6:
                    //    //    case 7:
                    //    //        int leg;
                    //    //        leg6Or7Queue.Add(flight);
                    //    //        leg = await Task.Run(() => WaitForLeg6Or7(flight));
                    //    //        lock (_airport.GetLeg(leg))
                    //    //        {
                    //    //            Console.WriteLine($"leg {leg} is ocupied by flight {flight.Id}");
                    //    //            Thread.Sleep(2000);
                    //    //            flight.Road.Last().Airplanes.Remove(flight);
                    //    //            flight.Road.Add(_airport.GetLeg(leg));
                    //    //            _airport.GetLeg(leg).Airplanes.Add(flight);
                    //    //            Console.ForegroundColor = ConsoleColor.Red;
                    //    //            Console.WriteLine($"Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                    //    //            Console.ForegroundColor = ConsoleColor.White;
                    //    //        }
                    //    //        Console.WriteLine($"leg {leg} is free by flight {flight.Id}");
                    //    //        break;
                    //    //    default:
                    //    //        flight.Road.Last().Airplanes.Remove(flight);
                    //    //        break;
                    //    //}
                    //    //if (flight.Road.Last().Id == 6 || flight.Road.Last().Id == 7)
                    //    //{
                    //    //    Console.ForegroundColor = ConsoleColor.Red;
                    //    //}       
                    //    //Console.ForegroundColor = ConsoleColor.White;
                    //    //nextLeg = GetNextPoint(flight);
                    //}
                    //else //flight is takeoff
                    //{
                    //    if (flight.Road == null || flight.Road.Count == 0)
                    //    {
                    //        Console.WriteLine($"TakeOff flight - {flight.Id} | {DateTime.Now}");
                    //        int leg;
                    //        leg6Or7Queue.Add(flight);
                    //        leg = await Task.Run(() => WaitForLeg6Or7(flight));
                    //        lock (_airport.GetLeg(leg))
                    //        {
                    //            Console.WriteLine($"leg {leg} is ocupied by flight {flight.Id}");
                    //            Thread.Sleep(2000);
                    //            flight.Road.Add(_airport.GetLeg(leg));
                    //            _airport.GetLeg(leg).Airplanes.Add(flight);
                    //            Console.WriteLine($"Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                    //            Console.WriteLine($"leg {leg} is free by flight {flight.Id}");
                    //        }
                    //    }

                    //    switch (nextLeg.Id)
                    //    {
                    //        case 8:
                    //            flight.Road.Last().Airplanes.Remove(flight);
                    //            flight.Road.Add(nextLeg);
                    //            nextLeg.Airplanes.Add(flight);
                    //            Console.WriteLine($"Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                    //            break;
                    //        case 4:
                    //            leg4Queue.Add(flight);
                    //            Task.Run(() => WaitForLeg4(flight));
                    //            lock (_airport.GetLeg(4))
                    //            {
                    //                Console.WriteLine($"leg 4 is ocupied by flight {flight.Id}");
                    //                Thread.Sleep(2000);
                    //                flight.Road.Last().Airplanes.Remove(flight);
                    //                flight.Road.Add(nextLeg);
                    //                nextLeg.Airplanes.Add(flight);
                    //                Console.WriteLine($"Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                    //                Console.WriteLine($"leg 4 is free by flight {flight.Id}");
                    //            }
                    //            break;
                    //        case 9:
                    //            flight.Road.Last().Airplanes.Remove(flight);
                    //            flight.Road.Add(nextLeg);
                    //            nextLeg.Airplanes.Add(flight);
                    //            Console.ForegroundColor = ConsoleColor.Red;
                    //            Console.WriteLine($"Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                    //            Console.ForegroundColor = ConsoleColor.White;
                    //            break;
                    //        default:
                    //            flight.Road.Last().Airplanes.Remove(flight);
                    //            break;
                    //    }
                    //    nextLeg = GetNextPoint(flight);
                    //}
                    #endregion
                    await Task.Delay(1000);
                    nextLeg = GetNextPoint(flight);
                } while (nextLeg != null);
                flight.CurrentLeg.SubLeg.Airplanes.Remove(flight);
                flight.CurrentLeg = null;
                lock(Console.Out)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"flight-{flight.Id} || moved to no where");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
        private async Task<LegStation> MoveFlightToNextLeg(Flight flight)
        {
            var nextLeg = GetNextPoint(flight);
            // Check if the flight is currently not on any leg
            if (flight.CurrentLeg == null)
            {
                // Check if the next leg allows only one airplane at a time
                if (nextLeg.SubLeg.ForOneAirplaneOnly)
                {
                    // Wait for the next leg to be available and acquire a lock on it
                    WaitForLeg(nextLeg.SubLeg, flight);
                    lock (nextLeg.SubLeg)
                    {
                        // Update the current leg of the flight
                        flight.CurrentLeg = nextLeg;
                        // Add the flight to the airplanes list of the next leg
                        flight.CurrentLeg.SubLeg.Airplanes.Add(flight);
                        Console.WriteLine($"leg {nextLeg.SubLeg.Id} is ocupied by flight - {flight.Id} | at {DateTime.Now}");
                        Task.Run(async () =>
                        {
                            await Task.Delay(2000); // Wait for 2 seconds asynchronously
                                                    // Code to execute after the delay
                        }).Wait();
                    }
                    return nextLeg;
                }
                // Update the current leg of the flight
                flight.CurrentLeg = nextLeg;
                return nextLeg;
            }
            // Check if there is no next leg available
            if (nextLeg == null)
            {
                lock (flight.CurrentLeg.SubLeg.Airplanes)
                {
                    // Remove the flight from the airplanes list of the current leg
                    flight.CurrentLeg.SubLeg.Airplanes.Remove(flight);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"flight-{flight.Id} || moved from leg - {flight.CurrentLeg.SubLeg.Id} to no where");
                Console.ForegroundColor = ConsoleColor.White;
                flight.CurrentLeg = null;
                return null;
            }
            // Check if the next leg allows only one airplane at a time
            if (nextLeg.SubLeg.ForOneAirplaneOnly)
            {
                // Wait for the next leg to be available and acquire a lock on it
                WaitForLeg(nextLeg.SubLeg, flight);
                lock(nextLeg.SubLeg) 
                {
                    // Check if the flight is still in the airplanes list of the current leg
                    if (flight.CurrentLeg.SubLeg.Airplanes.Contains(flight))
                    {
                        // Remove the flight from the airplanes list of the current leg
                        lock (flight.CurrentLeg.SubLeg.Airplanes)
                        {
                            flight.CurrentLeg.SubLeg.Airplanes.Remove(flight);
                        }
                    }
                    Console.WriteLine($"flight-{flight.Id} || moved from leg - {flight.CurrentLeg.SubLeg.Id} to leg - {nextLeg.SubLeg.Id}");
                    // Update the current leg of the flight
                    flight.CurrentLeg = nextLeg;
                    // Add the flight to the airplanes list of the next leg
                    flight.CurrentLeg.SubLeg.Airplanes.Add(flight);
                    Console.WriteLine($"leg {nextLeg.SubLeg.Id} is ocupied by flight - {flight.Id} | at {DateTime.Now}");
                }
                Task.Run(async () =>
                {
                    await Task.Delay(2000); // Wait for 2 seconds asynchronously
                                            // Code to execute after the delay
                }).Wait();
                return nextLeg;
            }
            // Check if the current leg allows only one airplane at a time
            if (flight.CurrentLeg.SubLeg.ForOneAirplaneOnly) 
            {
                Console.WriteLine($"leg {flight.CurrentLeg.SubLeg.Id} is free by flight - {flight.Id} | at {DateTime.Now}");
            }
            if (flight.CurrentLeg.SubLeg.Airplanes.Contains(flight))
            {
                lock(flight.CurrentLeg.SubLeg.Airplanes)
                {
                    flight.CurrentLeg.SubLeg.Airplanes.Remove(flight);
                }
            }
            Console.WriteLine($"flight-{flight.Id} || moved from leg - {flight.CurrentLeg.SubLeg.Id} to leg - {nextLeg.SubLeg.Id}");
            flight.CurrentLeg = nextLeg;
            flight.CurrentLeg.SubLeg.Airplanes.Add(flight);
            return nextLeg;
        }
        private void WaitForLeg(Leg leg, Flight flight)
        {
            // Add the flight to the waiting list of the leg
            leg.WaitingList.Add(flight);
            while (true)
            {
                // Check if there are no airplanes currently on the leg
                if (leg.Airplanes?.Count == 0)
                {
                    // Check if the flight is the first one in the waiting list
                    if (leg.WaitingList.FirstOrDefault() == flight)
                    {
                        // Remove the flight from the waiting list and return
                        leg.WaitingList.Remove(flight);
                            return;
                    }
                }
                Task.Delay(1000);
            }
        }
    }

}

