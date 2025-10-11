using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Delete;

public class DeletePartnersSectionHandler : IRequestHandler<DeletePartnersSectionCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public DeletePartnersSectionHandler(
        IRepositoryWrapper repositoryWrapper,
        IReorderService reorderService)
    {
        _reorderService = reorderService;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeletePartnersSectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var sectionToDelete = await _repositoryWrapper.PartnerSectionsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<PartnerSection>
                {
                    Filter = s => s.Id == request.Id,
                    Include = q => q.Include(s => s.Partners)
                });

            if (sectionToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(PartnerSection)));
            }

            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                _repositoryWrapper.PartnerSectionsRepository.Delete(sectionToDelete);
                await _repositoryWrapper.SaveChangesAsync();

                await _reorderService.RenumberPriorityAsync<PartnerSection>();
                await _repositoryWrapper.SaveChangesAsync();

                scope.Complete();
            }

            return Result.Ok(request.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(PartnerSection)));
        }
    }
}
