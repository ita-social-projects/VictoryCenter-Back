using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Donations.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Donations.CreateForeign;

public class CreateForeignBankDetailsHandler : IRequestHandler<CreateForeignBankDetailsCommand, Result<BankDetailsDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateForeignBankDetailsCommand> _validator;

    public CreateForeignBankDetailsHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateForeignBankDetailsCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<BankDetailsDto>> Handle(CreateForeignBankDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entity = _mapper.Map<ForeignBankDetails>(request.CreateForeignBankDetailsDto);

            await _repositoryWrapper.ForeignBankDetailsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<BankDetailsDto>("Failed to create foreign bank details.");
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
