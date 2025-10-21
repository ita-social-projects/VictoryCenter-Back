using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Update;
public class UpdateForeignBankDetailsHandler : BaseHandler<UpdateForeignBankDetailsCommand, ForeignBankDetailsDto>
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

    public override async Task<ForeignBankDetailsDto> HandleRequest(UpdateForeignBankDetailsCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Entities.ForeignBankDetails? foreignBankDetailsEntity = await _repositoryWrapper.ForeignBankDetailsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.ForeignBankDetails>
            {
                Filter = foreignBankDetails => foreignBankDetails.Id == request.Id
            });

        if (foreignBankDetailsEntity is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Entities.ForeignBankDetails)));
        }

        Entities.ForeignBankDetails entityToUpdate = _mapper.Map(request.UpdateForeignBankDetailsDto, foreignBankDetailsEntity);

        _repositoryWrapper.ForeignBankDetailsRepository.Update(entityToUpdate);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            ForeignBankDetailsDto responseDto = _mapper.Map<ForeignBankDetailsDto>(entityToUpdate);
            return responseDto;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.ForeignBankDetails)));
    }
}
