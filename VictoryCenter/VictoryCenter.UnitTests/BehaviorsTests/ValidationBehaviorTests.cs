using FluentValidation;
using FluentValidation.Results;
using MediatR;
using VictoryCenter.BLL.Behaviors;
using VictoryCenter.BLL.Behaviors.Abstractions;

namespace VictoryCenter.UnitTests.BehaviorsTests;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidatorsRegistered()
    {
        var behavior = new ValidationBehavior<TestRequest, bool>([]);

        var result = await behavior.Handle(new TestRequest(), NextReturnsTrue(), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenAllValidatorsPass()
    {
        var behavior = new ValidationBehavior<TestRequest, bool>([new AlwaysValidValidator()]);

        var result = await behavior.Handle(new TestRequest(), NextReturnsTrue(), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenMultipleValidatorsAllPass()
    {
        var behavior = new ValidationBehavior<TestRequest, bool>(
            [new AlwaysValidValidator(), new AlwaysValidValidator()]);

        var result = await behavior.Handle(new TestRequest(), NextReturnsTrue(), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenSingleValidatorFails()
    {
        var failure = new ValidationFailure("Property", "Must not be empty");
        var behavior = new ValidationBehavior<TestRequest, bool>([new AlwaysInvalidValidator(failure)]);

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(new TestRequest(), NextReturnsTrue(), CancellationToken.None));

        Assert.Single(exception.Errors);
        Assert.Equal(failure.ErrorMessage, exception.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenMultipleValidatorsFailWithCombinedErrors()
    {
        var failure1 = new ValidationFailure("PropertyA", "Error A");
        var failure2 = new ValidationFailure("PropertyB", "Error B");

        var behavior = new ValidationBehavior<TestRequest, bool>(
            [new AlwaysInvalidValidator(failure1), new AlwaysInvalidValidator(failure2)]);

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(new TestRequest(), NextReturnsTrue(), CancellationToken.None));

        Assert.Equal(2, exception.Errors.Count());
        Assert.Contains(exception.Errors, e => e.ErrorMessage == failure1.ErrorMessage);
        Assert.Contains(exception.Errors, e => e.ErrorMessage == failure2.ErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldNotCallNext_WhenValidationFails()
    {
        var failure = new ValidationFailure("Property", "Error");
        var behavior = new ValidationBehavior<TestRequest, bool>([new AlwaysInvalidValidator(failure)]);

        var nextCalled = false;
        RequestHandlerDelegate<bool> next = _ =>
        {
            nextCalled = true;
            return Task.FromResult(true);
        };

        await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(new TestRequest(), next, CancellationToken.None));

        Assert.False(nextCalled);
    }

    [Fact]
    public async Task Handle_ShouldOnlyCollectFailuresFromInvalidValidators_WhenMixed()
    {
        var failure = new ValidationFailure("PropertyA", "Error A");

        var behavior = new ValidationBehavior<TestRequest, bool>(
            [new AlwaysInvalidValidator(failure), new AlwaysValidValidator()]);

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(new TestRequest(), NextReturnsTrue(), CancellationToken.None));

        Assert.Single(exception.Errors);
        Assert.Equal(failure.ErrorMessage, exception.Errors.First().ErrorMessage);
    }

    private static RequestHandlerDelegate<bool> NextReturnsTrue()
        => _ => Task.FromResult(true);

    private sealed class AlwaysValidValidator : AbstractValidator<TestRequest>
    {
    }

    private sealed class AlwaysInvalidValidator(ValidationFailure failure) : AbstractValidator<TestRequest>
    {
        public override Task<ValidationResult> ValidateAsync(
            ValidationContext<TestRequest> context,
            CancellationToken cancellation = default)
            => Task.FromResult(new ValidationResult([failure]));
    }
}

public sealed record TestRequest : IValidatableRequest;
