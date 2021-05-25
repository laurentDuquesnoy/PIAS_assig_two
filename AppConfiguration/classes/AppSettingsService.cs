using AppConfiguration.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AppConfiguration
{
    /// <summary>
    /// Generieke class met die de appsettings.json file leest
    /// de class kan worden gebruikt om de volledige inhoud van het appsetttings.json bestand te consumeren
    /// de class kan ook woren gebruikt om aparte secties van het appsettings.json bestand te lezen
    /// </summary>
    /// <typeparam name="TAppSettings"></typeparam>
    public class AppSettingsService<TAppSettings> 
    {
        //singleton van de de class AppSettingsService
        public static AppSettingsService<TAppSettings> Instance { get; } = new AppSettingsService<TAppSettings>();

        /// <summary>
        /// de root van een configuratie structuur 
        /// </summary>
        private IConfigurationRoot _configRoot { get; set; }

        /// <summary>
        /// De property representeert een object dat alle data van het appsettings.json bestand bevat
        /// </summary>
        private TAppSettings _appSettings { get; set; }

        /// <summary>
        /// alleen lezen property, dus geen publieke setter
        /// </summary>
        public TAppSettings AppSettings { get => _appSettings; }

        /// <summary>
        /// Base pad naar configuratie bestanden van de applicatie
        /// </summary>
        private string _appSettingsBasePath { get => @"c:\VivesTestFiles"; }

        public AppSettingsService()
        {
            BuildConfigurationRoot();
            
            // haal de volledige configuratie uit het bestand
            // en stockeer die in de service via de _appsettings variabele
            // bij het ophalen wordt onmiddellijk een CAST uitgevoerd naar het object van generieke type 
            // in de type parameter TAppSettings
            _appSettings = _configRoot.Get<TAppSettings>();
        }

        /// <summary>
        /// Bouw de configuratie root op basis van de opgegeven bestanden "appsettings.json", een object met key/value pairs
        /// </summary>
        private void BuildConfigurationRoot()
        {
            // Instantie van de ConfigurationBuilder class;
            // de bouwer van ons configuratieobject. 
            var configBuilder = new ConfigurationBuilder();

            // zet de basis directory van de applicatie
            configBuilder.SetBasePath(_appSettingsBasePath);

            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/ref#code-try-0
            AddJSONSettingsFileToBuilder(ref configBuilder, "appconfig.json");

            //bouw de applicatie configuratie root
            _configRoot = configBuilder.Build();
        }

        /// <summary>
        /// Voeg het json bestand toe aan de builder
        /// </summary>
        /// <param name="builder">Passing parameter By Reference</param>
        /// <param name="filename"></param>
        private void AddJSONSettingsFileToBuilder(ref ConfigurationBuilder builder, string filename)
        {
            var fullFilePath = Path.Combine(_appSettingsBasePath, filename);
            if (File.Exists(fullFilePath))
            {
                //voeg het json bestand toe aan de builder
                builder.AddJsonFile(filename);
            }
        }

        public ConfigurationQueryResult<TSection> GetConfigurationSection<TSection>(string sectionName)
        {
            var result = new ConfigurationQueryResult<TSection>() { Status = QueryStatus.Ok };
            // demo GetSection method
            // controleer of de sectie bestaat in de appsettings file
            // section name is de key name in het json bestand, dus als de key bestaat in het json bestand
            if (!_configRoot.GetSection(sectionName).Exists())
            {
                result.Status = QueryStatus.HasError;
                result.Error = new Exception("The sectionname is not found in the settingsfile");
                //exit of method, function
                return result;
            }
            // demo GetSection method, let op de extra Get met Type Parameter
            // Get<TSection>() verzorgt de effectieve binding, anders is het result null !!!
            result.QueryResult = _configRoot.GetSection(sectionName).Get<TSection>();
            return result;
        }
    }
}
