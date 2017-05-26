using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AutoRenter.Api.Controllers;
using AutoRenter.Api.Services;
using AutoRenter.Domain.Interfaces;
using AutoRenter.Domain.Models;

namespace AutoRenter.Api.Tests
{
    public class LocationsControllerTests
    {
        [Fact]
        public async void GetAll_WhenFound()
        {
            // arrange
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.GetAll())
                .ReturnsAsync(() => new Result<IEnumerable<Location>>(ResultCode.Success, TestLocations()));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.GetAll();
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Dictionary<string, object>;

            // assert
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async void GetAll_WhenNotFound()
        {
            // arrange
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.GetAll())
                .ReturnsAsync(() => new Result<IEnumerable<Location>>(ResultCode.NotFound));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.GetAll();
            var notFoundResult = result as NotFoundResult;
            
            // assert
            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async void GetAll_ReturnsData()
        {
            // arrange
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.GetAll())
                .ReturnsAsync(() => new Result<IEnumerable<Location>>(ResultCode.Success, TestLocations()));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.GetAll();
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Dictionary<string, object>;

            // assert
            Assert.NotNull(response.Values);
        }

        [Fact]
        public async void Get_WhenFound()
        {
            // arrange
            var resultCodeProcessor = new ResultCodeProcessor();
            var targetId = new Guid("a341dc33-fe65-4c8d-a7b5-16be1741c02e");
            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Location>(ResultCode.Success, TestLocation()));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Get(targetId);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Dictionary<string, object>;

            // assert
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async void Get_WhenNotFound()
        {
            // arrange
            var targetId = new Guid("b341dc33-fe65-4c8d-a7b5-16be1741c02e");
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Location>(ResultCode.NotFound));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Get(targetId);
            var notFoundResult = result as NotFoundResult;

            // assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async void Get_WithBadId()
        {
            // arrange
            var targetId = Guid.Empty;
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Location>(ResultCode.Success, TestLocation()));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Get(targetId);
            var badRequestResult = result as BadRequestObjectResult;

            // assert
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async void Get_ReturnsData()
        {
            // arrange
            var targetId = new Guid("a341dc33-fe65-4c8d-a7b5-16be1741c02e");
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Location>(ResultCode.Success, TestLocation()));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Get(targetId);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Dictionary<string, object>;

            // assert
            Assert.NotNull(response.Values);
        }

        [Fact]
        public async void Post_WhenValid()
        {
            // arrange
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Insert(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.Success, TestLocation().Id));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
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
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Insert(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.BadRequest, TestLocation().Id));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
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
        public async void Post_WhenConflict()
        {
            // arrange
            var testLocation = TestLocation();
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Insert(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.Conflict, testLocation.Id));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Post(testLocation);
            var conflictResult = result as StatusCodeResult;

            // assert
            Assert.Equal(409, conflictResult.StatusCode);
        }

        [Fact]
        public async void Put_WhenValid()
        {
            // arrange
            var testLocation = TestLocation();
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Update(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.Success, testLocation.Id));

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Put(testLocation.Id, testLocation);
            var okResult = result as OkObjectResult;

            // assert
            Assert.NotNull(okResult);
        }

        [Fact]
        public async void Put_WhenNotValid()
        {
            // arrange
            var testLocation = TestLocation();
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Update(It.IsAny<Location>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.BadRequest, testLocation.Id));
            var validationServiceMoq = new Mock<IValidationService>();

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
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
        public async void Delete_WhenValid()
        {
            // arrange
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(() => ResultCode.Success);

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
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
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(() => ResultCode.BadRequest);

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
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
            var resultCodeProcessor = new ResultCodeProcessor();

            var locationServiceMoq = new Mock<ILocationService>();
            locationServiceMoq.Setup(x => x.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(() => ResultCode.NotFound);

            var sut = new LocationsController(locationServiceMoq.Object, resultCodeProcessor)
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
            return new Helpers.ControllerContextHelper().GetContext();
        }

        private Location TestLocation()
        {
            return new Helpers.LocationHelper().Get();
        }

        private IEnumerable<Location> TestLocations()
        {
            return new Helpers.LocationHelper().GetMany();
        }
    }
}
