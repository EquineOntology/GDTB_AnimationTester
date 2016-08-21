using System.Collections.Generic;
using System.IO;
using System;

namespace com.immortalhydra.gdtb.animationtester
{
    public static class IO
    {
        /// Return the first instance of the given folder.
        /// This is a non-recursive, breadth-first search algorithm.
        private static string GetFirstInstanceOfFolder(string aFolderName)
        {
            var projectDirectoryPath = Directory.GetCurrentDirectory();
            var projectDirectoryInfo = new DirectoryInfo(projectDirectoryPath);
            var listOfAssetsDirs = projectDirectoryInfo.GetDirectories("Assets");
            var assetsDir = "";
            foreach (var dir in listOfAssetsDirs)
            {
                if (dir.FullName.EndsWith("\\Assets"))
                {
                    assetsDir = dir.FullName;
                }
            }
            var path = assetsDir;

            var q = new Queue<string>();
            q.Enqueue(path);
            var absolutePath = "";
            while (q.Count > 0)
            {
                path = q.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        q.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.Log(ex.Message);
                    UnityEngine.Debug.Log(ex.Data);
                    UnityEngine.Debug.Log(ex.StackTrace);
                }

                string[] folders = null;
                try
                {
                    folders = Directory.GetDirectories(path);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.Log(ex.Message);
                    UnityEngine.Debug.Log(ex.Data);
                    UnityEngine.Debug.Log(ex.StackTrace);
                }

                if (folders != null)
                {
                    for (int i = 0; i < folders.Length; i++)
                    {
                        if (folders[i].EndsWith(aFolderName))
                        {
                            absolutePath = folders[i];
                        }
                    }
                }
            }
            var relativePath = absolutePath.Remove(0, projectDirectoryPath.Length + 1);
            return relativePath;
        }


        /// Get the path of a file based on the ending provided.
        private static string GetFilePath(string aPathEnd)
        {
            var assetsPaths = UnityEditor.AssetDatabase.GetAllAssetPaths();
            var filePath = "";
            foreach (var path in assetsPaths)
            {
                if (path.EndsWith(aPathEnd))
                {
                    filePath = path;
                    break;
                }
            }
            return filePath;
        }


        public static void OverwriteShortcut(string aShortcut)
        {
            var tempFile = Path.GetTempFileName();
            var file = GetFilePath("Gamedev Toolbelt/Editor/AnimationTester/WindowMain.cs");

            var writer = new StreamWriter(tempFile, false);
            var reader = new StreamReader(file);

            var line = "";
            try
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if(line.Contains("[MenuItem"))
                    {
                        writer.WriteLine("        [MenuItem(" + '"' + "Window/Gamedev Toolbelt/AnimationTester " + aShortcut + '"' + ")]");
                    }
                    else
                    {
                        writer.WriteLine(line);
                    }
                }
                reader.Close();
                writer.Close();

                // Overwrite the old file with the temp file.
                File.Delete(file);
                File.Move(tempFile, file);
                UnityEditor.AssetDatabase.ImportAsset(file);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.Message);
                UnityEngine.Debug.Log(ex.Data);
                UnityEngine.Debug.Log(ex.StackTrace);
                reader.Dispose();
                writer.Dispose();
            }
        }
    }
}