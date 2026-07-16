using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Create;

public class CreateReportFundsExpendituresCategoryHandler
    : IRequestHandler<CreateReportFundsExpendituresCategoryCommand, Result<ReportFundsExpendituresCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateReportFundsExpendituresCategoryCommand> _validator;

    public CreateReportFundsExpendituresCategoryHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<CreateReportFundsExpendituresCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ReportFundsExpendituresCategoryDto>> Handle(
        CreateReportFundsExpendituresCategoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var duplicateCategoriesCount = await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository.CountAsync(
                new QueryOptions<ReportFundsExpendituresCategory>
                {
                    Filter = category =>
                        category.Name == request.CreateReportFundsExpendituresCategoryDto.Name &&
                        category.Type == request.CreateReportFundsExpendituresCategoryDto.Type
                });

            if (duplicateCategoriesCount > 0)
            {
                return Result.Fail<ReportFundsExpendituresCategoryDto>(
                    ReportFundsExpendituresCategoryConstants.DuplicateCategoryName);
            }

            var entity = _mapper.Map<ReportFundsExpendituresCategory>(request.CreateReportFundsExpendituresCategoryDto);
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var responseDto = _mapper.Map<ReportFundsExpendituresCategoryDto>(entity);
                return Result.Ok(responseDto);
            }

            return Result.Fail<ReportFundsExpendituresCategoryDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresCategory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ReportFundsExpendituresCategoryDto>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresCategoryDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresCategory)));
        }
    }
}
