using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;
public class CreateSupportOptionsHandler : BaseHandler<CreateSupportOptionsCommand, SupportOptionsDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateSupportOptionsCommand> _validator;

    public CreateSupportOptionsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateSupportOptionsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public override async Task<SupportOptionsDto> HandleRequest(CreateSupportOptionsCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Entities.SupportOptions entity = _mapper.Map<Entities.SupportOptions>(request.CreateSupportOptionsDto);
        await _repositoryWrapper.SupportOptionsRepository.CreateAsync(entity, cancellationToken);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            SupportOptionsDto responseDto = _mapper.Map<SupportOptionsDto>(entity);
            return responseDto;
        }

        throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.SupportOptions)));
    }
}
