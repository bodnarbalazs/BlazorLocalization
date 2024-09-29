namespace Shared.Translations.Layout.NavMenu;

public class NavMenuTranslationManager : TranslationManager, INavMenuTranslation
{
    public override ITranslation Translation => this;

    public string Home { get; set; }
    public string Counter { get; set; }
}
