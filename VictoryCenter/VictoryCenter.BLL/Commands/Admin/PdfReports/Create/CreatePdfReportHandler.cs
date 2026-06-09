using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.PdfStorage;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Create;

public class CreatePdfReportHandler : IRequestHandler<CreatePdfReportCommand, Result<PdfReportDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IPdfService _pdfService;
    private readonly IValidator<CreatePdfReportCommand> _validator;
    private readonly IMapper _mapper;
    private readonly IReorderService _reorderService;

    public CreatePdfReportHandler(
        IRepositoryWrapper repositoryWrapper,
        IPdfService pdfService,
        IValidator<CreatePdfReportCommand> validator,
        IMapper mapper,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _pdfService = pdfService;
        _mapper = mapper;
        _reorderService = reorderService;
        _validator = validator;
    }

    public async Task<Result<PdfReportDto>> Handle(CreatePdfReportCommand request, CancellationToken cancellationToken)
    {
        string? blobName = null;
        bool committed = false;
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dto = request.CreatePdfReportDto;
            var fileName = Guid.NewGuid().ToString().Replace("-", string.Empty);

            using var transaction = _repositoryWrapper.BeginTransaction();

            blobName = await _pdfService.UploadPdfAsync(dto.File, fileName);
            var nextPriority = await _reorderService.GetNextDisplayOrderAsync<PdfReport>();

            var pdfReport = new PdfReport
            {
                Name = Path.GetFileNameWithoutExtension(dto.File.FileName),
                BlobName = blobName,
                FileSizeBytes = dto.File.Length,
                Priority = nextPriority,
                LanguageId = dto.LanguageId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _repositoryWrapper.PdfReportRepository.CreateAsync(pdfReport);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<PdfReportDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(PdfReport)));
            }

            transaction.Complete();
            committed = true;

            var result = _mapper.Map<PdfReportDto>(pdfReport);
            return Result.Ok(result);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PdfReportDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (InvalidPdfFormatException ex)
        {
            return Result.Fail<PdfReportDto>(ex.Message);
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<PdfReportDto>(ErrorMessagesConstants.BlobStorageError(e.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PdfReportDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PdfReport)));
        }
        finally
        {
            if (!committed && blobName != null)
            {
                TryDeleteBlob(blobName);
            }
        }
    }

    private void TryDeleteBlob(string blobName)
    {
        try
        {
            _pdfService.DeletePdf(blobName);
        }
        catch
        {
        }
    }
}
