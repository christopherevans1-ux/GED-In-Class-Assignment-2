using UnityEngine;

public class RoadLooper : MonoBehaviour
{
    public Transform[] roadTiles;
    public float speed = 12f;
    public float tileLength = 100f; 

    void Update()
    {
        foreach (Transform tile in roadTiles)
        {
            tile.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

            if (tile.position.z <= -tileLength)
            {
                float highestZ = GetHighestZ();
                tile.position = new Vector3(
                    tile.position.x,
                    tile.position.y,
                    highestZ + tileLength
                );
            }
        }
    }

    float GetHighestZ()
    {
        float highest = roadTiles[0].position.z;

        foreach (Transform tile in roadTiles)
        {
            if (tile.position.z > highest)
                highest = tile.position.z;
        }

        return highest;
    }
}