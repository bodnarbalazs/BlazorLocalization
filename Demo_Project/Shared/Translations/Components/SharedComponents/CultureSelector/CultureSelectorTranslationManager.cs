namespace Shared.Translations.Components.SharedComponents.CultureSelector;

public class CultureSelectorTranslationManager : TranslationManager, ICultureSelectorTranslation
{
    public override ITranslation Translation => this;

    public string Selectlocale { get; set; }
}
