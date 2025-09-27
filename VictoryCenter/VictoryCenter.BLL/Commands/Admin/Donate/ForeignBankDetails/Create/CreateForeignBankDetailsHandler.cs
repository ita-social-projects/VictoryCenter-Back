using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;
public class CreateForeignBankDetailsHandler : IRequestHandler<CreateForeignBankDetailsCommand, Result<ForeignBankDetailsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateForeignBankDetailsCommand> _validator;

    public CreateForeignBankDetailsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateForeignBankDetailsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ForeignBankDetailsDto>> Handle(CreateForeignBankDetailsCommand request, CancellationToken cancellationToken)
    {
        {
            try
            {
                await _validator.ValidateAndThrowAsync(request, cancellationToken);

                Entities.ForeignBankDetails entity = _mapper.Map<Entities.ForeignBankDetails>(request.CreateForeignBankDetailsDto);
                await _repositoryWrapper.ForeignBankDetailsRepository.CreateAsync(entity);

                if (await _repositoryWrapper.SaveChangesAsync() > 0)
                {
                    ForeignBankDetailsDto responseDto = _mapper.Map<ForeignBankDetailsDto>(entity);
                    return Result.Ok(responseDto);
                }

                return Result.Fail<ForeignBankDetailsDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.ForeignBankDetails)));
            }
            catch (ValidationException ex)
            {
                return Result.Fail<ForeignBankDetailsDto>(ex.Errors.Select(e => e.ErrorMessage));
            }
        }
    }
}
