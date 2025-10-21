using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Update;
public class UpdateUahBankDetailsHandler : BaseHandler<UpdateUahBankDetailsCommand, UahBankDetailsDto>
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

    public override async Task<UahBankDetailsDto> HandleRequest(UpdateUahBankDetailsCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Entities.UahBankDetails? uahBankDetailsEntity = await _repositoryWrapper.UahBankDetailsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.UahBankDetails>
            {
                Filter = uahBankDetails => uahBankDetails.Id == request.Id
            });

        if (uahBankDetailsEntity is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Entities.UahBankDetails)));
        }

        Entities.UahBankDetails entityToUpdate = _mapper.Map(request.UpdateUahBankDetailsDto, uahBankDetailsEntity);

        _repositoryWrapper.UahBankDetailsRepository.Update(entityToUpdate);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            UahBankDetailsDto responseDto = _mapper.Map<UahBankDetailsDto>(entityToUpdate);
            return responseDto;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.UahBankDetails)));
    }
}
