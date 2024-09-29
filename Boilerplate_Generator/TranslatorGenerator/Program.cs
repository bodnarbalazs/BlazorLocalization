using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TranslatorGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // For testing purposes, you can set the translationsFolderPath directly
            // string translationsFolderPath = @"path-to-Translations";

            // Comment these lines out if you want to test it with the code commented out above
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: TranslationGenerator <translations_folder_path>");
                return;
            }
            string translationsFolderPath = args[0];

            if (!Directory.Exists(translationsFolderPath))
            {
                Console.WriteLine($"Translations folder not found: {translationsFolderPath}");
                return;
            }

            // Find the TranslationManager.cs file
            string translationManagerPath = Path.Combine(translationsFolderPath, "TranslationManager.cs");
            if (!File.Exists(translationManagerPath))
            {
                Console.WriteLine($"TranslationManager.cs not found in: {translationsFolderPath}");
                    return;
            }

            // Parse the base namespace from TranslationManager.cs
            string baseNamespace = ParseNamespaceFromFile(translationManagerPath);

            if (string.IsNullOrEmpty(baseNamespace))
            {
                Console.WriteLine("Could not determine the namespace from TranslationManager.cs");
                return;
            }

            try
            {
                // Traverse the Translations folder and process CSV files
                ProcessTranslations(translationsFolderPath, baseNamespace);
                Console.WriteLine("Translation files generated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static string ParseNamespaceFromFile(string filePath)
        {
            foreach (var line in File.ReadLines(filePath))
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("namespace"))
                {
                    var parts = trimmedLine.Split(' ');
                    if (parts.Length >= 2)
                    {
                        // Extract the namespace (e.g., 'namespace Shared.Translations;')
                        return parts[1].Trim(';');
                    }
                }
            }

            return null;
        }

        static void ProcessTranslations(string translationsFolderPath, string baseNamespace)
        {
            var csvFiles = Directory.GetFiles(translationsFolderPath, "*.csv", SearchOption.AllDirectories);

            foreach (var csvFilePath in csvFiles)
            {
                // Determine the output directory (same as the CSV file's directory)
                var outputDir = Path.GetDirectoryName(csvFilePath);

                // Build the namespace by adding the relative path
                var relativePath = Path.GetRelativePath(translationsFolderPath, outputDir);

                // Split the relative path into parts
                var pathParts = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                // Clean up each part to be a valid identifier
                var namespaceParts = pathParts.Select(CleanNamespacePart).Where(part => !string.IsNullOrEmpty(part));

                var namespaceSuffix = string.Join(".", namespaceParts);

                var namespaceName = string.IsNullOrEmpty(namespaceSuffix)
                    ? baseNamespace
                    : $"{baseNamespace}.{namespaceSuffix}";

                GenerateTranslations(csvFilePath, outputDir, namespaceName);
            }
        }

        static string CleanNamespacePart(string part)
        {
            if (string.IsNullOrWhiteSpace(part))
                return null;

            // Remove invalid characters, keep letters, digits, and underscores
            var cleanPart = new string(part.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());

            // Ensure it doesn't start with a digit
            if (string.IsNullOrEmpty(cleanPart))
                return null;

            if (char.IsDigit(cleanPart[0]))
                cleanPart = "_" + cleanPart;

            return cleanPart;
        }

        static void GenerateTranslations(string csvFilePath, string outputDir, string namespaceName)
        {
            // Read the CSV file
            var records = ReadCsv(csvFilePath);

            if (records.Count == 0)
            {
                Console.WriteLine($"No records found in CSV file: {csvFilePath}");
                return;
            }

            // Get the headers from the first record
            var headers = records.First().Keys;

            // Check if 'property_name' header exists
            if (!headers.Contains("property_name", StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception($"CSV file {csvFilePath} does not contain a 'property_name' column.");
            }

            // Extract component name from CSV file name
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(csvFilePath);
            var componentName = fileNameWithoutExtension;

            // Remove 'Translations' suffix if present
            if (componentName.EndsWith("Translations", StringComparison.OrdinalIgnoreCase))
            {
                componentName = componentName.Substring(0, componentName.Length - "Translations".Length);
            }

            var interfaceName = $"I{componentName}Translation";
            var managerName = $"{componentName}TranslationManager";

            // Get list of languages and properties
            var languages = headers.Where(k => !string.Equals(k, "property_name", StringComparison.OrdinalIgnoreCase))
                .ToList();
            var properties = records.Select(r => r["property_name"]).ToList();

            // Generate the interface
            GenerateInterface(interfaceName, properties, namespaceName, outputDir);

            // Generate the manager class
            GenerateManagerClass(managerName, interfaceName, properties, namespaceName, outputDir);

            // Generate translation classes for each language
            foreach (var language in languages)
            {
                var languageName = ToPascalCase(language);

                var className = $"{componentName}Translation{languageName}";
                var translations = records.ToDictionary(
                    r => r["property_name"],
                    r => r[language]
                );

                GenerateTranslationClass(className, interfaceName, translations, namespaceName, outputDir);
            }
        }

        static List<Dictionary<string, string>> ReadCsv(string csvFilePath)
        {
            var records = new List<Dictionary<string, string>>();

            using (var reader = new StreamReader(csvFilePath))
            {
                string headerLine = reader.ReadLine();
                if (string.IsNullOrEmpty(headerLine))
                {
                    throw new Exception($"CSV file is empty: {csvFilePath}");
                }

                var headers = ParseCsvLine(headerLine).Select(h => h.Trim()).ToArray();

                // Normalize headers to lowercase for consistent access
                var normalizedHeaders = headers.Select(h => h.ToLowerInvariant()).ToArray();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = ParseCsvLine(line);
                    if (fields.Length != headers.Length)
                    {
                        throw new Exception(
                            $"CSV file is malformed in file {csvFilePath}. Number of fields does not match number of headers.");
                    }

                    var record = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < headers.Length; i++)
                    {
                        record[normalizedHeaders[i]] = fields[i];
                    }

                    records.Add(record);
                }
            }

            return records;
        }

        static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            string field = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Escaped quote within quoted field
                        field += '"';
                        i++; // Skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ';' && !inQuotes)
                {
                    // Field separator outside quotes
                    fields.Add(field);
                    field = "";
                }
                else
                {
                    field += c;
                }
            }

            fields.Add(field);

            return fields.ToArray();
        }

        static void GenerateInterface(string interfaceName, List<string> properties, string namespaceName,
            string outputDir)
        {
            var filePath = Path.Combine(outputDir, $"{interfaceName}.cs");
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"namespace {namespaceName};");
                writer.WriteLine();
                writer.WriteLine($"public interface {interfaceName} : ITranslation");
                writer.WriteLine("{");
                foreach (var prop in properties)
                {
                    var propName = ToPascalCase(prop);
                    writer.WriteLine($"    string {propName} {{ get; }}");
                }

                writer.WriteLine("}");
            }

            Console.WriteLine($"Generated interface: {filePath}");
        }

        static void GenerateManagerClass(string managerName, string interfaceName, List<string> properties,
            string namespaceName, string outputDir)
        {
            var filePath = Path.Combine(outputDir, $"{managerName}.cs");
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"namespace {namespaceName};");
                writer.WriteLine();
                writer.WriteLine($"public class {managerName} : TranslationManager, {interfaceName}");
                writer.WriteLine("{");
                writer.WriteLine("    public override ITranslation Translation => this;");
                writer.WriteLine();
                foreach (var prop in properties)
                {
                    var propName = ToPascalCase(prop);
                    writer.WriteLine($"    public string {propName} {{ get; set; }}");
                }

                writer.WriteLine("}");
            }

            Console.WriteLine($"Generated manager class: {filePath}");
        }

        static void GenerateTranslationClass(string className, string interfaceName,
            Dictionary<string, string> translations, string namespaceName, string outputDir)
        {
            var filePath = Path.Combine(outputDir, $"{className}.cs");
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"namespace {namespaceName};");
                writer.WriteLine();
                writer.WriteLine($"public class {className} : {interfaceName}");
                writer.WriteLine("{");
                foreach (var kvp in translations)
                {
                    var propName = ToPascalCase(kvp.Key);
                    var value = kvp.Value.Replace("\\", "\\\\").Replace("\"", "\\\"");

                    // Escape backslashes and quotes in the value
                    writer.WriteLine($"    public string {propName} => \"{value}\";");
                }

                writer.WriteLine("}");
            }

            Console.WriteLine($"Generated translation class: {filePath}");
        }

        static string ToPascalCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var words = text.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var pascalCase = string.Join("", words.Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower()));
            return pascalCase;
        }
    }
}
