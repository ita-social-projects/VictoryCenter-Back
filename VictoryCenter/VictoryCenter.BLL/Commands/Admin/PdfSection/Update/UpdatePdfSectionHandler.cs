using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.PdfSection.Update;

public class UpdatePdfSectionHandler : IRequestHandler<UpdatePdfSectionCommand, Result<PdfSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdatePdfSectionCommand> _validator;

    public UpdatePdfSectionHandler(
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdatePdfSectionCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<PdfSectionDto>> Handle(
        UpdatePdfSectionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var count = await _repositoryWrapper.PdfSectionRepository.CountAsync();
            if (count == 0)
            {
                return Result.Fail<PdfSectionDto>(PdfSectionConstants.SectionNotFound);
            }

            if (count > 1)
            {
                return Result.Fail<PdfSectionDto>("Multiple PdfSection records found. Expected exactly one.");
            }

            var pdfSection = await _repositoryWrapper.PdfSectionRepository.GetFirstOrDefaultAsync(
                new QueryOptions<DAL.Entities.PdfSection>
                {
                    AsNoTracking = false
                });

            if (pdfSection == null)
            {
                return Result.Fail<PdfSectionDto>(PdfSectionConstants.SectionNotFound);
            }

            var normalizedTitle = NormalizeText(request.Dto.Title);
            var normalizedDescription = NormalizeText(request.Dto.Description);
            var hasChanges = pdfSection.Title != normalizedTitle || pdfSection.Description != normalizedDescription;

            if (hasChanges)
            {
                pdfSection.Title = normalizedTitle;
                pdfSection.Description = normalizedDescription;

                if (await _repositoryWrapper.SaveChangesAsync() <= 0)
                {
                    return Result.Fail<PdfSectionDto>(
                        ErrorMessagesConstants.FailedToUpdateEntity(typeof(DAL.Entities.PdfSection)));
                }
            }

            var dto = new PdfSectionDto
            {
                Title = pdfSection.Title,
                Description = pdfSection.Description,
            };

            return Result.Ok(dto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PdfSectionDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PdfSectionDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(DAL.Entities.PdfSection)));
        }
    }

    private static string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var trimmed = text.Trim();
        while (trimmed.Contains("  "))
        {
            trimmed = trimmed.Replace("  ", " ");
        }

        return trimmed;
    }
}
