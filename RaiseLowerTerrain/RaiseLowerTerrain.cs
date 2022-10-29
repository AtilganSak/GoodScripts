using System.Collections.Generic;
using UnityEngine;

public class RaiseLowerTerrain : MonoBehaviour
{
    public Terrain TerrainComponent;
    [Tooltip("You can raise the terrain by pressing right mouse or lower the terrain by press left mouse")]
    public bool enableMouseControl;
    [Tooltip("If it is true will use OnCollision, if it is false will use OnTrigger for deformer detect")]
    public bool detectCollision;
    public string collisionTag;
    [Range(0, 5)]
    public float hardness;

    [Header("Digging Size")]
    public float dirtAmount;
    public int smooth;
    [Tooltip("Area is digging by mouse")]
    public int HoleSize = 2;

    [Header("Retexture Ground")]
    public bool ChangeTexture = true;
    [Tooltip("The texture ID to change texture to.")]
    public int ChangeTextureTo = 3;

    TerrainData terrData;
    int xResolution;
    int zResolution;
    float[,] heightMapsBackup;
    const float DEPTH_METER_CONVERT = 0.05f;
    const float TEXTURE_SIZE_MULTIPLIER = 1.25f;
    int alphaMapWidth;
    int alphaMapHeight;
    int numOfAlphaLayers;
    float[,,] alphaMapBackup;
    Dictionary<Collider, Vector3> triggerCheckPositions = new Dictionary<Collider, Vector3>();

    private void OnEnable()
    {
        if(TerrainComponent == null || terrData == null) Reset();

        xResolution = terrData.heightmapResolution;
        zResolution = terrData.heightmapResolution;
        alphaMapWidth = terrData.alphamapWidth;
        alphaMapHeight = terrData.alphamapHeight;
        numOfAlphaLayers = terrData.alphamapLayers;

        heightMapsBackup = terrData.GetHeights(0, 0, xResolution, zResolution);
        alphaMapBackup = terrData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
    }
    private void OnDisable()
    {
        terrData.SetHeights(0, 0, heightMapsBackup);
        terrData.SetAlphamaps(0, 0, alphaMapBackup);
    }

    /// <summary>
    /// Used for testing - Point mouse to the Terrain. Left mouse button to raise/Right to lower
    /// NOTE: Comment this out once testing is over
    /// </summary>
    void Update()
    {
        if(enableMouseControl)
        {
            if(Input.GetMouseButton(0))
            {
                MoveDirtByMouse(true);
            }
            if(Input.GetMouseButton(1))
            {
                MoveDirtByMouse(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!detectCollision) return;
        if(collisionTag != "" && !collision.gameObject.CompareTag(collisionTag)) return;
        if(!triggerCheckPositions.ContainsKey(collision.collider))
        {
            triggerCheckPositions.Add(collision.collider, collision.collider.bounds.center);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if(!detectCollision) return;
        if(collisionTag != "" && !collision.gameObject.CompareTag(collisionTag)) return;
        if(collision.collider.bounds.center != triggerCheckPositions[collision.collider])
        {
            triggerCheckPositions[collision.collider] = collision.collider.bounds.center;
            float xSize = collision.collider.bounds.max.x - collision.collider.bounds.min.x;
            float ySize = collision.collider.bounds.max.y - collision.collider.bounds.min.y;
            RaiselowerTerrainArea(collision.contacts[0].point, (int)xSize , (int)ySize, smooth, dirtAmount);
            Debug.Log("Runing");
            if(ChangeTexture)
                TextureDeformation(collision.collider.bounds.center, xSize * ySize, ChangeTextureTo);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(!detectCollision) return;
        if(collisionTag != "" && !collision.gameObject.CompareTag(collisionTag)) return;
        if(triggerCheckPositions.ContainsKey(collision.collider))
        {
            triggerCheckPositions.Remove(collision.collider);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(detectCollision) return;
        if(collisionTag != "" && !other.CompareTag(collisionTag)) return;
        if(!triggerCheckPositions.ContainsKey(other))
        {
            triggerCheckPositions.Add(other, other.bounds.center);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(detectCollision) return;
        if(collisionTag != "" && !other.CompareTag(collisionTag)) return;
        if(other.bounds.center != triggerCheckPositions[other])
        {
            triggerCheckPositions[other] = other.bounds.center;
            float xSize = other.bounds.max.x - other.bounds.min.x;
            float ySize = other.bounds.max.y - other.bounds.min.y;
            RaiselowerTerrainArea(other.bounds.center, (int)xSize , (int)ySize, smooth, dirtAmount);
            if(ChangeTexture)
                TextureDeformation(other.bounds.center, xSize * ySize, ChangeTextureTo);
            Debug.Log("Runing");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(detectCollision) return;
        if(collisionTag != "" && !other.CompareTag(collisionTag)) return;
        if(triggerCheckPositions.ContainsKey(other))
        {
            triggerCheckPositions.Remove(other);
        }
    }

    /// <summary>
    /// Lower or Raise dirt based on where player is looking
    /// </summary>
    /// <param name="raise">If set to <c>true</c> raise.</param>
    private void MoveDirtByMouse(bool raise)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            // area middle point x and z, area width, area height, smoothing distance, area height adjust
            RaiselowerTerrainArea(hit.point, HoleSize, HoleSize, smooth, dirtAmount);
            //RaiselowerTerrainPoint(hit.point, dirtAmount);
            //MoveDirt(hit);

            // area middle point x and z, area size, texture ID from terrain textures
            if(ChangeTexture)
                TextureDeformation(hit.point, HoleSize * HoleSize, ChangeTextureTo);
        }
    }

    private void Reset()
    {
        if(TerrainComponent == null)
        {
            TerrainComponent = GetComponent<Terrain>();
            if(TerrainComponent == null)
            {
                TerrainComponent = GetComponentInChildren<Terrain>();
                if(TerrainComponent == null)
                {
                    TerrainComponent = GetComponentInParent<Terrain>();
                    if(TerrainComponent == null) return;
                }
            }
        }
        terrData = TerrainComponent.terrainData;
        if(collisionTag != "Deformer")
        {
            collisionTag = "Deformer";
#if UNITY_EDITOR
            if(!LayerTagHelper.IsPresentTag("Deformer"))
            {
                LayerTagHelper.AddTag("Deformer");
            }
#endif
        }
    }

    /// <summary>
    /// Raise or lower 1 section of terrain by an amount, but will be jagged and not smoothed
    /// </summary>
    /// <param name="point">Point to change height at x/z</param>
    /// <param name="incdec">Amount to raise or lower (1 = small, 100 = large)</param>
    void RaiselowerTerrainChunk(Vector3 point, float incdec)
    {
        incdec *= 0.00001f; //NOTE: These are in units of 0-1 so need to be very small at FPS level

        int terX = (int)((point.x / terrData.size.x) * xResolution);
        int terZ = (int)((point.z / terrData.size.z) * zResolution);

        if(terX < 0) terX = 0;
        if(terX > xResolution) terX = xResolution;
        if(terZ < 0) terZ = 0;
        if(terZ > zResolution) terZ = zResolution;

        float[,] heights = terrData.GetHeights(terX, terZ, 1, 1);
        float y = heights[0, 0];

        heights[0, 0] += incdec;

        terrData.SetHeights(terX, terZ, heights);

    }
    /// <summary>
    /// Raise or lowers the terrain in an area and applies smoothing.
    /// </summary>
    /// <param name="point">Point to change height at x/z</param>
    /// <param name="lenx">X Width of tiles to modify height</param>
    /// <param name="lenz">Z Width of tiles to modify height</param>
    /// <param name="smooth">Number of tiles radius to smooth</param>
    /// <param name="incdec">Amount to raise or lower (1 = small, 100 = large)</param>
    void RaiselowerTerrainArea(Vector3 point, int lenx, int lenz, int smooth, float incdec)
    {
        //From http://answers.unity3d.com/questions/420634/how-do-you-dynamically-alter-terrain.html, modified by Jay

        incdec *= 0.00001f; //NOTE: These are in units of 0-1 so need to be very small at FPS level

        int areax;
        int areaz;
        smooth += 1;

        float smoothing;
        int terX = (int)((point.x / terrData.size.x) * xResolution);
        int terZ = (int)((point.z / terrData.size.z) * zResolution);
        lenx += smooth;
        lenz += smooth;
        terX -= (lenx / 2);
        terZ -= (lenz / 2);
        if(terX < 0) terX = 0;
        if(terX > xResolution) terX = xResolution;
        if(terZ < 0) terZ = 0;
        if(terZ > zResolution) terZ = zResolution;

        float[,] heights = terrData.GetHeights(terX, terZ, lenx, lenz);
        //float y = heights[lenx / 2, lenz / 2];
        //y += incdec;

        for(smoothing = 1; smoothing < (smooth + 1); smoothing++)
        {
            float multiplier = smoothing / smooth;
            for(areax = (int)(smoothing / 2); areax < lenx - (smoothing / 2); areax++)
            {
                for(areaz = (int)(smoothing / 2); areaz < lenz - (smoothing / 2); areaz++)
                {
                    if((areax > -1) && (areaz > -1) && (areax < xResolution) && (areaz < zResolution))
                    {
                        heights[areax, areaz] += (float)(incdec * multiplier);
                        heights[areax, areaz] = Mathf.Clamp(heights[areax, areaz], 0, 1);
                    }
                }
            }
        }
        terrData.SetHeights(terX, terZ, heights);
    }
    void RaiselowerTerrainPoint(Vector3 point, float incdec)
    {
        int terX = (int)((point.x / terrData.size.x) * xResolution);
        int terZ = (int)((point.z / terrData.size.z) * zResolution);
        float[,] heights = terrData.GetHeights(0, 0, xResolution, zResolution);
        float y = heights[terX, terZ];
        y += incdec;
        float[,] height = new float[1, 1];
        height[0, 0] = Mathf.Clamp(y, 0, 1);
        heights[terX, terZ] = Mathf.Clamp(y, 0, 1);
        terrData.SetHeights(terX, terZ, height);
    }
    /// <summary>
    /// Blend Textures into Terrain
    /// </summary>
    /// <param name="pos">Position.</param>
    /// <param name="craterSizeInMeters">Area size in meters.</param>
    /// <param name="textureIDnum">Texture identifier number (from Terrain textures).</param>
    void TextureDeformation(Vector3 pos, float craterSizeInMeters, int textureIDnum)
    {
        Vector3 alphaMapTerrainPos = GetRelativeTerrainPositionFromPos(pos, TerrainComponent, alphaMapWidth, alphaMapHeight);
        int alphaMapCraterWidth = (int)(craterSizeInMeters * (alphaMapWidth / terrData.size.x));
        int alphaMapCraterLength = (int)(craterSizeInMeters * (alphaMapHeight / terrData.size.z));
        int alphaMapStartPosX = (int)(alphaMapTerrainPos.x - (alphaMapCraterWidth / 2));
        int alphaMapStartPosZ = (int)(alphaMapTerrainPos.z - (alphaMapCraterLength / 2));
        float[,,] alphas = terrData.GetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphaMapCraterWidth, alphaMapCraterLength);
        float circlePosX;
        float circlePosY;
        float distanceFromCenter;
        for(int i = 0; i < alphaMapCraterLength; i++) //width
        {
            for(int j = 0; j < alphaMapCraterWidth; j++) //height
            {
                circlePosX = (j - (alphaMapCraterWidth / 2)) / (alphaMapWidth / terrData.size.x);
                circlePosY = (i - (alphaMapCraterLength / 2)) / (alphaMapHeight / terrData.size.z);
                distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
                if(distanceFromCenter < (craterSizeInMeters / 2.0f))
                {
                    for(int layerCount = 0; layerCount < numOfAlphaLayers; layerCount++)
                    {
                        //could add blending here in the future
                        if(layerCount == textureIDnum)
                        {
                            alphas[i, j, layerCount] = 1;
                        }
                        else
                        {
                            alphas[i, j, layerCount] = 0;
                        }
                    }
                }
            }
        }
        terrData.SetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphas);
    }
    Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos, Terrain terrain)
    {
        //code based on: http://answers.unity3d.com/questions/3633/modifying-terrain-height-under-a-gameobject-at-runtime
        // get the normalized position of this game object relative to the terrain
        Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / terrData.size.x;
        coord.y = tempCoord.y / terrData.size.y;
        coord.z = tempCoord.z / terrData.size.z;

        return coord;
    }
    Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos, Terrain terrain, int mapWidth, int mapHeight)
    {
        Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos, terrain);
        // get the position of the terrain heightmap where this game object is
        return new Vector3((coord.x * mapWidth), 0, (coord.z * mapHeight));
    }
}