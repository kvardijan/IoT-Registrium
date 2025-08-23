using location_service.DTOs;
using location_service.Models;
using location_service.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;

namespace location_service.Tests
{
    public class LocationServiceTests
    {
        private LocationsDbContext GetDbContext(string dbName, bool seed = true)
        {
            var options = new DbContextOptionsBuilder<LocationsDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new LocationsDbContext(options);

            if (seed && !context.Locations.Any())
            {
                context.Locations.Add(new Location
                {
                    Id = 1,
                    Latitude = "10.000",
                    Longitude = "20.000",
                    Address = "Pavlinska 2, HR 42000 Varaždin",
                    Description = "University"
                });
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public void GetLocations_ShouldReturnAllLocations()
        {
            var context = GetDbContext("GetLocationsDb");
            var service = new LocationService(context);

            var result = service.GetLocations();

            Assert.NotNull(result);
            Assert.True(result.Count >= 1);
        }

        [Fact]
        public void GetLocationById_ShouldReturnLocation_WhenExists()
        {
            var context = GetDbContext("GetLocationByIdDb");
            var service = new LocationService(context);

            var location = service.GetLocationById(1);

            Assert.NotNull(location);
            Assert.Equal("Pavlinska 2, HR 42000 Varaždin", location.Address);
        }

        [Fact]
        public void GetLocationById_ShouldReturnNull_WhenNotFound()
        {
            var context = GetDbContext("GetLocationByIdNotFoundDb");
            var service = new LocationService(context);

            var location = service.GetLocationById(999);

            Assert.Null(location);
        }

        [Fact]
        public void CreateLocation_ShouldAddNewLocation()
        {
            var context = GetDbContext("CreateLocationDb", seed: false);
            var service = new LocationService(context);

            var dto = new LocationCreationDto
            {
                Latitude = "11.111",
                Longitude = "22.222",
                Address = "nova adresa 1, Negdje",
                Description = "opis"
            };

            var result = service.CreateLocation(dto);

            Assert.NotNull(result);
            Assert.Equal("nova adresa 1, Negdje", result.Address);
            Assert.Equal(1, context.Locations.Count());
        }

        [Fact]
        public void UpdateLocation_ShouldUpdateExistingLocation()
        {
            var context = GetDbContext("UpdateLocationDb", seed: false);
            context.Locations.Add(new Location
            {
                Id = 1,
                Latitude = "10.000",
                Longitude = "20.000",
                Address = "Old Address",
                Description = "Old Desc"
            });
            context.SaveChanges();

            var service = new LocationService(context);

            var dto = new LocationUpdateDto { Address = "promjenjena 12" };

            var result = service.UpdateLocation(1, dto);

            Assert.NotNull(result);
            Assert.Equal("promjenjena 12", result.Address);
        }

        [Fact]
        public void UpdateLocation_ShouldReturnNull_WhenLocationNotFound()
        {
            var context = GetDbContext("UpdateLocationNotFoundDb", seed: false);
            var service = new LocationService(context);

            var dto = new LocationUpdateDto { Address = "ulica ne postoji" };

            var result = service.UpdateLocation(999, dto);

            Assert.Null(result);
        }

        [Fact]
        public void UpdateLocation_ShouldNotOverwriteFields_WhenEmptyStringsProvided()
        {
            var context = GetDbContext("UpdateLocationEmptyDb", seed: false);
            context.Locations.Add(new Location
            {
                Id = 1,
                Latitude = "10.000",
                Longitude = "20.000",
                Address = "Old Address",
                Description = "Old Desc"
            });
            context.SaveChanges();

            var service = new LocationService(context);

            var dto = new LocationUpdateDto
            {
                Latitude = "",
                Longitude = "",
                Address = "",
                Description = ""
            };

            var original = context.Locations.First();
            var result = service.UpdateLocation(1, dto);

            Assert.NotNull(result);
            Assert.Equal(original.Latitude, result.Latitude);
            Assert.Equal(original.Longitude, result.Longitude);
            Assert.Equal(original.Address, result.Address);
            Assert.Equal(original.Description, result.Description);
        }

        [Fact]
        public void UpdateLocation_ShouldUpdateOnlySpecifiedFields()
        {
            var context = GetDbContext("UpdateLocationPartialDb", seed: false);
            context.Locations.Add(new Location
            {
                Id = 1,
                Latitude = "10.000",
                Longitude = "20.000",
                Address = "Old Address",
                Description = "University"
            });
            context.SaveChanges();

            var service = new LocationService(context);

            var dto = new LocationUpdateDto { Address = "nova adresa" };

            var result = service.UpdateLocation(1, dto);

            Assert.NotNull(result);
            Assert.Equal("nova adresa", result.Address);
            Assert.Equal("10.000", result.Latitude);
            Assert.Equal("20.000", result.Longitude);
            Assert.Equal("University", result.Description);
        }

        [Fact]
        public void CreateLocation_ShouldMapAllFieldsCorrectly()
        {
            var context = GetDbContext("CreateLocationMappingDb", seed: false);
            var service = new LocationService(context);

            var dto = new LocationCreationDto
            {
                Latitude = "50.123",
                Longitude = "15.456",
                Address = "Test Address",
                Description = "Test Desc"
            };

            var result = service.CreateLocation(dto);

            Assert.NotNull(result);
            Assert.Equal("50.123", result.Latitude);
            Assert.Equal("15.456", result.Longitude);
            Assert.Equal("Test Address", result.Address);
            Assert.Equal("Test Desc", result.Description);
        }

        [Fact]
        public void GetLocations_ShouldReturnEmptyList_WhenNoLocations()
        {
            var context = GetDbContext("EmptyLocationsDb", seed: false);
            var service = new LocationService(context);

            var result = service.GetLocations();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void UpdateLocation_ShouldNotChangeFields_WhenDtoHasNulls()
        {
            var context = GetDbContext("UpdateNullFieldsDb", seed: false);
            context.Locations.Add(new Location
            {
                Id = 1,
                Latitude = "10.000",
                Longitude = "20.000",
                Address = "Old Address",
                Description = "Old Desc"
            });
            context.SaveChanges();

            var service = new LocationService(context);

            var dto = new LocationUpdateDto
            {
                Latitude = null,
                Longitude = null,
                Address = null,
                Description = null
            };

            var original = context.Locations.First();
            var result = service.UpdateLocation(1, dto);

            Assert.NotNull(result);
            Assert.Equal(original.Latitude, result.Latitude);
            Assert.Equal(original.Longitude, result.Longitude);
            Assert.Equal(original.Address, result.Address);
            Assert.Equal(original.Description, result.Description);
        }

        [Fact]
        public void UpdateLocation_ShouldUpdateMultipleFields()
        {
            var context = GetDbContext("UpdateMultipleFieldsDb", seed: false);
            context.Locations.Add(new Location
            {
                Id = 1,
                Latitude = "10.000",
                Longitude = "20.000",
                Address = "Old Address",
                Description = "Old Desc"
            });
            context.SaveChanges();

            var service = new LocationService(context);

            var dto = new LocationUpdateDto
            {
                Latitude = "11.111",
                Longitude = "22.222",
                Address = "New Address",
                Description = "New Desc"
            };

            var result = service.UpdateLocation(1, dto);

            Assert.NotNull(result);
            Assert.Equal("11.111", result.Latitude);
            Assert.Equal("22.222", result.Longitude);
            Assert.Equal("New Address", result.Address);
            Assert.Equal("New Desc", result.Description);
        }
    }
}
