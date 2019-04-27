using MartinParkerAngularCV.SharedUtils;
using MartinParkerAngularCV.SharedUtils.Enums;
using MartinParkerAngularCV.SharedUtils.Models.Configuration;
using MartinParkerAngularCV.SharedUtils.Models.ServiceBus;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TranslationsUpload
{
    class Program
    {
        private class UnResolvedTag
        {
            public string name { get; set; }
            public string selectValue { get; set; }
            public DateTime? dateValue { get; set; }
            public bool? booleanValue { get; set; }
        }

        private class UnResolvedTile
        {
            public List<UnResolvedTag> tags { get; set; }
        }

        private class TileConfiguration
        {
            public List<UnResolvedTile> tiles { get; set; }
        }

        private class SelectFilterOption
        {
            public string value { get; set; }
            public string translation { get; set; }
        }

        private class SelectFilter
        {
            public string name { get; set; }
            public string titleTranslation { get; set; }
            public List<SelectFilterOption> options { get; set; }
        }

        private class DateFilter
        {
            public string name { get; set; }
            public string titleTranslation { get; set; }
            public DateTime minimumValue { get; set; }
            public DateTime maximumValue { get; set; }
        }

        private class BooleanFilter
        {
            public string name { get; set; }
            public string titleTranslation { get; set; }
        }


        static async Task Main(string[] args)
        {
            Console.WriteLine("Resolving translations");

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            string rootPath = AppDomain.CurrentDomain.BaseDirectory,
                unresolvedTranslationPath = Path.Combine(rootPath, "UnResolvedTranslations"),
                resolvedUploadPath = Path.Combine(rootPath, "Upload", "Internal", "Translations");

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

            Console.WriteLine("Resolving tile definitions");

            string tileDefinitions = Path.Combine(rootPath, "Upload", "Public", "DataFiles", "V1"),
                unresolvedTileDefinitions = Path.Combine(tileDefinitions, "TileDefinition.json"),
                resolvedTileDefinitionsUploadPath = Path.Combine(tileDefinitions, "TileSearch.json");

            IEnumerable<UnResolvedTag> tags = JsonConvert.DeserializeObject<TileConfiguration>(File.ReadAllText(unresolvedTileDefinitions)).tiles.SelectMany(t => t.tags);

            List<SelectFilter> selectFilters = new List<SelectFilter>();
            List<DateFilter> dateFilters = new List<DateFilter>();
            List<BooleanFilter> booleanFilters = new List<BooleanFilter>();

            foreach (UnResolvedTag tag in tags)
            {
                string titleTranslation = $"tag{tag.name}TitleTranslation";

                if (!string.IsNullOrWhiteSpace(tag.selectValue))
                {
                    SelectFilter currentSelectFilter = selectFilters.FirstOrDefault(t => t.name == tag.name);
                    SelectFilterOption currentOption = new SelectFilterOption
                    {
                        value = tag.selectValue,
                        translation = $"tag{tag.name}Value{tag.selectValue}Translation"
                    };

                    if (currentSelectFilter == null)
                        selectFilters.Add(new SelectFilter()
                        {
                            name = tag.name,
                            titleTranslation = titleTranslation,
                            options = new List<SelectFilterOption>
                            {
                                currentOption
                            }
                        });

                    else if (!currentSelectFilter.options.Any(t => t.value == tag.selectValue))
                        currentSelectFilter.options.Add(currentOption);
                }
                else if (tag.dateValue.HasValue)
                {
                    DateFilter currentDateFilter = dateFilters.FirstOrDefault(t => t.name == tag.name);
                    DateTime currentDate = tag.dateValue.Value;

                    if (currentDateFilter == null)
                        dateFilters.Add(new DateFilter
                        {
                            name = tag.name,
                            titleTranslation = titleTranslation,
                            minimumValue = currentDate,
                            maximumValue = currentDate
                        });

                    else if (currentDate < currentDateFilter.minimumValue)
                        currentDateFilter.minimumValue = currentDate;

                    else if (currentDate > currentDateFilter.maximumValue)
                        currentDateFilter.maximumValue = currentDate;
                }
                else if (tag.booleanValue.HasValue && !booleanFilters.Any(t => t.name == tag.name))
                {
                    booleanFilters.Add(new BooleanFilter
                    {
                        name = tag.name,
                        titleTranslation = titleTranslation
                    });
                }
            }

            File.WriteAllText(resolvedTileDefinitionsUploadPath, JsonConvert.SerializeObject(new
            {
                selectFilters,
                dateFilters,
                booleanFilters
            }));

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

            var blobHelper = new BlobStoreHelper(
                Options.Create(
                    new BlobStoreConfiguration()
                    {
                        InternalStoreSecretURL = "internalFilesStoreConnectionString",
                        PublicStoreSecretURL = "publicFilesStoreConnectionString"
                    }
                ),
                keyVaultHelper
            );

            await blobHelper.UploadFolderToBlob(Path.Combine(rootPath, "Upload", "Internal"), BlobContainerType.Internal);
            await blobHelper.UploadFolderToBlob(Path.Combine(rootPath, "Upload", "Public"), BlobContainerType.Public);

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
