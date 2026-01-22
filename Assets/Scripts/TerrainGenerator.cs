using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Tinh chinh dia hinh")]
    public float scale = 5f;       // Càng nhỏ đồi càng to rộng ra (Nên để 3 - 10)
    public float height = 50f;     // Độ cao tối đa của núi (Nên để 30 - 100)

    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        // Lấy kích thước hiện tại
        int width = terrainData.heightmapResolution;
        int length = terrainData.heightmapResolution;

        // QUAN TRỌNG: Cập nhật lại kích thước thật của Terrain theo ý muốn
        // X = Rộng, Y = Cao (chính là height), Z = Dài
        terrainData.size = new Vector3(terrainData.size.x, height, terrainData.size.z);

        float[,] heights = new float[width, length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                // Tính tọa độ Perlin Noise
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / length * scale;

                // Gán giá trị
                heights[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }

        terrainData.SetHeights(0, 0, heights);
        return terrainData;
    }
}