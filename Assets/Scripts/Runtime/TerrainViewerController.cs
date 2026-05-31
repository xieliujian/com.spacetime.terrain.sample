using System.Collections.Generic;
using UnityEngine;
using ST.Core;

public enum TerrainDisplayMode
{
    OriginalTerrain,
    MeshLOD0,
    MeshLOD1,
    MeshLOD2,
}

public class TerrainViewerController : MonoBehaviour
{
    [SerializeField] private TerrainDisplayMode currentMode = TerrainDisplayMode.OriginalTerrain;

    private Terrain[] _originalTerrains;

    // key: parent transform, value: [LOD0, LOD1, LOD2] (null if missing)
    private List<Transform[]> _lodGroups = new List<Transform[]>();

    private static readonly string[] LodSuffixes = { "_LOD0", "_LOD1", "_LOD2" };

    private void Start()
    {
        _originalTerrains = FindObjectsByType<Terrain>(FindObjectsSortMode.None);

        foreach (Transform child in transform)
        {
            var lods = new Transform[3];
            for (int i = 0; i < 3; i++)
            {
                lods[i] = child.Find(child.name + LodSuffixes[i]);
            }
            _lodGroups.Add(lods);
        }

        ApplyMode(currentMode);

        // 初始化摄像机
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("[TerrainViewer] 未找到主摄像机，跳过摄像机初始化。");
            return;
        }

        // 添加 SceneRoamCamera 组件（如果还没有）
        SceneRoamCamera roamCam = mainCam.GetComponent<SceneRoamCamera>();
        if (roamCam == null)
        {
            roamCam = mainCam.gameObject.AddComponent<SceneRoamCamera>();
            Debug.Log("[TerrainViewer] 已添加 SceneRoamCamera 组件到主摄像机。");
        }

        // 设置移动速度为默认值的 2 倍
        roamCam.moveSpeed = 20f;

        // 计算地形中心和合适的摄像机位置
        Vector3 terrainCenter = CalculateTerrainCenter();
        float terrainHeight = CalculateTerrainMaxHeight();

        // 将摄像机放置在地形中心上方，高度为地形高度的 1/9，向下俯视 45 度
        float cameraHeight = terrainCenter.y + terrainHeight * 0.11f;
        Vector3 cameraPos = new Vector3(terrainCenter.x, cameraHeight, terrainCenter.z - terrainHeight * 0.5f);
        mainCam.transform.position = cameraPos;
        mainCam.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

        Debug.Log($"[TerrainViewer] 摄像机已放置到 {cameraPos}，俯视角度 45°，移动速度 {roamCam.moveSpeed}");
    }

    private Vector3 CalculateTerrainCenter()
    {
        if (_originalTerrains == null || _originalTerrains.Length == 0)
            return Vector3.zero;

        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (var terrain in _originalTerrains)
        {
            if (terrain == null) continue;

            Vector3 pos = terrain.transform.position;
            Vector3 size = terrain.terrainData.size;

            min = Vector3.Min(min, pos);
            max = Vector3.Max(max, pos + size);
        }

        return (min + max) * 0.5f;
    }

    private float CalculateTerrainMaxHeight()
    {
        if (_originalTerrains == null || _originalTerrains.Length == 0)
            return 0f;

        float maxHeight = 0f;
        foreach (var terrain in _originalTerrains)
        {
            if (terrain == null) continue;
            maxHeight = Mathf.Max(maxHeight, terrain.terrainData.size.y);
        }

        return maxHeight;
    }

    public void SetDisplayMode(TerrainDisplayMode mode)
    {
        currentMode = mode;
        ApplyMode(mode);
    }

    private void ApplyMode(TerrainDisplayMode mode)
    {
        bool showOriginal = mode == TerrainDisplayMode.OriginalTerrain;

        if (_originalTerrains != null)
        {
            foreach (var t in _originalTerrains)
            {
                if (t != null)
                    t.gameObject.SetActive(showOriginal);
            }
        }

        int lodIndex = mode switch
        {
            TerrainDisplayMode.MeshLOD0 => 0,
            TerrainDisplayMode.MeshLOD1 => 1,
            TerrainDisplayMode.MeshLOD2 => 2,
            _ => -1,
        };

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!showOriginal);
        }

        if (lodIndex >= 0)
        {
            foreach (var lods in _lodGroups)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (lods[i] != null)
                        lods[i].gameObject.SetActive(i == lodIndex);
                }
            }
        }
    }

    private void OnGUI()
    {
        float x = 10f;
        float y = 10f;
        float w = 110f;
        float h = 40f;
        float gap = 5f;

        DrawModeButton(TerrainDisplayMode.OriginalTerrain, "Original", x, y, w, h);
        DrawModeButton(TerrainDisplayMode.MeshLOD0, "LOD0", x + (w + gap) * 1, y, w, h);
        DrawModeButton(TerrainDisplayMode.MeshLOD1, "LOD1", x + (w + gap) * 2, y, w, h);
        DrawModeButton(TerrainDisplayMode.MeshLOD2, "LOD2", x + (w + gap) * 3, y, w, h);
    }

    private void DrawModeButton(TerrainDisplayMode mode, string label, float x, float y, float w, float h)
    {
        var prev = GUI.backgroundColor;
        if (currentMode == mode)
            GUI.backgroundColor = Color.green;

        if (GUI.Button(new Rect(x, y, w, h), label))
            SetDisplayMode(mode);

        GUI.backgroundColor = prev;
    }
}
