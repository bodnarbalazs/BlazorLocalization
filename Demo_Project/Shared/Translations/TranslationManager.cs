using System.Reflection;

namespace Shared.Translations;

public abstract class TranslationManager
{
    public Language Language { get; private set; }

    public abstract ITranslation Translation { get; }

    public void SetTranslation(Language language)
    {
        Language = language;

        // Get the type of the current TranslationManager (e.g., LoginTranslationManager)
        var managerType = this.GetType();
        
        // Get the type of the corresponding translation class based on the language
        var translationType = GetTranslationTypeForLanguage(managerType, language);

        if (translationType is null)
        {
            throw new InvalidOperationException("Translation type not found for the specified language.");
        }

        // Create an instance of the translation class
        var translationInstance = Activator.CreateInstance(translationType);

        // Use reflection to set the properties of the manager class
        foreach (var property in managerType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.CanWrite)
            {
                var translationProperty = translationType.GetProperty(property.Name);
                if (translationProperty != null)
                {
                    var value = translationProperty.GetValue(translationInstance);
                    property.SetValue(this, value);
                }
            }
            else
            {
                
                if (property.Name is not "Translation" && property.Name is not "Language")
                {
                    throw new Exception("Property can't be written. Are you missing a setter? (yes, you are)");
                }
            }
        }
    }

    private Type? GetTranslationTypeForLanguage(Type managerType, Language language)
    {
        // Assuming translation classes follow the pattern: [ManagerName]Translation[Language]
        var managerName = managerType.Name.Replace("TranslationManager", "");
        var translationClassName = $"{managerName}Translation{language}";
        
        // Search for the class in the same assembly
        var translationType = managerType.Assembly.GetTypes()
            .FirstOrDefault(t => t.Name == translationClassName);

        return translationType;
    }
}
