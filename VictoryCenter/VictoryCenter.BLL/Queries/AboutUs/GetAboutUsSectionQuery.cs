using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.AboutUsSectionDto;

namespace VictoryCenter.BLL.Queries.AboutUs;

public record GetAboutUsSectionQuery(string SectionType) : IRequest<Result<AboutUsSectionDto>>;
