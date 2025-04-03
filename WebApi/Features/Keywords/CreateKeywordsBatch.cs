using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using Quartz;
using WebApi.Data.Entities;
using WebApi.Errors;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Features.Keywords.Jobs;
using WebApi.ResultType;

namespace WebApi.Features.Keywords;

public class CreateKeywordsBatch
{
    public class CreateKeywordsBatchRequest : IRequest<Result<CreateKeywordsBatchResponse>>
    {
        public List<CreateKeywordDto> Keywords { get; set; } = new();
    }

    public class Validator : AbstractValidator<CreateKeywordsBatchRequest>
    {
        public Validator()
        {
            RuleFor(k => k.Keywords)
                .Must(k => k.Select(n => n.Name).Distinct().Count() == k.Count)
                .WithMessage("Keywords must be unique");
        }
    }

    public class CreateKeywordsBatchResponse
    {
        public String Status { get; set; } = null!;
        public String JobId { get; set; } = null!;
    }
    
    internal sealed class Handler : IRequestHandler<CreateKeywordsBatchRequest, Result<CreateKeywordsBatchResponse>>
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IValidator<CreateKeywordsBatchRequest> _validator;

        public Handler(ISchedulerFactory schedulerFactory, IValidator<CreateKeywordsBatchRequest> validator)
        {
            _schedulerFactory = schedulerFactory;
            _validator = validator;
        }

        public async Task<Result<CreateKeywordsBatchResponse>> Handle(CreateKeywordsBatchRequest request, CancellationToken cancellationToken)
        { 
           var validationResult = await _validator.ValidateAsync(request, cancellationToken);
           if (!validationResult.IsValid)
               return Result.Failure<CreateKeywordsBatchResponse>(validationResult.Errors.ExtractToList());

           var scheduler = await _schedulerFactory.GetScheduler(cancellationToken); 
           
           var jobData = new JobDataMap
           {
               { "keywords", JsonConvert.SerializeObject(request.Keywords) }
           };
           
           var trigger = TriggerBuilder.Create()
               .ForJob(BatchInsertNewKeywords.Name )
               .WithIdentity(Guid.NewGuid() + "-keywordBatch")
               .UsingJobData(jobData)
               .StartNow()
               .Build();

           try
           {
               await scheduler.ScheduleJob(trigger, cancellationToken);
               return Result.Success(new CreateKeywordsBatchResponse
               {
                   JobId = trigger.JobKey.Name,
                   Status = "processing"
               });
           }
           catch (Exception ex)
           {
                return Result.Failure<CreateKeywordsBatchResponse>(KeywordErrors.BatchKeywordFailed);   
           }
        }
    }
}