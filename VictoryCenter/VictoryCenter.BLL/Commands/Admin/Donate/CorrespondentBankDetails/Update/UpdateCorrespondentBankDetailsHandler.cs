using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Update;
public class UpdateCorrespondentBankDetailsHandler : BaseHandler<UpdateCorrespondentBankDetailsCommand, CorrespondentBankDetailsDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateCorrespondentBankDetailsCommand> _validator;

    public UpdateCorrespondentBankDetailsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateCorrespondentBankDetailsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public override async Task<CorrespondentBankDetailsDto> HandleRequest(UpdateCorrespondentBankDetailsCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Entities.CorrespondentBankDetails? correspondentBankDetailsEntity = await _repositoryWrapper.CorrespondentBankDetailsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.CorrespondentBankDetails>
            {
                Filter = correspondentBankDetails => correspondentBankDetails.Id == request.Id
            });

        if (correspondentBankDetailsEntity is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Entities.CorrespondentBankDetails)));
        }

        Entities.CorrespondentBankDetails entityToUpdate = _mapper.Map(request.UpdateCorrespondentBankDetailsDto, correspondentBankDetailsEntity);

        _repositoryWrapper.CorrespondentBankDetailsRepository.Update(entityToUpdate);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            CorrespondentBankDetailsDto responseDto = _mapper.Map<CorrespondentBankDetailsDto>(entityToUpdate);
            return responseDto;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.CorrespondentBankDetails)));
    }
}
