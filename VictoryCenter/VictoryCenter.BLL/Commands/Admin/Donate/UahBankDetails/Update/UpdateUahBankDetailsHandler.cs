using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Update;
public class UpdateUahBankDetailsHandler : IRequestHandler<UpdateUahBankDetailsCommand, Result<UahBankDetailsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateUahBankDetailsCommand> _validator;

    public UpdateUahBankDetailsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateUahBankDetailsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<UahBankDetailsDto>> Handle(UpdateUahBankDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Entities.UahBankDetails? uahBankDetailsEntity = await _repositoryWrapper.UahBankDetailsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<Entities.UahBankDetails>
                {
                    Filter = uahBankDetails => uahBankDetails.Id == request.Id
                });

            if (uahBankDetailsEntity is null)
            {
                return Result.Fail<UahBankDetailsDto>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(Entities.UahBankDetails)));
            }

            Entities.UahBankDetails entityToUpdate = _mapper.Map(request.UpdateUahBankDetailsDto, uahBankDetailsEntity);

            _repositoryWrapper.UahBankDetailsRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                UahBankDetailsDto responseDto = _mapper.Map<UahBankDetailsDto>(entityToUpdate);
                return Result.Ok(responseDto);
            }

            return Result.Fail<UahBankDetailsDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.UahBankDetails)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<UahBankDetailsDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
