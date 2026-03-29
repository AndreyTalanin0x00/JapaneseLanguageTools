using System;
using System.Linq;

using Xunit;

using ImportResponseMessageType = JapaneseLanguageTools.Contracts.Models.Responses.Base.ImportResponseMessageType;
using ImportResponseMessageTypeSource = AndreyTalanin0x00.Integrations.Import.Responses.ImportResponseMessageType;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Contracts.UnitTests.Enumerations;

public class ImportResponseMessageTypeTests
{
    [Fact]
    public void ImportResponseMessageType_ValuesMirrorSourceEnumeration()
    {
        (int Value, string Name)[] targetValues = Enum.GetValues<ImportResponseMessageType>()
            .Select(targetValue => (Value: (int)targetValue, Name: targetValue.ToString()))
            .ToArray();

        (int Value, string Name)[] sourceValues = Enum.GetValues<ImportResponseMessageTypeSource>()
            .Select(targetValue => (Value: (int)targetValue, Name: targetValue.ToString()))
            .ToArray();

        Assert.Equal(sourceValues, targetValues);
    }
}
