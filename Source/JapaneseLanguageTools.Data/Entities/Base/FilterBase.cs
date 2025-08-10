using System.Linq;

namespace JapaneseLanguageTools.Data.Entities.Base;

public class FilterBase<TEntity>
    where TEntity : class
{
    public CustomFilter<TEntity>? CustomFilter { get; set; }

    public virtual IQueryable<TEntity> Filter(IQueryable<TEntity> entities)
    {
        if (CustomFilter is not null)
        {
            entities = CustomFilter.Invoke(entities);
        }

        return entities;
    }
}

public delegate IQueryable<TEntity> CustomFilter<TEntity>(IQueryable<TEntity> entities) where TEntity : class;
