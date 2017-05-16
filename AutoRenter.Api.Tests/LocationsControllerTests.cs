using System;
using System.Collections.Generic;
using Xunit;
using AutoRenter.Domain.Models;
using AutoRenter.Domain.Interfaces;
using AutoRenter.Api.Controllers;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AutoRenter.Api.Tests
{
    public class LocationsControllerTests
    {
        [Fact]
        public async void GetAll_ReturnsData()
        {
            // arrange
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.GetAll())
                .ReturnsAsync(() => new Result<IEnumerable<Location>>(ResultCode.Success, TestLocations()));
            var validationServiceMoq = new Mock<IValidationService>();

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.GetAll();
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Dictionary<string, object>;

            // assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotEmpty(response.Values);
        }

        [Fact]
        public async void Get_WhenFound()
        {
            // arrange
            var targetId = new Guid("a341dc33-fe65-4c8d-a7b5-16be1741c02e");
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Location>(ResultCode.Success, TestLocation()));
            var validationServiceMoq = new Mock<IValidationService>();

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Get(targetId);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Dictionary<string, object>;

            // assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(response.Values.FirstOrDefault());
        }

        [Fact]
        public async void Get_WhenNotFound()
        {
            // arrange
            var targetId = new Guid("b341dc33-fe65-4c8d-a7b5-16be1741c02e");
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Location>(ResultCode.NotFound));
            var validationServiceMoq = new Mock<IValidationService>();

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Get(targetId);
            var notFoundResult = result as NotFoundResult;

            // assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async void Post_WhenValid()
        {
            // arrange
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Insert(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.Success, TestLocation().Id));
            var validationServiceMoq = new Mock<IValidationService>();
            validationServiceMoq.Setup(x => x.IsValidInsert(It.IsAny<Location>()))
                .ReturnsAsync(() => true);

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Post(TestLocation());
            var createdAtRouteResult = result as CreatedAtRouteResult;

            // assert
            Assert.NotNull(createdAtRouteResult);
        }

        [Fact]
        public async void Post_WhenNotValid()
        {
            // arrange
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Insert(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.BadRequest, TestLocation().Id));
            var validationServiceMoq = new Mock<IValidationService>();
            validationServiceMoq.Setup(x => x.IsValidInsert(It.IsAny<Location>()))
                .ReturnsAsync(() => false);

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Post(TestLocation());
            var badRequestResult = result as BadRequestResult;

            // assert
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async void Put_WhenValid()
        {
            // arrange
            var testLocation = TestLocation();
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Update(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.Success, testLocation.Id));
            var validationServiceMoq = new Mock<IValidationService>();
            validationServiceMoq.Setup(x => x.IsValidUpdate(It.IsAny<Location>()))
                .ReturnsAsync(() => true);

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Put(testLocation.Id, testLocation);
            var okResult = result as OkObjectResult;

            // assert
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async void Put_WhenNotValid()
        {
            // arrange
            var testLocation = TestLocation();
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Update(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.BadRequest, testLocation.Id));
            var validationServiceMoq = new Mock<IValidationService>();
            validationServiceMoq.Setup(x => x.IsValidUpdate(It.IsAny<Location>()))
                .ReturnsAsync(() => false);

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Put(testLocation.Id, testLocation);
            var badRequestResult = result as BadRequestResult;

            // assert
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async void Put_WhenConflict()
        {
            // arrange
            var testLocation = TestLocation();
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Update(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.Conflict, testLocation.Id));
            var validationServiceMoq = new Mock<IValidationService>();
            validationServiceMoq.Setup(x => x.IsValidUpdate(It.IsAny<Location>()))
                .ReturnsAsync(() => true);

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Put(testLocation.Id, testLocation);
            var conflictResult = result as StatusCodeResult;

            // assert
            Assert.NotNull(conflictResult);
            Assert.Equal(409, conflictResult.StatusCode);
        }

        [Fact]
        public async void Delete_WhenValid()
        {
            // arrange
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(() => ResultCode.Success);
            var validationServiceMoq = new Mock<IValidationService>();
            validationServiceMoq.Setup(x => x.IsValidDelete(It.IsAny<Location>()))
                .ReturnsAsync(() => true);

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Delete(TestLocation().Id);
            var noContentResult = result as NoContentResult;

            // assert
            Assert.NotNull(noContentResult);
        }

        [Fact]
        public async void Delete_WhenNotValid()
        {
            // arrange
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(() => ResultCode.BadRequest);
            var validationServiceMoq = new Mock<IValidationService>();
            validationServiceMoq.Setup(x => x.IsValidDelete(It.IsAny<Location>()))
                .ReturnsAsync(() => false);

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Delete(TestLocation().Id);
            var badRequestResult = result as BadRequestResult;

            // assert
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async void Delete_WhenNotFound()
        {
            // arrange
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(() => ResultCode.NotFound);
            var validationServiceMoq = new Mock<IValidationService>();
            validationServiceMoq.Setup(x => x.IsValidDelete(It.IsAny<Location>()))
                .ReturnsAsync(() => true);

            var sut = new LocationsController(locationServiceMoq.Object, validationServiceMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Delete(TestLocation().Id);
            var notFoundResult = result as NotFoundResult;

            // assert
            Assert.NotNull(notFoundResult);
        }

        private ControllerContext DefaultControllerContext()
        {
            var headerDictionary = new HeaderDictionary();
            var response = new Mock<HttpResponse>();
            response.SetupGet(r => r.Headers).Returns(headerDictionary);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(a => a.Response).Returns(response.Object);

            return new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
        }

        private Location TestLocation()
        {
            var locationId = Guid.Parse("a341dc33-fe65-4c8d-a7b5-16be1741c02e");
            var vehicleId = Guid.Parse("52731074-43be-4e67-8374-17ee4ec3369d");
            var makeId = Guid.Parse("4f307da8-6bb6-404d-b214-38028a9be953");
            var modelId = Guid.Parse("aa23f971-b3be-43c7-8073-ea2c36e3a6fe");

            var make = new Make()
            {
                Id = makeId,
                ExternalId = "MakeId",
                Name = "MakeName"
            };

            var model = new Model()
            {
                Id = modelId,
                ExternalId = "ModelId",
                Name = "ModelName"
            };

            var vehicles = new List<Vehicle>()
            {
                new Vehicle()
                {
                    Id = Guid.NewGuid(),
                    Color = "blue",
                    IsRentToOwn = false,
                    LocationId = locationId,
                    MakeId = make.ExternalId,
                    Make = make,
                    ModelId = model.ExternalId,
                    Model = model,
                    Miles = 1000,
                    Vin = "0XJ9TTYZ6N7M81234",
                    Year = 2016
                }
            };

            return new Location()
            {
                Id = locationId,
                City = "Indianapolis",
                StateCode = "IN",
                Name = "Indy Location",
                SiteId = "1",
                Vehicles = vehicles
            };
        }

        private IEnumerable<Location> TestLocations()
        {
            return new List<Location>()
            {
                TestLocation()
            };
        }
    }
}
