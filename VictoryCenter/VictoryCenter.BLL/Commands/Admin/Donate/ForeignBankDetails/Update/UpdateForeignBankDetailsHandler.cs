using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Update;
public class UpdateForeignBankDetailsHandler : IRequestHandler<UpdateForeignBankDetailsCommand, Result<ForeignBankDetailsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateForeignBankDetailsCommand> _validator;

    public UpdateForeignBankDetailsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateForeignBankDetailsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ForeignBankDetailsDto>> Handle(UpdateForeignBankDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Entities.ForeignBankDetails? foreignBankDetailsEntity = await _repositoryWrapper.ForeignBankDetailsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<Entities.ForeignBankDetails>
                {
                    Filter = foreignhBankDetails => foreignhBankDetails.Id == request.Id
                });

            if (foreignBankDetailsEntity is null)
            {
                return Result.Fail<ForeignBankDetailsDto>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(Entities.ForeignBankDetails)));
            }

            _mapper.Map(request.UpdateForeignBankDetailsDto, foreignBankDetailsEntity);

            var existingCorrespondentBanks = await _repositoryWrapper.CorrespondentBankDetailsRepository
                .GetAllAsync(new QueryOptions<Entities.CorrespondentBankDetails>
                {
                    Filter = cb => cb.ForeignBankDetailsId == request.Id
                });

            foreach (var existingCb in existingCorrespondentBanks)
            {
                _repositoryWrapper.CorrespondentBankDetailsRepository.Delete(existingCb);
            }

            foreach (var cbDto in request.UpdateForeignBankDetailsDto.CorrespondentBanks)
            {
                var cbEntity = _mapper.Map<Entities.CorrespondentBankDetails>(cbDto);
                cbEntity.ForeignBankDetailsId = foreignBankDetailsEntity.Id;
                cbEntity.Id = 0;
                await _repositoryWrapper.CorrespondentBankDetailsRepository.CreateAsync(cbEntity);
            }

            _repositoryWrapper.ForeignBankDetailsRepository.Update(foreignBankDetailsEntity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var updatedEntity = await _repositoryWrapper.ForeignBankDetailsRepository
                    .GetFirstOrDefaultAsync(new QueryOptions<Entities.ForeignBankDetails>
                    {
                        Filter = fb => fb.Id == request.Id,
                        Include = q => q.Include(fb => fb.CorrespondentBanks)
                    });
                ForeignBankDetailsDto responseDto = _mapper.Map<ForeignBankDetailsDto>(updatedEntity);
                return Result.Ok(responseDto);
            }

            return Result.Fail<ForeignBankDetailsDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.ForeignBankDetails)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ForeignBankDetailsDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
