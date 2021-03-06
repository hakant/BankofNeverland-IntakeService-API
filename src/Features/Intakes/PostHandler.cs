using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using IntakeApi.Configuration;
using IntakeApi.Entities;
using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;

namespace IntakeApi.Features.Intakes
{
    public class Post
    {
        public class Request : IRequest<Get.Response>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime BirthDate { get; set; }
            public decimal InitialDeposit { get; set; }
            public int HorizonMonth { get; set; }
            public int HorizonYear { get; set; }
            public decimal GoalAmount { get; set; }
            public InvestmentProfile? InvestmentProfile { get; set; }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(request => request.FirstName).NotNull().NotEmpty();
                RuleFor(request => request.LastName).NotNull().NotEmpty();
                RuleFor(request => request.BirthDate).NotNull().NotEmpty();
                RuleFor(request => request.InitialDeposit).NotNull().GreaterThan(0);
                RuleFor(request => request.HorizonMonth).NotNull().InclusiveBetween(1,12);
                RuleFor(request => request.GoalAmount).NotNull();
                RuleFor(request => request.InvestmentProfile).NotNull();

                var startYear = DateTime.Now.Year;
                var endYear = startYear + 60;
                RuleFor(request => request.HorizonYear).NotNull().InclusiveBetween(startYear, endYear);
            }
        }

        public class Handler : IRequestHandler<Request, Get.Response>
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

            public async Task<Get.Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var intakeEntity = _mapper.Map<IntakeEntity>(request);
                intakeEntity.Id = Guid.NewGuid().ToString();

                await _documentClient.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(
                        _cosmosDbConfig.DatabaseId,
                        _cosmosDbConfig.Collections.IntakesCollection),
                    intakeEntity, 
                    cancellationToken: cancellationToken);

                return _mapper.Map<Get.Response>(intakeEntity);
            }
        }
    }
}