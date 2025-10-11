using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Create;

public class CreatePartnersSectionHandler : IRequestHandler<CreatePartnersSectionCommand, Result<PartnersSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreatePartnersSectionCommand> _validator;
    private readonly IMapper _mapper;
    private readonly IReorderService _reorderService;

    public CreatePartnersSectionHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreatePartnersSectionCommand> validator,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _reorderService = reorderService;
    }

    public async Task<Result<PartnersSectionDto>> Handle(CreatePartnersSectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var imageIds = request.CreatePartnersSectionDto.Partners.Select(p => p.ImageId).ToList();

            var requestedImageIds = request.CreatePartnersSectionDto.Partners
                .Select(p => p.ImageId)
                .Distinct()
                .ToList();

            var existingImageIds = (await _repositoryWrapper.ImageRepository
                .GetAllAsync(new QueryOptions<Image>
                {
                    Filter = i => requestedImageIds.Contains(i.Id)
                }))
                .Select(i => i.Id)
                .ToList();

            var nonExistingImageIds = requestedImageIds.Except(existingImageIds).ToList();

            if (nonExistingImageIds.Any())
            {
                return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.NotFound(nonExistingImageIds, typeof(Image)));
            }

            var partnersSectionEntity = _mapper.Map<PartnerSection>(request.CreatePartnersSectionDto);
            partnersSectionEntity.CreatedAt = DateTimeOffset.UtcNow;
            partnersSectionEntity.Priority = await _reorderService.GetNextDisplayOrderAsync<PartnerSection>();

            long currentDisplayOrder = 1;
            foreach (var partnerEntity in partnersSectionEntity.Partners)
            {
                partnerEntity.CreatedAt = DateTimeOffset.UtcNow;
                partnerEntity.Priority = currentDisplayOrder++;
            }

            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                await _repositoryWrapper.PartnerSectionsRepository.CreateAsync(partnersSectionEntity);
                await _repositoryWrapper.SaveChangesAsync();
                scope.Complete();
            }

            var createdSection = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(new()
            {
                Filter = s => s.Id == partnersSectionEntity.Id,
                Include = q => q.Include(s => s.Partners).ThenInclude(p => p.Image!)
            });

            var resultDto = _mapper.Map<PartnersSectionDto>(createdSection);
            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PartnersSectionDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail<PartnersSectionDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnerSection)));
        }
    }
}
