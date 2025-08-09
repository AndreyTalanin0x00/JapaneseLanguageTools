using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.Sqlite.Repositories;

public class SqliteWordExerciseRepository : WordExerciseRepository
{
    public SqliteWordExerciseRepository(MainDbContext context, SqliteWordExerciseRerunRepository wordExerciseRerunRepository)
        : base(context, wordExerciseRerunRepository)
    {
    }
}
