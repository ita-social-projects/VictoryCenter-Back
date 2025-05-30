using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Test;

namespace VictoryCenter.BLL.Queries.Test.GetTestData;

public record GetTestDataQuery(int Id)
    : IRequest<Result<TestDataDto>>;
