namespace Shared.Translations.Components.Pages.Home;

public class HomeTranslationManager : TranslationManager, IHomeTranslation
{
    public override ITranslation Translation => this;

    public string Pagetitle { get; set; }
    public string Greetingmessage { get; set; }
    public string Helloworld { get; set; }
}
