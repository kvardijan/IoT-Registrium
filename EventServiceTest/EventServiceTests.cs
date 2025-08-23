using event_service.DTOs;
using event_service.Models;
using event_service.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;

namespace EventServiceTests
{
    public class EventServiceTests
    {
        private EventsDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<EventsDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new EventsDbContext(options);

            if (!context.Types.Any())
            {
                context.Types.Add(new event_service.Models.Type { Id = 1, Description = "device added" });
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public void GetEventById_ShouldReturnEvent_WhenExists()
        {
            var context = GetDbContext("GetEventByIdDb");
            var evnt = new Event { Device = "SN123", Type = 1, Data = "data" };
            context.Events.Add(evnt);
            context.SaveChanges();

            var service = new EventService(context);

            var result = service.GetEventById(evnt.Id);

            Assert.NotNull(result);
            Assert.Equal("SN123", result.Device);
            Assert.Equal("device added", result.Type);
        }

        [Fact]
        public void GetEventById_ShouldReturnNull_WhenNotFound()
        {
            var context = GetDbContext("GetEventByIdNullDb");
            var service = new EventService(context);

            var result = service.GetEventById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetEvents_ShouldReturnAllEvents()
        {
            var context = GetDbContext("GetEventsDb");
            context.Events.Add(new Event { Device = "SN1", Type = 1, Data = "data" });
            context.Events.Add(new Event { Device = "SN2", Type = 1, Data = "data" });
            context.SaveChanges();

            var service = new EventService(context);

            var result = service.GetEvents();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Device == "SN1");
            Assert.Contains(result, e => e.Device == "SN2");
        }

        [Fact]
        public void GetEventsOfDevice_ShouldReturnEventsForThatDevice()
        {
            var context = GetDbContext("GetEventsOfDeviceDb");
            context.Events.Add(new Event { Device = "SNX", Type = 1, Data = "data" });
            context.Events.Add(new Event { Device = "SNY", Type = 1, Data = "data" });
            context.SaveChanges();

            var service = new EventService(context);

            var result = service.GetEventsOfDevice("SNX");

            Assert.Single(result);
            Assert.Equal("SNX", result[0].Device);
        }

        [Fact]
        public void CreateEvent_ShouldInsertEventAndReturnResponse()
        {
            var context = GetDbContext("CreateEventDb");
            var service = new EventService(context);

            var dto = new EventCreationDto
            {
                Device = "SN555",
                Type = 1,
                Data = "data"
            };

            var response = service.CreateEvent(dto);

            Assert.NotNull(response);
            Assert.Equal("SN555", response.Device);
            Assert.Equal("device added", response.Type);
            Assert.Equal("data", response.Data);

            var saved = context.Events.FirstOrDefault(e => e.Id == response.Id);
            Assert.NotNull(saved);
            Assert.Equal("SN555", saved.Device);
        }

        [Fact]
        public void CreateEvent_ShouldThrow_WhenInvalidType()
        {
            var context = GetDbContext("InvalidTypeDb");
            var service = new EventService(context);

            var dto = new EventCreationDto
            {
                Device = "SN999",
                Type = 999,
                Data = "xxx"
            };

            Assert.Throws<Exception>(() => service.CreateEvent(dto));
        }

        [Fact]
        public void GetEvents_ShouldReturnEmptyList_WhenNoEventsExist()
        {
            var context = GetDbContext("EmptyEventsDb");
            var service = new EventService(context);

            var result = service.GetEvents();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetEventsOfDevice_ShouldReturnEmptyList_WhenNoEventsForDevice()
        {
            var context = GetDbContext("NoEventsForDeviceDb");
            context.Events.Add(new Event { Device = "SN1", Type = 1, Data = "data" });
            context.SaveChanges();

            var service = new EventService(context);

            var result = service.GetEventsOfDevice("SN999");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void CreateEvent_ShouldSetTimestamp()
        {
            var context = GetDbContext("CreateEventTimestampDb");
            var service = new EventService(context);

            var dto = new EventCreationDto { Device = "SN777", Type = 1, Data = "data" };
            var response = service.CreateEvent(dto);

            Assert.True(response.Timestamp <= DateTime.UtcNow && response.Timestamp >= DateTime.UtcNow.AddSeconds(-5));
        }

        [Fact]
        public void CreateEvent_ShouldReturnCorrectTypeDescription()
        {
            var context = GetDbContext("CreateEventTypeDescDb");
            context.Types.Add(new event_service.Models.Type { Id = 2, Description = "device updated" });
            context.SaveChanges();

            var service = new EventService(context);
            var dto = new EventCreationDto { Device = "SN888", Type = 2, Data = "updated data" };

            var response = service.CreateEvent(dto);

            Assert.Equal("device updated", response.Type);
        }

        [Fact]
        public void GetEventById_ShouldReturnTypeDescription_FromNavigationProperty()
        {
            var context = GetDbContext("GetEventByIdTypeNavDb");
            context.Types.Add(new event_service.Models.Type { Id = 3, Description = "custom type" });
            context.Events.Add(new Event { Device = "SN333", Type = 3, Data = "custom" });
            context.SaveChanges();

            var service = new EventService(context);

            var result = service.GetEventById(1);

            Assert.NotNull(result);
            Assert.Equal("custom type", result.Type);
        }

        [Fact]
        public void GetEvents_ShouldMapAllFieldsCorrectly()
        {
            var context = GetDbContext("GetEventsMappingDb");
            var evt = new Event { Device = "SN101", Type = 1, Data = "xyz" };
            context.Events.Add(evt);
            context.SaveChanges();

            var service = new EventService(context);
            var result = service.GetEvents().First();

            Assert.Equal(evt.Device, result.Device);
            Assert.Equal(evt.Type, result.TypeId);
            Assert.Equal(evt.Data, result.Data);
            Assert.Equal(evt.Timestamp, result.Timestamp);
        }
    }
}
