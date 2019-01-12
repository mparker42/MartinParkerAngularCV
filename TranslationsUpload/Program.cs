using MartinParkerAngularCV.SharedUtilities;
using MartinParkerAngularCV.SharedUtilities.Models.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Options;

namespace TranslationsUpload
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Resolving translations");

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            string rootPath = AppDomain.CurrentDomain.BaseDirectory,
                unresolvedTranslationPath = Path.Combine(rootPath, "UnResolvedTranslations"),
                fallbackTranslationsAsString = File.ReadAllText(Path.Combine(unresolvedTranslationPath, "en.json")),
                resolvedUploadPath = Path.Combine(rootPath, "Upload", "Translations");

            Dictionary<string, Dictionary<string, string>> fallbackTranslations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(fallbackTranslationsAsString);

            if (!Directory.Exists(resolvedUploadPath))
                Directory.CreateDirectory(resolvedUploadPath);

            foreach (CultureInfo currentCulture in cultures)
            {
                string locale = currentCulture.TwoLetterISOLanguageName,
                    unresolvedPath = Path.Combine(unresolvedTranslationPath, $"{locale}.json"),
                    resolvedPath = Path.Combine(resolvedUploadPath, $"{locale}.json");

                if (locale == "")
                    continue;

                if (!File.Exists(unresolvedPath))
                    File.WriteAllText(resolvedPath, fallbackTranslationsAsString);
                else
                {
                    Dictionary<string, Dictionary<string, string>> unresolvedTranslations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(unresolvedPath));

                    foreach (string section in fallbackTranslations.Keys)
                    {
                        if (!unresolvedTranslations.ContainsKey(section))
                            unresolvedTranslations.Add(section, fallbackTranslations[section]);
                        else
                        {
                            Dictionary<string, string> fallbackTranslationsSection = fallbackTranslations[section],
                                unresolvedTranslationsSection = unresolvedTranslations[section];

                            foreach (KeyValuePair<string, string> translation in fallbackTranslationsSection)
                                if (!unresolvedTranslationsSection.ContainsKey(translation.Key))
                                    unresolvedTranslationsSection.Add(translation.Key, translation.Value);
                        }
                    }

                    File.WriteAllText(resolvedPath, JsonConvert.SerializeObject(unresolvedTranslations));
                }
            }



            var provider = new ServiceCollection()
                       .AddMemoryCache()
                       .BuildServiceProvider();

            var cache = provider.GetService<IMemoryCache>();

            new BlobStoreHelper(Options.Create(new BlobStoreConfiguration() { StoreSecretURL = "martinparkercvstoreConnectionString/c9c196809fe04584a65159ed0fc14d8d" }), new KeyVaultHelper(Options.Create(new KeyVaultConfiguration() { BaseSecretURL = "https://cvvault.vault.azure.net/secrets/" }), cache)).UploadFolderToBlob(Path.Combine(rootPath, "Upload")).Wait();

            //cultures[0].TextInfo.IsRightToLeft

            Console.WriteLine("Putting all the files into blob");
        }
    }
}
