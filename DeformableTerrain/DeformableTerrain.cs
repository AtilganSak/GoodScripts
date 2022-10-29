using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum CollisionMethod { Trigger, Collision, Both }
public class OnDeformingTerrain : UnityEvent<float> { }
public class OnRaisingTerrain : UnityEvent<float> { }
public class DeformableTerrain : MonoBehaviour{
    [SerializeField] string m_CollisionTag;
    [SerializeField] bool m_SmoothingDeformation;
    [SerializeField] CollisionMethod m_DetectCollision;
    [SerializeField] float m_SmootingRadius;
    [SerializeField] bool m_SetBorderOnStart;
    [SerializeField] int m_BorderOut;
    [SerializeField] int m_BorderIn;
    [SerializeField] float m_BorderHeight;
    [SerializeField] bool m_DebugTerrain;

    [SerializeField] bool m_MouseControlEnable;
    [SerializeField] int m_Radius;
    [SerializeField] int m_XLeng;
    [SerializeField] int m_YLeng;
    [SerializeField] float m_Height;

    public float AccumulativeVolume { get { return m_AccumulativeVolume; } }
    public float RemovedVolume { get { return m_RemovedVolume; } }
    public float PrecalculatedTotalRemovedVolume { get { return m_PrecalculatedRemovedVolume; } }
    public bool TerrainChanged { get { return m_TerrainChanged; } }

    public OnDeformingTerrain OnDeformingTerrainEvent;
    public OnRaisingTerrain OnRaisingTerrainEvent;

    Dictionary<Collider, Vector3> triggerCheckPositions = new Dictionary<Collider, Vector3>();
    TerrainData m_TerrainData;
    Terrain m_TerrainComponent;
    int m_HeightMapResolution;
    int m_MaxLoopCount = 1;
    float m_AccumulativeVolume;
    float m_RemovedVolume;
    float m_PrecalculatedRemovedVolume;
    float[,] m_CurrentHeights;
    float[,] m_AllHeights;
    List<UpdateTerrainRequest> terrainUpdates;
    Queue<UpdateTerrainRequest> allocatedTerrainRequests;
    bool m_TerrainChanged;

    //************************************************************************
    private void Awake()
    {
        allocatedTerrainRequests = new Queue<UpdateTerrainRequest>();
        terrainUpdates = new List<UpdateTerrainRequest>();
    }
    private void Reset()
    {
        m_BorderOut = 5;
        m_BorderIn = 4;
        m_BorderHeight = 0;
        m_Radius = 3;
        m_XLeng = 5;
        m_YLeng = 5;
        m_Height = 0.01f;
    }
    private void Start() {
        if(m_TerrainComponent == null)
        {
            m_TerrainComponent = GetComponent<Terrain>();
            if(m_TerrainComponent == null)
            {
                m_TerrainComponent = GetComponentInChildren<Terrain>();
                if(m_TerrainComponent == null)
                {
                    m_TerrainComponent = GetComponentInParent<Terrain>();
                    if(m_TerrainComponent == null) return;
                }
            }
        }
        m_TerrainData = m_TerrainComponent.terrainData;
        m_HeightMapResolution = m_TerrainData.heightmapResolution;
        m_AllHeights = m_TerrainData.GetHeights(0, 0, m_HeightMapResolution, m_HeightMapResolution);
        m_CurrentHeights = m_TerrainData.GetHeights(0, 0, m_HeightMapResolution, m_HeightMapResolution);
        float xRes = m_TerrainData.size.x / m_TerrainData.heightmapResolution;
        float yRes = m_TerrainData.size.z / m_TerrainData.heightmapResolution;
        m_PrecalculatedRemovedVolume = xRes * yRes * m_TerrainData.size.y * 2;
        if(m_SetBorderOnStart)
        {
            SetBorder();
        }
        //ResetTerrain();
    }
    private void OnDisable(){
        m_TerrainData.SetHeights(0, 0, m_AllHeights);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(m_DetectCollision != CollisionMethod.Both && m_DetectCollision != CollisionMethod.Collision) return;
        if(!triggerCheckPositions.ContainsKey(collision.collider))
        {
            triggerCheckPositions.Add(collision.collider, collision.collider.bounds.center);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if(m_DetectCollision != CollisionMethod.Both && m_DetectCollision != CollisionMethod.Collision) return;
        if(collision.collider.bounds.center != triggerCheckPositions[collision.collider])
        {
            triggerCheckPositions[collision.collider] = collision.collider.bounds.center;
            float terrainHeight = (collision.collider.bounds.min.y - m_TerrainComponent.transform.position.y) / m_TerrainData.size.y;
            if(terrainHeight == 0)
            {
                return;
            }
            float newHeight = (collision.collider.bounds.max.y - collision.collider.bounds.min.y) + 1f;
            int xSize = (int)(collision.collider.bounds.max.x - collision.collider.bounds.min.x);
            int ySize = (int)(collision.collider.bounds.max.y - collision.collider.bounds.min.y);
            if(xSize == 0)
                xSize = 1;
            if(ySize == 0)
                ySize = 1;
            TerrainRaise(collision.contacts[0].point, xSize + 1, ySize + 1, xSize, -newHeight / 100);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(m_DetectCollision != CollisionMethod.Both && m_DetectCollision != CollisionMethod.Collision) return;
        if(triggerCheckPositions.ContainsKey(collision.collider))
        {
            triggerCheckPositions.Remove(collision.collider);
        }
    }
    private void OnTriggerEnter(Collider other){
        if(m_DetectCollision != CollisionMethod.Both && m_DetectCollision != CollisionMethod.Trigger) return;
        if(!triggerCheckPositions.ContainsKey(other))
        {
            triggerCheckPositions.Add(other, other.bounds.center);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(m_DetectCollision != CollisionMethod.Both && m_DetectCollision != CollisionMethod.Trigger) return;
        if (m_CollisionTag == "" || !other.CompareTag(m_CollisionTag))
            return;
        if(other.bounds.center != triggerCheckPositions[other])
        {
            triggerCheckPositions[other] = other.bounds.center;

            allocatedTerrainRequests.Enqueue(new UpdateTerrainRequest());

            float newHeight = WorldHeightToTerrainHeight(other.bounds.min.y - 0.01f, m_TerrainComponent);

            Vector2Int minIndices = WorldPositionToTerrainMap(other.bounds.min, m_TerrainComponent);
            Vector2Int maxIndices = WorldPositionToTerrainMap(other.bounds.max, m_TerrainComponent);

            int xMinIndex = Mathf.Clamp(minIndices.x - 1, 0, m_HeightMapResolution - 1);
            int xMaxIndex = Mathf.Clamp(maxIndices.x + 1, 0, m_HeightMapResolution - 1);
            int yMinIndex = Mathf.Clamp(minIndices.y - 1, 0, m_HeightMapResolution - 1);
            int yMaxIndex = Mathf.Clamp(maxIndices.y + 1, 0, m_HeightMapResolution - 1);

            float[,] newHeights = new float[yMaxIndex - yMinIndex + 1, xMaxIndex - xMinIndex + 1];

            for(int x = xMinIndex; x <= xMaxIndex; x++)
            {
                for(int y = yMinIndex; y <= yMaxIndex; y++)
                {
                    float height = m_TerrainData.GetHeight(x, y);
                    if(height > 0 && height > newHeight)
                    {
                        newHeights[y - yMinIndex, x - xMinIndex] = newHeight;
                    }
                    else
                    {
                        newHeights[y - yMinIndex, x - xMinIndex] = height;
                    }
                }
            }

            UpdateTerrainRequest terrainRequest = allocatedTerrainRequests.Dequeue();
            terrainUpdates.Add(terrainRequest.UpdateRequest(xMinIndex, yMinIndex, newHeights));

            //float newHeight = (other.bounds.max.y - other.bounds.min.y) + 1f;
            //int xSize = Mathf.RoundToInt((other.bounds.max.x - other.bounds.min.x));
            //int ySize = Mathf.RoundToInt((other.bounds.max.y - other.bounds.min.y));
            //if(xSize == 0)
            //    xSize = 1;
            //if(ySize == 0)
            //    ySize = 1;
            //Debug.Log("XSize: " + xSize);
            //Debug.Log("YSize: " + ySize);
            //Debug.Log("Height: " + newHeight);
            //Debug.Log("Radius: " + (float)xSize / 2);
            //TerrainRaise(other.bounds.center, xSize + 1, ySize + 1, xSize, -newHeight / 100);
        }
    }
    private void OnTriggerExit(Collider other) {
        if(m_DetectCollision != CollisionMethod.Both && m_DetectCollision != CollisionMethod.Trigger) return;
        if(m_CollisionTag != "" && !other.CompareTag(m_CollisionTag)) return;
        if(triggerCheckPositions.ContainsKey(other))
        {
            triggerCheckPositions.Remove(other);
        }
    }
    private void Update() {
        DeformWithMouse();
        ProcessDeformation();
#if UNITY_EDITOR
        GetShortcuts();
#endif
    }
    public void ResetTerrain()
    {
        for(int x = 0; x < m_HeightMapResolution; x++)
        {
            for(int y = 0; y < m_HeightMapResolution; y++)
            {
                m_CurrentHeights[y, x] = m_BorderHeight / 100;
            }
        }
        m_TerrainData.SetHeights(0, 0, m_CurrentHeights);
        float xRes = m_TerrainData.size.x / m_TerrainData.heightmapResolution;
        float yRes = m_TerrainData.size.z / m_TerrainData.heightmapResolution;
        m_PrecalculatedRemovedVolume = xRes * yRes * m_TerrainData.size.y * 2;
        m_RemovedVolume = 0;
        m_AccumulativeVolume = 0;
    }
    void ProcessDeformation()
    {
        if(terrainUpdates.Count == 0)
        {
            m_TerrainChanged = false;
            return;
        }

        m_TerrainChanged = false;
        int xMinIndex = m_HeightMapResolution;
        int yMinIndex = m_HeightMapResolution;
        int xMaxIndex = 0;
        int yMaxIndex = 0;
        int currentCount = terrainUpdates.Count;

        for(int i = 0; i < currentCount; i++)
        {
            UpdateTerrainRequest request = terrainUpdates[i];

            if(!request.isNewRequest)
            {
                request.ResetRequest();
                allocatedTerrainRequests.Enqueue(request);
                continue;
            }

            int xMax = request.xStart + request.heights.GetLength(1) - 1;
            int yMax = request.yStart + request.heights.GetLength(0) - 1;

            for(int x = request.xStart; x <= xMax; x++)
            {
                for(int y = request.yStart; y <= yMax; y++)
                {
                    float diff = m_CurrentHeights[y, x] - request.heights[y - request.yStart, x - request.xStart];
                    if(diff <= 0)
                        continue;
                    m_TerrainChanged = true;

                    if(x < xMinIndex)
                        xMinIndex = x;
                    if(x > xMaxIndex)
                        xMaxIndex = x;
                    if(y < yMinIndex)
                        yMinIndex = y;
                    if(y > yMaxIndex)
                        yMaxIndex = y;

                    float removedVolume = m_PrecalculatedRemovedVolume;
                    if(x != xMax && y != yMax)
                    {
                        float upCenter = m_CurrentHeights[y, x] +
                                         m_CurrentHeights[y, x + 1] +
                                         m_CurrentHeights[y + 1, x] +
                                         m_CurrentHeights[y + 1, x + 1];
                        float downCenter = request.heights[y - request.yStart, x - request.xStart] +
                                           request.heights[y - request.yStart, x - request.xStart + 1] +
                                           request.heights[y - request.yStart + 1, x - request.xStart] +
                                           request.heights[y - request.yStart + 1, x - request.xStart + 1];

                        float height = (upCenter - downCenter) / 4;
                        removedVolume *= height;
                    }
                    else
                    {
                        float upCenter = m_CurrentHeights[y, x];
                        float downCenter = request.heights[y - request.yStart, x - request.xStart];

                        if(x == xMax)
                        {
                            upCenter += m_CurrentHeights[y, x];
                            downCenter += m_BorderHeight;
                        }
                        if(y == yMax)
                        {
                            upCenter += m_CurrentHeights[y, x];
                            downCenter += m_BorderHeight;
                        }
                        if(x == xMax && y == yMax)
                        {
                            upCenter += m_CurrentHeights[y, x];
                            downCenter += m_BorderHeight;
                        }

                        float height = (upCenter - downCenter) / 4;
                        removedVolume *= height;
                    }

                    m_RemovedVolume = removedVolume;
                    m_AccumulativeVolume += removedVolume;
                    m_CurrentHeights[y, x] = request.heights[y - request.yStart, x - request.xStart];
                }
            }

            request.ResetRequest();
            allocatedTerrainRequests.Enqueue(request);
        }

        terrainUpdates.Clear();
        if(m_TerrainChanged)
        {
            float[,] changedSection = new float[yMaxIndex - yMinIndex + 1, xMaxIndex - xMinIndex + 1];
            for(int x = xMinIndex; x <= xMaxIndex; x++)
            {
                for(int y = yMinIndex; y <= yMaxIndex; y++)
                {
                    changedSection[y - yMinIndex, x - xMinIndex] = m_CurrentHeights[y, x];
                }
            }

            m_TerrainData.SetHeights(xMinIndex, yMinIndex, changedSection);

            if(m_SmoothingDeformation)
            {
                m_CurrentHeights = m_TerrainData.GetHeights(0, 0, m_HeightMapResolution, m_HeightMapResolution);
                m_TerrainData.SetHeights(0, 0, m_CurrentHeights);
            }
        }
    }
    public void Pouring(float _amount)
    {

    }
    void SetBorder()
    {
        for(int x = 0; x < m_HeightMapResolution; x++)
        {
            for(int y = 0; y < m_HeightMapResolution; y++)
            {
                if(x < m_BorderOut || x >= m_HeightMapResolution - m_BorderOut - 1)
                {
                    // x is in border
                    m_CurrentHeights[y, x] = m_BorderHeight;
                }
                else if(y < m_BorderOut || y >= m_HeightMapResolution - m_BorderOut - 1)
                {
                    // y is in border
                    m_CurrentHeights[y, x] = m_BorderHeight;
                }
            }
        }
        m_TerrainData.SetHeights(0, 0, m_CurrentHeights);
    }
    void GetShortcuts()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            SetBorder();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetTerrain();
        }
    }
    void DeformWithMouse()
    {
        if(!m_MouseControlEnable) return;
        if(Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                TerrainRaise(hit.point, m_XLeng, m_YLeng, m_Radius, m_Height);
            }
        }
        if(Input.GetMouseButton(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                TerrainRaise(hit.point, m_XLeng, m_YLeng, m_Radius, -m_Height);
            }
        }
    }
    public void TerrainRaise(Vector3 point, int xLeng, int yLeng, float radius, float height)
    {
        Vector2Int terrainPos = WorldPositionToTerrainMap(point, m_TerrainComponent);

        int xMin = Mathf.Clamp(terrainPos.x - xLeng, 0, m_HeightMapResolution);
        int xMax = Mathf.Clamp(terrainPos.x + xLeng, 0, m_HeightMapResolution);
        int yMin = Mathf.Clamp(terrainPos.y - yLeng, 0, m_HeightMapResolution);
        int yMax = Mathf.Clamp(terrainPos.y + yLeng, 0, m_HeightMapResolution);

        for(int i = xMin; i < xMax; i++)
        {
            for(int j = yMin; j < yMax; j++)
            {
                if(Vector2.Distance(terrainPos, new Vector2(i, j)) <= radius)
                {
                    m_CurrentHeights[j, i] += height / 100;
                }
            }
        }
        if(m_SmoothingDeformation)
            TerrainSmooth(xMin, xMax, yMin, yMax, terrainPos, m_CurrentHeights);
        m_TerrainData.SetHeights(0, 0, m_CurrentHeights);
    }
    public void TerrainSmooth(int xMin, int xMax, int yMin, int yMax, Vector2 center, float[,] currentHeightMap)
    {
        // Note: MapWidth and MapHeight should be equal and power-of-two values
        int adjacentSections = 0;
        float sectionsTotal = 0.0f;

        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                if(Vector2.Distance(center, new Vector2(x, y)) > m_SmootingRadius) continue;
                adjacentSections = 0;
                sectionsTotal = 0.0f;

                if ((x - 1) > xMin) // Check to left
                {
                    sectionsTotal += currentHeightMap[y, x - 1];
                    adjacentSections++;

                    if ((y - 1) > yMin) // Check up and to the left
                    {
                        sectionsTotal += currentHeightMap[y - 1, x - 1];
                        adjacentSections++;
                    }

                    if ((y + 1) < yMax) // Check down and to the left
                    {
                        sectionsTotal += currentHeightMap[y + 1, x - 1];
                        adjacentSections++;
                    }
                }

                if ((x + 1) < xMax) // Check to right
                {
                    sectionsTotal += currentHeightMap[y, x + 1];
                    adjacentSections++;

                    if ((y - 1) > yMin) // Check up and to the right
                    {
                        sectionsTotal += currentHeightMap[y - 1, x + 1];
                        adjacentSections++;
                    }

                    if ((y + 1) < yMax) // Check down and to the right
                    {
                        sectionsTotal += currentHeightMap[y + 1, x + 1];
                        adjacentSections++;
                    }
                }

                if ((y - 1) > yMin) // Check above
                {
                    sectionsTotal += currentHeightMap[y - 1, x];
                    adjacentSections++;
                }

                if ((y + 1) < yMax) // Check below
                {
                    sectionsTotal += currentHeightMap[y + 1, x];
                    adjacentSections++;
                }
              
                currentHeightMap[y, x] = (currentHeightMap[y, x] + (sectionsTotal / adjacentSections)) * 0.5f;
            }
        }

        m_TerrainData.SetHeights(0, 0, currentHeightMap);
    }
    Vector2Int WorldPositionToTerrainMap(Vector3 worldPosition, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        var netPosition = worldPosition - terrain.gameObject.transform.position;
        int xzRes = terrainData.heightmapResolution; // ex:513
        float xScale = xzRes / terrainData.size.x; // width  500 1 world unit heightmap'de hangi noktaya geliyor... 
        float zScale = xzRes / terrainData.size.z; // length 400

        int x = Mathf.Clamp(Mathf.RoundToInt(netPosition.x * xScale), 0, xzRes);
        int y = Mathf.Clamp(Mathf.RoundToInt(netPosition.z * zScale), 0, xzRes);
        return new Vector2Int(x, y);
    }
    float WorldHeightToTerrainHeight(float worldHeight, Terrain terrain)
    {
        float terrainHeight = (worldHeight - terrain.transform.position.y) / terrain.terrainData.size.y;

        if(terrainHeight < 0 || terrainHeight > 1)
        {
            // Lebug.LogWarning("WorldHeightToTerrainHeight calculated out of range terrainHeight : " + terrainHeight);
            terrainHeight = Mathf.Clamp01(terrainHeight);
        }

        return terrainHeight;
    }
}
struct UpdateTerrainRequest
{
    public bool isNewRequest;
    public int xStart;
    public int yStart;
    public float[,] heights;

    public UpdateTerrainRequest UpdateRequest(int _xStart, int _yStart,float[,] _heights)
    {
        xStart = _xStart;
        yStart = _yStart;
        heights = _heights;
        isNewRequest = true;

        return this;
    }

    public void ResetRequest()
    {
        xStart = 0;
        yStart = 0;
        heights = null;
        isNewRequest = false;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(DeformableTerrain))]
public class DeformableTerrainEditor : Editor
{
    DeformableTerrain script { get => target as DeformableTerrain; }
    SerializedObject script_ser;
    SerializedProperty collisionTag_ser;
    SerializedProperty smoothingDeformation_ser;
    SerializedProperty smoothingRadius_ser;
    SerializedProperty detectCollision_ser;
    SerializedProperty setBorderOnStart_ser;
    SerializedProperty borderOut_ser;
    SerializedProperty borderIn_ser;
    SerializedProperty borderHeight_ser;
    SerializedProperty debugTerrain_ser;
    SerializedProperty mouseControlEnable_ser;
    SerializedProperty radius_ser;
    SerializedProperty xLeng_ser;
    SerializedProperty yLeng_ser;
    SerializedProperty height_ser;

    private void OnEnable()
    {
        script_ser = new SerializedObject(script);
        collisionTag_ser = script_ser.FindProperty("m_CollisionTag");
        smoothingDeformation_ser = script_ser.FindProperty("m_SmoothingDeformation");
        smoothingRadius_ser = script_ser.FindProperty("m_SmootingRadius");
        detectCollision_ser = script_ser.FindProperty("m_DetectCollision");
        setBorderOnStart_ser = script_ser.FindProperty("m_SetBorderOnStart");
        borderOut_ser = script_ser.FindProperty("m_BorderOut");
        borderIn_ser = script_ser.FindProperty("m_BorderIn");
        borderHeight_ser = script_ser.FindProperty("m_BorderHeight");
        debugTerrain_ser = script_ser.FindProperty("m_DebugTerrain");
        mouseControlEnable_ser = script_ser.FindProperty("m_MouseControlEnable");
        radius_ser = script_ser.FindProperty("m_Radius");
        xLeng_ser = script_ser.FindProperty("m_XLeng");
        yLeng_ser = script_ser.FindProperty("m_YLeng");
        height_ser = script_ser.FindProperty("m_Height");

        script_ser.Update();
        if(collisionTag_ser.stringValue != "Deformer")
        {
            collisionTag_ser.stringValue = "Deformer";
#if UNITY_EDITOR
            if(!LayerTagHelper.IsPresentTag("Deformer"))
            {
                LayerTagHelper.AddTag("Deformer");
            }
#endif
        }
        script_ser.ApplyModifiedProperties();
    }
    public override void OnInspectorGUI()
    {
        script_ser.Update();
        EditorGUILayout.PropertyField(collisionTag_ser);
        EditorGUILayout.PropertyField(smoothingDeformation_ser);
        EditorGUILayout.PropertyField(smoothingRadius_ser);
        EditorGUILayout.PropertyField(detectCollision_ser);
        EditorGUILayout.PropertyField(debugTerrain_ser);

        EditorGUILayout.BeginVertical("Box");//VER1
        EditorGUILayout.LabelField("Border Settings",EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(setBorderOnStart_ser);
        EditorGUILayout.PropertyField(borderOut_ser);
        EditorGUILayout.PropertyField(borderIn_ser);
        EditorGUILayout.PropertyField(borderHeight_ser);
        EditorGUILayout.EndVertical();//VER1

        EditorGUILayout.BeginVertical("Box");//VER2
        EditorGUILayout.PropertyField(mouseControlEnable_ser);
        if(mouseControlEnable_ser.boolValue)
        {
            EditorGUILayout.PropertyField(radius_ser);
            EditorGUILayout.PropertyField(xLeng_ser);
            EditorGUILayout.PropertyField(yLeng_ser);
            EditorGUILayout.PropertyField(height_ser);
        }
        EditorGUILayout.EndVertical();//VER2

        EditorGUILayout.BeginVertical("Box");//VER3
        if(EditorApplication.isPlaying)
        {
            if(GUILayout.Button("Reset"))
        {
            script.ResetTerrain();
        }
        }
        else
        {
            EditorGUILayout.HelpBox("These buttons shows only on play mode!", MessageType.Warning);
        }
        EditorGUILayout.EndVertical();//VER3
        script_ser.ApplyModifiedProperties();
    }
}
#endif