using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Test;

namespace VictoryCenter.BLL.Commands.Test.UpdateTestData;

public record UpdateTestDataCommand(UpdateTestDataDto UpdateTestData)
    : IRequest<Result<TestDataDto>>;
