using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using IntakeApi.Configuration;
using IntakeApi.Entities;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;

namespace IntakeApi.Features.Intakes
{
    public class Get
    {
        public class Request : IRequest<Response>
        {
            public string Id { get; set; }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(request => request.Id).NotNull().NotEmpty();
            }
        }

        public class Response
        {
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

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IDocumentClient _documentClient;
            private readonly IMapper _mapper;
            private readonly CosmosDbConfig _cosmosDbConfig;

            public Handler(
                IDocumentClient documentClient,
                IOptions<CosmosDbConfig> cosmosDbConfig,
                IMapper mapper
                )
            {
                this._documentClient = documentClient;
                this._cosmosDbConfig = cosmosDbConfig.Value;
                this._mapper = mapper;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var query = _documentClient.CreateDocumentQuery<IntakeEntity>(
                    UriFactory.CreateDocumentCollectionUri(
                        _cosmosDbConfig.DatabaseId,
                        _cosmosDbConfig.Collections.IntakesCollection
                    ),
                    new FeedOptions { MaxItemCount = 1 }
                )
                .Where(i => i.Id == request.Id)
                .AsDocumentQuery();

                IntakeEntity intake = null;
                if (query.HasMoreResults)
                {
                    intake = (await query.ExecuteNextAsync<IntakeEntity>())
                        .FirstOrDefault();
                }

                return _mapper.Map<Get.Response>(intake);
            }
        }
    }
}