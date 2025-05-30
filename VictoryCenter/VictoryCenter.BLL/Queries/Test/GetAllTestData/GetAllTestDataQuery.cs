using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Test;

namespace VictoryCenter.BLL.Queries.Test.GetAllTestData;

public record GetAllTestDataQuery()
    : IRequest<Result<IEnumerable<TestDataDto>>>;
