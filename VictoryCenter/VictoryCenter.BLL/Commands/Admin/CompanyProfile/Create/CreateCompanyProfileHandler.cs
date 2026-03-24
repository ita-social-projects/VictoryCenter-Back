using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.CompanyProfile.Create;

public class CreateCompanyProfileHandler : IRequestHandler<CreateCompanyProfileCommand, Result<CompanyProfileDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCompanyProfileCommand> _validator;

    public CreateCompanyProfileHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateCompanyProfileCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<CompanyProfileDto>> Handle(CreateCompanyProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            if ((await _repositoryWrapper.CompanyProfileRepository.GetFirstOrDefaultAsync()) is not null)
            {
                return Result.Fail<CompanyProfileDto>(errorMessage: ErrorMessagesConstants.OnlyOneEntityOfTypeIsAllowed(nameof(DAL.Entities.CompanyProfile)));
            }

            var entity = _mapper.Map<DAL.Entities.CompanyProfile>(request.CreateCompanyProfileDto);

            var now = DateTimeOffset.UtcNow;
            entity.CreatedAt = now;
            entity.Contact.CreatedAt = now;
            entity.Requisite.CreatedAt = now;

            foreach (var socialLink in entity.SocialLinks)
            {
                socialLink.CreatedAt = now;
            }

            await _repositoryWrapper.CompanyProfileRepository.CreateAsync(entity);
            await _repositoryWrapper.SaveChangesAsync();

            var created = await _repositoryWrapper.CompanyProfileRepository.GetFirstOrDefaultAsync(new QueryOptions<DAL.Entities.CompanyProfile>
            {
                Filter = p => p.Id == entity.Id,
                Include = q => q
                    .Include(p => p.Contact)
                        .ThenInclude(c => c.Localizations)
                        .ThenInclude(l => l.Language)
                    .Include(p => p.Requisite)
                        .ThenInclude(r => r.Localizations)
                        .ThenInclude(l => l.Language)
                    .Include(p => p.SocialLinks)
            });

            var resultDto = _mapper.Map<CompanyProfileDto>(created);
            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<CompanyProfileDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<CompanyProfileDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(DAL.Entities.CompanyProfile)));
        }
    }
}
