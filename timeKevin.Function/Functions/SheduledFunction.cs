using System;
using System.Collections.Generic;
using System.Linq;
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
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Table("consolidate", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            ILogger log)
        {
            log.LogInformation($"consolidated the day function executed at: {DateTime.Now}");

            string filter = TableQuery.GenerateFilterConditionForBool("Consolidate",
                QueryComparisons.Equal, false);

            TableQuery<TimeEntity> query = new TableQuery<TimeEntity>().Where(filter);
            TableQuerySegment<TimeEntity> completedTimes = await timeTable.ExecuteQuerySegmentedAsync
                (query, null);

            List<TimeEntity> queryOrderer = completedTimes.OrderBy(x => x.IdEmployee).ThenBy(x => x.Date).ToList();
            int numConsolidated = 0;
            bool isUpdateConsolidated = false;

            foreach (TimeEntity completedTime in queryOrderer)
            {
                log.LogInformation("Recive a new time.");
                double minutes = 0;
                foreach (TimeEntity completedTimeId in queryOrderer)
                {
                    string filterEmployee = TableQuery.GenerateFilterConditionForInt("IdEmployee",
                        QueryComparisons.Equal, completedTime.IdEmployee);

                    string filterDate = TableQuery.GenerateFilterConditionForDate("Date",
                        QueryComparisons.Equal, DateTime.Today);

                    TableQuery<ConsolidatedEntity> queryConsolidated = new TableQuery<ConsolidatedEntity>()
                        .Where(filterEmployee)
                        .Where(filterDate);

                    TableQuerySegment<ConsolidatedEntity> consolidatedResult = await consolidatedTable.ExecuteQuerySegmentedAsync(queryConsolidated, null);

                    if (completedTime.IdEmployee == completedTimeId.IdEmployee)
                    {
                        
                        if (completedTime.Date < completedTimeId.Date)
                        {
                            minutes = (completedTimeId.Date - completedTime.Date).TotalMinutes;
                            foreach (ConsolidatedEntity consolidateRequest in consolidatedResult)
                            {
                                if (completedTime.IdEmployee == consolidateRequest.IdEmployee)
                                {
                                    minutes += consolidateRequest.MinutesWork;
                                    consolidateRequest.MinutesWork = (int)minutes;
                                    TableOperation updateConsolidate = TableOperation.Replace(consolidateRequest);
                                    await consolidatedTable.ExecuteAsync(updateConsolidate);
                                    isUpdateConsolidated = true;
                                    log.LogInformation($"The employee's consolidated: {consolidateRequest.IdEmployee} is update at: {DateTime.Now}");
                                }
                            }
                            completedTime.Consolidate = true;
                            completedTimeId.Consolidate = true;
                            TableOperation updateTimeEntityEntrance = TableOperation.Replace(completedTime);
                            TableOperation updateTimeEntityExit = TableOperation.Replace(completedTimeId);
                            await timeTable.ExecuteAsync(updateTimeEntityEntrance);
                            await timeTable.ExecuteAsync(updateTimeEntityExit);
                            break;
                        }
                    }
                }
                if (minutes > 0 && !isUpdateConsolidated)
                {
                    ConsolidatedEntity consolidateEntity = new ConsolidatedEntity
                    {
                        Date = DateTime.Today,
                        IdEmployee = completedTime.IdEmployee,
                        MinutesWork = (int)minutes,
                        ETag = "*",
                        PartitionKey = "CONSOLIDATED",
                        RowKey = Guid.NewGuid().ToString()
                    };

                    TableOperation addOperationConsolidated = TableOperation.Insert(consolidateEntity);
                    await consolidatedTable.ExecuteAsync(addOperationConsolidated);

                    numConsolidated++;
                    log.LogInformation($"Consolidated: {numConsolidated} items at: {DateTime.Now}");
                }
            }
        }
    }
}
