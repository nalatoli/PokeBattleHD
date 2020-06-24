using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    /// <summary> Backing field of terrain. </summary>
    private static Terrain terrain;

    private float[,] originalTerrainHeightMap;

    private void Awake()
    {
        /* Establish References */
        terrain = Terrain.activeTerrain;

        /* Save Original Height Map */
        originalTerrainHeightMap = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
    }

    /// <summary> Offsets terrain by designated height offset map. </summary>
    /// <param name="pos"> Center of offset map. Heightmap is centered around this positon. </param>
    /// <param name="heightMap"> Offset of current height. [y,x]. </param>
    /// <param name="add_subtract"> True for adding offset, false for subtracting offset. </param>
    public static void SetTerrainHeightOffset(Vector3 pos, float[,] heightMap, bool add_subtract)
    {
        /* Get Normalized Coordinates Of Position */
        Vector3 coord = pos.DividedBy(terrain.terrainData.size);

        /* Get Size Of Height Map */
        Vector2Int mapSize = new Vector2Int(
            heightMap.GetLength(1),
            heightMap.GetLength(0));

        /* Get Offset Of Height Map */
        Vector2Int mapOffset = new Vector2Int(
            mapSize.x / 2,
            mapSize.y / 2);

        /* Get World Coordinates Of Center */
        int posXInTerrain = (int)(coord.x * terrain.terrainData.heightmapResolution);
        int posYInTerrain = (int)(coord.z * terrain.terrainData.heightmapResolution);

        /* Get Heights Surrounding The Center */
        float[,] heights = terrain.terrainData.GetHeights(posXInTerrain - mapOffset.x, posYInTerrain - mapOffset.y, mapSize.x, mapSize.y);

        /* Set HeightMap */
        for (int i = 0; i < mapSize.y; i++)
            for (int j = 0; j < mapSize.x; j++)
            {
                if(add_subtract == true)
                    heights[i, j] += heightMap[i, j];
                else
                    heights[i, j] -= heightMap[i, j];
            }
                

        /* Apply HeightMap */
        terrain.terrainData.SetHeights(posXInTerrain - mapOffset.x, posYInTerrain - mapOffset.y, heights);
    }

    /// <summary> Offsets terrain by designated texture alpha levels. </summary>
    /// <param name="center"> Center of offset map. Heightmap is centered around this positon. </param>
    /// <param name="texture"> Black and White texture texture where UV colors represent height (black 0, white 1). </param>
    /// <param name="sampleRadius"> Radius in samples of effect from center of offset map. </param>
    /// <param name="riseMultiplier"> Factor to multiply to green. </param>
    /// <param name="fallMultiplier"> Factor to multiply to red. </param>
    public static void TessellateTerrain(Vector3 center, Texture2D texture, int sampleRadius, float riseMultiplier, float fallMultiplier)
    {
        /* Get Normalized Coordinates Of Center Position Within Terrain */
        Vector3 coord = center.DividedBy(terrain.terrainData.size);

        /* Get World Coordinates Of Center Position From Normalized Coordinates */
        int posXInTerrain = (int)(coord.x * terrain.terrainData.heightmapResolution);
        int posYInTerrain = (int)(coord.z * terrain.terrainData.heightmapResolution);

        /* Get Sample Diameter (Width/Height of New Height Map) */
        int sampleDiameter = sampleRadius * 2;

        /* Initialize Mip Size  */
        int mipSize = 0;

        /* If Texture Size Is Larger Than The Diameter (Must Be Scaled Down) */
        if (texture.width > sampleDiameter)
        {
            /* If Sample Diameter Is Already A Power Of 2, Get The Corrosponding Mip Size */
            if (Mathf.IsPowerOfTwo(sampleDiameter))
                mipSize = (int)Mathf.Log(
                    texture.width / 
                    sampleDiameter, 
                    2);

            /* Else, Get The Largest Power Of 2 Below This Diameter
             * and Use That Value To Get The Corrosponding Mip Size */
            else
                mipSize = (int)Mathf.Log(
                    texture.width /
                    Mathf.NextPowerOfTwo(sampleDiameter) * 2, 
                    2);

        }

        /* Get Colors From Texture With New Mip Size */
        Color[] values = texture.GetPixels(mipSize);

        /* Get Size Of New Texture Dimensions */
        int texSize = (int)Mathf.Sqrt(values.Length);

        /* Get Ratio Between Adjusted Texture and New Height Map Sizes */
        float ratio = (float)texSize / sampleDiameter;

        /* Initialize New Height Map Using Sample Diameter */
        float[,] heights = terrain.terrainData.GetHeights(posXInTerrain - sampleRadius, posYInTerrain - sampleRadius, sampleDiameter, sampleDiameter);

        /* For All Height Map Coordinates */
        for (int y = 0; y < sampleDiameter; y++)
            for (int x = 0; x < sampleDiameter; x++)
            {
                /* Get Texture Map Alpha For Current Coordinates Using Ratio */
                Color value = values[(int)(x * ratio) + (int)(y * ratio) * texSize];

                /* Load Adjusted Alpha As New Height For This Coordinate */
                heights[x, y] += (value.g * riseMultiplier - value.r * fallMultiplier) /  256;

            }

        /* Apply New HeightMap */
        terrain.terrainData.SetHeights(posXInTerrain - sampleRadius, posYInTerrain - sampleRadius, heights);

    }

    private void OnApplicationQuit()
    {
        /* Restore Original Height Map */
        terrain.terrainData.SetHeights(0, 0, originalTerrainHeightMap);
    }
}
