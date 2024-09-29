 # BlazorLocalization
 
 An alternative localization approach for Blazor applications using CSV files, providing IntelliSense support within components and seamless integration with Blazor WebAssembly.
 
 ## Table of Contents
 
 - [Introduction](#introduction)
 - [Features](#features)
 - [Pros and Cons](#pros-and-cons)
 - [Getting Started](#getting-started)
   - [Prerequisites](#prerequisites)
   - [Setup Instructions](#setup-instructions)
   - [Creating Translation Files](#creating-translation-files)
   - [Generating Boilerplate Code](#generating-boilerplate-code)
 - [Usage](#usage)
   - [Server-Side Configuration](#server-side-configuration)
   - [Client-Side Configuration](#client-side-configuration)
   - [Using Translations in Components](#using-translations-in-components)
 - [Technical Explanation](#technical-explanation)
   - [Overview](#overview)
   - [Key Components](#key-components)
   - [Translation Workflow](#translation-workflow)
 - [Contributing](#contributing)
 - [License](#license)
 
 ## Introduction
 
 This repository showcases an alternative to the conventional localization approach in Blazor applications, especially when working with Blazor WebAssembly. The standard method using resource files (`.resx`) may not always work seamlessly with WebAssembly. This project introduces a custom localization strategy that:
 
 - Utilizes CSV files for managing translations.
 - Provides type-safe access to translations with IntelliSense support.
 - Works effectively on both Blazor Server and Blazor WebAssembly.
 
 ## Features
 
 - **CSV-Based Localization**: Manage translations using CSV files, which are easy to edit and maintain.
 - **IntelliSense Support**: Access translations with full IntelliSense support, eliminating magic strings and reducing typos.
 - **WebAssembly Compatibility**: Fully compatible with Blazor WebAssembly applications.
 - **Automated Code Generation**: Use the provided code generator to automate the creation of boilerplate code.
 
 ## Pros and Cons
 
 ### Pros
 
 - **Ease of Editing**: CSV files are straightforward to edit, even for non-developers.
 - **Type Safety**: Strongly-typed access to translations reduces runtime errors.
 - **WebAssembly Support**: Overcomes limitations of the standard localization approach in WebAssembly.
 
 ### Cons
 
 - **Initial Setup Time**: The setup process may take around 30 minutes.
 - **Additional Code**: Generates extra code, which may increase project size.
 - **CSV Limitations**: Lacks some features of resource files, like pluralization or advanced localization tools.
 
 ## Getting Started
 
 ### Prerequisites
 
 - [.NET 8 SDK](https:dotnet.microsoft.com/download/dotnet/8.0) or later.
 - Basic understanding of Blazor (Server and WebAssembly) applications.
 - Familiarity with C# and .NET localization concepts.
 
 ### Setup Instructions
 
 1. **Clone the Repository**
 
    ```bash
    git clone https:github.com/yourusername/BlazorLocalization.git
    ```
 
 2. **Create the Translations Folder**
 
    In your shared project (commonly named `Shared`), create a `Translations` directory:
 
    ```
    Shared/
    └── Translations/
    ```
 
 3. **Add Base Translation Files**
 
    Place the following files into the `Translations` folder:
 
    - `ITranslation.cs`
    - `Language.cs`
    - `LanguageExtensions.cs`
    - `TranslationManager.cs`
 
    These files define the core interfaces and classes for the localization system.
 
 4. **Add the Translation Service**
 
    Add `TranslationService.cs` to your services directory or where you manage your services.
 
 5. **Add the Culture Controller**
 
    In your server project, add `CultureController.cs` within the `Controllers` folder. This controller handles culture changes from the client side.
 
 ### Creating Translation Files
 
 1. **Mirror Component Structure**
 
    Replicate your component folder structure inside the `Translations` folder. For example, if you have a component at `Components/Pages/Counter.razor`, create the following structure:
 
    ```
    Shared/
    └── Translations/
        └── Components/
            └── Pages/
                └── Counter/
                    └── Counter.csv
    ```
 
 2. **Create CSV Files**
 
    The CSV file should be named after your component (e.g., `Counter.csv` or `CounterTranslations.csv`) and follow this format:
 
    - The first row is the header, starting with `property_name`, followed by language names.
    - Subsequent rows contain the property names and their corresponding translations.
 
    **Example `Counter.csv`:**
 
    ```csv
    property_name;English;Spanish;Hungarian
    Page_title;Counter;Contador;Számláló
    Current_count;Current count;Recuento actual;Jelenlegi állás
    Click_me;Click me;Haz clic en mí;Kattints rám
    ```
 
    **Important Notes:**
 
    - Use `;` as the delimiter.
 
 ### Generating Boilerplate Code
 
 To automate the creation of necessary classes and interfaces from your CSV files, use the provided code generator.
 
 1. **Build the Code Generator**
 
    Navigate to the `Boilerplate_Generator` project and build it:
 
    ```bash
    cd Boilerplate_Generator
    dotnet build
    ```
 
 2. **Run the Code Generator**
 
    Execute the generator, specifying the path to your `Translations` folder:
 
    ```bash
    dotnet run -- <path_to_translations_folder>
    ```
 
    **Example:**
 
    ```bash
    dotnet run -- ../Shared/Translations
    ```
 
    The generator will process all CSV files in the `Translations` directory and generate the necessary code files in their respective folders.
 
 ## Usage
 
 ### Server-Side Configuration
 
 In your server project's `Program.cs`:
 
 #### Enable Blazor Components and WebAssembly Support
 
 ```csharp
 builder.Services.AddRazorComponents()
     .AddInteractiveServerComponents()
     .AddInteractiveWebAssemblyComponents();
 ```
 
 #### Register Controllers
 
 ```csharp
 builder.Services.AddControllers();
 app.MapControllers();
 ```
 
 #### Register the Translation Service
 
 ```csharp
 builder.Services.AddScoped(typeof(TranslationService<>));
 ```
 
 #### Add Localization Services
 
 ```csharp
 builder.Services.AddLocalization();
 ```
 
 #### Configure Supported Cultures
 
 ```csharp
 var supportedCultures = new[] { "en-US", "es-ES", "hu-HU" };
 var localizationOptions = new RequestLocalizationOptions()
     .SetDefaultCulture(supportedCultures[0])
     .AddSupportedCultures(supportedCultures)
     .AddSupportedUICultures(supportedCultures)
     .AddInitialRequestCultureProvider(new CookieRequestCultureProvider
     {
         CookieName = "UserLanguage"
     });
 
 app.UseRequestLocalization(localizationOptions);
 ```
 
 ### Client-Side Configuration
 
 In your client project's `Program.cs`:
 
 #### Register the Translation Service
 
 ```csharp
 builder.Services.AddSingleton(typeof(TranslationService<>));
 ```
 
 #### Add Localization Services
 
 ```csharp
 builder.Services.AddLocalization();
 ```
 
 #### Set the Culture
 
 Add the following method to set the culture based on a cookie or default value:
 
 ```csharp
 async Task SetCultureAsync(IServiceCollection services)
 {
     const string defaultCulture = "en-US";
     var js = services.BuildServiceProvider().GetRequiredService<IJSRuntime>();
     var result = await js.InvokeAsync<string>("getLanguageCookie");
     var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);
 
     CultureInfo.DefaultThreadCurrentCulture = culture;
     CultureInfo.DefaultThreadCurrentUICulture = culture;
 }
 ```
 
 Call this method before `await builder.Build().RunAsync();`:
 
 ```csharp
 await SetCultureAsync(builder.Services);
 ```
 
 ### Using Translations in Components
 
 In your Blazor components, inject the `TranslationService` and use the translations as shown:
 
 **Example `Counter.razor`:**
 
 ```razor
 @page "/counter"
 @using Shared.Services
 @using Shared.Translations.Components.Pages.Counter
 @layout MainLayout
 @inject TranslationService<CounterTranslationManager> TranslationService
 
 <PageTitle>@Translations.PageTitle</PageTitle>
 
 <h1>@Translations.PageTitle</h1>
 
 <p role="status">@Translations.CurrentCount: @currentCount</p>
 
 <button class="btn btn-primary" @onclick="IncrementCount">@Translations.ClickMe</button>
 
 @code {
     private ICounterTranslation Translations;
     
     private int currentCount = 0;
 
     private void IncrementCount()
     {
         currentCount++;
     }
 
     protected override void OnInitialized()
     {
         Translations = TranslationService.Translations;
     }
 }
 ```
 
 **Explanation:**
 
 - **Injection**: Inject `TranslationService<CounterTranslationManager>` to access translations specific to the `Counter` component.
 - **Usage**: Access translation properties like `Translations.PageTitle` and `Translations.ClickMe`.
 - **Type Safety**: Since `ICounterTranslation` defines the translation properties, you get compile-time checking and IntelliSense support.
 
 ## Technical Explanation
 
 ### Overview
 
 This localization approach generates strongly-typed translation classes and interfaces from CSV files, enabling type-safe access to localized strings within Blazor components. It leverages code generation and reflection to provide translations based on the current culture.
 
 ### Key Components
 
 #### ITranslation Interface
 
 A marker interface for all translation interfaces.
 
 ```csharp
 public interface ITranslation { }
 ```
 
 #### Language Enum
 
 Enumerates the supported languages.
 
 ```csharp
 public enum Language
 {
     English,
     Spanish,
     Hungarian
 }
 ```
 
 #### LanguageExtensions Class
 
 Contains extension methods for mapping culture strings to `Language` enum values.
 
 ```csharp
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
 ```
 
 #### TranslationManager Abstract Class
 
 Manages the loading and setting of translations.
 
 ```csharp
 public abstract class TranslationManager
 {
     public Language Language { get; private set; }
 
     public abstract ITranslation Translation { get; }
 
     public void SetTranslation(Language language)
     {
         Language = language;
 
         var managerType = this.GetType();
         var translationType = GetTranslationTypeForLanguage(managerType, language);
 
         if (translationType == null)
         {
             throw new InvalidOperationException("Translation type not found for the specified language.");
         }
 
         var translationInstance = Activator.CreateInstance(translationType);
 
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
             else if (property.Name != "Translation" && property.Name != "Language")
             {
                 throw new Exception($"Property '{property.Name}' cannot be written. Ensure it has a setter.");
             }
         }
     }
 
     private Type? GetTranslationTypeForLanguage(Type managerType, Language language)
     {
         var managerName = managerType.Name.Replace("TranslationManager", "");
         var translationClassName = $"{managerName}Translation{language}";
         var translationType = managerType.Assembly.GetTypes()
             .FirstOrDefault(t => t.Name == translationClassName);
 
         return translationType;
     }
 }
 ```
 
 #### TranslationService Class
 
 Provides translations to components and handles language changes.
 
 ```csharp
 public class TranslationService<TTranslationManager> where TTranslationManager : TranslationManager, new()
 {
     private readonly TTranslationManager _translationManager;
 
     public TranslationService()
     {
         _translationManager = new TTranslationManager();
         SetLanguage(CultureInfo.CurrentCulture.Name.ToLanguage() ?? Language.English);
     }
 
     public TTranslationManager Translations => _translationManager;
 
     public void SetLanguage(Language language)
     {
         _translationManager.SetTranslation(language);
     }
 }
 ```
 
 ### Translation Workflow
 
 1. **CSV File Creation**
 
    - Developers create CSV files containing translations for each component.
    - Each CSV file corresponds to a specific component or set of strings.
 
 2. **Code Generation**
 
    - The code generator reads the CSV files and generates:
      - An interface defining the translation properties (e.g., `ICounterTranslation`).
      - A translation manager class (e.g., `CounterTranslationManager`).
      - Concrete translation classes for each language (e.g., `CounterTranslationEnglish`).
 
 3. **Translation Loading**
 
    - When the application starts, the `TranslationService` sets the current language based on the culture.
    - The `TranslationManager` uses reflection to instantiate the appropriate translation class.
 
 4. **Component Usage**
 
    - Components inject the `TranslationService` and access translations via strongly-typed properties.
    - Any change in culture will update the translations automatically.
 
 ## Contributing
 
 Contributions are welcome! If you have suggestions for improvements or encounter any issues, please feel free to open an issue or submit a pull request.
 
 ## License
 
 This project is licensed under the [MIT License](LICENSE).
