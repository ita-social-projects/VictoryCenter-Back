using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Programs.Create;

public class CreateProgramHandler : BaseHandler<CreateProgramCommand, ProgramDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateProgramCommand> _validator;

    public CreateProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateProgramCommand> validator, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public override async Task<ProgramDto> HandleRequest(CreateProgramCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var categories = (await _repositoryWrapper
            .ProgramCategoriesRepository.GetAllAsync(new QueryOptions<ProgramCategory>
            {
                Filter = category => request.CreateProgramDto.CategoryIds.Contains(category.Id),
                AsNoTracking = false
            })).ToList();

        Program entity = _mapper.Map<Program>(request.CreateProgramDto);

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
            throw new Exception(ErrorMessagesConstants.ReorderingContainsInvalidIds(typeof(ProgramCategory), notFound));
        }

        entity.Categories = [.. categories];
        entity.CreatedAt = DateTimeOffset.UtcNow;

        await _repositoryWrapper.ProgramsRepository.CreateAsync(entity);

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntity(typeof(Program)));
        }

        ProgramDto result = _mapper.Map<ProgramDto>(entity);

        return result;
    }
}
