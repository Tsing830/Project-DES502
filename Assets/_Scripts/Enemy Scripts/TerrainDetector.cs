using UnityEngine;

public class TerrainDetector
{
    public int GetActiveTerrainTextureIdx(Vector3 worldPos)
    {
        var terrain = Terrain.activeTerrain;
        if (terrain == null || terrain.terrainData == null)
        {
            return 2;
        }

        var terrainPos = terrain.GetPosition();
        var terrainData = terrain.terrainData;
        var terrainSize = terrainData.size;

        float normalizedX = Mathf.Clamp01((worldPos.x - terrainPos.x) / terrainSize.x);
        float normalizedZ = Mathf.Clamp01((worldPos.z - terrainPos.z) / terrainSize.z);

        int mapX = Mathf.Clamp(
            Mathf.FloorToInt(normalizedX * terrainData.alphamapWidth),
            0,
            terrainData.alphamapWidth - 1);
        int mapZ = Mathf.Clamp(
            Mathf.FloorToInt(normalizedZ * terrainData.alphamapHeight),
            0,
            terrainData.alphamapHeight - 1);

        float[,,] splatmap = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        int textureCount = splatmap.GetLength(2);
        if (textureCount == 0)
        {
            return 2;
        }

        int dominantTextureIndex = 0;
        float highestMix = 0f;

        for (int i = 0; i < textureCount; i++)
        {
            float mix = splatmap[0, 0, i];
            if (mix > highestMix)
            {
                highestMix = mix;
                dominantTextureIndex = i;
            }
        }

        return dominantTextureIndex;
    }
}
