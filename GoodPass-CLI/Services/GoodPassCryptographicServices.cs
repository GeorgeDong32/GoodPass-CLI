﻿namespace GoodPass_CLI.Services;

/// <summary>
/// 提供GPSES服务，用于对数据进行加解密
/// </summary>
public static class GoodPassCryptographicServices
{
    #region Decrypt Methods
    /// <summary>
    /// (基础的)解密输入的字符串
    /// </summary>
    /// <param name="ciphertext">待解密的字符串</param>
    /// <param name="cryptBase">加密基</param>
    /// <returns>解密结果</returns>
    /// <exception cref="ArgumentNullException">输入字符串为空</exception>
    public static string DecryptStr(string ciphertext)
    {
        if (ciphertext == null || ciphertext == string.Empty)
        {
            throw new ArgumentNullException("DecryptStr: input is null or empty");
        }
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        ciphertext = GoodPassAESServices.DecryptFromBase64(ciphertext, GoodPass_CLI.AESKey, GoodPass_CLI.AESIV);
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        var decStr = "";
        var baseStr = "";
        var cryptBase = GoodPass_CLI.EncryptBase;
        //初始化数组
        var NumPos = new int[41];
        var SpecPos = new int[41];
        Array.Fill(NumPos, -1);
        Array.Fill(SpecPos, -1);
        //找数字位置
        NumPos[0] = (int)ciphertext[0] - 'A';
        for (var i = 1; i <= NumPos[0]; i++)
        {
            switch (i % 2)
            {
                case 0:
                    NumPos[i] = (int)ciphertext[i] - 97;
                    break;
                case 1:
                    NumPos[i] = (int)ciphertext[i] - 65;
                    break;
            }
        }
        //找特殊字符位置
        var retemp = ciphertext.ToCharArray();
        Array.Reverse(retemp);
        var reinput = new string(retemp);
        SpecPos[0] = (int)reinput[0] - 'A';
        for (var i = 1; i <= SpecPos[0]; i++)
        {
            SpecPos[i] = (int)reinput[i] - 65;
        }
        var baseLength = ciphertext.Length - NumPos[0] - SpecPos[0] - 2;
        baseStr = ciphertext.Substring(NumPos[0] + 1, baseLength);

        for (var i = 0; i < baseLength; i++)
        {
            if (Array.FindIndex(NumPos, 1, p => p == i) != -1)//判断是否为数字
            {
                switch (i % 2)
                {
                    case 0:
#pragma warning disable CS8602 // 解引用可能出现空引用。
                        decStr += (char)(baseStr[i] - 'e' - cryptBase[i] + '0');
#pragma warning restore CS8602 // 解引用可能出现空引用。
                        break;
                    case 1:
#pragma warning disable CS8602 // 解引用可能出现空引用。
                        decStr += (char)(baseStr[i] - 'O' - cryptBase[i] + '0');
#pragma warning restore CS8602 // 解引用可能出现空引用。
                        break;
                }
            }
            else if (Array.FindIndex(SpecPos, 1, p => p == i) != -1)
            {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                decStr += (char)(baseStr[i] - cryptBase[i]);
#pragma warning restore CS8602 // 解引用可能出现空引用。
            }
            else
            {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                var temp = baseStr[i] - cryptBase[i];
#pragma warning restore CS8602 // 解引用可能出现空引用。
                if (temp >= 65 && temp <= 90)
                {
                    decStr += (char)(temp + 32);
                }
                else
                {
                    decStr += (char)(temp - 32);
                }
            }
        }
        return decStr;
    }
    #endregion

    #region Encrypt Methods
    /// <summary>
    /// (基础的)加密输入字符串
    /// </summary>
    /// <param name="plaintext">待加密的字符串</param>
    /// <param name="cryptBase">加密基</param>
    /// <returns>加密结果</returns>
    public static string EncryptStr(string plaintext)
    {
        if (plaintext == null || plaintext == string.Empty)
        {
            throw new ArgumentNullException("EncryptStr: input is null or empty");
        }
        //初始化数组
        var NumPos = new int[41];
        var SpecPos = new int[41];
        Array.Fill(NumPos, 0);
        Array.Fill(SpecPos, 0);
        var cryptBase = GoodPass_CLI.EncryptBase;
        //找数字位置
        var Strlength = plaintext.Length;
        var npCount = 1;
        var specCount = 1;
        var output = "";
        for (var i = 0; i < Strlength; i++)
        {
            if ((int)plaintext[i] >= 48 && (int)plaintext[i] <= 57)
            {
                NumPos[npCount] = i;
                npCount++;
                NumPos[0]++;
            }
        }
        //全串加密
        for (var i = 0; i < Strlength; i++)
        {
            var temp = (int)plaintext[i];
            //数字加密
            if (temp >= 48 && temp <= 57)
            {
                if (i % 2 == 0)
                {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    temp = 'e' + temp - '0' + cryptBase[i];
#pragma warning restore CS8602 // 解引用可能出现空引用。
                }
                else
                {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    temp = 'O' + temp - '0' + cryptBase[i];
#pragma warning restore CS8602 // 解引用可能出现空引用。
                }

                output += (char)temp;
            }
            //大写字母加密
            else if (temp >= 65 && temp <= 90)
            {
                temp += 32;
#pragma warning disable CS8602 // 解引用可能出现空引用。
                output += (char)(temp + cryptBase[i]);
#pragma warning restore CS8602 // 解引用可能出现空引用。
            }
            //小写字母加密
            else if (temp >= 97 && temp <= 122)
            {
                temp -= 32;
#pragma warning disable CS8602 // 解引用可能出现空引用。
                output += (char)(temp + cryptBase[i]);
#pragma warning restore CS8602 // 解引用可能出现空引用。
            }
            //特殊字符
            else
            {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                output += (char)(temp + cryptBase[i]);
#pragma warning restore CS8602 // 解引用可能出现空引用。
                SpecPos[specCount] = i;
                specCount++;
                SpecPos[0]++;
            }
        }
        //生成指示串头
        var head = "";
        head += (char)(NumPos[0] + 'A');
        for (var i = 1; i <= NumPos[0]; i++)
        {
            switch (i % 2)
            {
                case 0:
                    head += (char)(NumPos[i] + 97);
                    break;
                case 1:
                    head += (char)(NumPos[i] + 65);
                    break;
            }
        }
        //生成指示串尾
        var tail = "";
        for (var i = 1; i <= SpecPos[0]; i++)
        {
            tail += (char)(SpecPos[i] + 65);
        }
        tail += (char)(SpecPos[0] + 65);
        output = head + output + tail;
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        output = GoodPassAESServices.EncryptToBase64(output, GoodPass_CLI.AESKey, GoodPass_CLI.AESIV);
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        return output;
    }
    #endregion
}