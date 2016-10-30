using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.animationtester
{
	public class OtherAssetsFromIH : MonoBehaviour
	{

#region METHODS

	[MenuItem("Window/Gamedev Toolbelt/AnimationTester/Our other Assets")]
	private static void GoToAssetStorePage()
	{
		Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/70010?src=animationtester_menu");
	}

#endregion

	}
}
