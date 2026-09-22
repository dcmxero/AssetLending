using System.ComponentModel.DataAnnotations;
using DTOs.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Covers the future-date rule at instants chosen by the test rather than by the machine clock.
/// </summary>
public class FutureDateAttributeTests
{
    private static readonly DateTime Now = new(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Rejects_ADateBeforeToday()
    {
        Assert.False(IsValid(Now.AddDays(-1)));
    }

    [Fact]
    public void Accepts_Today_EvenAtItsLastMoment()
    {
        Assert.True(IsValid(Now.Date));
        Assert.True(IsValid(Now.Date.AddHours(23)));
    }

    [Fact]
    public void Accepts_ADateAfterToday()
    {
        Assert.True(IsValid(Now.AddDays(1)));
    }

    [Fact]
    public void FollowsTheProvidedClock_NotTheMachineClock()
    {
        var clock = new FakeTimeProvider(Now);
        var dueDate = Now.AddDays(3);

        Assert.True(IsValid(dueDate, clock));

        clock.Advance(TimeSpan.FromDays(10));

        Assert.False(IsValid(dueDate, clock));
    }

    private static bool IsValid(DateTime value, FakeTimeProvider? clock = null)
    {
        var services = new ServiceCollection()
            .AddSingleton<TimeProvider>(clock ?? new FakeTimeProvider(Now))
            .BuildServiceProvider();

        var context = new ValidationContext(new object(), services, null) { DisplayName = "DueDate" };

        return new FutureDateAttribute().GetValidationResult(value, context) is null;
    }
}
