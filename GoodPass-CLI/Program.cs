using GoodPass_CLI.Helpers;
using GoodPass_CLI.Models;
using GoodPass_CLI.Services;

namespace GoodPass_CLI;

public static class GoodPass_CLI
{
    public static readonly string _version = "3.2.0";
    public static int[]? EncryptBase;
    public static byte[]? AESKey;
    public static byte[]? AESIV;
    public static GPManager manager = new();
    public static async Task Main(string[] args)
    {
        #region Console Configs
        Console.CancelKeyPress += CancelKeyHandler;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.Title = $"GoodPass CLI {_version}";
        var CLS = new CommandLineServices();
        Console.Clear();
        #endregion
        #region Login
        var checkcount = 0;
        CommandLineServices.PrintStart();
        if (File.Exists(Path.Combine("Data", "MKConfig.txt")))
        {
            do
            {
                Console.Write(" Please enter your master password:");
                var masterkey = ConsoleHelper.ReadPassword();
                var mkcr = MasterKeyService.CheckMasterKeyAsync(masterkey, out AESIV, out AESKey, out EncryptBase);
                switch (mkcr)
                {
                    case "pass":
                        goto mainprocess;
                    case "npass":
                        ConsoleHelper.PrintError(" Master password incorrect, please try again.");
                        checkcount++;
                        break;
                    case "error: not found":
                    case "error: data broken":
                        ConsoleHelper.PrintError(" Master password config file broken, please reset...");
                        var resetresult = await ResetMP();
                        if (resetresult)
                        {
                            for (var i = 0; i < 11; i++)
                            {
                                ConsoleHelper.CleanConsoleLine(i);
                            }
                            ConsoleHelper.BackspaceToLine(11);
                            ConsoleHelper.PrintGreen(" Master password reset successfully, please login again.");
                            checkcount++;
                        }
                        else
                        {
                            ConsoleHelper.PrintError(" Master password reset failed, exiting...");
                            await Task.Delay(2000);
                            Environment.Exit(1);
                        }
                        break;
                    default:
                        ConsoleHelper.PrintError(" Unknown Error, please check and feedback to GoodPass team");
                        await Task.Delay(10000);
                        Environment.Exit('?');
                        break;
                }
            }
            while (checkcount <= 5);
            Console.WriteLine(" Too many attempts, exiting...");
            await Task.Delay(2000);
            Environment.Exit(1);
        }
        else
        {
            ConsoleHelper.PrintWarning(" Welcome to GoodPass CLI, please set master password first.");
            var setresult = await SetMP();
            if (setresult)
            {
                ConsoleHelper.PrintGreen(" Master password set successfully, please wait for app restart and login.");
                await Task.Delay(1000);
                System.Diagnostics.Process.Start(Path.Combine(Environment.CurrentDirectory, "GoodPass-CLI"));
                Environment.Exit(0);
            }
            else
            {
                ConsoleHelper.PrintError(" Master password set failed, exiting...");
                await Task.Delay(2000);
                Environment.Exit(1);
            }
        }
    #endregion
    mainprocess:
        Console.Clear();
        CommandLineServices.PrintStart();
        var loadStatus = GoodPass_CLI.manager.LoadFormFile(Path.Combine("Data", "GoodPassData.csv"));
        if (!loadStatus)
        {
            manager.AddData("Example", "", "test1", "ExamplePassword");
            manager.AddData("Example", "https://github.com/GeorgeDong32/GoodPass-v2", "test2", "ExamplePassword");
            manager.AddData("Example", "https://georgedong32.github.io/GoodPass/GoodPass-v2", "test3@test.com", "ExamplePassword");
        }
        ConsoleHelper.PrintGreen(" Master password is correct, welcome.");
        Console.Write(">>> ");
        while (true)
        {
            var command = Console.ReadLine();
            await CLS.CommandHandler(command);
            Console.Write(">>> ");
        }
    }

    public static async Task<bool> SetMP()
    {
        ConsoleHelper.PrintWarning(" Please make sure to set your master password in a secure environment!");
        Console.WriteLine(" Press \"E\" to exit. Press any other key to continue...");
        var input = Console.ReadKey(true);
        if (input.Key == ConsoleKey.E)
        {
            return false;
        }
        else
        {
            ConsoleHelper.CleanConsoleLine(1);
            ConsoleHelper.CleanConsoleLine(2);
            ConsoleHelper.CleanConsoleLine(3);
            ConsoleHelper.BackspaceToLine(3);
            Console.WriteLine(" Please enter a master password that meets the following requirements:\n" +
                              " - The password length is between 10-40 characters\n" +
                              " - The password should be complex enough");
            ConsoleHelper.PrintWarning(" - The password should not be the same as your usual password");
            ConsoleHelper.PrintWarning(" You should remember your master password, as if you lose it, you can't retrieve it!");
            Console.Write(" Password:");
            var mk1 = ConsoleHelper.ReadPassword();
            var mk1count = 1;
            while (string.IsNullOrEmpty(mk1) || mk1.Length < 10 || mk1.Length > 40)
            {
                if (mk1count > 5)
                {
                    ConsoleHelper.PrintError(" Too much try, exiting...");
                    await Task.Delay(2000);
                    Environment.Exit(1);
                }
                ConsoleHelper.CleanConsoleLine(1);
                ConsoleHelper.CleanConsoleLine(2);
                ConsoleHelper.BackspaceToLine(2);
                ConsoleHelper.PrintError(" The password does not meet the requirements, please try again");
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(" Password:");
                mk1 = ConsoleHelper.ReadPassword();
                mk1count++;
            }
            Console.Write(" Please enter your password again:");
            var mk2 = ConsoleHelper.ReadPassword();
            var resetcount = 1;
            while (mk2 != mk1)
            {
                if (resetcount + resetcount > 6)
                {
                    ConsoleHelper.PrintError(" Too much try, exiting...");
                    await Task.Delay(2000);
                    Environment.Exit(1);
                }
                ConsoleHelper.CleanConsoleLine(1);
                ConsoleHelper.CleanConsoleLine(2);
                ConsoleHelper.CleanConsoleLine(3);
                ConsoleHelper.BackspaceToLine(3);
                ConsoleHelper.PrintError(" Passwords do not match, please try again.");
                Console.Write(" Password:");
                mk1 = ConsoleHelper.ReadPassword();
                while (string.IsNullOrEmpty(mk1) || mk1.Length < 10 || mk1.Length > 40)
                {
                    if (mk1count + resetcount > 6)
                    {
                        ConsoleHelper.PrintError(" Too much try, exiting...");
                        await Task.Delay(2000);
                        Environment.Exit(1);
                    }
                    ConsoleHelper.CleanConsoleLine(1);
                    ConsoleHelper.CleanConsoleLine(2);
                    ConsoleHelper.BackspaceToLine(2);
                    ConsoleHelper.PrintWarning(" The password does not meet the requirements, please try again");
                    Console.Write(" Password:");
                    mk1 = ConsoleHelper.ReadPassword();
                    mk1count++;
                }
                Console.Write(" Please enter your password again:");
                mk2 = ConsoleHelper.ReadPassword();
                resetcount++;
            }
            MasterKeyService.SetLocalMKHash(mk1);
            return true;
        }
    }

    public static async Task<bool> ResetMP()
    {
        ConsoleHelper.PrintWarning(" Please make sure to reset your master password in a secure environment!");
        Console.WriteLine(" Press \"E\" to exit. Press any other key to continue...");
        var input = Console.ReadKey(true);
        if (input.Key == ConsoleKey.E)
        {
            return false;
        }
        else
        {
            Console.WriteLine(" Please enter a master password that meets the following requirements:\n" +
                              " - The password length is between 10-40 characters\n" +
                              " - The password should be complex enough");
            ConsoleHelper.PrintWarning(" - The password should not be the same as your usual password");
            ConsoleHelper.PrintWarning(" You should remember your master password, as if you lose it, you can't retrieve it!");
            Console.Write(" Password:");
            var mk1 = ConsoleHelper.ReadPassword();
            var mk1count = 1;
            while (string.IsNullOrEmpty(mk1) || mk1.Length < 10 || mk1.Length > 40)
            {
                if (mk1count > 5)
                {
                    ConsoleHelper.PrintError(" Too much try, exiting...");
                    await Task.Delay(2000);
                    Environment.Exit(1);
                }
                ConsoleHelper.CleanConsoleLine(1);
                ConsoleHelper.CleanConsoleLine(2);
                ConsoleHelper.BackspaceToLine(2);
                ConsoleHelper.PrintError(" The password does not meet the requirements, please try again");
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(" Password:");
                mk1 = ConsoleHelper.ReadPassword();
                mk1count++;
            }
            Console.Write(" Please enter your password again:");
            var mk2 = ConsoleHelper.ReadPassword();
            var resetcount = 1;
            while (mk2 != mk1)
            {
                if (resetcount + resetcount > 6)
                {
                    ConsoleHelper.PrintError(" Too much try, exiting...");
                    await Task.Delay(2000);
                    Environment.Exit(1);
                }
                ConsoleHelper.CleanConsoleLine(1);
                ConsoleHelper.CleanConsoleLine(2);
                ConsoleHelper.CleanConsoleLine(3);
                ConsoleHelper.BackspaceToLine(3);
                ConsoleHelper.PrintError(" Passwords do not match, please try again.");
                Console.Write(" Password:");
                mk1 = ConsoleHelper.ReadPassword();
                while (string.IsNullOrEmpty(mk1) || mk1.Length < 10 || mk1.Length > 40)
                {
                    if (mk1count + resetcount > 6)
                    {
                        ConsoleHelper.PrintError(" Too much try, exiting...");
                        await Task.Delay(2000);
                        Environment.Exit(1);
                    }
                    ConsoleHelper.CleanConsoleLine(1);
                    ConsoleHelper.CleanConsoleLine(2);
                    ConsoleHelper.BackspaceToLine(2);
                    ConsoleHelper.PrintError(" The password does not meet the requirements, please try again");
                    Console.Write(" Password:");
                    mk1 = ConsoleHelper.ReadPassword();
                    mk1count++;
                }
                Console.Write(" Please enter your password again:");
                mk2 = ConsoleHelper.ReadPassword();
                resetcount++;
            }
            MasterKeyService.SetLocalMKHash(mk1);
            return true;
        }
    }

    internal static void CancelKeyHandler(object? sender, ConsoleCancelEventArgs args)
    {
        Environment.Exit(0);
    }
}