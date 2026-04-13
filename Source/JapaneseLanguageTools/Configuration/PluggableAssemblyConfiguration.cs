using System.Collections.Generic;

namespace JapaneseLanguageTools.Configuration;

public class PluggableAssemblyConfiguration
{
    public static readonly string SectionName = "PluggableAssemblies";

    public IDictionary<string, string> PluggableAssemblies = new Dictionary<string, string>();
}
