using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.AboutUsSectionDto;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.AboutUs;

public class GetAboutUsSectionHandler : IRequestHandler<GetAboutUsSectionQuery, Result<AboutUsSectionDto>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;

    public GetAboutUsSectionHandler(IRepositoryWrapper repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<AboutUsSectionDto>> Handle(GetAboutUsSectionQuery request, CancellationToken cancellationToken)
    {
        if (Enum.TryParse<SectionType>(request.SectionType, out var parsedSectionType))
        {
            var section = await _repository.AboutUsSectionsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<AboutUsSection>
                {
                    Filter = s => s.SectionType == parsedSectionType,
                    Include = s => s.Include(c => c.Contents)
                });
            Console.WriteLine("saasga");
            return Result.Ok(_mapper.Map<AboutUsSectionDto>(section));
        }
        else
        {
            return Result.Fail("fafasf");
        }
    }
}
