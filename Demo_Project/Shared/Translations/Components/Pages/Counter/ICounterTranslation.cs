namespace Shared.Translations.Components.Pages.Counter;

public interface ICounterTranslation : ITranslation
{
    string PageTitle { get; }
    string CurrentCount { get; }
    string ClickMe { get; }
}
