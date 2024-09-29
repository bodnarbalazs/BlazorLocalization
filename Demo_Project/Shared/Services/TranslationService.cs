using System.Globalization;
using Shared.Translations;

namespace Shared.Services;

public class TranslationService<TTranslationManager> where TTranslationManager : TranslationManager, new()
{
    private readonly TTranslationManager _translationManager;

    public TranslationService()
    {
        _translationManager = new TTranslationManager(); 
        Console.WriteLine($"culture: {CultureInfo.CurrentCulture.Name}");
        SetLanguage(CultureInfo.CurrentCulture.Name.ToLanguage()??Language.English);
    }

    public TTranslationManager Translations => _translationManager;

    public void SetLanguage(Language language)
    {
        _translationManager.SetTranslation(language);
    }
}




