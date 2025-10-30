using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;

public class CreateHippotherapyProgramHandler : BaseHandler<CreateHippotherapyProgramCommand, HippotherapyProgramDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateHippotherapyProgramCommand> _validator;

    public CreateHippotherapyProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateHippotherapyProgramCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public override async Task<HippotherapyProgramDto> HandleRequest(CreateHippotherapyProgramCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        IEnumerable<HippotherapyProgramCategory> categories = await _repositoryWrapper
                .HippotherapyProgramCategoriesRepository.GetAllAsync(new QueryOptions<HippotherapyProgramCategory>
                {
                    Filter = category => request.CreateProgramDto.CategoryIds.Contains(category.Id!),
                    AsNoTracking = false
                });

        HippotherapyProgram entity = _mapper.Map<HippotherapyProgram>(request.CreateProgramDto);

        if (request.CreateProgramDto.ImageId is not null)
        {
            Image? newImage = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = image => image.Id == request.CreateProgramDto.ImageId,
                AsNoTracking = false
            });

            if(newImage is null)
            {
                throw new Exception(ErrorMessagesConstants.NotFound(request.CreateProgramDto.ImageId.Value, typeof(Image)));
            }

            entity.Image = newImage;
        }

        var notFound = request.CreateProgramDto.CategoryIds.Except(categories.Select(c => c.Id)).ToList();
        if (notFound.Count > 0)
        {
            throw new Exception(ErrorMessagesConstants.ReorderingContainsInvalidIds(typeof(HippotherapyProgramCategory), notFound));
        }

        entity.Categories = [.. categories];
        entity.CreatedAt = DateTimeOffset.UtcNow;

        await _repositoryWrapper.HippotherapyProgramsRepository.CreateAsync(entity, cancellationToken);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgram)));
            }

        HippotherapyProgramDto responceDto = _mapper.Map<HippotherapyProgramDto>(entity);
        return responceDto;
    }
}
