# Linked list approach to TeamMember management in VC

## Basic entity structure proposed:

```
public class TeamMember
{
    [Key]
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? Description { get; set; }

#pragma warning disable SA1011
    public long? ImageId { get; set; }
#pragma warning restore SA1011

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }

    // LinkedList reference
    public int? AfterMemberId { get; set; }

    public Category Category { get; set; } = null!;
    public TeamMember AfterTeamMember { get; set; } = null!;
}
```

## Setup

```
modelBuilder.Entity<TeamMember>()
            .HasOne(p => p.Category)
            .WithMany(c => c.TeamMembers)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<TeamMember>()
            .HasOne(p => p.AfterTeamMember)
            .WithOne()
            .HasForeignKey<TeamMember>(p => p.AfterMemberId)
            .OnDelete(DeleteBehavior.NoAction);

```

## Migration Script:


```
BEGIN TRANSACTION;
GO

CREATE TABLE [TeamMembers] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(max) NOT NULL,
    [CategoryId] int NOT NULL,
    [Description] nvarchar(max) NULL,
    [ImageId] bigint NULL,
    [Email] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [AfterMemberId] int NULL,
    CONSTRAINT [PK_TeamMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeamMembers_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeamMembers_TeamMembers_AfterMemberId] FOREIGN KEY ([AfterMemberId]) REFERENCES [TeamMembers] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_TeamMembers_AfterMemberId] ON [TeamMembers] ([AfterMemberId]) WHERE [AfterMemberId] IS NOT NULL;
GO

CREATE INDEX [IX_TeamMembers_CategoryId] ON [TeamMembers] ([CategoryId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250907132438_teammemberlinked', N'8.0.0');
GO

COMMIT;
GO


BEGIN TRANSACTION;
GO

ALTER TABLE [TeamMembers] DROP CONSTRAINT [FK_TeamMembers_TeamMembers_AfterMemberId];
GO

ALTER TABLE [TeamMembers] ADD CONSTRAINT [FK_TeamMembers_TeamMembers_AfterMemberId] FOREIGN KEY ([AfterMemberId]) REFERENCES [TeamMembers] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250907133430_teammemberlinked', N'8.0.0');
GO

COMMIT;
GO
```

## SaveChangesInterceptor Approach 

```
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class TeamMemberOrderInterceptor : SaveChangesInterceptor
{
    private readonly AppDbContext _context;

    public TeamMemberOrderInterceptor(AppDbContext context)
    {
        _context = context;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context ?? _context;

        var changedMembers = context.ChangeTracker.Entries<TeamMember>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .Select(e => e.Entity.CategoryId)
            .Distinct()
            .ToList();

        foreach (var categoryId in changedMembers)
        {
            var isValid = await ValidateOrderAsync(context, categoryId);
            if (!isValid)
            {
                throw new InvalidOperationException(
                    $"Invalid order detected in category {categoryId}: cycle or broken chain.");
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task<bool> ValidateOrderAsync(DbContext context, long categoryId)
    {
        var members = await context.Set<TeamMember>()
            .Where(m => m.CategoryId == categoryId)
            .ToListAsync();

        if (!members.Any())
            return true;

        var startNodes = members.Where(m => m.AfterMemberId == null).ToList();
        if (startNodes.Count != 1)
            return false;

        var visited = new HashSet<long>();
        var current = startNodes.First();

        while (current != null)
        {
            if (visited.Contains(current.Id))
                return false; 

            visited.Add(current.Id);

            current = members.FirstOrDefault(m => m.AfterMemberId == current.Id);
        }

        return visited.Count == members.Count;
    }
}
```

Setup Interceptor in DbContext

```
public class VictoryCenterDbContext : DbContext
{
    private readonly TeamMemberOrderInterceptor _orderInterceptor;

    public VictoryCenterDbContext(DbContextOptions<VictoryCenterDbContext> options,
                        TeamMemberOrderInterceptor orderInterceptor)
        : base(options)
    {
        _orderInterceptor = orderInterceptor;
    }

    public DbSet<TeamMember> TeamMembers { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_orderInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}
```

In Program.cs:

```
builder.Services.AddScoped<TeamMemberOrderInterceptor>();
```