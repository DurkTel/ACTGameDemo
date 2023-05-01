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

    private double m_totalDuration;

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
            m_totalDuration = asset.duration;

            foreach (TrackAsset track in asset.GetOutputTracks())
            {
                if (track is SkillAnimationTrack)
                    InitAnimationTrack(track as SkillAnimationTrack, skillObj);
                else if (track is SkillHitTrack)
                    InitHitTrack(track as SkillHitTrack, skillObj);
                else if (track is SkillParamTrack)
                    InitParamTrack(track as SkillParamTrack, skillObj);
                else if (track is SkillPerformTrack)
                    InitPerformTrack(track as SkillPerformTrack, skillObj);
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


    private void InitAnimationTrack(SkillAnimationTrack track, CombatSkillConfig skillObj)
    {
        skillObj.animationName = track.skillAnimationName;
    }

    private void InitHitTrack(SkillHitTrack track, CombatSkillConfig skillObj)
    {
        skillObj.hits ??= new List<HitStruct>();
        skillObj.hits.Clear();
        foreach (var item in track.GetClips())
        {
            SkillHitClip skillHit = item.asset as SkillHitClip;

            HitStruct hit = new HitStruct();
            hit.start = (item.start / m_totalDuration);
            hit.end = (item.end / m_totalDuration);
            hit.effectCount = skillHit.effectCout;
            hit.repulsionDistance = skillHit.repulsionDistance;
            hit.strikeFly = skillHit.strikeFly;
            hit.shakeOrient = skillHit.shakeOrient;
            hit.period = skillHit.period;
            hit.shakeTime = skillHit.shakeTime;
            hit.maxWave = skillHit.maxWave;
            hit.minWave = skillHit.minWave;
            hit.shakeCurve = skillHit.shakeCurve;

            skillObj.hits.Add(hit);
        }
    }

    private void InitParamTrack(SkillParamTrack track, CombatSkillConfig skillObj)
    {
        foreach (var item in track.GetClips())
        {
            SkillParamClip skillParam = item.asset as SkillParamClip;
            skillObj.autoLock = skillParam.autoLock;
            skillObj.force = skillParam.force;
            skillObj.priority = skillParam.priority;
            skillObj.tag = skillParam.tag;
            skillObj.condition = skillParam.condition;
        }
    }

    private void InitPerformTrack(SkillPerformTrack track, CombatSkillConfig skillObj)
    {
        foreach (var item in track.GetClips())
        {
            if (item.asset is SkillPerformPiontClip)
                skillObj.attackPoint = (float)(item.end / m_totalDuration);
            else if (item.asset is SkillPerformBackswingClip)
                skillObj.attackBackswing = (float)(item.start / m_totalDuration);
        }
    }
}
