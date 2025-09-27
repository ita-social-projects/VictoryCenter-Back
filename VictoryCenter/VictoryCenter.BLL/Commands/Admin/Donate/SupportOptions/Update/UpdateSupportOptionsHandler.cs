using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;
public class UpdateSupportOptionsHandler : IRequestHandler<UpdateSupportOptionsCommand, Result<SupportOptionsDto>>
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

    public async Task<Result<SupportOptionsDto>> Handle(UpdateSupportOptionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Entities.SupportOptions? supportOptionsEntity = await _repositoryWrapper.SupportOptionsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<Entities.SupportOptions>
                {
                    Filter = supportOptions => supportOptions.Id == request.Id
                });

            if (supportOptionsEntity is null)
            {
                return Result.Fail<SupportOptionsDto>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(Entities.SupportOptions)));
            }

            Entities.SupportOptions entityToUpdate = _mapper.Map(request.UpdateSupportOptionsDto, supportOptionsEntity);

            _repositoryWrapper.SupportOptionsRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                SupportOptionsDto responseDto = _mapper.Map<SupportOptionsDto>(entityToUpdate);
                return Result.Ok(responseDto);
            }

            return Result.Fail<SupportOptionsDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.SupportOptions)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<SupportOptionsDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
