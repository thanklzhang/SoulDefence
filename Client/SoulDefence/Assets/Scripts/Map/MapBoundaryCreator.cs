using UnityEngine;
using UnityEditor;

namespace SoulDefence.Map
{
#if UNITY_EDITOR
    /// <summary>
    /// 地图边界创建器
    /// 用于在编辑器中创建和配置MapBoundary
    /// </summary>
    public class MapBoundaryCreator : EditorWindow
    {
        // 边界设置
        private float minX = -10f;
        private float maxX = 10f;
        private float minZ = -10f;
        private float maxZ = 10f;
        private bool enforceYLimit = false;
        private float minY = 0f;
        private float maxY = 5f;
        private MapBoundary.BoundaryType boundaryType = MapBoundary.BoundaryType.Box;
        private Color boundaryColor = Color.red;
        private bool showBoundary = true;

        [MenuItem("Tools/Map/Create Boundary")]
        public static void ShowWindow()
        {
            GetWindow<MapBoundaryCreator>("地图边界创建器");
        }

        private void OnGUI()
        {
            GUILayout.Label("地图边界设置", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            // 边界类型
            boundaryType = (MapBoundary.BoundaryType)EditorGUILayout.EnumPopup("边界类型", boundaryType);

            EditorGUILayout.Space();

            // X轴范围
            EditorGUILayout.LabelField("X轴范围");
            EditorGUILayout.BeginHorizontal();
            minX = EditorGUILayout.FloatField("最小值", minX);
            maxX = EditorGUILayout.FloatField("最大值", maxX);
            EditorGUILayout.EndHorizontal();

            // Z轴范围
            EditorGUILayout.LabelField("Z轴范围");
            EditorGUILayout.BeginHorizontal();
            minZ = EditorGUILayout.FloatField("最小值", minZ);
            maxZ = EditorGUILayout.FloatField("最大值", maxZ);
            EditorGUILayout.EndHorizontal();

            // Y轴限制
            enforceYLimit = EditorGUILayout.Toggle("启用Y轴限制", enforceYLimit);
            if (enforceYLimit)
            {
                EditorGUILayout.LabelField("Y轴范围");
                EditorGUILayout.BeginHorizontal();
                minY = EditorGUILayout.FloatField("最小值", minY);
                maxY = EditorGUILayout.FloatField("最大值", maxY);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            // 可视化设置
            showBoundary = EditorGUILayout.Toggle("显示边界", showBoundary);
            boundaryColor = EditorGUILayout.ColorField("边界颜色", boundaryColor);

            EditorGUILayout.Space();

            // 创建按钮
            if (GUILayout.Button("创建地图边界"))
            {
                CreateMapBoundary();
            }

            // 查找并更新按钮
            if (GUILayout.Button("查找并更新现有边界"))
            {
                UpdateExistingBoundary();
            }
        }

        /// <summary>
        /// 创建地图边界
        /// </summary>
        private void CreateMapBoundary()
        {
            // 检查是否已存在MapBoundary
            MapBoundary existingBoundary = GameObject.FindObjectOfType<MapBoundary>();
            if (existingBoundary != null)
            {
                if (EditorUtility.DisplayDialog("警告", "场景中已存在MapBoundary，是否要更新它？", "更新", "取消"))
                {
                    UpdateBoundary(existingBoundary);
                }
                return;
            }

            // 创建新的MapBoundary
            GameObject boundaryObject = new GameObject("MapBoundary");
            MapBoundary boundary = boundaryObject.AddComponent<MapBoundary>();
            UpdateBoundary(boundary);

            // 选中新创建的对象
            Selection.activeGameObject = boundaryObject;

            Debug.Log("已创建新的MapBoundary");
        }

        /// <summary>
        /// 更新现有边界
        /// </summary>
        private void UpdateExistingBoundary()
        {
            MapBoundary boundary = GameObject.FindObjectOfType<MapBoundary>();
            if (boundary != null)
            {
                UpdateBoundary(boundary);
                Debug.Log("已更新现有的MapBoundary");
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "场景中没有找到MapBoundary", "确定");
            }
        }

        /// <summary>
        /// 更新边界参数
        /// </summary>
        /// <param name="boundary">要更新的边界</param>
        private void UpdateBoundary(MapBoundary boundary)
        {
            Undo.RecordObject(boundary, "Update Map Boundary");

            boundary.MinX = minX;
            boundary.MaxX = maxX;
            boundary.MinZ = minZ;
            boundary.MaxZ = maxZ;
            boundary.EnforceYLimit = enforceYLimit;
            boundary.MinY = minY;
            boundary.MaxY = maxY;
            boundary.Type = boundaryType;
            boundary.ShowBoundary = showBoundary;
            boundary.BoundaryColor = boundaryColor;

            EditorUtility.SetDirty(boundary);
        }
    }
#endif
} 