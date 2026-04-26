# JapaneseLanguageTools

A "kanji card" application for learning Japanese language: Hiragana, Katakana, Kanji hieroglyphs, numbers and words.

## Cloning

This repository contains Git submodules, so it is required to clone the repository with the `--recursive` flag:

```bash
git clone --recurse-submodules <this-repository-url>
```

If the repository was cloned without the `--recursive` flag, initialize and update the submodules with the following commands:

```bash
git submodule update --init --recursive
```

## Building and Running

The project can be built as a regular ASP.NET Core application. Use the following command to build the project:

```bash
cd ./Source
dotnet build ./JapaneseLanguageTools.sln
```

Before running the application, create a Microsoft SQL Server or SQLite database and update the connection string in the `appsettings.json` file located in the `JapaneseLanguageTools` project.

The database provider is configured by setting the `JapaneseLanguageTools.Data` pluggable assembly to either `JapaneseLanguageTools.Data.Sqlite` or `JapaneseLanguageTools.Data.SqlServer`. Set the desired provider by updating the `appsettings.json` file:

```json
{
  "PluggableAssemblies": {
    "JapaneseLanguageTools.Data": "JapaneseLanguageTools.Data.Sqlite"
  }
}
```

After creating the database, apply migrations to create the necessary tables. The `dotnet-ef` command-line tool is required to apply migrations. Install it globally with the following command:

```bash
dotnet tool install --global dotnet-ef
```

Apply the latest migration to the SQLite database with the following commands:

```bash
cd ./Source/JapaneseLanguageTools.Data.Sqlite
dotnet ef database update WordExerciseMigration \
  --context SqliteMainDbContext \
  --startup-project ../JapaneseLanguageTools/JapaneseLanguageTools.csproj \
  --project JapaneseLanguageTools.Data.Sqlite.csproj
```

... or for Microsoft SQL Server:

```bash
cd ./Source/JapaneseLanguageTools.Data.SqlServer
dotnet ef database update WordExerciseMigration \
  --context SqlServerMainDbContext \
  --startup-project ../JapaneseLanguageTools/JapaneseLanguageTools.csproj \
  --project JapaneseLanguageTools.Data.SqlServer.csproj
```

With a database created and migrations applied, run the application with the following command:

```bash
dotnet run --project ./JapaneseLanguageTools/JapaneseLanguageTools.csproj
```

The React.js client application starts automatically as a child Node.js process. Access the application at `http://localhost:5176` in a web browser.

## Importing Application Dictionary Data

Navigate to the `Integrations / Application Dictionary / Export` page and export the template. The template contains XML comments with sample XML nodes for importing dictionary data. The XML file should be structured as follows:

```xml
<?xml version="1.0" encoding="utf-16"?>
<ApplicationDictionaryObjectPackage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" SnapshotType="GeneralNoAction">
  <SnapshotTime>2026-04-26T19:56:21</SnapshotTime>
  <SnapshotHash>0000000000000000000000000000000000000000000000000000000000000000</SnapshotHash>
  <ApplicationDictionary>
    <!-- Snapshot Object Actions: None, Add, Update, ChangeState, Remove. -->
    <Characters>
      <!-- Kana Template: <Character Action="Add" Symbol="" Type="Katakana" Pronunciation="" Syllable="" /> -->
      <!-- Kana Template: <Character Action="Add" Symbol="" Type="Hiragana" Pronunciation="" Syllable="" /> -->
      <!-- Kanji Template: <Character Action="Add" Symbol="" Type="Kanji" Onyomi="" Kunyomi="" Meaning="" Tags="" /> -->
      <!-- Kanji Template: <Character Action="Add" Symbol="" Type="Kanji" Onyomi="" Kunyomi="" Meaning="" /> -->
    </Characters>
    <CharacterGroups>
      <!-- Template: <CharacterGroup Action="Add" Caption="" Enabled="true" AlwaysUse="false" Hidden="false"><Comment /><Characters /></CharacterGroup> -->
    </CharacterGroups>
    <Words>
      <!-- Kana Template: <Word Action="Add" Characters="" CharacterTypes="Katakana" Pronunciation="" Meaning="" /> -->
      <!-- Kana Template: <Word Action="Add" Characters="" CharacterTypes="Hiragana" Pronunciation="" Meaning="" /> -->
      <!-- Kanji Template: <Word Action="Add" Characters="" CharacterTypes="Kanji" Pronunciation="" Furigana="" Okurigana="" Meaning="" Tags="" /> -->
      <!-- Kanji Template: <Word Action="Add" Characters="" CharacterTypes="Kanji" Pronunciation="" Furigana="" Okurigana="" Meaning="" /> -->
    </Words>
    <WordGroups>
      <!-- Template: <WordGroup Action="Add" Caption="" Enabled="true" AlwaysUse="false" Hidden="false"><Comment /><Words /></WordGroup> -->
    </WordGroups>
    <Tags>
      <!-- Template: <Tag Action="Add" Caption="" /> -->
    </Tags>
  </ApplicationDictionary>
  <ForceMode>false</ForceMode>
</ApplicationDictionaryObjectPackage>
```

Then, navigate to the `Integrations / Application Dictionary / Import` page and import the created XML file. The application will process the file and update the database accordingly.

Navigate to the `Exercises / Characters` or `Exercise / Words` pages to create a test exercise and verify that the imported characters are available.

## Configuring Exercise Profiles

Navigate to the `Preferences / Character Exercises` or `Preferences / Word Exercises` page to create and manage exercise profiles.

Configure the repeated challenge progression settings to determine how many times a failed challenge will be presented before it is considered "mastered" and removed from the database. Each consecutive failure will reschedule the challenge with a larger number of required repetitions if multiple numbers are specified.

Configure how many characters or words are used in each exercise batch using floating-point fractions, min, or max values. Setting both min and max to the same number results in a fixed number of tag inclusions per exercise batch.

The default exercise profile and parameters like exercise batch size may also be configured on the same page.

## Performing Exercises

Navigate to the `Exercises / Characters` or `Exercise / Words` page to start an exercise. After pressing the `Generate` button, a batch of characters or words is presented with several fields hidden according to the selected exercise mode.

Press the `Complete` or `Fail` button to mark the exercise as completed or failed and display the hidden fields for self-assessment.

After the entire batch is either completed or failed, the results will be submitted to the database.

## Screenshots

[See screenshots of the React.js client application.](./Screenshots/README.md)
