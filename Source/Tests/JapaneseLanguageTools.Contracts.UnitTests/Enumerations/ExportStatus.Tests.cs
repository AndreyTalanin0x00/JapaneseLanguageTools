using System;
using System.Linq;

using Xunit;

using ExportStatus = JapaneseLanguageTools.Contracts.Models.Responses.Base.ExportStatus;
using ExportStatusSource = AndreyTalanin0x00.Integrations.Export.Enumerations.ExportStatus;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Contracts.UnitTests.Enumerations;

public class ExportStatusTests
{
    [Fact]
    public void ExportStatus_ValuesMirrorSourceEnumeration()
    {
        (int Value, string Name)[] targetValues = Enum.GetValues<ExportStatus>()
            .Select(targetValue => (Value: (int)targetValue, Name: targetValue.ToString()))
            .ToArray();

        (int Value, string Name)[] sourceValues = Enum.GetValues<ExportStatusSource>()
            .Select(targetValue => (Value: (int)targetValue, Name: targetValue.ToString()))
            .ToArray();

        Assert.Equal(sourceValues, targetValues);
    }
}
