using AutoMapper;
using CommonObjects;
using InvoicesDatabase;

namespace InvoicesApi.Mappers;

public static class InvoiceMapper {

    public static TDto ToDto<TDto>(this AbstractEntity entity, IMapper mapper)
        where TDto : AbstractDto
        => mapper.Map<TDto>(entity);

    public static TEntity ToEntity<TEntity>(this AbstractDto dto, IMapper mapper)
        where TEntity : AbstractEntity
        => mapper.Map<TEntity>(dto);

    public static IEnumerable<TDto> ToDtos<TDto>(this IEnumerable<AbstractEntity> entities, IMapper mapper)
        where TDto : AbstractDto
        => mapper.Map<IEnumerable<TDto>>(entities);

    public static IEnumerable<TEntity> ToEntities<TEntity>(this IEnumerable<AbstractDto> dtos, IMapper mapper)
        where TEntity : AbstractEntity
        => mapper.Map<IEnumerable<TEntity>>(dtos);
}

