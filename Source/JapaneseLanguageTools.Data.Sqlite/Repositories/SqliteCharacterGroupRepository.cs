using System;

using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.Sqlite.Repositories;

public class SqliteCharacterGroupRepository : CharacterGroupRepository
{
    public SqliteCharacterGroupRepository(MainDbContext context, SqliteCharacterRepository characterRepository, TimeProvider timeProvider)
        : base(context, characterRepository, timeProvider)
    {
    }
}
