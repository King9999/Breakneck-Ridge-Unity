using UnityEngine;
using System.Collections;

public class StaticSpawnScript : MonoBehaviour {

	// Use this for initialization
    public int numberOfTrees = 500;
    public GameObject treePrefab;
    public GameObject rockPrefab;

    private int numberOfRocks = 0;

    private Terrain mountain;

    private float minX = 0;
    private float maxX = 0;
    private float minZ = 0;
    private float maxZ = 0;

    void Start () 
    {
        mountain = Terrain.activeTerrain;
        //Debug.Log("=====" + mountain.terrainData.size.ToString());

        minX = -mountain.terrainData.size.x / 2 + mountain.terrainData.size.x / 10;
        maxX = mountain.terrainData.size.x / 2 - mountain.terrainData.size.x / 10;
        minZ = -mountain.terrainData.size.z / 2 + mountain.terrainData.size.z / 10;
        maxZ = mountain.terrainData.size.z / 2 - mountain.terrainData.size.z / 10;

        Debug.Log("MinX = " + minX + " MinZ = " + minZ);
        for (var i = 0; i < numberOfTrees; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            var pos = new Vector3(randomX, 0, randomZ);

            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.up, out hit))
            {
                pos.y = hit.distance;
            }

            Instantiate(treePrefab, pos, Quaternion.identity);
        }

        numberOfRocks = 750;
        for (var j = 0; j < numberOfRocks; j++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            var pos = new Vector3(randomX, 0, randomZ);

            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.up, out hit))
            {
                pos.y = hit.distance - 2;
            }
            
            Instantiate(rockPrefab, pos, Quaternion.identity);
        }

    }


}
