using System;

using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.Sqlite.Repositories;

public class SqliteCharacterRepository : CharacterRepository
{
    public SqliteCharacterRepository(MainDbContext context, TimeProvider timeProvider)
        : base(context, timeProvider)
    {
    }
}
