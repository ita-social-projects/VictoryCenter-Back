using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Update;

public class UpdateReportFundsExpendituresCategoryHandler
    : IRequestHandler<UpdateReportFundsExpendituresCategoryCommand, Result<ReportFundsExpendituresCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateReportFundsExpendituresCategoryCommand> _validator;

    public UpdateReportFundsExpendituresCategoryHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateReportFundsExpendituresCategoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ReportFundsExpendituresCategoryDto>> Handle(
        UpdateReportFundsExpendituresCategoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var normalizedRequestedName = NormalizeName(request.UpdateReportFundsExpendituresCategoryDto.Name);

            var categories = (await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository
                .GetAllAsync(new QueryOptions<ReportFundsExpendituresCategory>
                {
                    AsNoTracking = false,
                    Filter = category =>
                        category.Id == request.Id ||
                        category.Name.Trim().ToUpper() == normalizedRequestedName
                }))
                .ToList();

            var categoryToUpdate = categories.FirstOrDefault(category => category.Id == request.Id);

            if (categoryToUpdate is null)
            {
                return Result.Fail<ReportFundsExpendituresCategoryDto>(
                    ErrorMessagesConstants.NotFound(request.Id, typeof(ReportFundsExpendituresCategory)));
            }

            if (ReportFundsExpendituresCategoryValidationHelper.IsReservedCategoryName(categoryToUpdate.Name, categoryToUpdate.Type))
            {
                return Result.Fail<ReportFundsExpendituresCategoryDto>(
                    ReportFundsExpendituresCategoryConstants.CantUpdateReservedCategory);
            }

            if (ReportFundsExpendituresCategoryValidationHelper.IsReservedCategoryName(
                    request.UpdateReportFundsExpendituresCategoryDto.Name,
                    categoryToUpdate.Type))
            {
                return Result.Fail<ReportFundsExpendituresCategoryDto>(
                    ReportFundsExpendituresCategoryConstants.ReservedCategoryName);
            }

            if (NormalizeName(categoryToUpdate.Name) != normalizedRequestedName &&
                categories.Any(category =>
                    category.Id != request.Id &&
                    category.Type == categoryToUpdate.Type))
            {
                return Result.Fail<ReportFundsExpendituresCategoryDto>(
                    ReportFundsExpendituresCategoryConstants.DuplicateCategoryName);
            }

            _mapper.Map(request.UpdateReportFundsExpendituresCategoryDto, categoryToUpdate);

            _repositoryWrapper.ReportFundsExpendituresCategoriesRepository.Update(categoryToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var responseDto = _mapper.Map<ReportFundsExpendituresCategoryDto>(categoryToUpdate);
                return Result.Ok(responseDto);
            }

            return Result.Fail<ReportFundsExpendituresCategoryDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportFundsExpendituresCategory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ReportFundsExpendituresCategoryDto>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresCategoryDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportFundsExpendituresCategory)));
        }
    }

    private static string NormalizeName(string name)
    {
        return name.Trim().ToUpper();
    }
}
