using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using timeKevin.Common.Models;
using timeKevin.Function.Entities;

namespace timeKevin.Function
{
    public static class SheduledFunction
    {
        [FunctionName("SheduledFunction")]
        static async public Task Run(
            [TimerTrigger("0 */2 * * * *")]TimerInfo myTimer,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            ILogger log)
        {
            log.LogInformation($"consolidated the day function executed at: {DateTime.Now}");

            string filter = TableQuery.GenerateFilterConditionForBool("Consolidate",
                QueryComparisons.Equal, false);

            TableQuery<TimeEntity> query = new TableQuery<TimeEntity>().Where(filter);
            TableQuerySegment<TimeEntity> completedTimes = await timeTable.ExecuteQuerySegmentedAsync
                (query, null);

            int numConsolidated = 0;

            foreach (TimeEntity completedTime in completedTimes)
            {
                log.LogInformation("Recive a new time.");
                double minutes = 0;
                foreach (TimeEntity completedTimeId in completedTimes)
                {
                    if (completedTime.IdEmployee == completedTimeId.IdEmployee)
                    {
                        if (completedTime.Date > completedTimeId.Date &&
                            completedTime.Type.Equals("0") &&
                            completedTimeId.Type.Equals("1"))
                        {
                            minutes = (completedTime.Date - completedTimeId.Date).TotalMinutes;
                            break;
                        }
                        else if (completedTime.Date < completedTimeId.Date &&
                            completedTime.Type.Equals("0") &&
                            completedTimeId.Type.Equals("1"))
                        {
                            minutes = (completedTimeId.Date - completedTime.Date).TotalMinutes;
                            break;
                        }
                    }
                }
                ConsolidatedEntity consolidateEntity = new ConsolidatedEntity
                {
                    Date = DateTime.UtcNow,
                    IdEmployee = completedTime.IdEmployee,
                    MinutesWork = (int)minutes,
                    ETag = "*",
                    PartitionKey = "CONSOLIDATED",
                    RowKey = Guid.NewGuid().ToString()
                };

                TableOperation addOperation = TableOperation.Insert(consolidateEntity);
                await timeTable.ExecuteAsync(addOperation);

                numConsolidated++;
                log.LogInformation($"Consolidated: {numConsolidated} items at: {DateTime.Now}");
            }

        }
    }
}
