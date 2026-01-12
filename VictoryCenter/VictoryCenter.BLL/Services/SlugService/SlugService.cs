using Slugify;
using VictoryCenter.BLL.Interfaces.SlugService;

namespace VictoryCenter.BLL.Services.SlugService;

public class SlugService : ISlugService
{
    private readonly ISlugHelper _slugHelper;

    public SlugService(ISlugHelper slugHelper)
    {
        _slugHelper = slugHelper;
    }

    public string GenerateSlug(string source)
        => _slugHelper.GenerateSlug(source);
}
