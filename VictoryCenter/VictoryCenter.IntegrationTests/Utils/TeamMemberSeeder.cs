using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils;

internal static class TeamMemberSeeder
{
    private static readonly List<TeamMember> _testTeamMembers =
    [
        new () {
            FirstName = "FirstName1", LastName="LastName1", MiddleName = "MiddleName1",
            CategoryId = 0, Priority = 1, Status = Status.Draft, CreatedAt = DateTime.Now.AddDays(-1) },
        new () {
            FirstName = "FirstName2", LastName="LastName2", MiddleName = "MiddleName2",
            CategoryId = 0, Priority = 2, Status = Status.Draft, CreatedAt = DateTime.Now.AddDays(-2) },
        new () {
            FirstName = "FirstName3", LastName="LastName3", MiddleName = "MiddleName3",
            CategoryId = 0, Priority = 3, Status = Status.Draft, CreatedAt = DateTime.Now.AddDays(-3) }
    ];

    public static void SeedTeamMembers(VictoryCenterDbContext dbContext, List<Category> categories)
    {
        for (int i = 0; i < _testTeamMembers.Count; i++)
        {
            _testTeamMembers[i].CategoryId = categories[i % categories.Count].Id;
        }
        dbContext.AddRange(_testTeamMembers);
        dbContext.SaveChanges();
    }

    public static void DeleteExistingTeamMembers(VictoryCenterDbContext dbContext)
    {
        dbContext.TeamMembers.RemoveRange(dbContext.TeamMembers);
        dbContext.SaveChanges();
    }
}
