using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;
public class UpdateSupportOptionsHandler : BaseHandler<UpdateSupportOptionsCommand, SupportOptionsDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateSupportOptionsCommand> _validator;

    public UpdateSupportOptionsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateSupportOptionsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public override async Task<SupportOptionsDto> HandleRequest(UpdateSupportOptionsCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Entities.SupportOptions? supportOptionsEntity = await _repositoryWrapper.SupportOptionsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Entities.SupportOptions>
            {
                Filter = supportOptions => supportOptions.Id == request.Id
            });

        if (supportOptionsEntity is null)
        {
            throw new Exception(ErrorMessagesConstants
                .NotFound(request.Id, typeof(Entities.SupportOptions)));
        }

        Entities.SupportOptions entityToUpdate = _mapper.Map(request.UpdateSupportOptionsDto, supportOptionsEntity);

        _repositoryWrapper.SupportOptionsRepository.Update(entityToUpdate);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            SupportOptionsDto responseDto = _mapper.Map<SupportOptionsDto>(entityToUpdate);
            return responseDto;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.SupportOptions)));
    }
}
