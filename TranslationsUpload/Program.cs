using MartinParkerAngularCV.SharedUtils;
using MartinParkerAngularCV.SharedUtils.Models.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Options;
using MartinParkerAngularCV.SharedUtils.Models.ServiceBus;
using MartinParkerAngularCV.SharedUtils.Enums;
using System.Threading.Tasks;

namespace TranslationsUpload
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Resolving translations");

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            string rootPath = AppDomain.CurrentDomain.BaseDirectory,
                unresolvedTranslationPath = Path.Combine(rootPath, "UnResolvedTranslations"),
                resolvedUploadPath = Path.Combine(rootPath, "Upload", "Translations");

            Dictionary<string, Dictionary<string, string>> fallbackTranslations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(Path.Combine(unresolvedTranslationPath, "en.json")));

            List<string> sections = new List<string>();

            foreach (string section in fallbackTranslations.Keys)
            {
                string resolvedSectionUploadPath = Path.Combine(resolvedUploadPath, section);
                if (!Directory.Exists(resolvedSectionUploadPath))
                    Directory.CreateDirectory(resolvedSectionUploadPath);
            }

            foreach (CultureInfo currentCulture in cultures)
            {
                string locale = currentCulture.TwoLetterISOLanguageName,
                    unresolvedPath = Path.Combine(unresolvedTranslationPath, $"{locale}.json");

                if (locale == "")
                    continue;

                if (!File.Exists(unresolvedPath))
                    foreach (string section in fallbackTranslations.Keys)
                    {
                        File.WriteAllText(Path.Combine(resolvedUploadPath, section, $"{locale}.json"), JsonConvert.SerializeObject(fallbackTranslations[section]));

                        sections.Add($"{section}/{locale}.json");
                    }
                else
                {
                    Dictionary<string, Dictionary<string, string>> unresolvedTranslations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(unresolvedPath));

                    foreach (string section in fallbackTranslations.Keys)
                    {
                        if (!unresolvedTranslations.ContainsKey(section))
                            File.WriteAllText(Path.Combine(resolvedUploadPath, section, $"{locale}.json"), JsonConvert.SerializeObject(fallbackTranslations[section]));
                        else
                        {
                            Dictionary<string, string> fallbackTranslationsSection = fallbackTranslations[section],
                                unresolvedTranslationsSection = unresolvedTranslations[section];

                            foreach (KeyValuePair<string, string> translation in fallbackTranslationsSection)
                                if (!unresolvedTranslationsSection.ContainsKey(translation.Key))
                                    unresolvedTranslationsSection.Add(translation.Key, translation.Value);

                            File.WriteAllText(Path.Combine(resolvedUploadPath, section, $"{locale}.json"), JsonConvert.SerializeObject(unresolvedTranslationsSection));
                        }

                        sections.Add($"{section}/{locale}.json");
                    }
                }
            }

            Console.WriteLine("Putting all the files into blob");

            var provider = new ServiceCollection()
                       .AddMemoryCache()
                       .BuildServiceProvider();

            var cache = provider.GetService<IMemoryCache>();

            var keyVaultHelper = new KeyVaultHelper(
                    Options.Create(
                        new KeyVaultConfiguration()
                        {
                            BaseSecretURL = "https://cvvault.vault.azure.net/secrets/"
                        }
                    ), cache);

            await new BlobStoreHelper(
                Options.Create(
                    new BlobStoreConfiguration()
                    {
                        StoreSecretURL = "martinparkercvstoreConnectionString"
                    }
                ), 
                keyVaultHelper
            ).UploadFolderToBlob(Path.Combine(rootPath, "Upload"));

            Console.WriteLine("Sending update message to subscribers");

            await new ServiceBusHelper(
                Options.Create(
                    new ServiceBusConfiguration()
                    {
                        ServiceBusSecretURL = "martinparkercvserviceBusConnectionString"
                    }
                ), 
                keyVaultHelper
            ).SendMessage(new ResetTranslationsCacheMessage(ServiceBusTopic.ResetTranslationsCache, sections));
        }
    }
}
