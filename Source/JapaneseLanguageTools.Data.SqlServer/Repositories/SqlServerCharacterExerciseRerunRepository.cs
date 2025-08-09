using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Repositories;

namespace JapaneseLanguageTools.Data.SqlServer.Repositories;

public class SqlServerCharacterExerciseRerunRepository : CharacterExerciseRerunRepository
{
    public SqlServerCharacterExerciseRerunRepository(MainDbContext context)
        : base(context)
    {
    }
}
