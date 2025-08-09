using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.SqlServer.Repositories;

public class SqlServerWordExerciseRerunRepository : WordExerciseRerunRepository
{
    public SqlServerWordExerciseRerunRepository(MainDbContext context)
        : base(context)
    {
    }
}
