using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.Faq;

public class FaqQuestionSeeder : BaseSeeder<FaqQuestion>
{
    public FaqQuestionSeeder(VictoryCenterDbContext dbContext, ILogger<FaqQuestionSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => nameof(FaqQuestionSeeder);
    public override int Order => 3;

    protected override async Task<List<FaqQuestion>> GenerateEntitiesAsync()
    {
        var pages = await DbContext.VisitorPages.ToListAsync();

        if (pages.Count < 3)
        {
            throw new InvalidOperationException("You need at least 3 pages seeded to seed faq");
        }

        var questions = new List<FaqQuestion>
        {
            new()
            {
                QuestionText = "How can I reset my password?",
                AnswerText = "Click the 'Forgot Password' link on the login page and follow the instructions.",
                Status = Status.Published,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                Placements = new List<FaqPlacement>
                {
                    new()
                    {
                        PageId = pages[0].Id,
                        Priority = 1
                    },
                    new()
                    {
                        PageId = pages[1].Id,
                        Priority = 1
                    }
                }
            },
            new()
            {
                Id = 2,
                QuestionText = "Where can I view my purchase history?",
                AnswerText = "Go to your profile and click 'Order History' to see all your past orders.",
                Status = Status.Published,
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                Placements = new List<FaqPlacement>
                {
                    new()
                    {
                        PageId = pages[0].Id,
                        Priority = 2
                    },
                    new()
                    {
                        PageId = pages[1].Id,
                        Priority = 2
                    },
                    new()
                    {
                        PageId = pages[2].Id,
                        Priority = 1
                    }
                }
            },
            new()
            {
                Id = 3,
                QuestionText = "How do I contact customer support?",
                AnswerText = "Use the form on our 'Contact Us' page or call 1-800-555-0199.",
                Status = Status.Draft,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Placements = new List<FaqPlacement>
                {
                    new()
                    {
                        PageId = pages[1].Id,
                        Priority = 3
                    },
                    new()
                    {
                        PageId = pages[2].Id,
                        Priority = 2
                    }
                }
            },
            new()
            {
                Id = 4,
                QuestionText = "Can I change my delivery address?",
                AnswerText = "Yes, before shipment, go to your order details and update the address.",
                Status = Status.Published,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Placements = new List<FaqPlacement>
                {
                    new()
                    {
                        PageId = pages[1].Id,
                        Priority = 4
                    },
                }
            }
        };

        return questions;
    }
}
