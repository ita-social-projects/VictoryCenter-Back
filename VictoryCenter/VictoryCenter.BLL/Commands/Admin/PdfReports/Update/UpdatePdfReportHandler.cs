using System.Text.RegularExpressions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Update;

public class UpdatePdfReportHandler : IRequestHandler<UpdatePdfReportCommand, Result<PdfReportDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdatePdfReportCommand> _validator;
    private readonly IMapper _mapper;
    private static readonly Regex MultipleSpaces = new(@" {2,}", RegexOptions.Compiled);

    public UpdatePdfReportHandler(
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdatePdfReportCommand> validator,
        IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PdfReportDto>> Handle(
        UpdatePdfReportCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var pdfReport = await _repositoryWrapper.PdfReportRepository.GetFirstOrDefaultAsync(
                new QueryOptions<PdfReport>
                {
                    Filter = pr => pr.Id == request.Id,
                    AsNoTracking = false
                });

            if (pdfReport == null)
            {
                return Result.Fail<PdfReportDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(PdfReport)));
            }

            var normalizedName = NormalizeText(request.Name);
            var hasChanges = pdfReport.Name != normalizedName;

            if (hasChanges)
            {
                pdfReport.Name = normalizedName;

                if (await _repositoryWrapper.SaveChangesAsync() <= 0)
                {
                    return Result.Fail<PdfReportDto>(
                        ErrorMessagesConstants.FailedToUpdateEntity(typeof(PdfReport)));
                }
            }

            var dto = _mapper.Map<PdfReportDto>(pdfReport);
            return Result.Ok(dto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PdfReportDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PdfReportDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PdfReport)));
        }
    }

    private static string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return MultipleSpaces.Replace(text.Trim(), " ");
    }
}
