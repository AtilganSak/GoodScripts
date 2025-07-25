using UnityEngine;
using UnityEditor;

public class GridPlacementTool : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int amountX = 5;
    public int amountZ = 5;
    public float spacingX = 2f;
    public float spacingZ = 2f;
    public float positionNoise = 0.5f;
    public Vector3 rotationOffset = Vector3.zero;
    public float enemySpeed = 5f;
    public float enemyHP = 1;
    public bool enabledCollider = true;

    public void SpawnGrid()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Prefab to spawn is not assigned. Please assign a prefab in the Inspector.");
            return;
        }

        ClearAllChildren();

        Vector3 spawnOrigin = this.transform.position;

        for (int x = 0; x < amountX; x++)
        {
            for (int z = 0; z < amountZ; z++)
            {
                Vector3 localSpawnPos = new Vector3(x * spacingX, 0, z * spacingZ);

                localSpawnPos.x += Random.Range(-positionNoise, positionNoise);
                localSpawnPos.z += Random.Range(-positionNoise, positionNoise);

                GameObject spawnedObject = PrefabUtility.InstantiatePrefab(prefabToSpawn) as GameObject;

                spawnedObject.transform.SetParent(this.transform);
                spawnedObject.transform.localPosition = localSpawnPos;
                spawnedObject.transform.localRotation = Quaternion.Euler(rotationOffset);

                Enemy enemy = spawnedObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.SetSpeed(enemySpeed);
                    enemy.SetHP(enemyHP);
                    enemy.SetCollider(enabledCollider);
                    EditorUtility.SetDirty(enemy);
                }

                Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawn Grid Object");
            }
        }
    }

    public void ClearAllChildren()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Undo.DestroyObjectImmediate(child.gameObject);
        }
    }

    public void ApplyRotationToChildren()
    {
        foreach (Transform child in transform)
        {
            Undo.RecordObject(child, "Change Child Rotation");
            child.localRotation = Quaternion.Euler(rotationOffset);
            EditorUtility.SetDirty(child);
        }
    }

    public void SetEnemySpeeds()
    {
        foreach (var enemy in GetComponentsInChildren<Enemy>())
        {
            Undo.RecordObject(enemy, "Change Enemy Speed");
            enemy.SetSpeed(enemySpeed);
            EditorUtility.SetDirty(enemy);
        }
    }
    public void SetEnemyHP()
    {
        foreach (var enemy in GetComponentsInChildren<Enemy>())
        {
            Undo.RecordObject(enemy, "Change Enemy Speed");
            enemy.SetHP(enemyHP);
            EditorUtility.SetDirty(enemy);
        }
    }

    public void SetEnabledCollider()
    {
        foreach (var enemy in GetComponentsInChildren<Enemy>())
        {
            Undo.RecordObject(enemy, "Change Enemy Speed");
            enemy.SetCollider(enabledCollider);
            EditorUtility.SetDirty(enemy);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridPlacementTool))]
public class GridPlacementToolEditor : Editor
{
    private GridPlacementTool myScript;

    private SerializedProperty prefabToSpawnProp;
    private SerializedProperty amountXProp;
    private SerializedProperty amountZProp;
    private SerializedProperty spacingXProp;
    private SerializedProperty spacingZProp;
    private SerializedProperty positionNoiseProp;
    private SerializedProperty rotationOffsetProp;
    private SerializedProperty enemySpeedProp;
    private SerializedProperty enemyHPProp;
    private SerializedProperty enabledColliderProp;

    private void OnEnable()
    {
        myScript = (GridPlacementTool)target;
        prefabToSpawnProp = serializedObject.FindProperty("prefabToSpawn");
        amountXProp = serializedObject.FindProperty("amountX");
        amountZProp = serializedObject.FindProperty("amountZ");
        spacingXProp = serializedObject.FindProperty("spacingX");
        spacingZProp = serializedObject.FindProperty("spacingZ");
        positionNoiseProp = serializedObject.FindProperty("positionNoise");
        rotationOffsetProp = serializedObject.FindProperty("rotationOffset");
        enemySpeedProp = serializedObject.FindProperty("enemySpeed");
        enemyHPProp = serializedObject.FindProperty("enemyHP");
        enabledColliderProp = serializedObject.FindProperty("enabledCollider");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        bool regenerateGrid = false;
        bool applyRotation = false;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(prefabToSpawnProp);
        if (EditorGUI.EndChangeCheck())
        {
            regenerateGrid = true;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(amountXProp);
        EditorGUILayout.PropertyField(amountZProp);
        EditorGUILayout.PropertyField(spacingXProp);
        EditorGUILayout.PropertyField(spacingZProp);
        EditorGUILayout.PropertyField(positionNoiseProp);
        if (EditorGUI.EndChangeCheck())
        {
            regenerateGrid = true;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(rotationOffsetProp);
        if (EditorGUI.EndChangeCheck())
        {
            applyRotation = true;
        }

        EditorGUILayout.PropertyField(enemySpeedProp);
        EditorGUILayout.PropertyField(enemyHPProp);
        EditorGUILayout.PropertyField(enabledColliderProp);

        serializedObject.ApplyModifiedProperties();

        if (regenerateGrid)
        {
            myScript.SpawnGrid();
        }
        else if (applyRotation)
        {
            myScript.ApplyRotationToChildren();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Manual Spawn"))
        {
            myScript.SpawnGrid();
        }

        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Clear All Children",
                                            $"Are you sure you want to delete ALL child objects under '{myScript.gameObject.name}'?",
                                            "Yes, Clear Them", "No, Cancel"))
            {
                myScript.ClearAllChildren();
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Set Rotation"))
        {
            myScript.ApplyRotationToChildren();
        }

        if (GUILayout.Button("Set Speed"))
        {
            myScript.SetEnemySpeeds();
        }
        
        if (GUILayout.Button("Set HP"))
        {
            myScript.SetEnemyHP();
        }
        
        if (GUILayout.Button("Set Collider"))
        {
            myScript.SetEnabledCollider();
        }
    }
}
#endif