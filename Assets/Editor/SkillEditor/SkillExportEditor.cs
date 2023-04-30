using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEditor.VersionControl;
using SkillEditor;
using System.IO;
using UnityEngine.Playables;

public class SkillExportEditor : EditorWindow
{

    private string m_newName;

    [MenuItem("Tools/SkillEditor")]
    static void Init()
    {

        SkillExportEditor actionEditor = (SkillExportEditor)EditorWindow.GetWindow(typeof(SkillExportEditor));
        actionEditor.titleContent = new GUIContent("ActionEditor");
        actionEditor.Show();

    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("技能名字：");
        m_newName = EditorGUILayout.TextField(m_newName);

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("生成技能时间轴"))
        {
            CreateSkillTimeLine();
        }

        if (GUILayout.Button("导出技能数据"))
        {
            ExportSkillData();
        }

        EditorGUILayout.EndVertical();

    }

    private void CreateSkillTimeLine()
    {
        if (string.IsNullOrEmpty(m_newName))
        {
            EditorUtility.DisplayDialog("警告", "请输入新技能名字", "确定");
            return;
        }

        string skillAssetsFolder = string.Format("Assets/Prefabs/SkillTimeLines/{0}/", m_newName);
        bool isExist = File.Exists(skillAssetsFolder);
        if (!isExist)
            Directory.CreateDirectory(skillAssetsFolder);
        else
        {
            EditorUtility.DisplayDialog("警告", "已存在相同技能", "确定");
            return;
        }
        SkillTimeLine timelineAsset = ScriptableObject.CreateInstance<SkillTimeLine>();
        AssetDatabase.CreateAsset(timelineAsset, skillAssetsFolder + string.Format("{0}.playable", m_newName));
        GameObject gameObject = new GameObject();
        PlayableDirector playableDirector = gameObject.AddComponent<PlayableDirector>();
        playableDirector.playableAsset = timelineAsset;

        timelineAsset.CreateTrack<SkillAnimationTrack>("技能动画轨道");
        timelineAsset.CreateTrack<SkillPerformTrack>("技能演出轨道");
        timelineAsset.CreateTrack<SkillHitTrack>("技能打击轨道");
        timelineAsset.CreateTrack<SkillParamTrack>("技能参数轨道");

        bool isSavePrefabSuccess = false;
        PrefabUtility.SaveAsPrefabAsset(gameObject, skillAssetsFolder + string.Format("{0}.prefab", m_newName), out isSavePrefabSuccess);
        Debug.LogFormat("预制件保存成功{0}", isSavePrefabSuccess);
        AssetDatabase.SaveAssets();
        GameObject.DestroyImmediate(gameObject);
        Debug.Log("技能生成完成");
    }


    public void ExportSkillData()
    {
        Object select = Selection.activeObject;
        if (select == null || !(select is SkillTimeLine))
        {
            EditorUtility.DisplayDialog("警告", "请选中技能SkillTimeLine", "确定");
            return;
        }

        if (select != null && select is SkillTimeLine)
        {
            SkillTimeLine asset = select as SkillTimeLine;
            CombatSkillConfig skillObj = ScriptableObject.CreateInstance<CombatSkillConfig>();
            double totalDuration = asset.duration;

            foreach (TrackAsset track in asset.GetOutputTracks())
            {
                if (track is SkillAnimationTrack)
                {
                    SkillAnimationTrack ani = track as SkillAnimationTrack;
                    skillObj.skillName = ani.skillAnimationName;
                }
                else if (track is SkillParamTrack)
                {
                    foreach (var item in track.GetClips())
                    {
                        SkillParamClip skillParam = item.asset as SkillParamClip;
                        skillObj.effectCount = skillParam.effectCount;
                        skillObj.autoLock = skillParam.autoLock;
                        skillObj.force = skillParam.force;
                        skillObj.priority = skillParam.priority;
                        skillObj.tag = skillParam.tag;
                        skillObj.condition = skillParam.condition;
                    }
                }
                else if (track is SkillPerformTrack)
                {
                    foreach (var item in track.GetClips())
                    {
                        if (item.asset is SkillPerformPiontClip)
                            skillObj.attackPoint = (float)(item.end / totalDuration);
                        else if (item.asset is SkillPerformBackswingClip)
                            skillObj.attackBackswing = (float)(item.start / totalDuration);
                    }

                }
            }


            string skillAssetsFolder = string.Format("Assets/SO/{0}/", select.name);
            if (!File.Exists(skillAssetsFolder))
                Directory.CreateDirectory(skillAssetsFolder);
            else
            {
                EditorUtility.DisplayDialog("警告", "已存在相同技能", "确定");
                return;
            }

            AssetDatabase.CreateAsset(skillObj, string.Format("Assets/SO/{0}/{1}.asset", select.name, select.name));
            Debug.Log("导出技能完成");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}
