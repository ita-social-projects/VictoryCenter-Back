using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.ServiceTests;

public class StorageReferenceQueryTranslationTests
{
    [Theory]
    [InlineData(StorageFileKind.Image, "image.jpg")]
    [InlineData(StorageFileKind.Image, "image.png")]
    [InlineData(StorageFileKind.Image, "image.webp")]
    [InlineData(StorageFileKind.Pdf, "report.pdf")]
    public async Task References_TranslateEqualsPredicateBeforeOpeningSqlConnection(StorageFileKind kind, string name)
    {
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer("Server=unused;Database=QueryTranslationOnly;Integrated Security=True")
            .AddInterceptors(new BlockConnection()).Options;
        await using var context = new VictoryCenterDbContext(options);

        // Translation failures occur before this interceptor; no SQL connection is opened.
        await Assert.ThrowsAsync<ConnectionBlockedException>(() => StorageReferences.ExistsAsync(context, kind, name, CancellationToken.None));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Protection_TranslatesOptionalExclusionAndTemporaryOwner(bool excludeCurrent, bool temporary)
    {
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer("Server=unused;Database=QueryTranslationOnly;Integrated Security=True")
            .AddInterceptors(new BlockConnection()).Options;
        await using var context = new VictoryCenterDbContext(options);
        var name = temporary ? $".image.png.{Guid.NewGuid():N}.tmp" : "image.png";
        Guid? excluded = excludeCurrent ? Guid.NewGuid() : null;

        await Assert.ThrowsAsync<ConnectionBlockedException>(() => StorageReferences.IsProtectedAsync(
            context, StorageFileKind.Image, name, CancellationToken.None, excluded));
    }

    [Theory]
    [InlineData(StorageFileKind.Image, false)]
    [InlineData(StorageFileKind.Image, true)]
    [InlineData(StorageFileKind.Pdf, false)]
    [InlineData(StorageFileKind.Pdf, true)]
    public async Task BatchQueries_TranslateFileCollectionsAndTemporaryOwners(StorageFileKind kind, bool temporary)
    {
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer("Server=unused;Database=QueryTranslationOnly;Integrated Security=True")
            .AddInterceptors(new BlockConnection()).Options;
        await using var context = new VictoryCenterDbContext(options);
        var names = temporary ? new[] { $".report.v2.pdf.{Guid.NewGuid():N}.tmp", "other.png" } : new[] { "image.png", "report.pdf" };

        await Assert.ThrowsAsync<ConnectionBlockedException>(() => StorageReferences.ReferencedNames(context, kind, names).ToListAsync());
        await Assert.ThrowsAsync<ConnectionBlockedException>(() => StorageReferences.ProtectedNamesAsync(context, kind, names, CancellationToken.None));
    }

    private sealed class BlockConnection : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            throw new ConnectionBlockedException();
        }
    }

    private sealed class ConnectionBlockedException : Exception
    {
    }
}
