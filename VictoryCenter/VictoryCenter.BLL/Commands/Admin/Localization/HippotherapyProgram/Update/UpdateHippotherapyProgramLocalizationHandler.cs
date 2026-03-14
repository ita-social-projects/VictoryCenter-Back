using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.HippotherapyPrograms;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyProgramEntity = VictoryCenter.DAL.Entities.HippotherapyProgram;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Update;
public class UpdateHippotherapyProgramLocalizationHandler : IRequestHandler<UpdateHippotherapyProgramLocalizationCommand, Result<HippotherapyProgramLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHippotherapyProgramLocalizationCommand> _validator;
    private readonly IProgramSectionContentService _programSectionContentService;
    private readonly ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization> _programLocalizationService;
    private readonly ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization> _contentLocalizationService;

    public UpdateHippotherapyProgramLocalizationHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateHippotherapyProgramLocalizationCommand> validator,
        IProgramSectionContentService programSectionContentService,
        ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization> programLocalizationService,
        ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization> contentLocalizationService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _programSectionContentService = programSectionContentService;
        _programLocalizationService = programLocalizationService;
        _contentLocalizationService = contentLocalizationService;
    }

    public async Task<Result<HippotherapyProgramLocalizationDto>> Handle(UpdateHippotherapyProgramLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var program = await _repositoryWrapper.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(new QueryOptions<HippotherapyProgramEntity>
            {
                Filter = c => c.Id == request.EntityId,
                Include = query => query.Include(p => p.Sections).ThenInclude(p => p.Contents)
            });

            if (program is null)
            {
                return Result.Fail<HippotherapyProgramLocalizationDto>("Not found programEntity");
            }

            var contentTypesById = await _programSectionContentService.GetContentTypesByProgramIdAsync(request.EntityId);

            ProgramSectionContentLocalizationValidationHelper
                .ValidateSections<UpdateHippotherapyProgramSectionLocalizationDto, UpdateHippotherapyProgramSectionContentLocalizationDto>(
                    request.UpdateHippotherapyProgramLocalizationDto.Sections,
                    contentTypesById,
                    content => content.EntityId,
                    program);

            var dto = request.UpdateHippotherapyProgramLocalizationDto;
            HippotherapyProgramLocalization programLocalizationEntity = _mapper.Map<HippotherapyProgramLocalization>(dto);
            programLocalizationEntity.EntityId = request.EntityId;
            programLocalizationEntity.LanguageId = request.LanguageId;
            await _programLocalizationService.TrackEntityLocalizationForUpdateAsync(programLocalizationEntity);

            var contentDtos = dto.Sections
                .SelectMany(section => section.Contents ?? [])
                .ToList();

            var contentLocalizations = _mapper.Map<List<ProgramSectionContentLocalization>>(contentDtos);

            for (int i = 0; i < contentLocalizations.Count; i++)
            {
                contentLocalizations[i].EntityId = contentDtos[i].EntityId;
                contentLocalizations[i].LanguageId = request.LanguageId;
            }

            await _contentLocalizationService.TrackEntityLocalizationAsync(contentLocalizations, true);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<HippotherapyProgramLocalizationDto>(
                    ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyProgramLocalization)));
            }

            var programLocalizationInfo = await _repositoryWrapper.LocalizationLanguagesRepository
                .GetFirstOrDefaultAsync(new QueryOptions<LocalizationLanguage>
                {
                    Filter = l => l.Id == request.LanguageId
                });

            var response = _mapper.Map<HippotherapyProgramLocalizationDto>(programLocalizationEntity) with
            {
                LocalizationInfoDto = _mapper.Map<LocalizationInfoDto>(programLocalizationInfo),
                Sections = await _programSectionContentService.GetProgramSectionsAsync(request.EntityId, request.LanguageId)
            };

            return Result.Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(ex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgramLocalization)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyProgramLocalization)));
        }
    }
}
