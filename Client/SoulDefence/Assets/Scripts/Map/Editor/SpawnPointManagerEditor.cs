using UnityEngine;
using UnityEditor;

/// <summary>
/// SpawnPointManager的自定义编辑器
/// 处理Scene视图中的交互
/// </summary>
[CustomEditor(typeof(SpawnPointManager))]
public class SpawnPointManagerEditor : Editor
{
    private SpawnPointManager spawnManager;

    void OnEnable()
    {
        spawnManager = (SpawnPointManager)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (spawnManager == null) return;

        // 获取所有出怪点
        var spawnPoints = spawnManager.GetAllSpawnPoints();
        if (spawnPoints == null) return;

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var point = spawnPoints[i];
            if (point == null) continue;

            // 设置控制器大小
            float handleSize = HandleUtility.GetHandleSize(point.position) * 0.2f;

            // 保存当前颜色
            Color oldColor = Handles.color;
            Handles.color = point.pointColor;

            // 检查位置变化
            EditorGUI.BeginChangeCheck();

            // 使用FreeMoveHandle创建可拖拽的控制点
            var fmh_50_17_638946570576522690 = Quaternion.identity; Vector3 newPosition = Handles.FreeMoveHandle(
                point.position,
                handleSize,
                Vector3.zero,
                Handles.SphereHandleCap
            );

            if (EditorGUI.EndChangeCheck())
            {
                // 记录撤销操作
                Undo.RecordObject(spawnManager, $"Move {point.pointName}");

                // 应用Y轴限制
                if (spawnManager.IsYAxisLocked)
                {
                    newPosition.y = spawnManager.FixedYValue;
                }

                // 更新出怪点位置
                point.position = newPosition;

                // 标记对象为已修改
                EditorUtility.SetDirty(spawnManager);

                // Debug.Log($"[SpawnPointManager] 移动出怪点 {point.pointName} 到位置: {newPosition}" + 
                //          (spawnManager.IsYAxisLocked ? $" (Y轴锁定为: {spawnManager.FixedYValue})" : ""));
            }

            // 绘制坐标轴线帮助定位
            float axisLength = handleSize * 2f;
            
            // X轴 - 红色
            Handles.color = Color.red;
            Handles.DrawLine(point.position, point.position + Vector3.right * axisLength);
            
            // Y轴 - 绿色
            Handles.color = Color.green;
            Handles.DrawLine(point.position, point.position + Vector3.up * axisLength);
            
            // Z轴 - 蓝色
            Handles.color = Color.blue;
            Handles.DrawLine(point.position, point.position + Vector3.forward * axisLength);

            // 绘制标签
            Handles.color = Color.white;
            Vector3 labelPos = point.position + Vector3.up * (point.pointSize + 1f);
            Handles.Label(labelPos, $"[{i}] {point.pointName}");

            // 恢复颜色
            Handles.color = oldColor;
        }
    }

    public override void OnInspectorGUI()
    {
        // 绘制默认Inspector
        DrawDefaultInspector();

        GUILayout.Space(10);

        // Y轴限制控制区域
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Y轴限制控制", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("设置当前Y为固定值"))
        {
            if (spawnManager.GetSpawnPointCount() > 0)
            {
                Undo.RecordObject(spawnManager, "Set Fixed Y Value");
                
                // 使用第一个出怪点的Y值作为固定值
                var firstPoint = spawnManager.GetSpawnPointByIndex(0);
                if (firstPoint != null)
                {
                    spawnManager.SetFixedYValue(firstPoint.position.y);
                    EditorUtility.SetDirty(spawnManager);
                }
            }
        }
        
        if (GUILayout.Button("应用固定Y到所有点"))
        {
            if (spawnManager.IsYAxisLocked)
            {
                Undo.RecordObject(spawnManager, "Apply Fixed Y to All Points");
                
                var allPoints = spawnManager.GetAllSpawnPoints();
                foreach (var point in allPoints)
                {
                    point.position = new Vector3(point.position.x, spawnManager.FixedYValue, point.position.z);
                }
                EditorUtility.SetDirty(spawnManager);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        if (spawnManager.IsYAxisLocked)
        {
            EditorGUILayout.HelpBox($"Y轴已锁定为: {spawnManager.FixedYValue:F2}\n拖拽时Y轴将保持此值不变。", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Y轴未锁定，可以自由拖拽。", MessageType.None);
        }
        
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // 添加一些帮助信息
        EditorGUILayout.HelpBox("在Scene视图中选中此对象后，可以直接拖拽出怪点进行移动。", MessageType.Info);

        if (GUILayout.Button("添加新出怪点"))
        {
            Undo.RecordObject(spawnManager, "Add Spawn Point");
            
            // 创建新出怪点
            SpawnPoint newPoint = new SpawnPoint();
            newPoint.pointName = $"SpawnPoint_{spawnManager.GetSpawnPointCount() + 1}";
            
            // 设置位置，如果Y轴锁定则使用固定Y值
            Vector3 newPos = spawnManager.transform.position;
            if (spawnManager.IsYAxisLocked)
            {
                newPos.y = spawnManager.FixedYValue;
            }
            newPoint.position = newPos;
            
            spawnManager.AddSpawnPoint(newPoint);
            EditorUtility.SetDirty(spawnManager);
        }
    }
} 