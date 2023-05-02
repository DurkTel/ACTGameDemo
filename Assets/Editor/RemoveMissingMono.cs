using UnityEditor;
using UnityEngine;

public class RemoveMissingMono : Editor
{
    [MenuItem("Tools/删除选中GameObject的丢失的Mono脚本")]
    public static void CleanupSelectedPrefabs()
    {
        foreach (var s in Selection.gameObjects)
        {
            var assetPath = AssetDatabase.GetAssetPath(s);
            var go = PrefabUtility.LoadPrefabContents(assetPath);
            ClearUp(go);
            PrefabUtility.SaveAsPrefabAsset(go, assetPath);
            PrefabUtility.UnloadPrefabContents(go);
        }

        void ClearUp(GameObject go)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

            for (int i = 0; i < go.transform.childCount; i++)
            {
                ClearUp(go.transform.GetChild(i).gameObject);
            }
        }
    }
}
