using Microsoft.CodeAnalysis.Elfie.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAirportService
    {
        Task<List<Flight>> StartSim();
        Flight AddFlight();
        Leg GetNextPoint(IFlight flight);

    }
    public class AirportService : IAirportService
    {
        private IAirport _airport;
        private List<Flight> leg4Queue;
        private List<Flight> leg6Or7Queue;
        public AirportService(IAirport airport)
        {
            _airport = airport;
            leg4Queue = new List<Flight>();
            leg6Or7Queue = new List<Flight>();
        }
        private Flight CreateRandomFlight()
        {
            Random rand = new Random();
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
            return new Flight
            {
                Id = _airport.Flights.Count == 0 ? 1 : _airport.Flights.Max(f => f.Id) + 1,
                Airplane = airplanesModels[rand.Next(airplanesModels.Count)],
                Road = new List<Leg>(),
                TakeOff = DateTime.Now,
                IsLanding = true/*rand.Next(2) == 0*/
            };
        }
        public Flight AddFlight()
        {
            Flight flight = CreateRandomFlight();
            if (_airport.Flights == null || _airport.Flights.Count == 0) { _airport.Flights = new List<Flight> { flight }; }
            else { _airport.Flights.Add(flight); }
            return flight;
        }
        public Leg GetNextPoint(IFlight flight)
        {
            if (flight.Road == null || flight.Road?.Count == 0)
            {
                Random rnd = new Random();
                return flight.IsLanding
                    ? _airport.GetLeg(1)
                    : _airport.GetLeg(6).Airplanes?.Count == 0 ? _airport.GetLeg(6) : _airport.GetLeg(7);
            }
            Leg currentLeg = flight.Road![flight.Road.Count - 1];
            Leg nextLeg = null;
            switch (flight.IsLanding)
            {
                case false:
                    switch (flight.Road.Last().Id)
                    {
                        case 6:
                        case 7:
                            nextLeg = _airport.GetLeg(8);
                            break;
                        case 8:
                            nextLeg = _airport.GetLeg(4);
                            break;
                        case 4:
                            nextLeg = _airport.GetLeg(9);
                            break;
                        default:
                            break;
                    }
                    break;
                case true:
                    switch (flight.Road.Last().Id)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            nextLeg = _airport.GetLeg(flight.Road.Last().Id + 1);
                            break;
                        case 5:
                            nextLeg = _airport.GetLeg(6).Airplanes?.Count == 0
                                ? _airport.GetLeg(6)
                                : _airport.GetLeg(7);
                            break;
                        default:
                            break;
                    }
                    break;
            }
            return nextLeg;
        }

        public async Task<List<Flight>> StartSim()
        {
            Console.WriteLine("starting simulator");
            _airport.Flights = new List<Flight> { new Flight { Id = 1, Airplane = "mama", IsLanding = true, Road = new List<Leg>(), TakeOff = DateTime.Now },
                                                  new Flight { Id = 2, Airplane = "lolo", IsLanding = true, Road = new List<Leg>(), TakeOff = DateTime.Now }};
            AddFlight();

            var tasks = _airport.Flights.Select(flight => Task.Run(() => MakeFlight(flight)));
            await Task.WhenAll(tasks);

            Console.WriteLine("all flights ended");
            return _airport.Flights;
            //todo: make a loop that each time make another flight with AddFlight()
        }

        public void WaitForLeg4(Flight flight)
        {
            while (true)
            {
                if (_airport.GetLeg(4).Airplanes?.Count == 0)
                {
                    if (leg4Queue.FirstOrDefault() == flight)
                    {
                        leg4Queue.Remove(flight);
                        return;
                    }
                }
                Task.Delay(1000);
            }
        }
        public int WaitForLeg6Or7(Flight flight)
        {
            while (true)
            {
                if (_airport.GetLeg(6).Airplanes?.Count == 0 || _airport.GetLeg(7).Airplanes?.Count == 0)
                {
                    if (leg6Or7Queue.FirstOrDefault() == flight)
                    {
                        leg6Or7Queue.Remove(flight);
                        return _airport.GetLeg(6).Airplanes?.Count == 0 ? 6 : 7;
                    }
                }
                Task.Delay(1000);
            }
        }

        public async Task MakeFlight(Flight flight)
        {
            var nextLeg = GetNextPoint(flight);
            if (nextLeg == null) return;
            else
            {
                do
                {
                    if (flight.IsLanding)
                    {
                        if (flight.Road == null || flight.Road.Count == 0)
                        {
                            flight.Road = new List<Leg> { _airport.GetLeg(1) };
                            _airport.GetLeg(1).Airplanes.Add(flight);
                            Console.WriteLine($"Landing Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                            nextLeg = GetNextPoint(flight);
                        }

                        switch (nextLeg.Id)
                        {
                            case 2:
                            case 3:
                            case 5:
                                flight.Road.Last().Airplanes.Remove(flight);
                                flight.Road.Add(nextLeg);
                                nextLeg.Airplanes.Add(flight);
                                break;
                            case 4:
                                leg4Queue.Add(flight);
                                Task.Run(() => WaitForLeg4(flight));
                                lock (_airport.GetLeg(4))
                                {
                                    Console.WriteLine($"leg 4 is ocupied by flight {flight.Id}");
                                    Thread.Sleep(2000);
                                    flight.Road.Last().Airplanes.Remove(flight);
                                    flight.Road.Add(nextLeg);
                                    nextLeg.Airplanes.Add(flight);
                                    Console.WriteLine($"leg 4 is free by flight {flight.Id}");
                                }
                                break;
                            case 6:
                            case 7:
                                int leg;
                                leg6Or7Queue.Add(flight);
                                leg = await Task.Run(() => WaitForLeg6Or7(flight));
                                lock (_airport.GetLeg(leg))
                                {
                                    Console.WriteLine($"leg {leg} is ocupied by flight {flight.Id}");
                                    Thread.Sleep(2000);
                                    flight.Road.Last().Airplanes.Remove(flight);
                                    flight.Road.Add(_airport.GetLeg(leg));
                                    _airport.GetLeg(leg).Airplanes.Add(flight);
                                    Console.WriteLine($"leg {leg} is free by flight {flight.Id}");
                                }
                                break;
                            default:
                                flight.Road.Last().Airplanes.Remove(flight);
                                break;
                        }
                        if (flight.Road.Last().Id == 6 || flight.Road.Last().Id == 7)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        Console.WriteLine($"Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                        Console.ForegroundColor = ConsoleColor.White;
                        nextLeg = GetNextPoint(flight);
                    }
                    else //flight is takeoff
                    {
                        if (flight.Road == null || flight.Road.Count == 0)
                        {
                            int leg;
                            leg6Or7Queue.Add(flight);
                            leg = await Task.Run(() => WaitForLeg6Or7(flight));
                            lock (_airport.GetLeg(leg))
                            {
                                Console.WriteLine($"leg {leg} is ocupied by flight {flight.Id}");
                                Thread.Sleep(2000);
                                flight.Road.Last().Airplanes.Remove(flight);
                                flight.Road.Add(_airport.GetLeg(leg));
                                _airport.GetLeg(leg).Airplanes.Add(flight);
                                Console.WriteLine($"leg {leg} is free by flight {flight.Id}");
                            }
                        }

                        switch (nextLeg.Id)
                        {
                            case 8:
                                flight.Road.Last().Airplanes.Remove(flight);
                                flight.Road.Add(nextLeg);
                                nextLeg.Airplanes.Add(flight);
                                break;
                            case 4:
                                leg4Queue.Add(flight);
                                Task.Run(() => WaitForLeg4(flight));
                                lock (_airport.GetLeg(4))
                                {
                                    Console.WriteLine($"leg 4 is ocupied by flight {flight.Id}");
                                    Thread.Sleep(2000);
                                    flight.Road.Last().Airplanes.Remove(flight);
                                    flight.Road.Add(nextLeg);
                                    nextLeg.Airplanes.Add(flight);
                                    Console.WriteLine($"leg 4 is free by flight {flight.Id}");
                                }
                                break;
                            case 9:
                            default:
                                flight.Road.Last().Airplanes.Remove(flight);
                                break;
                        }
                        if (flight.Road.Last().Id == 6 || flight.Road.Last().Id == 7)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        Console.WriteLine($"Flight - {flight.Id} | {DateTime.Now} | Leg - {flight.Road.Last().Id}");
                        Console.ForegroundColor = ConsoleColor.White;
                        nextLeg = GetNextPoint(flight);
                    }
                    await Task.Delay(1000);
                } while (nextLeg != null);

            }
        }
    }

}

