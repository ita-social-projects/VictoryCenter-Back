using AutoMapper;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Test;

public class TestProfile : Profile
{
    public TestProfile()
    {
        CreateMap<TestEntity, TestDataDto>();
        CreateMap<CreateTestDataDto, TestEntity>();
        CreateMap<UpdateTestDataDto, TestEntity>();
    }
}
