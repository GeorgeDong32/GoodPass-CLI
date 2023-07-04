using System;
using System.Text;
using GoodPass_CLI.Helpers;
using GoodPass_CLI.Models;

namespace GoodPass_CLI.Services;
public class CommandLineServices
{
    public CommandLineServices()
    {
        currentDataInfo = new CurrentDataInfo() { PlatformName = "", AccountName = "" , index = -1};
        UpdateStatus = false;
    }

    public bool UpdateStatus { get; set; }

    CurrentDataInfo currentDataInfo;

    public async Task<bool> CommandHandler(string command)
    {
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
                    //TODO: 搜索
                    PrintDeveloping();
                    await Task.CompletedTask;
                    return true;
                case "-g":
                    GetOptionhandler(buffer);
                    return true;
                case "-l":
                    ListHandler();
                    return true;
                case "-u":
                    UpdateOptionHandler(buffer);
                    return true;
                case "-rp":
                    PrintDeveloping();
                    return true;
                case "-ra":
                    PrintDeveloping();
                    return true;
                case "-rw":
                    PrintDeveloping();
                    return true;
                case "-rpw":
                    PrintDeveloping();
                    return true;
                default:
                    Console.WriteLine($"Unknown option: \"{buffer[0]}\". Pleae check your input, or type \"help\" or \"-h\" to view command list.");
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
                //TODO: 搜索
                PrintDeveloping();
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
                PrintDeveloping();
                return true;
            case "update":
                UpdateCommandHandler();
                return true;
            case "exportp":
                PrintDeveloping();
                return true;
            case "exportc":
                PrintDeveloping();
                return true;
            case "clear":
                Console.Clear();
                PrintStart();
                return true;
            case "reset-platform":
                PrintDeveloping();
                return true;
            case "reset-account":
                PrintDeveloping();
                return true;
            case "reset-password":
                PrintDeveloping();
                return true;
            case "reset-website":
                PrintDeveloping();
                return true;
            case "reset-all":
                PrintDeveloping();
                return true;
            default:
                Console.WriteLine($"Unknown command: \"{command}\". Pleae check your input, or type \"help\" or \"-h\" to view command list.");
                return false;
        }
    }

    public async Task<bool> Exit()
    {
        Console.WriteLine("Leaving GoodPass CLI, please wait...");
        await Task.CompletedTask;
        return true;
    }

    public static void PrintAbout()
    {
        Console.WriteLine(" GoodPass CLI");
        Console.WriteLine($" Version:\t{GoodPass_CLI._version}");
        Console.WriteLine(" Author:\tGeorgeDong32");
        Console.WriteLine(" License:\tGoodPass Opensource License 1.0");
        Console.WriteLine(" Go to https://github.com/GeorgeDong32/GoodPass-v2 for more information");
        Console.WriteLine();
    }

    public static void PrintHelp()
    {
        Console.WriteLine("GoodPass CLI Help");
        Console.WriteLine("Usage: >>> [options] [args]/ [command]");
        Console.WriteLine("Options:");
        Console.WriteLine("  -h\t\t\t\tShow help information");
        Console.WriteLine("  -s   [content]\t\tSearch by platform name or account name");
        Console.WriteLine("  -g   [platform] [account]\tGet data with account name an platform name");
        Console.WriteLine("       [index]\t\t\tGet data with index");
        Console.WriteLine("  -l\t\t\t\tList all account");
        Console.WriteLine("  -a\t\t\t\tAdd an account");
        Console.WriteLine("  -u   [account index]\t\tOpen update mode with account, default is the recently get account");
        Console.WriteLine("  -rp  [new platform]\t\tReset platform name");
        Console.WriteLine("  -ra  [new account]\t\tReset account name");
        Console.WriteLine("  -rw  [new website]\t\tReset website");
        Console.WriteLine("  -rpw [new password]\t\tReset password");
        Console.WriteLine("Commands:");
        Console.WriteLine("  help\t\t\tShow help information");
        Console.WriteLine("  exit\t\t\tExit GoodPass CLI");
        Console.WriteLine("  search\t\tSearch by platform name or account name");
        Console.WriteLine("  get\t\t\tGet data with account name an platform name");
        Console.WriteLine("  about\t\t\tShow about information");
        Console.WriteLine("  claer\t\t\tClear Command Line");
        Console.WriteLine("  generate\t\tGenerate a password");
        Console.WriteLine("  list\t\t\tList all account");
        Console.WriteLine("  add\t\t\tAdd an account");
        Console.WriteLine("  update\t\tGo in or go out update mode");
        Console.WriteLine("  exportp\t\tExport all plaintext passwords");
        Console.WriteLine("  exportc\t\tExport all ciphertext passwords");
        Console.WriteLine("  reset-platform\tReset platform name");
        Console.WriteLine("  reset-account\t\tReset account name");
        Console.WriteLine("  reset-password\tReset password");
        Console.WriteLine("  reset-website\t\tReset website");
        Console.WriteLine("  reset-all\t\tReset all");
        Console.WriteLine("");
    }

    public static void PrintStart()
    {
        Console.WriteLine($"GoodPass CLI {GoodPass_CLI._version} [.NET {Environment.Version}] on {Environment.OSVersion}");
        Console.WriteLine("Type \"help\" or \"about\" to get more information.");
    }

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
                if (Int32.TryParse(pwlength1, out int length1))
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
                if (Int32.TryParse(pwlength2, out int length2))
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

    public static void PrintDeveloping()
    {
        Console.WriteLine("The feature is developing...");
    }

    public void Update()
    {
        if (GoodPass_CLI.manager.GetData(currentDataInfo.index) != null)
        {
            if (!UpdateStatus)
            {
                UpdateStatus = true;
                ConsoleHelper.PrintGreen(" Update mode is actived");
                Console.WriteLine(" Please use reset options or commands to update data");
            }
            else
            {
                UpdateStatus = false;
                ConsoleHelper.PrintWarning(" Upgrade mode is deactivated");
            }
        }
        else
        {
            ConsoleHelper.PrintError(" Unable to enter update mode: no current data");
            UpdateStatus = false;
        }

    }

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
                }
                else
                {
                    UpdateStatus = false;
                    ConsoleHelper.PrintWarning(" Upgrade mode is deactivated");
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
            }
        }
    }

    public void UpdateCommandHandler()
    {
        Console.Write(" Please enter update index, null for default:");
        var indexstr = Console.ReadLine();
        if (Int32.TryParse(indexstr, out int index))
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

    public void UpdateOptionHandler(string[]? buffer)
    {
        if (buffer.Length > 1)
        {
            if (string.IsNullOrEmpty(buffer[1]))
                Update();
            else
            {
                if (Int32.TryParse(buffer[1], out int index))
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

    public static void ListHandler()
    {
        Console.WriteLine(" Data List");
        Console.WriteLine(" {0,-15}  {1,-20}  {2,-10}  {3,-30}","Platform Name", "Account name", "Index", "Website");
        var datas = GoodPass_CLI.manager.GetAllDatas();
        var index = 0;
        foreach (var data in datas)
        {
            Console.WriteLine(" {0,-15}  {1,-20}  {2,-10}  {3,-30}", data.PlatformName, data.AccountName, index, data.PlatformUrl);
            index++;
        }
        Console.WriteLine();
    }

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
        }
    }

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
        }
    }

    public void GetCommandhandler()
    {
        Console.Write(" Choose get mode, i for index, n for name:");
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
                if (Int32.TryParse(indexstr, out int index))
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

    public void GetOptionhandler(string[]? buffer)
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
                if (Int32.TryParse(buffer[1], out int index))
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
            if (Int32.TryParse(buffer[1], out int index))
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
}

internal struct CurrentDataInfo
{
    public string? PlatformName;
    public string? AccountName;
    public int index;
}