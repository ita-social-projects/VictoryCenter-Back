using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.FAQ;

namespace VictoryCenter.BLL.Queries.Public.FaqQuestions.GetPublished;

public record GetPublishedFaqQuestionsBySlugQuery(string Slug)
    : IRequest<Result<List<PublishedFaqQuestionDto>>>;
