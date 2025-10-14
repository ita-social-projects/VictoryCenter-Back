using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Create;
public class CreateUahBankDetailsHandler : IRequestHandler<CreateUahBankDetailsCommand, Result<UahBankDetailsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateUahBankDetailsCommand> _validator;

    public CreateUahBankDetailsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateUahBankDetailsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<UahBankDetailsDto>> Handle(CreateUahBankDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Entities.UahBankDetails entity = _mapper.Map<Entities.UahBankDetails>(request.CreateUahBankDetailsDto);
            await _repositoryWrapper.UahBankDetailsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                UahBankDetailsDto responseDto = _mapper.Map<UahBankDetailsDto>(entity);
                return Result.Ok(responseDto);
            }

            return Result.Fail<UahBankDetailsDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.UahBankDetails)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<UahBankDetailsDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<UahBankDetailsDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(Entities.UahBankDetails)));
        }
    }
}
