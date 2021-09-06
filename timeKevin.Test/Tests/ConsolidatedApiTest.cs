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
    public class ConsolidatedApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public void GetConsolidatedByDate_Should_Return_200()
        {
            //Arrenge
            Consolidated consolidatedRequest = TestFactory.GetConsolidatedRequest();
            DateTime date = DateTime.Today;
            ConsolidatedEntity consolidatedEntity = TestFactory.GetConsolidatedEntity();

            DefaultHttpRequest request = TestFactory.CreateHttpRequest(date, consolidatedRequest);

            //Act
            IActionResult response = ConsolidateApi.GetConsolidatedByDate(request, consolidatedEntity, date, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
