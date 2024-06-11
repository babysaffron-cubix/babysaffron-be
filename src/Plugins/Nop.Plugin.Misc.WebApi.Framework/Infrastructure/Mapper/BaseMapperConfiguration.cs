using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper;

public abstract class BaseMapperConfiguration : Profile, IOrderedMapperProfile
{
    #region Utilities

    /// <summary>
    /// Creates an Entity/DTO mapping configuration from the <typeparamref name="TEntity"/> type to the <typeparamref name="TDto"/> type
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TDto">DTO type</typeparam>
    /// <param name="entityMappingExpression">Entity mapping configuration options</param>
    /// <param name="dtoMappingExpression">DTO mapping configuration options</param>
    protected virtual void CreateDtoMap<TEntity, TDto>(Action<IMappingExpression<TDto, TEntity>> entityMappingExpression = null, Action<IMappingExpression<TEntity, TDto>> dtoMappingExpression = null)
        where TDto : BaseDto
    {
        var dtoMap = CreateMap<TEntity, TDto>();
        dtoMappingExpression?.Invoke(dtoMap);

        var entityMap = CreateMap<TDto, TEntity>();
        entityMappingExpression?.Invoke(entityMap);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Order of this mapper implementation
    /// </summary>
    public int Order => 0;

    #endregion
}