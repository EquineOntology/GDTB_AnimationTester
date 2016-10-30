using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.animationtester
{
	public class LeaveReview : MonoBehaviour
	{

#region METHODS

	[MenuItem("Window/Gamedev Toolbelt/AnimationTester/★ Leave a review ★")]
	private static void GoToAssetStorePage()
	{
		Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:15617&src=animationtester_menu");
	}

#endregion

	}
}
