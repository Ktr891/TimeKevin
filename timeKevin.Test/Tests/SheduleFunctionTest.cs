using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using timeKevin.Function;
using timeKevin.Test.Helpers;
using Xunit;

namespace timeKevin.Test.Tests
{
    public class SheduleFunctionTest
    {
        [Fact]
        public void SheduleFunction_ShowId_Log_Message()
        {
            //Arrange
            MockCloudTableTimes mockTime = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableConsolidated mockConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            //Act
            SheduledFunction.Run(null, mockConsolidated, mockTime, logger);
            string message = logger.Logs[0];

            //Asery
            Assert.Contains("Created completed", message);
        }
    }
}
