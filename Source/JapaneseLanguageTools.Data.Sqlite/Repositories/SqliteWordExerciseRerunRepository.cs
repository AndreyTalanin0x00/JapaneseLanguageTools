using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.Sqlite.Repositories;

public class SqliteWordExerciseRerunRepository : WordExerciseRerunRepository
{
    public SqliteWordExerciseRerunRepository(MainDbContext context)
        : base(context)
    {
    }
}
