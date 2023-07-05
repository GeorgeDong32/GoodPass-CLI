namespace GoodPass_CLI.Services;

/// <summary>
/// Provide services such as verifying the master password and setting the master password.
/// </summary>
public static class MasterKeyService
{
    #region Get/Set/Check Masterkey
    /// <summary>
    /// 设置本地哈希校验值
    /// </summary>
    /// <param name="MasterKey">主密码</param>
    public static bool SetLocalMKHash(string MasterKey)
    {
        var dataPath = Path.Combine("Data");
        var MKconfigPath = Path.Combine(dataPath, "MKconfig.txt");
        if (!System.IO.Directory.Exists(dataPath))
        {
            System.IO.Directory.CreateDirectory(dataPath);
            if (System.IO.Directory.Exists(dataPath))
            {
                System.IO.File.Create(MKconfigPath).Close();
                if (System.IO.File.Exists(MKconfigPath))
                {
                    System.IO.File.WriteAllText(MKconfigPath, GoodPassSHAServices.getGPHES(MasterKey));
                    return true;
                }
                else
                {
                    return false;
                    throw new Exception("Failed to create config file!");
                }
            }
            else
            {
                return false;
                throw new Exception("Failed to create data folder!");
            }
        }
        else if (System.IO.Directory.Exists(dataPath))
        {
            if (!System.IO.File.Exists(MKconfigPath))
            {
                System.IO.File.Create(MKconfigPath).Close();
                if (System.IO.File.Exists(MKconfigPath))
                {
                    System.IO.File.WriteAllText(MKconfigPath, GoodPassSHAServices.getGPHES(MasterKey));
                    return true;
                }
                else
                {
                    return false;
                    throw new Exception("Failed to create config file!");
                }
            }
            else
            {
                System.IO.File.WriteAllText(MKconfigPath, GoodPassSHAServices.getGPHES(MasterKey));
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// (异步)获取本地主密码哈希校验值
    /// </summary>
    /// <returns>本地哈希校验值</returns>
    public static async Task<string> GetLocalMKHashAsync()/*ToDo：通过RATAsync的异常机制精简方法*/
    {
        var MKconfigPath = Path.Combine("Data", "MKconfig.txt");
        var localMKHash = string.Empty;
        try
        {
            //使用部分同步方法用以解决异步方法不抛出异常的问题
            var tryreadfile = File.ReadAllText(MKconfigPath);
            localMKHash = await File.ReadAllTextAsync(MKconfigPath);
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            localMKHash = "Not found";
        }
        catch (System.IO.FileNotFoundException)
        {
            localMKHash = "Not found";
        }
        finally
        {
            if (localMKHash == String.Empty)
                localMKHash = "Empty";
        }
        return localMKHash;
    }

    /// <summary>
    /// (封装的异步)校验主密码方法
    /// </summary>
    /// <param name="inputKey">输入的主密码</param>
    /// <returns>校验结果</returns>
    public static string CheckMasterKeyAsync(string? inputKey, out byte[] AESIV, out byte[] AESKey, out int[] MKBase)
    {
        if (string.IsNullOrEmpty(inputKey))
        {
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESIV = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESKey = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            MKBase = new int[40] { 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3, 8, 4, 6, 2, 6, 4, 3, 3, 8, 3, 2, 7, 9, 5, 0, 2, 8, 8, 4, 1, 9, 7, 1 };
            return "npass";
        }
        var InputKeyHash = GoodPassSHAServices.getGPHES(inputKey);
        var LocalMKHash = GetLocalMKHashAsync().Result;
        if (InputKeyHash == LocalMKHash)
        {
            AESIV = GoodPassAESServices.GetLocalIV(inputKey);
            AESKey = GoodPassAESServices.GenerateKey(inputKey, AESIV);
            ProcessMKArray(inputKey, out MKBase);
            return "pass";
        }
        else if (LocalMKHash == "Not found")
        {
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESIV = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESKey = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            MKBase = new int[40] { 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3, 8, 4, 6, 2, 6, 4, 3, 3, 8, 3, 2, 7, 9, 5, 0, 2, 8, 8, 4, 1, 9, 7, 1 };
            return "error: not found";
        }
        else if (LocalMKHash == "Empty")
        {
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESIV = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESKey = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            MKBase = new int[40] { 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3, 8, 4, 6, 2, 6, 4, 3, 3, 8, 3, 2, 7, 9, 5, 0, 2, 8, 8, 4, 1, 9, 7, 1 };
            return "error: data broken";
        }
        else if (InputKeyHash != LocalMKHash)
        {
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESIV = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESKey = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            MKBase = new int[40] { 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3, 8, 4, 6, 2, 6, 4, 3, 3, 8, 3, 2, 7, 9, 5, 0, 2, 8, 8, 4, 1, 9, 7, 1 };
            return "npass";
        }
        else
        {
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESIV = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            AESKey = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            MKBase = new int[40] { 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3, 8, 4, 6, 2, 6, 4, 3, 3, 8, 3, 2, 7, 9, 5, 0, 2, 8, 8, 4, 1, 9, 7, 1 };
            return "Unknown Error";
        }
    }

    #endregion

    #region Process Masterkey Array
    /// <summary>
    /// 生成App的加密基和主密码基
    /// </summary>
    public static void ProcessMKArray(string inputKey, out int[] mkBase)
    {
        var encryptBase = new int[40] { 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3, 8, 4, 6, 2, 6, 4, 3, 3, 8, 3, 2, 7, 9, 5, 0, 2, 8, 8, 4, 1, 9, 7, 1 };
        mkBase = encryptBase;
        var MaxLength = Math.Min(40, inputKey.Length);
        for (var i = 0; i < MaxLength; i++)
        {
            var key = inputKey[i];
            if (key >= 'a' && key <= 'z')
            {
                var temp = key - 'a';
                while (temp >= 10)
                {
                    mkBase[i] = temp / 10;
                    i++;
                    temp %= 10;
                }
                mkBase[i] = temp;
            }
            else if (key >= 'A' && key <= 'Z')
            {
                var temp = key - 'A';
                while (temp >= 10)
                {
                    mkBase[i] = temp / 10;
                    i++;
                    temp %= 10;
                }
                mkBase[i] = temp;
            }
            else if (key >= '0' && key <= '9')
            {
                mkBase[i] = key - '0';
            }
            else
            {
                mkBase[i] = encryptBase[i];
            }
            if (i >= 40)//防止溢出
            {
                break;
            }
        }
    }
    #endregion

    #region Check Masterkey Only
    public static string CheckMasterKey_NP(string inputKey)
    {
        var inputKeyHash = GoodPassSHAServices.getGPHES(inputKey);
        var LocalMKHash = GetLocalMKHashAsync().Result;
        if (string.IsNullOrEmpty(inputKey))
        {
            return "npass";
        }
        else if (inputKeyHash == LocalMKHash)
        {
            return "pass";
        }
        else if (LocalMKHash == "Not found")
        {
            return "error: not found";
        }
        else if (LocalMKHash == String.Empty)
        {
            return "error: data broken";
        }
        else if (inputKey != LocalMKHash)
        {
            return "npass";
        }
        else
        {
            return "Unknown Error";
        }
    }

    #endregion
}
