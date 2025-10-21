using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Create;
public class CreateUahBankDetailsHandler : BaseHandler<CreateUahBankDetailsCommand, UahBankDetailsDto>
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

    public override async Task<UahBankDetailsDto> HandleRequest(CreateUahBankDetailsCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Entities.UahBankDetails entity = _mapper.Map<Entities.UahBankDetails>(request.CreateUahBankDetailsDto);
        await _repositoryWrapper.UahBankDetailsRepository.CreateAsync(entity);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            UahBankDetailsDto responseDto = _mapper.Map<UahBankDetailsDto>(entity);
            return responseDto;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.UahBankDetails)));
    }
}
