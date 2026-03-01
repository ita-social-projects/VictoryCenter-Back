using FluentResults;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Helpers;

public static class FaqQuestionHelper
{
    public static async Task<Result> AssignSectionContentFaqQuestionsAsync(
        IRepositoryWrapper repositoryWrapper,
        IEnumerable<HippotherapyProgramSection> sections)
    {
        var faqQuestionIds = (sections ?? [])
            .SelectMany(s => s.Contents)
            .OfType<FaqQuestionProgramContent>()
            .Select(c => c.FaqQuestionId)
            .Distinct()
            .ToList();

        if (faqQuestionIds.Count == 0)
        {
            return Result.Ok();
        }

        var faqQuestions = await repositoryWrapper.FaqQuestionsRepository.GetAllAsync(
            new QueryOptions<FaqQuestion>
            {
                Filter = q => faqQuestionIds.Contains(q.Id),
                AsNoTracking = false
            });

        var faqQuestionsById = faqQuestions.ToDictionary(q => q.Id);

        foreach (var content in (sections ?? []).SelectMany(s => s.Contents).OfType<FaqQuestionProgramContent>())
        {
            content.FaqQuestion = faqQuestionsById[content.FaqQuestionId];
        }

        return Result.Ok();
    }
}
