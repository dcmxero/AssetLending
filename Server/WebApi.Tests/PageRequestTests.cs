using DTOs.Common;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Covers the clamping the listing endpoints rely on.
/// </summary>
public class PageRequestTests
{
    [Fact]
    public void Defaults_ToTheFirstPageOfTen()
    {
        var request = new PageRequest();

        Assert.Equal(1, request.Page);
        Assert.Equal(10, request.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void ClampsPage_ToTheFirstOne(int page)
    {
        Assert.Equal(1, new PageRequest { Page = page }.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void FallsBackToTheDefault_WhenPageSizeIsNotPositive(int pageSize)
    {
        Assert.Equal(10, new PageRequest { PageSize = pageSize }.PageSize);
    }

    [Fact]
    public void CapsPageSize_AtTheMaximum()
    {
        Assert.Equal(PageRequest.MaxPageSize, new PageRequest { PageSize = 10_000 }.PageSize);
    }

    [Fact]
    public void KeepsValuesInsideTheSupportedRange()
    {
        var request = new PageRequest { Page = 3, PageSize = 25 };

        Assert.Equal(3, request.Page);
        Assert.Equal(25, request.PageSize);
    }
}
