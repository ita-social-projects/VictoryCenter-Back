using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;
public class CreateSupportOptionsHandler : IRequestHandler<CreateSupportOptionsCommand, Result<SupportOptionsDto>>
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

    public async Task<Result<SupportOptionsDto>> Handle(CreateSupportOptionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Entities.SupportOptions entity = _mapper.Map<Entities.SupportOptions>(request.CreateSupportOptionsDto);
            await _repositoryWrapper.SupportOptionsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                SupportOptionsDto responseDto = _mapper.Map<SupportOptionsDto>(entity);
                return Result.Ok(responseDto);
            }

            return Result.Fail<SupportOptionsDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.SupportOptions)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<SupportOptionsDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<SupportOptionsDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(Entities.SupportOptions)));
        }
    }
}
