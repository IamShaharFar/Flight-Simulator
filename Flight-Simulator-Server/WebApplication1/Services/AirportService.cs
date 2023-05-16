using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using WebApplication1.Dal;
using WebApplication1.Hubs;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAirportService
    {
        Task<List<Flight>> StartSim();
        Task<Flight> AddFlight();
        Leg GetNextPoint(Flight flight);

    }
    public class AirportService : IAirportService
    {
        private IMongoCollection<Flight> _flights;
        private IMongoCollection<Leg> _legs;
        private IAirport _airport;
        private IHubContext<FlightHub> _flightHubContext;
        public AirportService(IMongoClient mongoClient, IHubContext<FlightHub> flightHubContext)
        {
            if(_airport == null) _airport = new Airport();
            var db = mongoClient.GetDatabase("FlightSimulator");
            _flights = db.GetCollection<Flight>("Flights");
            _legs = db.GetCollection<Leg>("Legs");
            if (_flights.AsQueryable() != null)
            {
                _airport.Flights = _flights.AsQueryable().ToList();
            }
            _flightHubContext = flightHubContext;
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
        public async Task<Flight> AddFlight()
        {
            // Create a new random flight
            Flight flight = CreateRandomFlight();
            var maxIdFlight = _flights?.AsQueryable()?
                          .OrderByDescending(f => f.Id)?
                          .FirstOrDefault();
            flight.Id = maxIdFlight == null ? 1 : maxIdFlight.Id + 1;
            // Add the flight to the airport's flights collection
            await _flights.InsertOneAsync(flight);
            _airport.Flights.Add(flight);

            // Print information about the newly added flight
            Console.WriteLine($"New flight | Flight number: {flight.Id} | {(flight.IsLanding ? "Landing flight" : "Takeoff flight")}");

            return flight;
        }
        public Leg GetNextPoint(Flight flight)
        {
            if (flight.CurrentLeg == null)
            {
                // If the flight doesn't have a current leg, determine the next leg to move to

                var leg = (Leg)flight.Road.First();
                return leg;
            }
            else
            {
                // If the flight has a current leg, determine the next leg based on its position in the road
                var currentIndex = flight.Road.ToList().IndexOf(flight.CurrentLeg.MultyLeg);

                // If the current leg is the last leg in the road, there is no next leg
                if (currentIndex >= flight.Road.Count - 1) return null;
                var nextLeg = flight.Road.ToList()[currentIndex + 1];
                return nextLeg;
            }
        }
        public async Task<List<Flight>> StartSim()
        {
            await _flights.DeleteManyAsync(FilterDefinition<Flight>.Empty);
            if (!_legs.AsQueryable().ToList().SequenceEqual(_airport.Legs))
            {
                await _legs.DeleteManyAsync(FilterDefinition<Leg>.Empty);
                await _legs.InsertManyAsync(_airport.Legs);
            }
            _airport = new Airport();
            Console.WriteLine("starting simulator");
            await AddFlight();

            var tasks = _airport.Flights.Select(flight => Task.Run(() => MakeFlight(flight)));
            await Task.WhenAll(tasks);


            Console.WriteLine("all flights ended");
            return _flights.AsQueryable().ToList();
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
                    await _flightHubContext.Clients.All.SendAsync("SendNextLegAndFlightId", flight.CurrentLeg.SubLeg.Id, flight.Id);
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
                await _flightHubContext.Clients.All.SendAsync("SendNextLegAndFlightId", -1, flight.Id);
                lock (Console.Out)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"flight-{flight.Id} || moved to no where");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
        private async Task UpdateFlightToDb(Flight flight)
        {
            var toUpdate = await _flights.FindAsync(f => f.Id == flight.Id);
            var existingFlight = await toUpdate.FirstOrDefaultAsync();
            if (existingFlight != null)
            {
                try
                {
                    var result = await _flights.ReplaceOneAsync(f => f.Id == flight.Id, flight);
                }
                catch (Exception ex)
                { Console.WriteLine(ex); }
                // Handle the update result if needed
            }
        }
        private async Task<LegStation> MoveFlightToNextLeg(Flight flight)
        {
            var next = GetNextPoint(flight);
            var nextLegStation = new LegStation { MultyLeg = next };
            // Check if the flight is currently not on any leg
            if (flight.CurrentLeg == null)
            {
                // Check if the next leg is MultyLeg and allows only one airplane at a time
                if (next is MultyLeg)
                {
                    var multyNext = (MultyLeg)next;
                    Leg leg = await WaitForMultyLegAsync(multyNext, flight);
                    lock(leg)
                    {
                        nextLegStation.SubLeg = leg;
                        flight.CurrentLeg = nextLegStation;
                        UpdateFlightToDb(flight);
                        // Add the flight to the airplanes list of the next leg
                        if (!flight.CurrentLeg.SubLeg.Airplanes.Contains(flight))
                        {
                            flight.CurrentLeg.SubLeg.Airplanes.Add(flight);
                        }
                        Console.WriteLine($"leg {leg.Id} is ocupied by flight - {flight.Id} | at {DateTime.Now}");
                        Task.Run(async () =>
                        {
                            await Task.Delay(2000); // Wait for 2 seconds asynchronously
                                                    // Code to execute after the delay
                        }).Wait();
                    }
                    //await _flightHubContext.Clients.All.SendAsync("SendNextLegAndFlightId", flight.CurrentLeg.SubLeg.Id, flight.Id);
                    return nextLegStation;
                }
                else if(next.ForOneAirplaneOnly)
                {
                    nextLegStation.SubLeg = nextLegStation.MultyLeg;
                    WaitForLeg(nextLegStation.SubLeg, flight);
                    lock (nextLegStation.SubLeg)
                    {
                        // Update the current leg of the flight
                        flight.CurrentLeg = nextLegStation;
                        UpdateFlightToDb(flight);
                        // Add the flight to the airplanes list of the next leg
                        if (!flight.CurrentLeg.SubLeg.Airplanes.Contains(flight))
                        {
                            flight.CurrentLeg.SubLeg.Airplanes.Add(flight);
                        }
                        Console.WriteLine($"leg {nextLegStation.SubLeg.Id} is ocupied by flight - {flight.Id} | at {DateTime.Now}");
                        Task.Run(async () =>
                        {
                            await Task.Delay(2000); // Wait for 2 seconds asynchronously
                                                    // Code to execute after the delay
                        }).Wait();
                    }
                    return nextLegStation;
                }

                // Update the current leg of the flight if the next leg is for more than one airplane
                nextLegStation.SubLeg = nextLegStation.MultyLeg;
                flight.CurrentLeg = nextLegStation;
                await UpdateFlightToDb(flight);
                //await _flightHubContext.Clients.All.SendAsync("SendNextLegAndFlightId", flight.CurrentLeg.SubLeg.Id, flight.Id);
                return nextLegStation;
            }
            // Check if there is no next leg available
            if (next == null)
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
            if (next is MultyLeg)
            {
                var multyNext = (MultyLeg)next;
                Leg leg = await WaitForMultyLegAsync(multyNext, flight);
                lock (leg)
                {
                    nextLegStation.SubLeg = leg;
                    // Check if the flight is still in the airplanes list of the current leg
                    if (flight.CurrentLeg.SubLeg.Airplanes.Contains(flight))
                    {
                        // Remove the flight from the airplanes list of the current leg
                        lock (flight.CurrentLeg.SubLeg.Airplanes)
                        {
                            flight.CurrentLeg.SubLeg.Airplanes.Remove(flight);
                        }
                    }
                    Console.WriteLine($"flight-{flight.Id} || moved from leg - {flight.CurrentLeg.SubLeg.Id} to leg - {nextLegStation.SubLeg.Id}");
                    // Update the current leg of the flight
                    flight.CurrentLeg = nextLegStation;
                    UpdateFlightToDb(flight);
                    // Add the flight to the airplanes list of the next leg
                    if (!flight.CurrentLeg.SubLeg.Airplanes.Contains(flight))
                    {
                        flight.CurrentLeg.SubLeg.Airplanes.Add(flight);
                    }
                    Console.WriteLine($"leg {nextLegStation.SubLeg.Id} is ocupied by flight - {flight.Id} | at {DateTime.Now}");
                    Task.Run(async () =>
                    {
                        await Task.Delay(2000); // Wait for 2 seconds asynchronously
                                                // Code to execute after the delay
                    }).Wait();
                }
                //await _flightHubContext.Clients.All.SendAsync("SendNextLegAndFlightId", flight.CurrentLeg.SubLeg.Id, flight.Id);
                return nextLegStation;
            }
            // Check if the current leg allows only one airplane at a time
            else if (next.ForOneAirplaneOnly)
            {
                nextLegStation.SubLeg = nextLegStation.MultyLeg;
                // Wait for the next leg to be available and acquire a lock on it
                WaitForLeg(nextLegStation.SubLeg, flight);
                lock (nextLegStation.SubLeg)
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
                    Console.WriteLine($"flight-{flight.Id} || moved from leg - {flight.CurrentLeg.SubLeg.Id} to leg - {nextLegStation.SubLeg.Id}");
                    // Update the current leg of the flight
                    flight.CurrentLeg = nextLegStation;
                    UpdateFlightToDb(flight);
                    // Add the flight to the airplanes list of the next leg
                    if (!flight.CurrentLeg.SubLeg.Airplanes.Contains(flight))
                    {
                        flight.CurrentLeg.SubLeg.Airplanes.Add(flight);
                    }
                    Console.WriteLine($"leg {nextLegStation.SubLeg.Id} is ocupied by flight - {flight.Id} | at {DateTime.Now}");
                    Task.Run(async () =>
                    {
                        await Task.Delay(2000); // Wait for 2 seconds asynchronously
                                                // Code to execute after the delay
                    }).Wait();
                }
                return nextLegStation;
            }
                      
            if (flight.CurrentLeg.SubLeg.ForOneAirplaneOnly)
            {
                Console.WriteLine($"leg {flight.CurrentLeg.SubLeg.Id} is free by flight - {flight.Id} | at {DateTime.Now}");
            }
            if (flight.CurrentLeg.SubLeg.Airplanes.Contains(flight))
            {
                lock (flight.CurrentLeg.SubLeg.Airplanes)
                {
                    flight.CurrentLeg.SubLeg.Airplanes.Remove(flight);
                }
            }
            nextLegStation.SubLeg = nextLegStation.MultyLeg;
            Console.WriteLine($"flight-{flight.Id} || moved from leg - {flight.CurrentLeg.SubLeg.Id} to leg - {nextLegStation.SubLeg.Id}");
            flight.CurrentLeg = nextLegStation;
            await UpdateFlightToDb(flight);
            flight.CurrentLeg.SubLeg.Airplanes.Add(flight);
            //await _flightHubContext.Clients.All.SendAsync("SendNextLegAndFlightId", flight.CurrentLeg.SubLeg.Id, flight.Id);
            return nextLegStation;
        }
        private async Task<Leg> WaitForMultyLegAsync(MultyLeg multyNext, Flight flight)
        {
            multyNext.WaitForAll.Add(flight);
            Leg leg = null;
            while (true)
            {
                foreach(var l in multyNext.Stations)
                {
                    if(l.Airplanes.Count == 0)
                    {
                        if (multyNext.WaitForAll.FirstOrDefault() == flight)
                        {
                            lock (l)
                            {
                                l.Airplanes.Add(flight);
                                multyNext.WaitForAll.Remove(flight);
                            }
                            // Remove the flight from the waiting list and return
                            return l;
                        }
                    }
                }
                await Task.Delay(1000); 
            }
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
                        lock (leg)
                        {
                            leg.Airplanes.Add(flight);
                            leg.WaitingList.Remove(flight);
                        }
                        // Remove the flight from the waiting list and return
                        return;
                    }
                }
                Task.Delay(1000);
            }
        }
    }

}

