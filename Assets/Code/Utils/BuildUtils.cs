﻿using SoftLiu.Misc;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class BuildUtils
{
    public static void HandleAndroidXml(string path)
    {
        string unityLibraryManifest = Path.Combine(path, "unityLibrary/src/main/AndroidManifest.xml");
        string writeManifestLines;
        using (StreamReader sr = new StreamReader(unityLibraryManifest))
        {
            string readLines = sr.ReadToEnd();
            int indexStart = readLines.IndexOf("<activity");
            int indexEnd = readLines.LastIndexOf("</activity>");
            if (indexStart >= 0 && indexEnd >= 0)
            {
                string startData = readLines.Substring(0, indexStart);
                string endData = readLines.Substring(indexEnd + 11);
                writeManifestLines = startData + endData;
            }
            else
            {
                writeManifestLines = readLines;
            }
        }
        using (StreamWriter sw = new StreamWriter(unityLibraryManifest))
        {
            sw.Write(writeManifestLines);
        }
        string unityStringXml = Path.Combine(path, "launcher/src/main/res/values/strings.xml");
        string writeStringLines;
        using (StreamReader sr = new StreamReader(unityStringXml))
        {
            string readData = sr.ReadToEnd();
            writeStringLines = readData.Replace(Application.productName, "孙海浪");
        }
        using (StreamWriter sw = new StreamWriter(unityStringXml))
        {
            sw.Write(writeStringLines);
        }
    }

    public static void HandleGradleVersion(string gradlePath, BuildVersionData gradleData)
    {
        if (!File.Exists(gradlePath) || gradleData == null) return;

        Type type = typeof(SoftLiu.Misc.BuildVersionData);
        PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        StringBuilder writeLines = new StringBuilder();
        using (StreamReader sr = new StreamReader(File.OpenRead(gradlePath)))
        {
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                var infos = propertyInfos.Where(item => { return line.Contains("ext." + item.Name.Substring(2)); });
                if (infos != null && infos.FirstOrDefault() != null)
                {
                    var info = infos.FirstOrDefault();
                    if (info.PropertyType == typeof(string))
                    {
                        writeLines.Append(string.Format("ext.{0} = \"{1}\"\n", info.Name.Substring(2), info.GetValue(gradleData).ToString()));
                    }
                    else
                    {
                        writeLines.Append(string.Format("ext.{0} = {1}\n", info.Name.Substring(2), info.GetValue(gradleData).ToString()));
                    }
                }
                else
                {
                    writeLines.Append(line + "\n");
                }
            }
        }
        if (File.Exists(gradlePath))
        {
            File.Delete(gradlePath);
        }
        File.WriteAllText(gradlePath, writeLines.ToString());
    }

    public static void RunGradleProcess(string buildPath, string gradleBuildType, string packageType = "assemble")
    {
        //FixPermission
        string executable = Path.Combine(buildPath, "gradlew.bat");
        string arguments = packageType + gradleBuildType;
        if (System.Environment.OSVersion.Platform == System.PlatformID.MacOSX
            || System.Environment.OSVersion.Platform == System.PlatformID.Unix)
        {
            executable = Path.Combine(buildPath, "gradlew");
        }
        // Run Python to start build
        using (Process proc = new Process())
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo();
            procStartInfo.FileName = executable;
            procStartInfo.Arguments = arguments;
            procStartInfo.UseShellExecute = false;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.WorkingDirectory = buildPath;
            procStartInfo.CreateNoWindow = true;
            UnityEngine.Debug.Log("RunGradleProcess: " + executable + " " + arguments);
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            string gradleLogPath = Path.Combine(buildPath, "../gradle_" + packageType + ".log");
            string gradleErrorLogPath = Path.Combine(buildPath, "../gradle_error_" + packageType + ".log");
            if (result.Length > 1)
            {
                if (File.Exists(gradleLogPath))
                {
                    File.Delete(gradleLogPath);
                }
                File.WriteAllText(gradleLogPath, result);
            }
            if (error.Length > 1)
            {
                UnityEngine.Debug.LogError("Run Gradle Error: " + error);
                if (File.Exists(gradleErrorLogPath))
                {
                    File.Delete(gradleErrorLogPath);
                }
                File.WriteAllText(gradleErrorLogPath, error);
            }
        }
    }

    public static void FixPermissionsForDirectory(string path)
    {
        try
        {
            Process proc = new Process();
#if UNITY_EDITOR_OSX
            proc.StartInfo.FileName = "chmod";
            proc.StartInfo.Arguments = string.Format("-R 777 {0}", path);
#else
            proc.StartInfo.FileName = Path.Combine(Application.dataPath, "../Tools/FixPermissions.exe");
            proc.StartInfo.Arguments = string.Format("{0}", path);
#endif
            proc.StartInfo.WorkingDirectory = Application.dataPath;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;

            if (proc.Start())
            {
                proc.WaitForExit();
            }
            else
            {
                UnityEngine.Debug.LogError("Could not start permission tool. " + path);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Fix permissions exception... Non critical... " + e.Message);
        }
    }
}
