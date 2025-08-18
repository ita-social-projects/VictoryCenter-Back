using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.TeamPage;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.TeamPage.GetPublished;

public class GetPublishedTeamMembersHandler : IRequestHandler<GetPublishedTeamMembersQuery,
    Result<List<CategoryWithPublishedTeamMembersDto>>>
{
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetPublishedTeamMembersHandler(IMapper mapper, IRepositoryWrapper repository, IBlobService blobService)
    {
        _mapper = mapper;
        _repository = repository;
        _blobService = blobService;
    }

    public async Task<Result<List<CategoryWithPublishedTeamMembersDto>>> Handle(GetPublishedTeamMembersQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<Category>
        {
            Filter = category => category.TeamMembers.Any(member => member.Status == Status.Published),
            Include = categories => categories.Include(category => category.TeamMembers
                    .Where(member => member.Status == Status.Published)
                    .OrderBy(members => members.Priority))
                .ThenInclude(member => member.Image!)
        };

        IEnumerable<Category> categoriesWithPublishedMembers =
            await _repository.CategoriesRepository.GetAllAsync(queryOptions);

        var publishedCategoriesDto = _mapper
            .Map<IEnumerable<CategoryWithPublishedTeamMembersDto>>(categoriesWithPublishedMembers)
            .ToList();

        IEnumerable<Task> imageLoadTasks = publishedCategoriesDto
            .SelectMany(category => category.TeamMembers)
            .Where(teamMembers => teamMembers.Image is not null)
            .Select(async teamMember =>
            {
                try
                {
                    teamMember.Image!.Base64 =
                        await _blobService.FindFileInStorageAsBase64Async(teamMember.Image.BlobName, teamMember.Image.MimeType);
                }
                catch (BlobStorageException)
                {
                    teamMember.Image!.Base64 = string.Empty;
                }
            });
        await Task.WhenAll(imageLoadTasks);

        return Result.Ok(publishedCategoriesDto);
    }
}
