namespace Shared.Translations;

public static class LanguageExtensions
{
    public static Language? ToLanguage(this string languageString)
    {
        return languageString switch
        {
            "en-US" => Language.English,
            "es-ES" => Language.Spanish,
            "hu-HU" => Language.Hungarian,
            _ => null
        };
    }
}