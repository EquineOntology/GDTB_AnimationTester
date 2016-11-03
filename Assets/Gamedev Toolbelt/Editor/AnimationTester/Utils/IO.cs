using System.Collections.Generic;
using System.IO;
using System;

namespace com.immortalhydra.gdtb.animationtester
{
    public static class IO
    {

#region METHODS

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
                        writer.WriteLine("        [MenuItem(" + '"' + "Window/Gamedev Toolbelt/AnimationTester/Open AnimationTester " + aShortcut + '"' + ", false, 1)]");
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

#endregion

    }
}