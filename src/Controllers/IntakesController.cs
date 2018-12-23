using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IntakeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntakesController : ControllerBase
    {
        private readonly IDocumentClient _documentClient;
        public readonly CosmosDbConfig _cosmosDbConfig;

        public IntakesController(
            IDocumentClient documentClient,
            IOptions<CosmosDbConfig> cosmosDbConfig
            )
        {
            this._documentClient = documentClient;
            this._cosmosDbConfig = cosmosDbConfig.Value;
        }

        [HttpGet("{id}", Name = "GetIntake")]
        public async Task<IActionResult> Get(string id)
        {
            var query = _documentClient.CreateDocumentQuery<IntakeModel>(
                UriFactory.CreateDocumentCollectionUri(
                    _cosmosDbConfig.DatabaseId,
                    _cosmosDbConfig.Collections.IntakesCollection
                ),
                new FeedOptions { MaxItemCount = 1 }
            )
            .Where(i => i.Id == id)
            .AsDocumentQuery();

            IntakeModel intake = null;
            if (query.HasMoreResults)
            {
                intake = (await query.ExecuteNextAsync<IntakeModel>())
                    .FirstOrDefault();
            }

            if (intake == null)
            {
                return NotFound();
            }

            return Ok(intake);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post(IntakeModel intake)
        {
            intake.Id = Guid.NewGuid().ToString();
            await _documentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(
                    _cosmosDbConfig.DatabaseId,
                    _cosmosDbConfig.Collections.IntakesCollection),
                intake
            );

            return CreatedAtRoute("GetIntake", new { id = intake.Id }, intake);
        }
    }

    public class IntakeModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal InitialDeposit { get; set; }
        public int HorizonMonth { get; set; }
        public int HorizonYear { get; set; }
        public decimal GoalAmount { get; set; }
        public string InvestmentProfile { get; set; }
    }
}