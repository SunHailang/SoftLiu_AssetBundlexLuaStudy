using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class FileUtils
{
    /// <summary>
    /// 删除文件夹，及其文件内所有文件
    /// </summary>
    /// <param name="source">文件夹目录</param>
    /// <param name="self">是否包含自己</param>
    public static void DeleteDirectory(string source, bool self = false)
    {
        if (Directory.Exists(source))
        {
            DirectoryInfo info = new DirectoryInfo(source);
            foreach (var item in info.GetDirectories())
            {
                DeleteDirectory(item.FullName, true);
            }
            FileInfo[] files = info.GetFiles();
            for (int i = files.Length - 1; i >= 0; i--)
            {
                File.Delete(files[i].FullName);
            }
            if (self)
            {
                Directory.Delete(source);
            }
        }
    }

}
