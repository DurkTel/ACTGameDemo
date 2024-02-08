using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetManifest : ScriptableObject, ISerializationCallbackReceiver
{
    public const string editorPath = "Assets/Plugins/EditorAssetManifest.asset";
    public const string resourcesPath = "Assets/Plugins/ResourcesAssetManifest.asset";
    [System.Serializable]
    public class AssetInfo
    {
        public string assetName;
        public string assetPath;
        public AssetInfo(string assetName, string assetPath)
        {
            this.assetName = assetName;
            this.assetPath = assetPath;
        }
    }
    public List<AssetInfo> assetList = new List<AssetInfo>(5000);

    public Dictionary<string, string> assetMap = new Dictionary<string, string>(5000);

    public void OnBeforeSerialize()
    {
        assetList.Clear();
        foreach (var item in this.assetMap)
            assetList.Add(new AssetInfo(item.Key, item.Value));
    }

    public void OnAfterDeserialize()
    {
        foreach (var item in assetList)
            if (!this.assetMap.ContainsKey(item.assetName))
                this.assetMap.Add(item.assetName, item.assetPath);
    }

#if UNITY_EDITOR
    [MenuItem("Assets/AssetsManifest/RefreshEditorAssets")]
    public static void RefreshEditorAssetsManifest()
    {
        AssetManifest assetManifest = GetAssetManifest(editorPath);
        assetManifest.Clear();

        string[] allfile = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
        int spos = Application.dataPath.Length - 6;
        foreach (var item in allfile)
        {
            string path = item.Substring(spos).Replace("\\", "/");
            if (path.StartsWith("Assets"))
            {
                string ex = Path.GetExtension(path);
                if (ex != ".meta" && ex != ".cs")
                { 
                    assetManifest.Add(path);
                }
            }
        }
        EditorUtility.SetDirty(assetManifest);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("������Դ�嵥���");
    }

    [MenuItem("Assets/AssetsManifest/RefreshResourceAssets")]
    public static void RefreshResourceAssetsManifest()
    {
        AssetManifest assetManifest = GetAssetManifest(resourcesPath);
        assetManifest.Clear();

        string assetPath = Application.dataPath + "/Resources";
        if (!Directory.Exists(assetPath)) return;
        string[] allfile = Directory.GetFiles(assetPath, "*", SearchOption.AllDirectories);
        int spos = Application.dataPath.Length + 11;
        foreach (var item in allfile)
        {
            string path = item.Substring(spos).Replace("\\", "/");
            string ex = Path.GetExtension(path);
            if (ex != ".meta" && ex != ".cs")
            {
                assetManifest.Add(path, true);
            }
        }
        EditorUtility.SetDirty(assetManifest);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("������Դ�嵥���");
    }

    public static AssetManifest GetAssetManifest(string path)
    {
        if (!Directory.Exists(Application.dataPath + "/Plugins"))
            Directory.CreateDirectory(Application.dataPath + "/Plugins");
        AssetManifest assetManifest = AssetDatabase.LoadAssetAtPath<AssetManifest>(path);

        if (assetManifest == null)
        {
            assetManifest = ScriptableObject.CreateInstance<AssetManifest>();
            AssetDatabase.CreateAsset(assetManifest, path);
        }

        return assetManifest;
    }

    public void Add(string assetPath, bool removeExtension = false)
    {
        string assetName = Path.GetFileName(assetPath);

        string newPath = removeExtension ? assetPath.Replace(Path.GetExtension(assetPath), "") : assetPath;

        if (assetMap.ContainsKey(assetName))
            assetMap[assetName] = newPath;
        else
            assetMap.Add(assetName, newPath);
    }

    public bool Contains(string assetName)
    {
        return assetMap.ContainsKey(assetName);
    }

    public string GetPath(string assetName)
    {
        if (assetMap.ContainsKey(assetName))
            return assetMap[assetName];

        Debug.LogWarning("��Դ�嵥��û����Ϊ��" + assetName + "����Դ���������Դ�嵥������Դ����");
        return string.Empty;
    }

    public void Clear()
    {
        assetMap.Clear();
    }
#endif

}