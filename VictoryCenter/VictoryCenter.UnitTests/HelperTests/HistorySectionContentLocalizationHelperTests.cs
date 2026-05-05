using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.HelperTests;

public class HistorySectionContentLocalizationHelperTests
{
    [Fact]
    public void ValidateHistoryContents_WithValidData()
    {
        var contents = new List<CreateHistorySectionContentLocalizationDto>
        {
            new() { EntityId = 10, Title = "History Title", Description = null },
            new() { EntityId = 11, Title = null, Description = "History Description" }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 10, ContentType.Title },
            { 11, ContentType.Description }
        };

        var ex = Record.Exception(() =>
            HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(contents, contentTypesById));

        Assert.Null(ex);
    }

    [Fact]
    public void ValidateHistoryContents_InvalidContentId_ThrowsNotFoundValidationException()
    {
        var contents = new List<CreateHistorySectionContentLocalizationDto>
        {
            new() { EntityId = 99 }
        };

        var contentTypesById = new Dictionary<long, ContentType>();

        var ex = Assert.Throws<ValidationException>(() =>
            HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(contents, contentTypesById));

        Assert.Contains(
            ErrorMessagesConstants.NotFound(99, typeof(HistorySectionContent)).Split(' ')[0],
            ex.Message);
    }

    [Fact]
    public void ValidateHistoryContents_UnsupportedContentType_ThrowsValidationException()
    {
        var contents = new List<CreateHistorySectionContentLocalizationDto>
        {
            new() { EntityId = 10, Title = "Some Title" }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 10, ContentType.Image }
        };

        var ex = Assert.Throws<ValidationException>(() =>
            HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(contents, contentTypesById));

        Assert.Contains("which is not allowed for history localization", ex.Message);
        Assert.Contains("Only Title and Description are allowed", ex.Message);
    }

    [Fact]
    public void ValidateHistoryContents_TitleType_WithoutTitle_ThrowsValidationException()
    {
        var contents = new List<CreateHistorySectionContentLocalizationDto>
        {
            new() { EntityId = 10, Title = "" }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 10, ContentType.Title }
        };

        var ex = Assert.Throws<ValidationException>(() =>
            HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(contents, contentTypesById));

        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired("Title"), ex.Message);
    }

    [Fact]
    public void ValidateHistoryContents_TitleType_WithForbiddenDescription_ThrowsValidationException()
    {
        var contents = new List<CreateHistorySectionContentLocalizationDto>
        {
            new() { EntityId = 10, Title = "Valid Title", Description = "Forbidden Description" }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 10, ContentType.Title }
        };

        var ex = Assert.Throws<ValidationException>(() =>
            HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(contents, contentTypesById));

        Assert.Contains(
            ErrorMessagesConstants.PropertyNotAllowedForContentType("Description", ContentType.Title),
            ex.Message);
    }

    [Fact]
    public void ValidateHistoryContents_DescriptionType_WithoutDescription_ThrowsValidationException()
    {
        var contents = new List<CreateHistorySectionContentLocalizationDto>
        {
            new() { EntityId = 11, Description = null }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 11, ContentType.Description }
        };

        var ex = Assert.Throws<ValidationException>(() =>
            HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(contents, contentTypesById));

        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired("Description"), ex.Message);
    }

    [Fact]
    public void ValidateHistoryContents_DescriptionType_WithForbiddenTitle_ThrowsValidationException()
    {
        var contents = new List<CreateHistorySectionContentLocalizationDto>
        {
            new() { EntityId = 11, Description = "Valid Description", Title = "Forbidden Title" }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 11, ContentType.Description }
        };

        var ex = Assert.Throws<ValidationException>(() =>
            HistorySectionContentLocalizationValidationHelper.ValidateHistoryContents(contents, contentTypesById));

        Assert.Contains(
            ErrorMessagesConstants.PropertyNotAllowedForContentType("Title", ContentType.Description),
            ex.Message);
    }
}
