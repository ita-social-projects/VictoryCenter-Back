using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Create;

public class CreateEventNewsCategoryHandler
    : IRequestHandler<CreateEventNewsCategoryCommand, Result<AdminEventNewsCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateEventNewsCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<AdminEventNewsCategoryDto>> Handle(
        CreateEventNewsCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedName = request.Category.Name.Trim();

        if (await _repositoryWrapper.EventNewsCategoryRepository.ExistsAsync(
                category => category.Name == normalizedName))
        {
            return Result.Fail<AdminEventNewsCategoryDto>(EventNewsCategoryConstants.DuplicateCategoryName);
        }

        var category = new EventNewsCategory
        {
            Name = normalizedName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repositoryWrapper.EventNewsCategoryRepository.CreateAsync(category);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<AdminEventNewsCategoryDto>(category));
            }
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintException())
        {
            return Result.Fail<AdminEventNewsCategoryDto>(EventNewsCategoryConstants.DuplicateCategoryName);
        }

        return Result.Fail<AdminEventNewsCategoryDto>(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(EventNewsCategory)));
    }
}
