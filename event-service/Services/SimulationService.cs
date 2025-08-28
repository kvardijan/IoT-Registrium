using event_service.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Timer = System.Timers.Timer;

namespace event_service.Services
{
    public class SimulationService
    {
        private readonly EventsDbContext _context;
        private readonly Dictionary<string, Timer> _activeSimulations = new();

        public SimulationService(EventsDbContext context)
        {
            _context = context;
        }

        public bool StartSimulation(string serialNumber, int typeId)
        {
            if (_activeSimulations.ContainsKey(serialNumber))
                return false;

            var timer = new Timer(1000);
            timer.Elapsed += (s, e) => GenerateMockEvent(serialNumber, typeId);
            timer.AutoReset = true;
            timer.Enabled = true;

            _activeSimulations[serialNumber] = timer;
            return true;
        }

        public bool StopSimulation(string serialNumber)
        {
            if (!_activeSimulations.TryGetValue(serialNumber, out var timer))
                return false;

            timer.Stop();
            timer.Dispose();
            _activeSimulations.Remove(serialNumber);

            return true;
        }

        public void StopAllSimulations()
        {
            foreach (var t in _activeSimulations.Values)
            {
                t.Stop();
                t.Dispose();
            }
            _activeSimulations.Clear();
        }

        private void GenerateMockEvent(string serialNumber, int typeId)
        {
            var recordedData = typeId switch
            {
                1 => new Dictionary<string, string> { { "temperature", $"{Random.Shared.Next(20, 40)}C" } },
                2 => new Dictionary<string, string> { { "humidity", $"{Random.Shared.Next(30, 90)}%" } },
                4 => new Dictionary<string, string> { { "capacity", $"{Random.Shared.Next(0, 100)}%" } },
                _ => new Dictionary<string, string> { { "unknown", "N/A" } }
            };

            var data = new { RecordedData = recordedData };

            var evnt = new Event
            {
                Device = serialNumber,
                Type = typeId,
                Data = JsonSerializer.Serialize(data),
                Timestamp = DateTime.UtcNow
            };

            _context.Events.Add(evnt);
            _context.SaveChanges();
        }
    }
}
