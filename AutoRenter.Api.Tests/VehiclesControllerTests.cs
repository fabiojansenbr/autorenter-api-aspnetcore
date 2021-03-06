﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AutoRenter.Api.Controllers;
using AutoRenter.Api.Models;
using AutoRenter.Api.Services;
using AutoRenter.Api.Tests.Helpers;
using AutoRenter.Domain.Interfaces;
using AutoRenter.Domain.Models;

namespace AutoRenter.Api.Tests
{
    public class VehiclesControllerTests
    {
        [Fact]
        public async void GetAll_WhenFound()
        {
            // arrange
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.GetAll())
                .ReturnsAsync(() => new Result<IEnumerable<Vehicle>>(ResultCode.Success, TestVehicles()));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();
            dataStructureConverterMoq.Setup(x => x.Convert(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(new Dictionary<string, object>
                        {
                            { "vehicles", TestVehicles() }
                        });

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
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
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.GetAll())
                .ReturnsAsync(() => new Result<IEnumerable<Vehicle>>(ResultCode.NotFound));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
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
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.GetAll())
                .ReturnsAsync(() => new Result<IEnumerable<Vehicle>>(ResultCode.Success, TestVehicles()));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();
            dataStructureConverterMoq.Setup(x => x.ConvertAndMap<IEnumerable<VehicleModel>, IEnumerable<Vehicle>>(It.IsAny<string>(), It.IsAny<IEnumerable<Vehicle>>()))
                .Returns(new Dictionary<string, object>
                        {
                            { "vehicles", TestVehicles() }
                        });

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
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
            var errorCodeConverter = new ErrorCodeConverter();
            var targetId = IdentifierHelper.VehicleId;
            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Vehicle>(ResultCode.Success, TestVehicle()));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();
            dataStructureConverterMoq.Setup(x => x.Convert(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(new Dictionary<string, object>
                        {
                            { "vehicle", TestVehicle() }
                        });

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
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
            var targetId = Guid.NewGuid();
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Vehicle>(ResultCode.NotFound));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
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
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Vehicle>(ResultCode.Success, TestVehicle()));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
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
            var targetId = IdentifierHelper.VehicleId; ;
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Result<Vehicle>(ResultCode.Success, TestVehicle()));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();
            dataStructureConverterMoq.Setup(x => x.ConvertAndMap<VehicleModel, Vehicle>(It.IsAny<string>(), It.IsAny<Vehicle>()))
                .Returns(new Dictionary<string, object>
                        {
                            { "vehicle", VehicleHelper.Get() }
                        });

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
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
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Insert(It.IsAny<Vehicle>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.Success, TestVehicle().Id));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Post(TestVehicleModel());
            var createdAtRouteResult = result as CreatedAtRouteResult;

            // assert
            Assert.NotNull(createdAtRouteResult);
        }

        [Fact]
        public async void Post_WhenNotValid()
        {
            // arrange
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Insert(It.IsAny<Vehicle>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.BadRequest, TestVehicle().Id));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Post(TestVehicleModel());
            var badRequestResult = result as BadRequestResult;

            // assert
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async void Post_WhenConflict()
        {
            // arrange
            var testVehicle = TestVehicleModel();
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Insert(It.IsAny<Vehicle>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.Conflict, testVehicle.Id));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Post(testVehicle);
            var conflictResult = result as StatusCodeResult;

            // assert
            Assert.Equal(409, conflictResult.StatusCode);
        }

        [Fact]
        public async void Put_WhenValid()
        {
            // arrange
            var testVehicle = TestVehicleModel();
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Update(It.IsAny<Vehicle>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.Success, testVehicle.Id));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Put(testVehicle.Id, testVehicle);
            var okResult = result as OkObjectResult;

            // assert
            Assert.NotNull(okResult);
        }

        [Fact]
        public async void Put_WhenNotValid()
        {
            // arrange
            var testVehicle = TestVehicleModel();
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Update(It.IsAny<Vehicle>()))
                .ReturnsAsync(() => new Result<Guid>(ResultCode.BadRequest, testVehicle.Id));

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Put(testVehicle.Id, testVehicle);
            var badRequestResult = result as BadRequestResult;

            // assert
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async void Delete_WhenValid()
        {
            // arrange
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(() => ResultCode.Success);

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Delete(TestVehicle().Id);
            var noContentResult = result as NoContentResult;

            // assert
            Assert.NotNull(noContentResult);
        }

        [Fact]
        public async void Delete_WhenNotValid()
        {
            // arrange
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(() => ResultCode.BadRequest);

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Delete(TestVehicle().Id);
            var badRequestResult = result as BadRequestResult;

            // assert
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async void Delete_WhenNotFound()
        {
            // arrange
            var errorCodeConverter = new ErrorCodeConverter();

            var vehicleServiceMoq = new Mock<IVehicleService>();
            vehicleServiceMoq.Setup(x => x.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(() => ResultCode.NotFound);

            var dataStructureConverterMoq = new Mock<IDataStructureConverter>();

            var sut = new VehiclesController(vehicleServiceMoq.Object, errorCodeConverter, dataStructureConverterMoq.Object)
            {
                ControllerContext = DefaultControllerContext()
            };

            // act
            var result = await sut.Delete(TestVehicle().Id);
            var notFoundResult = result as NotFoundResult;

            // assert
            Assert.NotNull(notFoundResult);
        }

        private Vehicle TestVehicle()
        {
            return VehicleHelper.Get();
        }

        private IEnumerable<Vehicle> TestVehicles()
        {
            return VehicleHelper.GetMany();
        }

        private VehicleModel TestVehicleModel()
        {
            return VehicleModelHelper.Get();
        }

        private IEnumerable<VehicleModel> TestVehicleModels()
        {
            return VehicleModelHelper.GetMany();
        }

        private ControllerContext DefaultControllerContext()
        {
            return ControllerContextHelper.GetContext();
        }
    }
}
