using System;
using System.Linq;

using Xunit;

using ExportResponseMessageType = JapaneseLanguageTools.Contracts.Models.Responses.Base.ExportResponseMessageType;
using ExportResponseMessageTypeSource = AndreyTalanin0x00.Integrations.Export.Responses.ExportResponseMessageType;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Contracts.UnitTests.Enumerations;

public class ExportResponseMessageTypeTests
{
    [Fact]
    public void ExportResponseMessageType_ValuesMirrorSourceEnumeration()
    {
        (int Value, string Name)[] targetValues = Enum.GetValues<ExportResponseMessageType>()
            .Select(targetValue => (Value: (int)targetValue, Name: targetValue.ToString()))
            .ToArray();

        (int Value, string Name)[] sourceValues = Enum.GetValues<ExportResponseMessageTypeSource>()
            .Select(targetValue => (Value: (int)targetValue, Name: targetValue.ToString()))
            .ToArray();

        Assert.Equal(sourceValues, targetValues);
    }
}
