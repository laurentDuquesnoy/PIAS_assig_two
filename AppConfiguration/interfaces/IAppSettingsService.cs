using AppConfiguration.Data;

namespace AppConfiguration
{
    // generiek interface die de methodes van de AppSettingsService beschrijft
    public interface IAppSettingsService<TAppSettings>
    {
        ConfigurationQueryResult<TSection> GetConfigurationSection<TSection>(string sectionName);
    }
}