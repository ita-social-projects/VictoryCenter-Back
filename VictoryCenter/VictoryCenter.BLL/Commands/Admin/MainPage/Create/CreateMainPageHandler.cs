using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Commands.Admin.MainPage.Create;
public class CreateMainPageHandler : IRequestHandler<CreateMainPageCommand, Result<MainPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateMainPageCommand> _validator;

    public CreateMainPageHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateMainPageCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<MainPageDto>> Handle(CreateMainPageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using var transaction = _repositoryWrapper.BeginTransaction();

            var entityIsExists = (await _repositoryWrapper.MainPageRepository
                .CountAsync()) > 0;

            if (entityIsExists)
            {
                return Result.Fail<MainPageDto>(
                    ErrorMessagesConstants.OnlyOneEntityOfTypeIsAllowed(nameof(DAL.Entities.MainPage)));
            }

            var requestedImageIds = new List<long>();

            if (request.CreateMainPageDto.ImageId.HasValue)
            {
                requestedImageIds.Add(request.CreateMainPageDto.ImageId.Value);
            }

            requestedImageIds.AddRange(request.CreateMainPageDto.ImpactStatistics
                .Where(s => s.ImageId.HasValue)
                .Select(s => s.ImageId!.Value));

            if (requestedImageIds.Count > 0)
            {
                var existingImageIds = (await _repositoryWrapper.ImageRepository
                    .GetAllAsync(new QueryOptions<Image>
                    {
                        Filter = i => requestedImageIds.Contains(i.Id)
                    }))
                    .Select(i => i.Id)
                    .ToList();

                var nonExistingImageIds = requestedImageIds.Except(existingImageIds).ToList();

                if (nonExistingImageIds.Count > 0)
                {
                    return Result.Fail<MainPageDto>(
                        ErrorMessagesConstants.NotFound(nonExistingImageIds, typeof(Image)));
                }
            }

            var mainPageEntity = _mapper.Map<CreateMainPageDto, MainPageEntity>(request.CreateMainPageDto);

            await _repositoryWrapper.MainPageRepository
                .CreateAsync(mainPageEntity);

            await _repositoryWrapper.SaveChangesAsync();

            var resultEntity = await _repositoryWrapper.MainPageRepository
                .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
                {
                    Filter = e => e.Id == mainPageEntity.Id,
                    Include = q => q
                        .Include(e => e.Image)
                        .Include(e => e.MainAboutUs)
                        .Include(e => e.MainPartners)
                        .Include(e => e.ImpactStatistics)
                        .ThenInclude(s => s.Image)
                        .Include(e => e.ImpactStatistics)
                        .ThenInclude(s => s.Metrics)
                });

            if (resultEntity is null)
            {
                return Result.Fail<MainPageDto>(
                    ErrorMessagesConstants.FailedToCreateEntity(typeof(MainPageEntity)));
            }

            var resultDto = _mapper.Map<MainPageEntity, MainPageDto>(resultEntity);

            transaction.Complete();

            return Result.Ok(resultDto);
        }
         catch (ValidationException vex)
        {
            return Result.Fail<MainPageDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<MainPageDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(MainPageEntity)));
        }
    }
}
