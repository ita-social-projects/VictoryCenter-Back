using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Donations.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Donations.CreateUah;

public class CreateUahBankDetailsHandler : IRequestHandler<CreateUahBankDetailsCommand, Result<BankDetailsDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateUahBankDetailsCommand> _validator;

    public CreateUahBankDetailsHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateUahBankDetailsCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<BankDetailsDto>> Handle(CreateUahBankDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entity = _mapper.Map<UahBankDetails>(request.CreateUahBankDetailsDto);

            await _repositoryWrapper.UahBankDetailsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<BankDetailsDto>("Failed to create UAH bank details.");
            }

            var resultDto = _mapper.Map<BankDetailsDto>(entity);
            return Result.Ok(resultDto);
        }
        catch (ValidationException ex)
        {
            return Result.Fail<BankDetailsDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
