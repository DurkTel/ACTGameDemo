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
        EditorGUILayout.LabelField("�������֣�");
        m_newName = EditorGUILayout.TextField(m_newName);

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("���ɼ���ʱ����"))
        {
            CreateSkillTimeLine();
        }

        if (GUILayout.Button("������������"))
        {
            ExportSkillData();
        }

        EditorGUILayout.EndVertical();

    }

    private void CreateSkillTimeLine()
    {
        if (string.IsNullOrEmpty(m_newName))
        {
            EditorUtility.DisplayDialog("����", "�������¼�������", "ȷ��");
            return;
        }

        string skillAssetsFolder = string.Format("Assets/Prefabs/SkillTimeLines/{0}/", m_newName);
        bool isExist = File.Exists(skillAssetsFolder);
        if (!isExist)
            Directory.CreateDirectory(skillAssetsFolder);
        else
        {
            EditorUtility.DisplayDialog("����", "�Ѵ�����ͬ����", "ȷ��");
            return;
        }
        SkillTimeLine timelineAsset = ScriptableObject.CreateInstance<SkillTimeLine>();
        AssetDatabase.CreateAsset(timelineAsset, skillAssetsFolder + string.Format("{0}.playable", m_newName));
        GameObject gameObject = new GameObject();
        PlayableDirector playableDirector = gameObject.AddComponent<PlayableDirector>();
        playableDirector.playableAsset = timelineAsset;

        timelineAsset.CreateTrack<SkillAnimationTrack>("���ܶ������");
        timelineAsset.CreateTrack<SkillPerformTrack>("�����ݳ����");
        timelineAsset.CreateTrack<SkillHitTrack>("���ܴ�����");
        timelineAsset.CreateTrack<SkillParamTrack>("���ܲ������");

        bool isSavePrefabSuccess = false;
        PrefabUtility.SaveAsPrefabAsset(gameObject, skillAssetsFolder + string.Format("{0}.prefab", m_newName), out isSavePrefabSuccess);
        Debug.LogFormat("Ԥ�Ƽ�����ɹ�{0}", isSavePrefabSuccess);
        AssetDatabase.SaveAssets();
        GameObject.DestroyImmediate(gameObject);
        Debug.Log("�����������");
    }


    public void ExportSkillData()
    {
        Object select = Selection.activeObject;
        if (select == null || !(select is SkillTimeLine))
        {
            EditorUtility.DisplayDialog("����", "��ѡ�м���SkillTimeLine", "ȷ��");
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
                EditorUtility.DisplayDialog("����", "�Ѵ�����ͬ����", "ȷ��");
                return;
            }

            AssetDatabase.CreateAsset(skillObj, string.Format("Assets/SO/{0}/{1}.asset", select.name, select.name));
            Debug.Log("�����������");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}
