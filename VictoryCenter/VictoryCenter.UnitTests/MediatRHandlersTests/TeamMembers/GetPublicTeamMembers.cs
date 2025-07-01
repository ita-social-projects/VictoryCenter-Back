using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Queries.TeamMembers.GetPublic;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class GetPublicTeamMembers
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;

    public GetPublicTeamMembers()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnPublicTeamMembersSuccessfully()
    {
        // Arrange
        var categories = GetCategoriesWithTeamMembers();
        var expectedDto = GetPublicCategoryWithTeamMembersDtoList();

        SetupRepository(categories);
        SetupMapper(expectedDto);

        var handler = new GetPublicTeamMembersHandler(_mockMapper.Object, _mockRepository.Object);
        var query = new GetPublicTeamMembersQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedDto.Count, result.Value.Count);
        Assert.Equal(expectedDto, result.Value);
    }

    private static List<Category> GetCategoriesWithTeamMembers()
    {
        return
        [
            new Category
            {
                Id = 1,
                Name = "Cool category 1",
                Description = "This is a cool group of a few guys",
                TeamMembers = new List<TeamMember>
                {
                    new()
                    {
                        Id = 1,
                        FullName = "John Doe",
                        Description = "Senior Developer",
                        Priority = 1,
                        Status = Status.Published,
                        CategoryId = 1
                    },
                    new()
                    {
                        Id = 2,
                        FullName = "Jane Smith",
                        Description = "Frontend Developer",
                        Priority = 2,
                        Status = Status.Published,
                        CategoryId = 1
                    }
                }
            },

            new Category
            {
                Id = 2,
                Name = "Cool category 2",
                Description = "This is another cool group of a few guys",
                TeamMembers = new List<TeamMember>
                {
                    new()
                    {
                        Id = 3,
                        FullName = "Mike Johnson",
                        Description = "UI Designer",
                        Priority = 1,
                        Status = Status.Published,
                        CategoryId = 2
                    }
                }
            }

        ];
    }

    private static List<PublicCategoryWithTeamMembersDto> GetPublicCategoryWithTeamMembersDtoList()
    {
        return
        [
            new PublicCategoryWithTeamMembersDto
            {
                Id = 1,
                CategoryName = "Cool category 1",
                Description = "This is a cool group of a few guys",
                TeamMembers =
                [
                    new PublicTeamMemberDto
                    {
                        Id = 1,
                        FullName = "John Doe",
                        Description = "Senior Developer"
                    },

                    new PublicTeamMemberDto
                    {
                        Id = 2,
                        FullName = "Jane Smith",
                        Description = "Frontend Developer"
                    }

                ]
            },
            new PublicCategoryWithTeamMembersDto
            {
                Id = 2,
                CategoryName = "Cool category 2",
                Description = "This is another cool group of a few guys",
                TeamMembers =
                [
                    new PublicTeamMemberDto
                    {
                        Id = 3,
                        FullName = "Mike Johnson",
                        Description = "UI Designer"
                    }

                ]
            }

        ];
    }

    private void SetupRepository(List<Category> categories)
    {
        _mockRepository.Setup(repositoryWrapper => repositoryWrapper.CategoriesRepository.GetAllAsync(
             It.IsAny<QueryOptions<Category>>()))
            .ReturnsAsync(categories);
    }

    private void SetupMapper(List<PublicCategoryWithTeamMembersDto> expectedDto)
    {
        _mockMapper
            .Setup(x => x.Map<IEnumerable<PublicCategoryWithTeamMembersDto>>(It.IsAny<IEnumerable<Category>>()))
            .Returns(expectedDto);
    }
}
