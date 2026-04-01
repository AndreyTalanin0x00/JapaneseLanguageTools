using System;
using System.Linq;

using Xunit;

using ImportStatus = JapaneseLanguageTools.Contracts.Models.Responses.Base.ImportStatus;
using ImportStatusSource = AndreyTalanin0x00.Integrations.Import.Enumerations.ImportStatus;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Contracts.UnitTests.Enumerations;

public class ImportStatusTests
{
    [Fact]
    public void ImportStatus_ValuesMirrorSourceEnumeration()
    {
        (int Value, string Name)[] targetValues = Enum.GetValues<ImportStatus>()
            .Select(targetValue => (Value: (int)targetValue, Name: targetValue.ToString()))
            .ToArray();

        (int Value, string Name)[] sourceValues = Enum.GetValues<ImportStatusSource>()
            .Select(targetValue => (Value: (int)targetValue, Name: targetValue.ToString()))
            .ToArray();

        Assert.Equal(sourceValues, targetValues);
    }
}
