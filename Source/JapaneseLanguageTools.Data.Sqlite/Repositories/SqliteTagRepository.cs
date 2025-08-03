using System;

using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.Sqlite.Repositories;

public class SqliteTagRepository : TagRepository
{
    public SqliteTagRepository(MainDbContext context, TimeProvider timeProvider)
        : base(context, timeProvider)
    {
    }
}
