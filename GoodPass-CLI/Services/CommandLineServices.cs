using GoodPass_CLI.Helpers;

namespace GoodPass_CLI.Services;
public class CommandLineServices
{
    private static readonly string _dataFilePath = Path.Combine("Data", "GoodPassData.csv");
    public CommandLineServices()
    {
        currentDataInfo = new CurrentDataInfo() { PlatformName = "", AccountName = "", index = -1 };
        UpdateStatus = false;
        GetInfoProtected = true;
    }

    public bool UpdateStatus
    {
        get; set;
    }

    public bool GetInfoProtected
    {
        get; set;
    }

    private CurrentDataInfo currentDataInfo;

    /// <summary>
    /// 控制台命令处理
    /// </summary>
    /// <param name="command">输入的命令</param>
    /// <returns>无意义的可等待值</returns>
    public async Task<bool> CommandHandler(string? command)
    {
        if (!GetInfoProtected)
        {
            ConsoleHelper.PasswordLineProtect(5, 1);
            GetInfoProtected = true;
        }
        if (string.IsNullOrWhiteSpace(command))
        {
            return false;
        }
        if (command.StartsWith("-"))
        {
            var buffer = command.Split(" ");
            switch (buffer[0])
            {
                case "-h":
                    PrintHelp();
                    return true;
                case "-s":
                    SearchOptionHandler(buffer);
                    return true;
                case "-g":
                    GetOptionhandler(buffer);
                    return true;
                case "-l":
                    ListHandler();
                    return true;
                case "-a":
                    return await AddOptionhandler(buffer);
                case "-d":
                    return await DeleteOptionHandler(buffer);
                case "-u":
                    UpdateOptionHandler(buffer);
                    return true;
                case "-rp":
                    return await RPOptionHandler(buffer);
                case "-ra":
                    return await RAOptionHandler(buffer);
                case "-rw":
                    return await RWOptionHandler(buffer);
                case "-rpw":
                    return await RPWOptionHandler(buffer);
                default:
                    ConsoleHelper.PrintError($" Unknown option: \"{buffer[0]}\". Pleae check your input, or type \"help\" or \"-h\" to view command list.");
                    return false;
            }
        }
        switch (command)
        {
            case "help":
                PrintHelp();
                return true;
            case "about":
                PrintAbout();
                return true;
            case "search":
                SearchCommandHandler();
                return true;
            case "get":
                GetCommandhandler();
                return true;
            case "list":
                ListHandler();
                return true;
            case "exit":
                await Exit();
                Environment.Exit(0);
                return true;
            case "generate":
                GeneratePW();
                return true;
            case "add":
                return await AddCommandhandler();
            case "delete":
                return await DeleteCommandhandler();
            case "update":
                UpdateCommandHandler();
                return true;
            case "exportp":
                return await ExportPlaintext();
            case "exportc":
                return await ExportCiphertext();
            case "clear":
                Console.Clear();
                PrintStart();
                return true;
            case "reset-platform":
                return await RPCommandHandler();
            case "reset-account":
                return await RACommandHandler();
            case "reset-password":
                return await RPWCommandHandler();
            case "reset-website":
                return await RWCommandHandler();
            default:
                ConsoleHelper.PrintError($" Unknown command: \"{command}\". Pleae check your input, or type \"help\" or \"-h\" to view command list.");
                return false;
        }
    }

    /// <summary>
    /// 安全退出
    /// </summary>
    /// <returns>无意义的可等待值</returns>
    public async Task<bool> Exit()
    {
        Console.WriteLine("Leaving GoodPass CLI, please wait...");
        UpdateStatus = false;
        await GoodPass_CLI.manager.SaveToFileAsync(_dataFilePath);
        await Task.Delay(500);
        return true;
    }

    /// <summary>
    /// 打印关于信息
    /// </summary>
    public static void PrintAbout()
    {
        Console.WriteLine(" GoodPass CLI");
        Console.WriteLine($" Version:\t{GoodPass_CLI._version}");
        Console.WriteLine(" Author:\tGeorgeDong32");
        Console.WriteLine(" License:\tGoodPass Opensource License 1.0");
        Console.WriteLine(" Go to https://github.com/GeorgeDong32/GoodPass-v2 for more information");
        Console.WriteLine();
    }

    /// <summary>
    /// 打印帮助菜单
    /// </summary>
    public static void PrintHelp()
    {
        Console.WriteLine("GoodPass CLI Help");
        Console.WriteLine("Usage: >>> [options] [args]/ [command]");
        Console.WriteLine("Options:");
        Console.WriteLine("  -h\t\t\t\tShow help information");
        Console.WriteLine("  -s   [content]\t\tSearch by platform name or account name");
        Console.WriteLine("  -g   [platform] [account]\tGet data with account name and platform name");
        Console.WriteLine("  -g   [index]\t\t\tGet data with index");
        Console.WriteLine("  -l\t\t\t\tList all account");
        Console.WriteLine("  -a   [platform] [account]\tAdd an account with 3 or 4 args, [website] is optional");
        Console.WriteLine("       [password] [website]");
        Console.WriteLine("  -d   [platform] [account]\tDelete data with account name and platform name");
        Console.WriteLine("  -d   [index]\t\t\tDelete an account with index");
        Console.WriteLine("  -u   [account index]\t\tOpen update mode with account, default is the recently get account");
        Console.WriteLine("  -rp  [new platform]\t\tReset platform name");
        Console.WriteLine("  -ra  [new account]\t\tReset account name");
        Console.WriteLine("  -rw  [new website]\t\tReset website");
        Console.WriteLine("  -rpw [new password]\t\tReset password");
        Console.WriteLine("Commands:");
        Console.WriteLine("  help\t\t\tShow help information");
        Console.WriteLine("  exit\t\t\tExit GoodPass CLI(Recommended)");
        Console.WriteLine("  search\t\tSearch by platform name or account name");
        Console.WriteLine("  get\t\t\tGet data with account name an platform name");
        Console.WriteLine("  about\t\t\tShow about information");
        Console.WriteLine("  claer\t\t\tClear Command Line");
        Console.WriteLine("  generate\t\tGenerate a password");
        Console.WriteLine("  list\t\t\tList all account");
        Console.WriteLine("  add\t\t\tAdd an account");
        Console.WriteLine("  delete\t\tDelete an account");
        Console.WriteLine("  update\t\tGo in or go out update mode");
        Console.WriteLine("  exportp\t\tExport all plaintext passwords");
        Console.WriteLine("  exportc\t\tExport all ciphertext passwords");
        Console.WriteLine("  reset-platform\tReset platform name");
        Console.WriteLine("  reset-account\t\tReset account name");
        Console.WriteLine("  reset-password\tReset password");
        Console.WriteLine("  reset-website\t\tReset website");
        Console.WriteLine("");
    }

    /// <summary>
    /// 打印程序欢迎信息
    /// </summary>
    public static void PrintStart()
    {
        Console.WriteLine($"GoodPass CLI {GoodPass_CLI._version} [.NET {Environment.Version}] on {Environment.OSVersion}");
        #region Dev test info
        //Console.WriteLine($"Running on path: {Environment.CurrentDirectory}");
        #endregion
        Console.WriteLine("Type \"help\" or \"about\" to get more information. Type \"exit\" to leave safely.");
    }

    /// <summary>
    /// 密码生成处理
    /// </summary>
    /// <returns>生成的密码</returns>
    public static string GeneratePW()
    {
        Console.WriteLine(" GoodPass Password Generator");
        Console.WriteLine(" Choose generate mode:\n" +
                      "   \"n\" for normal random password\n" +
                      "   \"s\" for random password with special characters\n" +
                      "   \"g\" for GoodPass style string password, require platform and account name");
        Console.Write(" Enter your choice:");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "n":
                Console.Write(" Enter password length:");
                var pwlength1 = Console.ReadLine();
                string pw1;
                if (int.TryParse(pwlength1, out var length1))
                {
                    if (length1 <= 40 && length1 > 0)
                    {
                        pw1 = GoodPassPWGService.RandomPasswordNormal(length1);
                        Console.WriteLine(" The password is:");
                        Console.WriteLine(pw1);
                        return pw1;
                    }
                    else if (length1 > 40)
                    {
                        ConsoleHelper.PrintWarning(" The length is too long and has been reduced to 40");
                        pw1 = GoodPassPWGService.RandomPasswordNormal(40);
                        Console.WriteLine(" The password is:");
                        Console.WriteLine($" {pw1}");
                        return pw1;
                    }
                    else
                    {
                        ConsoleHelper.PrintError(" Invalid input: password length is less than 1");
                        return "failed";
                    }
                }
                else
                {
                    ConsoleHelper.PrintError(" Invalid input: password length is not a number");
                    return "failed";
                }
            case "s":
                Console.Write(" Enter password length:");
                var pwlength2 = Console.ReadLine();
                string pw2;
                if (int.TryParse(pwlength2, out var length2))
                {
                    if (length2 <= 40 && length2 > 0)
                    {
                        pw2 = GoodPassPWGService.RandomPasswordNormal(length2);
                        Console.WriteLine(" The password is:");
                        Console.WriteLine(pw2);
                        return pw2;
                    }
                    else if (length2 > 40)
                    {
                        ConsoleHelper.PrintWarning(" The length is too long and has been reduced to 40");
                        pw2 = GoodPassPWGService.RandomPasswordNormal(40);
                        Console.WriteLine(" The password is:");
                        Console.WriteLine($" {pw2}");
                        return pw2;
                    }
                    else
                    {
                        ConsoleHelper.PrintError(" Invalid input: password length is less than 1");
                        return "failed";
                    }
                }
                else
                {
                    ConsoleHelper.PrintError(" Invalid input: password length is not a number");
                    return "failed";
                }
            case "g":
                Console.Write(" Enter platform name:");
                var platformName = Console.ReadLine();
                if (string.IsNullOrEmpty(platformName))
                {
                    ConsoleHelper.PrintError(" Invalid input: platform name is empty");
                    return "failed";
                }
                Console.Write(" Enter account name:");
                var accountName = Console.ReadLine();
                if (string.IsNullOrEmpty(accountName))
                {
                    ConsoleHelper.PrintError(" Invalid input: account name is empty");
                    return "failed";
                }
                var pw3 = GoodPassPWGService.GPstylePassword(platformName, accountName);
                Console.WriteLine(" The password is:");
                Console.WriteLine($" {pw3}");
                return pw3;
            default:
                ConsoleHelper.PrintError(" Invalid input: Password generate mode");
                return "failed";
        }
    }

    /// <summary>
    /// 打印正在开发中信息
    /// </summary>
    public static void PrintDeveloping()
    {
        Console.WriteLine("The feature is developing...");
    }

    /// <summary>
    /// 默认的更新模式
    /// </summary>
    public void Update()
    {
        if (GoodPass_CLI.manager.GetData(currentDataInfo.index) != null)
        {
            if (!UpdateStatus)
            {
                UpdateStatus = true;
                ConsoleHelper.PrintGreen(" Update mode is actived");
                Console.WriteLine(" Please use reset options or commands to update data");
                Console.WriteLine();
            }
            else
            {
                UpdateStatus = false;
                ConsoleHelper.PrintWarning(" Upgrade mode is deactivated");
                Console.WriteLine();
            }
        }
        else
        {
            ConsoleHelper.PrintError(" Unable to enter update mode: no current data");
            UpdateStatus = false;
        }

    }

    /// <summary>
    /// 使用index的更新模式
    /// </summary>
    /// <param name="index"></param>
    public void Update(int index)
    {
        var data = GoodPass_CLI.manager.GetData(index);
        if (data == null)
        {
            currentDataInfo.index = -1;
            currentDataInfo.PlatformName = null;
            currentDataInfo.AccountName = null;
            UpdateStatus = false;
            ConsoleHelper.PrintError(" Upgrade mode is deactivated. Target data not found");
        }
        else
        {
            if (data.PlatformName == currentDataInfo.PlatformName && data.AccountName == currentDataInfo.AccountName)
            {
                if (!UpdateStatus)
                {
                    UpdateStatus = true;
                    ConsoleHelper.PrintGreen(" Update mode is actived");
                    Console.WriteLine(" Please use reset options or commands to update data");
                    Console.WriteLine();
                }
                else
                {
                    UpdateStatus = false;
                    ConsoleHelper.PrintWarning(" Upgrade mode is deactivated");
                    Console.WriteLine();
                }
            }
            else
            {
                UpdateStatus = true;
                currentDataInfo.index = index;
                currentDataInfo.PlatformName = data.PlatformName;
                currentDataInfo.AccountName = data.AccountName;
                ConsoleHelper.PrintGreen(" Update mode is actived");
                Console.WriteLine(" Please use reset options or commands to update data");
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// update命令处理
    /// </summary>
    public void UpdateCommandHandler()
    {
        Console.Write(" Please enter update index, null for default:");
        var indexstr = Console.ReadLine();
        if (int.TryParse(indexstr, out var index))
        {
            Update(index);
        }
        else if (indexstr == string.Empty)
        {
            Update();
        }
        else
        {
            ConsoleHelper.PrintError(" Invalid input: [index] is not a number");
        }
    }

    /// <summary>
    /// -u选项处理
    /// </summary>
    /// <param name="buffer">输入的选项及参数缓存</param>
    public void UpdateOptionHandler(string[] buffer)
    {
        if (buffer.Length > 1)
        {
            if (string.IsNullOrEmpty(buffer[1]))
                Update();
            else
            {
                if (int.TryParse(buffer[1], out var index))
                {
                    Update(index);
                }
                else
                {
                    ConsoleHelper.PrintError(" Invalid input: [account index] is not a number");
                }
            }
        }
        else
            Update();
    }

    /// <summary>
    /// list、-l处理
    /// </summary>
    public static void ListHandler()
    {
        Console.WriteLine(" Data List");
        Console.WriteLine(" {0,-15}  {1,-20}  {2,-10}  {3,-30}", "Platform Name", "Account name", "Index", "Website");
        var datas = GoodPass_CLI.manager.GetAllDatas();
        var index = 0;
        foreach (var data in datas)
        {
            Console.WriteLine(" {0,-15}  {1,-20}  {2,-10}  {3,-30}", data.PlatformName, data.AccountName, index, data.PlatformUrl);
            index++;
        }
        Console.WriteLine();
    }

    /// <summary>
    /// 使用index的获取
    /// </summary>
    /// <param name="index">目标的索引值</param>
    public void Get(int index)
    {
        var data = GoodPass_CLI.manager.GetData(index);
        if (data == null)
        {
            ConsoleHelper.PrintError(" Target data is not found, please check your input");
        }
        else
        {
            data.DataDecrypt();
            Console.WriteLine(" Data Info");
            Console.WriteLine(" {0,-15}\t{1,-40}", "Platform Name", data.PlatformName);
            Console.WriteLine(" {0,-15}\t{1,-40}", "Account Name", data.AccountName);
            Console.WriteLine(" {0,-15}\t{1,-40}", "Password", data.GetPassword);
            Console.WriteLine(" {0,-15}\t{1,-40}", "Website Link", data.PlatformUrl);
            Console.WriteLine(" {0,-15}\t{1,-40}", "Latest Update", data.LatestUpdateTime);
            Console.WriteLine();
            GetInfoProtected = false;
        }
    }

    /// <summary>
    /// 使用名字的获取
    /// </summary>
    /// <param name="platformName">目标平台名</param>
    /// <param name="accountName">目标账号名</param>
    public void Get(string platformName, string accountName)
    {
        var data = GoodPass_CLI.manager.GetData(platformName, accountName);
        if (data == null)
        {
            ConsoleHelper.PrintError(" Target data is not found, please check your input");
        }
        else
        {
            data.DataDecrypt();
            Console.WriteLine(" Data Info");
            Console.WriteLine(" {0,-15}\t{1,-40}", "Platform Name", data.PlatformName);
            Console.WriteLine(" {0,-15}\t{1,-40}", "Account Name", data.AccountName);
            Console.WriteLine(" {0,-15}\t{1,-40}", "Password", data.GetPassword);
            Console.WriteLine(" {0,-15}\t{1,-40}", "Website Link", data.PlatformUrl);
            Console.WriteLine(" {0,-15}\t{1,-40}", "Latest Update", data.LatestUpdateTime);
            Console.WriteLine();
            GetInfoProtected = false;
        }
    }

    /// <summary>
    /// get命令处理
    /// </summary>
    public void GetCommandhandler()
    {
        Console.Write(" Please choose get mode, i for index, n for name:");
        var mode = Console.ReadLine();
        switch (mode)
        {
            case "n":
                Console.Write(" Please enter platform name:");
                var platformName = Console.ReadLine();
                if (string.IsNullOrEmpty(platformName))
                {
                    ConsoleHelper.PrintError(" Invalid input: [PlatformName] is empty");
                    return;
                }
                Console.Write(" Please enter account name:");
                var accountName = Console.ReadLine();
                if (string.IsNullOrEmpty(accountName))
                {
                    ConsoleHelper.PrintError(" Invalid input: [AccountName] is empty");
                    return;
                }
                Get(platformName, accountName);
                break;
            case "i":
                Console.Write(" Please enter target index:");
                var indexstr = Console.ReadLine();
                if (string.IsNullOrEmpty(indexstr))
                {
                    ConsoleHelper.PrintError(" Invalid input: [index] is empty");
                    return;
                }
                if (int.TryParse(indexstr, out var index))
                {
                    Get(index);
                    return;
                }
                else
                {
                    ConsoleHelper.PrintError(" Invalid input: [index] is not a number");
                }
                break;
            default:
                ConsoleHelper.PrintError(" Invalid input: [Get-mode] is not \"i\" or \"n\"");
                break;
        }
    }

    /// <summary>
    /// -g选项处理
    /// </summary>
    /// <param name="buffer">-g选项及参数缓存</param>
    public void GetOptionhandler(string[] buffer)
    {
        if (buffer.Length >= 3)
        {
            if (string.IsNullOrEmpty(buffer[1]))
            {
                ConsoleHelper.PrintError(" Invalid input: [-g args] is empty");
                return;
            }
            if (string.IsNullOrEmpty(buffer[2]))
            {
                if (int.TryParse(buffer[1], out var index))
                {
                    Get(index);
                    return;
                }
                else
                {
                    ConsoleHelper.PrintError(" Invalid input: [index] is not a number, please check your input.");
                    return;
                }
            }
            else
            {
                Get(buffer[1], buffer[2]);
                return;
            }
        }
        else if (buffer.Length == 2)
        {
            if (string.IsNullOrEmpty(buffer[1]))
            {
                ConsoleHelper.PrintError(" Invalid input: [-g args] is empty");
                return;
            }
            if (int.TryParse(buffer[1], out var index))
            {
                Get(index);
                return;
            }
            else
            {
                ConsoleHelper.PrintError(" Invalid input: [index] is not a number, please check your input.");
                return;
            }
        }
        else
        {
            ConsoleHelper.PrintError(" Invalid input: not enough input arguments");
            return;
        }
    }

    /// <summary>
    /// 添加数据
    /// </summary>
    /// <param name="platformName">平台名</param>
    /// <param name="accountName">账号名</param>
    /// <param name="password">密码</param>
    /// <param name="websiteUrl">(可选)网站地址</param>
    public static async Task<bool> Add(string platformName, string accountName, string password, string? websiteUrl)
    {
        var result = GoodPass_CLI.manager.AddData(platformName, websiteUrl, accountName, password);
        if (result)
        {
            await GoodPass_CLI.manager.SaveToFileAsync(_dataFilePath);
            ConsoleHelper.PrintGreen(" Data added successfully");
            Console.WriteLine();
            return true;
        }
        else
        {
            ConsoleHelper.PrintError(" Data already exists, pleas go to update");
            Console.WriteLine();
            return false;
        }
    }

    public static async Task<bool> AddCommandhandler()
    {
        Console.Write(" Please enter platform name:");
        var platformName = Console.ReadLine();
        if (string.IsNullOrEmpty(platformName))
        {
            ConsoleHelper.PrintError(" Invalid input: [PlatformName] is empty");
            return false;
        }
        Console.Write(" Please enter account name:");
        var accountName = Console.ReadLine();
        if (string.IsNullOrEmpty(accountName))
        {
            ConsoleHelper.PrintError(" Invalid input: [AccountName] is empty");
            return false;
        }
        Console.Write(" Please enter password:");
        var password = ConsoleHelper.ReadPassword();
        if (string.IsNullOrEmpty(password))
        {
            ConsoleHelper.PrintError(" Invalid input: [Password] is empty");
            return false;
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            ConsoleHelper.PrintError(" Invalid input: [Password] is whitespace");
            return false;
        }
        Console.Write(" Please enter website url:");
        var websiteUrl = Console.ReadLine();
        return await Add(platformName.Trim(), accountName.Trim(), password.Trim(), websiteUrl);
    }

    public static async Task<bool> AddOptionhandler(string[] buffer)
    {
        const int _line = 2;
        if (buffer.Length >= 5)
        {
            if (string.IsNullOrEmpty(buffer[1]))
            {
                ConsoleHelper.PrintError(" Invalid input: [PlatformName] is empty");
                ConsoleHelper.PasswordCommandProtect(_line);
                return false;
            }
            if (string.IsNullOrEmpty(buffer[2]))
            {
                ConsoleHelper.PrintError(" Invalid input: [AccountName] is empty");
                ConsoleHelper.PasswordCommandProtect(_line);
                return false;
            }
            if (string.IsNullOrEmpty(buffer[3]))
            {
                ConsoleHelper.PrintError(" Invalid input: [Password] is empty");
                ConsoleHelper.PasswordCommandProtect(_line);
                return false;
            }
            if (string.IsNullOrWhiteSpace(buffer[3]))
            {
                ConsoleHelper.PrintError(" Invalid input: [Password] is whitespace");
                return false;
            }
            ConsoleHelper.PasswordCommandProtect(1);
            return await Add(buffer[1].Trim(), buffer[2].Trim(), buffer[3].Trim(), buffer[4]);
        }
        else if (buffer.Length == 4)
        {
            if (string.IsNullOrEmpty(buffer[1]))
            {
                ConsoleHelper.PrintError(" Invalid input: [PlatformName] is empty");
                ConsoleHelper.PasswordCommandProtect(_line);
                return false;
            }
            if (string.IsNullOrEmpty(buffer[2]))
            {
                ConsoleHelper.PrintError(" Invalid input: [AccountName] is empty");
                ConsoleHelper.PasswordCommandProtect(_line);
                return false;
            }
            if (string.IsNullOrEmpty(buffer[3]))
            {
                ConsoleHelper.PrintError(" Invalid input: [Password] is empty");
                return false;
            }
            if (string.IsNullOrWhiteSpace(buffer[3]))
            {
                ConsoleHelper.PrintError(" Invalid input: [Password] is whitespace");
                return false;
            }
            ConsoleHelper.PasswordCommandProtect(1);
            return await Add(buffer[1], buffer[2], buffer[3], null);
        }
        else
        {
            ConsoleHelper.PrintError(" Invalid input: not enough input arguments");
            return false;
        }
    }

    public static void Search(string content)
    {
        var datalist = GoodPass_CLI.manager.SuggestSearch(content);
        if (datalist.Count == 0)
        {
            ConsoleHelper.PrintWarning(" No related data found");
            Console.WriteLine();
            return;
        }
        else
        {
            Console.WriteLine(" Releated data list");
            Console.WriteLine(" {0,-15}  {1,-20}  {2,-10}  {3,-30}", "Platform Name", "Account name", "Index", "Website");
            var index = 0;
            foreach (var data in datalist)
            {
                Console.WriteLine(" {0,-15}  {1,-20}  {2,-10}  {3,-30}", data.PlatformName, data.AccountName, index, data.PlatformUrl);
                index++;
            }
            Console.WriteLine();
        }
    }

    public static void SearchCommandHandler()
    {
        Console.Write(" Please enter search content:");
        var content = Console.ReadLine();
        if (string.IsNullOrEmpty(content))
        {
            ConsoleHelper.PrintError(" Invalid input: [content] is empty");
            return;
        }
        Search(content);
    }

    public static void SearchOptionHandler(string[] buffer)
    {
        if (buffer.Length >= 2)
        {
            if (string.IsNullOrEmpty(buffer[1]))
            {
                ConsoleHelper.PrintError(" Invalid input: [content] is empty");
                return;
            }
            Search(buffer[1]);
            return;
        }
        else
        {
            ConsoleHelper.PrintError(" Invalid input: not enough input arguments");
            return;
        }
    }

    public static async Task<bool> Delete(int index)
    {
        if (index < 0)
        {
            ConsoleHelper.PrintError(" Invalid input: [index] is less than 0");
            return false;
        }
        var result = GoodPass_CLI.manager.DeleteData(index);
        if (result)
        {
            ConsoleHelper.PrintGreen(" Data has been successfully deleted");
            Console.WriteLine();
            return await GoodPass_CLI.manager.SaveToFileAsync(_dataFilePath);
        }
        else
        {
            ConsoleHelper.PrintError(" Data deletion failed, please check whether the index is correct");
            Console.WriteLine();
            return false;
        }
    }

    public static async Task<bool> Delete(string platformName, string accountName)
    {
        if (string.IsNullOrEmpty(platformName))
        {
            ConsoleHelper.PrintError(" Invalid input: [PlatformName] is empty");
            return false;
        }
        if (string.IsNullOrEmpty(accountName))
        {
            ConsoleHelper.PrintError(" Invalid input: [AccountName] is empty");
            return false;
        }
        var result = GoodPass_CLI.manager.DeleteData(platformName, accountName);
        if (result)
        {
            ConsoleHelper.PrintGreen(" Data has been successfully deleted");
            Console.WriteLine();
            return await GoodPass_CLI.manager.SaveToFileAsync(_dataFilePath);
        }
        else
        {
            ConsoleHelper.PrintError(" Data deletion failed, please check whether the index is correct");
            Console.WriteLine();
            return false;
        }
    }

    public static async Task<bool> DeleteCommandhandler()
    {
        Console.Write(" Please choose delete mode, i for index, n for name:");
        var mode = Console.ReadLine();
        switch (mode)
        {
            case "i":
                Console.Write(" Please enter index:");
                var indexstr = Console.ReadLine();
                if (string.IsNullOrEmpty(indexstr))
                {
                    ConsoleHelper.PrintError(" Invalid input: [index] is empty");
                    return false;
                }
                if (int.TryParse(indexstr, out var index))
                {
                    return await Delete(index);
                }
                else
                {
                    ConsoleHelper.PrintError(" Invalid input: [index] is not a number");
                    return false;
                }
            case "n":
                Console.Write(" Please enter platform name:");
                var platformName = Console.ReadLine();
                if (string.IsNullOrEmpty(platformName))
                {
                    ConsoleHelper.PrintError(" Invalid input: [PlatformName] is empty");
                    return false;
                }
                Console.Write(" Please enter account name:");
                var accountName = Console.ReadLine();
                if (string.IsNullOrEmpty(accountName))
                {
                    ConsoleHelper.PrintError(" Invalid input: [AccountName] is empty");
                    return false;
                }
                return await Delete(platformName, accountName);
            default:
                ConsoleHelper.PrintError(" Invalid input: [mode] is not i or n");
                return false;
        }
    }

    public static async Task<bool> DeleteOptionHandler(string[] buffer)
    {
        if (buffer.Length >= 3)
        {
            if (string.IsNullOrEmpty(buffer[1]))
            {
                ConsoleHelper.PrintError(" Invalid input: [PlatformName] is empty");
                return false;
            }
            if (string.IsNullOrEmpty(buffer[2]))
            {
                ConsoleHelper.PrintError(" Invalid input: [AccountName] is empty");
                return false;
            }
            return await Delete(buffer[1], buffer[2]);
        }
        else if (buffer.Length == 2)
        {
            if (string.IsNullOrEmpty(buffer[1]))
            {
                ConsoleHelper.PrintError(" Invalid input: [index] is empty");
                return false;
            }
            if (int.TryParse(buffer[1], out var index))
            {
                return await Delete(index);
            }
            else
            {
                ConsoleHelper.PrintError(" Invalid input: [index] is not a number");
                return false;
            }
        }
        else
        {
            ConsoleHelper.PrintError(" Invalid input: not enough input arguments");
            return false;
        }
    }

    public async Task<bool> ResetPlatform(string newplatform)
    {
        var platform = currentDataInfo.PlatformName;
        if (newplatform == platform)
        {
            ConsoleHelper.PrintError(" Reset failed: Same platform name");
            Console.WriteLine();
            return false;
        }
        var account = currentDataInfo.AccountName;
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        var result = GoodPass_CLI.manager.ChangePlatformName(platform, account, newplatform);
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        if (result)
        {
            currentDataInfo.index = GoodPass_CLI.manager.AccurateSearch(newplatform, account);
            currentDataInfo.PlatformName = newplatform;
            currentDataInfo.AccountName = account;
            ConsoleHelper.PrintGreen(" Successfully reset platform name");
            Console.WriteLine();
            return await GoodPass_CLI.manager.SaveToFileAsync(_dataFilePath);
        }
        else
        {
            ConsoleHelper.PrintError(" Reset failed: account is already exist in new platform");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> RPCommandHandler()
    {
        if (UpdateStatus)
        {
            Console.Write(" Please enter new platform name:");
            var newplatform = Console.ReadLine();
            if (string.IsNullOrEmpty(newplatform))
            {
                ConsoleHelper.PrintError(" Invalid input: [new platform] is empty");
                Console.WriteLine();
                return false;
            }
            return await ResetPlatform(newplatform);
        }
        else
        {
            ConsoleHelper.PrintError(" Please go in update mode first");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> RPOptionHandler(string[] buffer)
    {
        if (UpdateStatus)
        {
            if (buffer.Length >= 2)
            {
                if (string.IsNullOrEmpty(buffer[1]))
                {
                    ConsoleHelper.PrintError(" Invalid input: [new platform] is empty");
                    Console.WriteLine();
                    return false;
                }
                return await ResetPlatform(buffer[1]);
            }
            else
            {
                ConsoleHelper.PrintError(" Invalid input: not enough input arguments");
                Console.WriteLine();
                return false;
            }
        }
        else
        {
            ConsoleHelper.PrintError(" Please go in update mode first");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> ResetAccount(string newaccount)
    {
        var platform = currentDataInfo.PlatformName;
        var account = currentDataInfo.AccountName;
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        var result = GoodPass_CLI.manager.ChangeAccountName(platform, account, newaccount);
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        if (result)
        {
            currentDataInfo.index = GoodPass_CLI.manager.AccurateSearch(platform, newaccount);
            currentDataInfo.PlatformName = platform;
            currentDataInfo.AccountName = newaccount;
            ConsoleHelper.PrintGreen(" Successfully reset account name");
            Console.WriteLine();
            return await GoodPass_CLI.manager.SaveToFileAsync(_dataFilePath);
        }
        else
        {
            ConsoleHelper.PrintError(" Reset failed: account is already exist");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> RACommandHandler()
    {
        if (UpdateStatus)
        {
            Console.Write(" Please enter new account name:");
            var newaccount = Console.ReadLine();
            if (string.IsNullOrEmpty(newaccount))
            {
                ConsoleHelper.PrintError(" Invalid input: [new account] is empty");
                Console.WriteLine();
                return false;
            }
            return await ResetAccount(newaccount);
        }
        else
        {
            ConsoleHelper.PrintError(" Please go in update mode first");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> RAOptionHandler(string[] buffer)
    {
        if (UpdateStatus)
        {
            if (buffer.Length >= 2)
            {
                if (string.IsNullOrEmpty(buffer[1]))
                {
                    ConsoleHelper.PrintError(" Invalid input: [new account] is empty");
                    Console.WriteLine();
                    return false;
                }
                return await ResetAccount(buffer[1]);
            }
            else
            {
                ConsoleHelper.PrintError(" Invalid input: not enough input arguments");
                Console.WriteLine();
                return false;
            }
        }
        else
        {
            ConsoleHelper.PrintError(" Please go in update mode first");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> ResetPassword(string? newpassword)
    {
        var platform = currentDataInfo.PlatformName;
        var account = currentDataInfo.AccountName;
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        var result = GoodPass_CLI.manager.ChangePassword(platform, account, newpassword);
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        switch (result)
        {
            case "Success":
                ConsoleHelper.PrintGreen(" Successfully reset password");
                Console.WriteLine();
                return await GoodPass_CLI.manager.SaveToFileAsync(_dataFilePath);
            case "SamePassword":
                ConsoleHelper.PrintError(" Reset failed: same password");
                Console.WriteLine();
                return false;
            case "Empty":
                ConsoleHelper.PrintError(" Reset failed: empty password");
                Console.WriteLine();
                return false;
            default:
                ConsoleHelper.PrintError(" Reset failed: unknown error");
                Console.WriteLine();
                return false;
        }
    }

    public async Task<bool> RPWCommandHandler()
    {
        if (UpdateStatus)
        {
            Console.Write(" Please enter new password:");
            var newpassword = ConsoleHelper.ReadPassword();
#pragma warning disable CS8602 // 解引用可能出现空引用。
            if (newpassword.Length >= 40)
            {
                ConsoleHelper.PrintError(" Invalid input: [new password] is too long");
                Console.WriteLine();
                return false;
            }
#pragma warning restore CS8602 // 解引用可能出现空引用。
            return await ResetPassword(newpassword);
        }
        else
        {
            ConsoleHelper.PrintError(" Please go in update mode first");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> RPWOptionHandler(string[] buffer)
    {
        ConsoleHelper.PasswordCommandProtect(1);
        if (UpdateStatus)
        {
            if (buffer.Length >= 2)
            {
                if (string.IsNullOrEmpty(buffer[1]))
                {
                    ConsoleHelper.PrintError(" Invalid input: [new password] is empty");
                    Console.WriteLine();
                    return false;
                }
                if (buffer[1].Length >= 40)
                {
                    ConsoleHelper.PrintError(" Invalid input: [new password] is too long");
                    Console.WriteLine();
                    return false;
                }
                return await ResetPassword(buffer[1]);
            }
            else
            {
                ConsoleHelper.PrintError(" Invalid input: not enough input arguments");
                Console.WriteLine();
                return false;
            }
        }
        else
        {
            ConsoleHelper.PrintError(" Please go in update mode first");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> ResetWebsite(string? newurl)
    {
        var platform = currentDataInfo.PlatformName;
        var account = currentDataInfo.AccountName;
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        var result = GoodPass_CLI.manager.ChangeUrl(platform, account, newurl);
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        if (result)
        {
            ConsoleHelper.PrintGreen(" Successfully reset website");
            Console.WriteLine();
            return await GoodPass_CLI.manager.SaveToFileAsync(_dataFilePath);
        }
        else
        {
            ConsoleHelper.PrintError(" Reset failed: same website url");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> RWCommandHandler()
    {
        if (UpdateStatus)
        {
            Console.Write(" Please enter new website:");
            var newurl = Console.ReadLine();
            return await ResetWebsite(newurl);
        }
        else
        {
            ConsoleHelper.PrintError(" Please go in update mode first");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> RWOptionHandler(string[] buffer)
    {
        if (UpdateStatus)
        {
            if (buffer.Length >= 2)
            {
                return await ResetWebsite(buffer[1]);
            }
            else
            {
                ConsoleHelper.PrintError(" Invalid input: not enough input arguments");
                Console.WriteLine();
                return false;
            }
        }
        else
        {
            ConsoleHelper.PrintError(" Please go in update mode first");
            Console.WriteLine();
            return false;
        }
    }

    public async Task<bool> ExportPlaintext()
    {
        Console.Write(" Please enter path to export file:");
        var path = Console.ReadLine();
        if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
        {
            ConsoleHelper.PrintError(" Invalid input: [path] is empty");
            Console.WriteLine();
            return false;
        }
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            ConsoleHelper.PrintError(" Invalid input: [path] is not found");
            Console.WriteLine();
            return false;
        }
        else
        {
            File.Create(Path.Combine(path, "GoodPassData.plaintext.csv")).Close();
            GoodPass_CLI.manager.DecryptAllDatas();
            return await GoodPass_CLI.manager.SavePlaintextToFile(Path.Combine(path, "GoodPassData.plaintext.csv"));
        }
    }

    public async Task<bool> ExportCiphertext()
    {
        Console.Write(" Please enter path to export file:");
        var path = Console.ReadLine();
        if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
        {
            ConsoleHelper.PrintError(" Invalid input: [path] is empty");
            Console.WriteLine();
            return false;
        }
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            ConsoleHelper.PrintError(" Invalid input: [path] is not found");
            Console.WriteLine();
            return false;
        }
        else
        {
            File.Create(Path.Combine(path, "GoodPassData.ciphertext.csv")).Close();
            return await GoodPass_CLI.manager.SaveToFileAsync(Path.Combine(path, "GoodPassData.ciphertext.csv"));
        }
    }
}

internal struct CurrentDataInfo
{
    public string? PlatformName;
    public string? AccountName;
    public int index;
}