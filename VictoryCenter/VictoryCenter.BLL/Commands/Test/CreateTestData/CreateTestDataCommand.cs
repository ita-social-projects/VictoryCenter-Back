using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Test;

namespace VictoryCenter.BLL.Commands.Test.CreateTestData;

public record CreateTestDataCommand(CreateTestDataDto CreateTestData)
    : IRequest<Result<TestDataDto>>;
