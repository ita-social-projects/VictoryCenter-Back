using System.Text.RegularExpressions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Hubs;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Update;

public class UpdatePdfReportHandler : IRequestHandler<UpdatePdfReportCommand, Result<PdfReportDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdatePdfReportCommand> _validator;
    private readonly IMapper _mapper;
    private static readonly Regex ExcessiveWhitespace = new(@"\s+", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private readonly IHubContext<PdfReportsHub> _hubContext;

    public UpdatePdfReportHandler(
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdatePdfReportCommand> validator,
        IMapper mapper,
        IHubContext<PdfReportsHub> hubContext)
    {
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<Result<PdfReportDto>> Handle(
    UpdatePdfReportCommand request,
    CancellationToken cancellationToken)
    {
        try
        {
            var normalizedRequest = request with { Name = NormalizeText(request.Name) };
            await _validator.ValidateAndThrowAsync(normalizedRequest, cancellationToken);

            var pdfReport = await _repositoryWrapper.PdfReportRepository.GetFirstOrDefaultAsync(
                new QueryOptions<PdfReport>
                {
                    Filter = pr => pr.Id == normalizedRequest.Id,
                    AsNoTracking = false,
                    Include = pr => pr.Include(p => p.Language)
                });

            if (pdfReport == null)
            {
                return Result.Fail<PdfReportDto>(ErrorMessagesConstants.NotFound(normalizedRequest.Id, typeof(PdfReport)));
            }

            var hasChanges = pdfReport.Name != normalizedRequest.Name;

            if (hasChanges)
            {
                pdfReport.Name = normalizedRequest.Name;

                if (await _repositoryWrapper.SaveChangesAsync() <= 0)
                {
                    return Result.Fail<PdfReportDto>(
                        ErrorMessagesConstants.FailedToUpdateEntity(typeof(PdfReport)));
                }
            }

            var dto = _mapper.Map<PdfReportDto>(pdfReport);

            await _hubContext.Clients.All.SendAsync("PdfReportUpdated", dto, cancellationToken: cancellationToken);
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

        return ExcessiveWhitespace.Replace(text.Trim(), " ");
    }
}
