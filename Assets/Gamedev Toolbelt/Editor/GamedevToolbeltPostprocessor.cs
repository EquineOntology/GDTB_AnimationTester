using UnityEditor;

// If you know what a postprocessor is and are worried about what this one will do: fear not!
// It just checks for adding/deleting files from the GamedevToolbelt, and add the relevant keys to EditorPrefs,
// so that the correct options are displayed in the "Options" window.
public class GamedevToolbeltPostprocessor : AssetPostprocessor 
{
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
	{
		foreach (var str in importedAssets)
		{
            if (str.EndsWith("CodeTODOs.cs"))
            {
				EditorPrefs.SetBool("GDTB_CodingTODOs_Enable", true);
                //Debug.Log(EditorPrefs.HasKey("GDTB_CodingTODOs_Enable"));
            }
        }
		foreach (var str in deletedAssets) 
		{
			if (str.EndsWith("CodeTODOs.cs"))
			{
                EditorPrefs.DeleteKey("GDTB_CodingTODOs_Enable");
				//Debug.Log(EditorPrefs.HasKey("GDTB_CodingTODOs_Enable"));
            }
		}
	}
}