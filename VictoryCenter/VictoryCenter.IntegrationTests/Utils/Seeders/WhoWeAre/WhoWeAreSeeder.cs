using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.WhoWeAre;

public class WhoWeAreSeeder : BaseSeeder<WhoWeAreSection>
{
    public WhoWeAreSeeder(VictoryCenterDbContext dbContext, ILogger<WhoWeAreSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override int Order => (int)SeederExecutionOrder.WhoWeAre;
    public override string Name => nameof(WhoWeAreSeeder);

    protected override Task<List<WhoWeAreSection>> GenerateEntitiesAsync()
    {
        var sections = new List<WhoWeAreSection>
        {
            new()
            {
                SectionType = SectionType.Main,
                Title = "Main",
                CreatedAt = DateTime.UtcNow,
                Contents = new List<WhoWeAreContent>()
                {
                    new ImageContent()
                    {
                        ContentType = ContentType.Image,
                        ImageId = null,
                    },
                    new TitleContent()
                    {
                        ContentType = ContentType.Title,
                        Title = "A PLACE OF TRUST, CARE AND INNER STRENGTH",
                    },
                    new DescriptionContent()
                    {
                        ContentType = ContentType.Description,
                        Description =
                            "Victory Center is a space where people can pause, find calm, and reconnect with themselves through community, nature, and supportive activities.",
                    }
                },
            },
            new()
            {
                SectionType = SectionType.WhatWeDo,
                Title = "What We Do",
                CreatedAt = DateTime.UtcNow,
                Contents = new List<WhoWeAreContent>()
                {
                    new DescriptionContent()
                    {
                        ContentType = ContentType.Description,
                        Description =
                            "We design therapeutic programs that combine interaction with horses, body practices, time in nature, and professional guidance. Each program is tailored to the needs of the group.",
                    },
                }
            },
            new()
            {
                SectionType = SectionType.WhoWeSupport,
                Title = "Who We Support",
                CreatedAt = DateTime.UtcNow,
                Contents = new List<WhoWeAreContent>()
                {
                    new CardContent()
                    {
                        ContentType = ContentType.Card,
                        ImageId = null,
                        Description =
                            "Veterans returning from service who seek to restore balance and connection with family and themselves.",
                    },
                    new CardContent()
                    {
                        ContentType = ContentType.Card,
                        ImageId = null,
                        Description =
                            "Volunteers and civilians who need emotional recovery and want to continue helping others.",
                    },
                    new CardContent()
                    {
                        ContentType = ContentType.Card,
                        ImageId = null,
                        Description =
                            "Children affected by war, experiencing loss or displacement, who need a safe space to rebuild trust and confidence.",
                    },
                }
            },
            new()
            {
                SectionType = SectionType.Team,
                Title = "Team",
                CreatedAt = DateTime.UtcNow,
                Contents = new List<WhoWeAreContent>()
                {
                    new ImageContent()
                    {
                        ContentType = ContentType.Image,

                        ImageId = null
                    },
                    new DescriptionContent()
                    {
                        ContentType = ContentType.Description,
                        Description =
                            "Our team consists of professionals and volunteers dedicated to creating supportive and healing experiences.",
                    },
                }
            },
            new()
            {
                SectionType = SectionType.People,
                Title = "People",
                CreatedAt = DateTime.UtcNow,
                Contents = new List<WhoWeAreContent>()
                {
                    new CardContent()
                    {
                        ContentType = ContentType.Card,
                        ImageId = null,
                        Description = "Partners who share our vision and values.",
                    },
                    new CardContent()
                    {
                        ContentType = ContentType.Card,
                        ImageId = null,
                        Description = "Community members who contribute to our mission.",
                    },
                    new CardContent()
                    {
                        ContentType = ContentType.Card,
                        ImageId = null,
                        Description = "Volunteers who stand with us to provide support.",
                    },
                    new CardContent()
                    {
                        ContentType = ContentType.Card,
                        ImageId = null,
                        Description = "Donors who help turn our ideas into reality.",
                    },
                }
            },
        };

        return Task.FromResult(sections);
    }
}
