using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.FaqQuestions;

namespace VictoryCenter.BLL.Queries.Public.FaqQuestions.GetPublished;

public record GetPublishedFaqQuestionsBySlugQuery(string Slug)
    : IRequest<Result<List<PublishedFaqQuestionDto>>>;
