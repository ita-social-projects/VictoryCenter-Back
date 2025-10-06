using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Delete;

public class DeletePartnersSectionHandler : IRequestHandler<DeletePartnersSectionCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;
    private readonly IBlobService _blobService;

    public DeletePartnersSectionHandler(
        IRepositoryWrapper repositoryWrapper,
        IReorderService reorderService,
        IBlobService blobService)
    {
        _reorderService = reorderService;
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
    }

    public async Task<Result<long>> Handle(DeletePartnersSectionCommand request, CancellationToken cancellationToken)
    {
        using var scope = _repositoryWrapper.BeginTransaction();

        var sectionToDelete = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<PartnerSection>
            {
                Filter = s => s.Id == request.Id,
                Include = q => q.Include(s => s.Partners)
                                .ThenInclude(p => p.Image!),
                AsNoTracking = false
            });

        if (sectionToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(PartnerSection)));
        }

        var partnersToDelete = sectionToDelete.Partners.ToList();
        var imagesToDelete = partnersToDelete.Select(p => p.Image).Where(img => img != null).ToList();

        try
        {
            if (imagesToDelete.Any())
            {
                _repositoryWrapper.ImageRepository.DeleteRange(imagesToDelete!);
            }

            if (partnersToDelete.Any())
            {
                _repositoryWrapper.PartnerRepository.DeleteRange(partnersToDelete);
            }

            _repositoryWrapper.PartnerSectionsRepository.Delete(sectionToDelete);

            await _repositoryWrapper.SaveChangesAsync();

            await _reorderService.RenumberPriorityAsync<PartnerSection>();

            await _repositoryWrapper.SaveChangesAsync();

            foreach (var image in imagesToDelete)
            {
                _blobService.DeleteFileInStorage(image!.BlobName, image.MimeType);
            }

            scope.Complete();

            return Result.Ok(request.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(PartnerSection)));
        }
    }
}
