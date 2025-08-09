using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.Sqlite.Repositories;

public class SqliteCharacterExerciseRerunRepository : CharacterExerciseRerunRepository
{
    public SqliteCharacterExerciseRerunRepository(MainDbContext context)
        : base(context)
    {
    }
}
