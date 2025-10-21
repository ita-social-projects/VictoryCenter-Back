using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Create;
public class CreateCorrespondentBankDetailsHandler : BaseHandler<CreateCorrespondentBankDetailsCommand, CorrespondentBankDetailsDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateCorrespondentBankDetailsCommand> _validator;

    public CreateCorrespondentBankDetailsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateCorrespondentBankDetailsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public override async Task<CorrespondentBankDetailsDto> HandleRequest(CreateCorrespondentBankDetailsCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Entities.ForeignBankDetails? foreignBankDetailsEntity = await _repositoryWrapper.ForeignBankDetailsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.ForeignBankDetails>
            {
                Filter = foreignBankDetails => foreignBankDetails.Id == request.CreateCorrespondentBankDetailsDto.ForeignBankDetailsId
            });

        if (foreignBankDetailsEntity is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.CreateCorrespondentBankDetailsDto.ForeignBankDetailsId, typeof(Entities.ForeignBankDetails)));
        }

        Entities.CorrespondentBankDetails entity = _mapper.Map<Entities.CorrespondentBankDetails>(request.CreateCorrespondentBankDetailsDto);
        await _repositoryWrapper.CorrespondentBankDetailsRepository.CreateAsync(entity);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            CorrespondentBankDetailsDto responseDto = _mapper.Map<CorrespondentBankDetailsDto>(entity);
            return responseDto;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.CorrespondentBankDetails)));
    }
}
