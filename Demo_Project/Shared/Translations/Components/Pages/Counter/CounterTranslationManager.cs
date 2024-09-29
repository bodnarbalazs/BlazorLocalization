namespace Shared.Translations.Components.Pages.Counter;

public class CounterTranslationManager : TranslationManager, ICounterTranslation
{
    public override ITranslation Translation => this;

    public string PageTitle { get; set; }
    public string CurrentCount { get; set; }
    public string ClickMe { get; set; }
}
