using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public static class PrefsTool
{
    /// <summary>
    /// 查找满足条件的单个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static T Find<T>
        (this T[] array, Func<T, bool> condition)
    {       
        for (int i = 0; i < array.Length; i++)
        {
            if (condition(array[i]))
            {
                return array[i];
            }
        }

        return default(T);
    }


    public static T JsonFileToObj<T>(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        T data = JsonToObj<T>(jsonContent);
        return data;
    }

    public static T JsonToObj<T>(string jsonContent)
    {
        T data = JsonConvert.DeserializeObject<T>(jsonContent);
        return data;
    }

    public static void ObjToPrefs<T>(T obj, string filePath)
    {
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
           
        File.WriteAllText(filePath, json);
    }

    public static string ObjToJson<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }

    public static string GenerateMD5FromFile(string file)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(file))
            {
                byte[] retVal = md5.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }

    public static string GetZipLength(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);

        return fileStream.Length.ToString();
    }
}