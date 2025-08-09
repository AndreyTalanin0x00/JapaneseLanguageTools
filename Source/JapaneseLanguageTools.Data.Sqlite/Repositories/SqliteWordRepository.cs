using System;

using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.Sqlite.Repositories;

public class SqliteWordRepository : WordRepository
{
    public SqliteWordRepository(MainDbContext context, TimeProvider timeProvider)
        : base(context, timeProvider)
    {
    }
}
