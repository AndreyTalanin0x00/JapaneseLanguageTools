# Disclaimer

## JapaneseLanguageTools.Data.SqlServer

This project intentionally mixes a lot of approaches to work with a SQL Server database.

It uses multiple API-s:

- ADO.NET
- Entity Framework Core
- Dapper

Entity Framework Core is used for read-only data retrieval and `IQueryable<T>` filtering operations, migrations and connection management, while ADO.NET and Dapper are used for data modification where having an option to use Table-Valued Parameters can be beneficial.

It uses multiple techniques:

- ORM-generated SQL queries
- ORM-generated SQL queries on top of User-Defined Function results
- Stored Procedures
- SQL queries stored in C# strings

It passes parameters multiple different ways:

- ORM-generated parameter lists
- Collections serialized as `DataTable`
- Collections serialized as `IEnumerable<SqlDataRecord>`

This is done intentionally to provide examples of different approaches and technologies. It is **discouraged** to use multiple of them in a single production-ready application unless there is a strong reason, as it complicates maintenance down the line.

Using Entity Framework Core and Dapper together can be a valid use case for achieving both flexibility and performance, but the difference will only be noticeable on large data sets and this approach requires careful connection handling.

For specific examples, see the repositories:

| Repository Class                            | Technology                                | Query                                                    | Parameters                                               |
| ------------------------------------------- | ----------------------------------------- | -------------------------------------------------------- | -------------------------------------------------------- |
| `SqlServerTagRepository`                    | Entity Framework Core (ORM, `ExecuteSql`) | ORM-generated, SQL in C# strings                         | ORM-generated, Manual                                    |
| `SqlServerCharacterRepository`              | Entity Framework Core (ORM), Dapper       | ORM-generated, User-Defined Functions, Stored Procedures | ORM-generated, `IEnumerable<SqlDataRecord>`              |
| `SqlServerCharacterGroupRepository`         | Entity Framework Core (ORM), Dapper       | ORM-generated, User-Defined Functions, Stored Procedures | ORM-generated, `DataTable`, `IEnumerable<SqlDataRecord>` |
| `SqlServerCharacterExerciseRepository`      | Entity Framework Core (ORM), ADO.NET      | ORM-generated, SQL in C# strings                         | ORM-generated, `DataTable`                               |
| `SqlServerCharacterExerciseRerunRepository` | Entity Framework Core (ORM)               | ORM-generated                                            | ORM-generated                                            |
| `SqlServerWordRepository`                   | Entity Framework Core (ORM), Dapper       | ORM-generated, User-Defined Functions, Stored Procedures | ORM-generated, `IEnumerable<SqlDataRecord>`              |
| `SqlServerWordGroupRepository`              | Entity Framework Core (ORM), Dapper       | ORM-generated, User-Defined Functions, Stored Procedures | ORM-generated, `DataTable`, `IEnumerable<SqlDataRecord>` |
| `SqlServerWordExerciseRepository`           | Entity Framework Core (ORM), ADO.NET      | ORM-generated, SQL in C# strings                         | ORM-generated, `DataTable`                               |
| `SqlServerWordExerciseRerunRepository`      | Entity Framework Core (ORM)               | ORM-generated                                            | ORM-generated                                            |
