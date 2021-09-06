using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using timeKevin.Common.Models;
using timeKevin.Function.Entities;
using timeKevin.Function.Functions;
using timeKevin.Test.Helpers;
using Xunit;

namespace timeKevin.Test.Tests
{
    public class TimeApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        async public void CreateTime_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRequest);

            //Act
            IActionResult response = await TimeApi.CreateTime(request, mockTimes, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        async public void UpdateTime_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            Guid todoId = Guid.NewGuid();

            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, timeRequest);

            //Act
            IActionResult response = await TimeApi.UpdateTime(request, mockTimes, todoId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public void GetTimeByIdTime_Should_Return_200()
        {
            //Arrenge
            Time timeRequest = TestFactory.GetTimeRequest();
            Guid todoId = Guid.NewGuid();
            TimeEntity timeEntity = TestFactory.GetTimeEntity();

            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, timeRequest);

            //Act
            IActionResult response = TimeApi.GetTimeById(request, timeEntity, todoId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        async public void GetAllTimesTime_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            //Time timeRequest = TestFactory.GetTimeRequest();

            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            //Act
            IActionResult response = await TimeApi.GetAllTimes(request, mockTimes, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        async public void DeleteTime_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            Guid todoId = Guid.NewGuid();
            TimeEntity timeEntity = TestFactory.GetTimeEntity();

            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, timeRequest);

            //Act
            IActionResult response = await TimeApi.DeleteTime(request, timeEntity, mockTimes, todoId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
