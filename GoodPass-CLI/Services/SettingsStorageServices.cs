using GoodPass_CLI.Models;

namespace GoodPass_CLI.Services;
public static class SettingsStorageServices
{
    static readonly string _settingsFolderPath = Path.Combine($"C:\\Users\\{Environment.UserName}\\AppData\\Local","GoodPass","GoodPass-CLI");
    static readonly string _settingFilePath = $"{_settingsFolderPath}\\settings.csv";

    public static bool SaveSettings(string settingName, string settingValue)
    {
        if (File.Exists(_settingsFolderPath))
        {
            //TODO: 存储数据
            return true;
        }
        else
        {
            if (Directory.Exists(_settingsFolderPath))
            {
                File.Create(_settingFilePath).Close();
                return true;
            }
            else
            {
                Directory.CreateDirectory(_settingsFolderPath);
                if (System.IO.Directory.Exists(_settingsFolderPath))
                {
                    System.IO.File.Create(_settingFilePath).Close();
                    if (System.IO.File.Exists(_settingFilePath))
                    {
                        //TODO: 存储数据
                        return true;
                    }
                    else
                    {
                        throw new Exception("Failed to create config file!");
                    }
                }
                else
                {
                    throw new Exception("Failed to create config file!");
                }
            }
        }
    }

    public static string GetSettings(string settingName)
    {
        if (File.Exists(_settingFilePath))
        {
            //TODO: 获取数据
            return "";
        }
        else
        {
            return "";
            //TODO: handle exp
            //throw new Exception("Config file not found!");
        }
    }
}
