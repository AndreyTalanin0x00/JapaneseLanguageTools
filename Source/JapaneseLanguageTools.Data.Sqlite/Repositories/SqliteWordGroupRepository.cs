using System;

using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.Sqlite.Repositories;

public class SqliteWordGroupRepository : WordGroupRepository
{
    public SqliteWordGroupRepository(MainDbContext context, SqliteWordRepository wordRepository, TimeProvider timeProvider)
        : base(context, wordRepository, timeProvider)
    {
    }
}
