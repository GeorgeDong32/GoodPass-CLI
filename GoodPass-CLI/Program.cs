using GoodPass_CLI.Services;
using GoodPass_CLI.Models;

namespace GoodPass_CLI;

public static class GoodPass_CLI
{
    public static readonly string _version = "3.2.0";
    public static int[] EncryptBase;
    public static byte[] AESKey;
    public static byte[] AESIV;
    public static GPManager manager = new GPManager();
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
        #region test data
        MasterKeyService.ProcessMKArray("TestGood320Pass", out EncryptBase);
        AESIV = GoodPassAESServices.GetLocalIV("TestGood320Pass");
        AESKey = GoodPassAESServices.GenerateKey("TestGood320Pass", AESIV);
        manager.AddData("Example", "", "test1", "testpwjfnasjkda5484");
        manager.AddData("Example", "", "test2", "4656dbajdkan#eeqbiuqh%");
        manager.AddData("Example", "https://georgedong32.github.io/GoodPass/GoodPass-v2", "test#@test.com", "4656dbajdkan#eeqbiuqh%");
        #endregion
        //manager.LoadFormFile(Path.Combine($"C:\\Users\\{Environment.UserName}\\AppData\\Local", "GoodPass", "GoodPass-CLI", "GoodPassData.csv"));
        CommandLineServices.PrintStart();
        Console.Write(">>> ");
        while (true)
        {
            var command = Console.ReadLine();
            await CLS.CommandHandler(command);
            Console.Write(">>> ");
        }
    }

    internal static async void CancelKeyHandler(object? sender, ConsoleCancelEventArgs args)
    {
        await ExitFunction();
        Environment.Exit(0);
    }

    internal static async Task<bool> ExitFunction()
    {
        Console.WriteLine();
        Console.WriteLine("Leaving GoodPass CLI, please wait...");
        //TODO: 保存数据
        await Task.Delay(1000);
        return true;
    }
}