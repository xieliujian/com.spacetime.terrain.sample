using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainViewerSetupWindow : EditorWindow
{
    private const string PrefabDir = "Assets/Scenes/SampleScene/RuntimeTerrainData/prefab";
    private const string RootName = "TerrainViewer";

    [MenuItem("SpaceTime/Terrain Viewer Setup")]
    public static void Open()
    {
        GetWindow<TerrainViewerSetupWindow>("Terrain Viewer Setup");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Terrain Viewer Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Setup - 加载模型地形到场景", GUILayout.Height(40)))
            Setup();

        EditorGUILayout.Space();

        if (GUILayout.Button("Clear - 清除场景中的模型地形", GUILayout.Height(30)))
            Clear();
    }

    private void Setup()
    {
        if (!AssetDatabase.IsValidFolder(PrefabDir))
        {
            EditorUtility.DisplayDialog("错误", $"找不到 prefab 目录：\n{PrefabDir}", "OK");
            return;
        }

        Clear();

        // 收集场景中的原始 Terrain 位置和尺寸信息
        var terrains = Object.FindObjectsByType<Terrain>(FindObjectsSortMode.None);
        var terrainInfo = new Dictionary<string, (Vector3 pos, Vector3 size)>();
        Debug.Log($"[TerrainViewer] 找到 {terrains.Length} 个原始 Terrain:");
        foreach (var terrain in terrains)
        {
            var size = terrain.terrainData.size;
            terrainInfo[terrain.name] = (terrain.transform.position, size);
            Debug.Log($"  - {terrain.name}: pos={terrain.transform.position}, size={size}");
        }

        var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabDir });
        var parentPrefabs = new List<GameObject>();

        foreach (var guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            if (!fileName.Contains("_LOD"))
                parentPrefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        }

        if (parentPrefabs.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "未找到父级 prefab。", "OK");
            return;
        }

        var rootGo = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(rootGo, "Create TerrainViewer");
        rootGo.AddComponent<TerrainViewerController>();

        int positionedCount = 0;
        foreach (var prefab in parentPrefabs)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, rootGo.transform);
            Undo.RegisterCreatedObjectUndo(instance, "Instantiate Terrain Prefab");

            // 计算子块位置：从 prefab 名称解析出父 Terrain 名和子块索引
            Vector3 calculatedPos = CalculateSubTerrainPosition(instance.name, terrainInfo);
            Debug.Log($"[TerrainViewer] Prefab: {instance.name}, 计算位置: {calculatedPos}");
            instance.transform.position = calculatedPos;
            if (calculatedPos != Vector3.zero)
                positionedCount++;

            // 默认显示原始地形：隐藏所有 LOD
            SetLodVisibility(instance.transform, -1);
        }

        EditorUtility.SetDirty(rootGo);
        Debug.Log($"[TerrainViewer] 已加载 {parentPrefabs.Count} 个地形块到场景，其中 {positionedCount} 个匹配到原始位置。");
    }

    private static Vector3 CalculateSubTerrainPosition(string prefabName,
        Dictionary<string, (Vector3 pos, Vector3 size)> terrainInfo)
    {
        // prefabName 格式: Terrain0_00_0 或 Terrain1_11_1
        // 原始 Terrain 名称: Terrain0_0, Terrain0_1, Terrain1_0, Terrain1_1

        // 按下划线分割: Terrain0_00_0 → ["Terrain0", "00", "0"]
        var parts = prefabName.Split('_');
        if (parts.Length < 3)
            return Vector3.zero;

        // parts[0] = Terrain0
        // parts[1] = 00/01/10/11 (两位数，十位是原始Terrain索引，个位是子块wi)
        // parts[2] = 0/1 (子块li)

        if (parts[1].Length != 2)
            return Vector3.zero;

        if (!int.TryParse(parts[1].Substring(0, 1), out int terrainIndex))
            return Vector3.zero;
        if (!int.TryParse(parts[1].Substring(1, 1), out int wi))
            return Vector3.zero;
        if (!int.TryParse(parts[2], out int li))
            return Vector3.zero;

        // 构造原始 Terrain 名称: Terrain0_0
        string terrainName = parts[0] + "_" + terrainIndex;

        Debug.Log($"  解析: prefab={prefabName}, terrain={terrainName}, wi={wi}, li={li}");

        // 查找父 Terrain 信息
        if (!terrainInfo.TryGetValue(terrainName, out var info))
        {
            Debug.LogWarning($"  未找到 Terrain: {terrainName}");
            return Vector3.zero;
        }

        // 计算子块位置：position = terrain.pos + (wi * secWidth, 0, li * secLength)
        float secWidth = info.size.x / 2f;
        float secLength = info.size.z / 2f;

        return info.pos + new Vector3(wi * secWidth, 0, li * secLength);
    }

    private void Clear()
    {
        var existing = GameObject.Find(RootName);
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing);
            Debug.Log("[TerrainViewer] 已清除场景中的模型地形。");
        }
    }

    private static void SetLodVisibility(Transform parent, int activeLodIndex)
    {
        string[] suffixes = { "_LOD0", "_LOD1", "_LOD2" };
        for (int i = 0; i < suffixes.Length; i++)
        {
            var child = parent.Find(parent.name + suffixes[i]);
            if (child != null)
                child.gameObject.SetActive(i == activeLodIndex);
        }
    }
}
