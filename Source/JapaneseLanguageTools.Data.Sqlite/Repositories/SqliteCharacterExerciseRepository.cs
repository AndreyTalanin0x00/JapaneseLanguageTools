using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.Sqlite.Repositories;

public class SqliteCharacterExerciseRepository : CharacterExerciseRepository
{
    public SqliteCharacterExerciseRepository(MainDbContext context, SqliteCharacterExerciseRerunRepository characterExerciseRerunRepository)
        : base(context, characterExerciseRerunRepository)
    {
    }
}
