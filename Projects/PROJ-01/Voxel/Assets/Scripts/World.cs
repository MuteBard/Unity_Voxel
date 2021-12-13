using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public static Vector3 worldDimension = new Vector3(10,10,10);
    public static Vector3 chunkDimensions = new Vector3(10,10,10);
    [SerializeField] GameObject chunkPrefab;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject fpc;
    [SerializeField] Slider loadingBar;
    
    void Start()
    {
        loadingBar.maxValue = worldDimension.x * worldDimension.y * worldDimension.z;
        StartCoroutine(BuildWorld());
    }

    IEnumerator BuildWorld(){
        for(int z = 0; z < worldDimension.z; z++){
            for(int y = 0; y < worldDimension.y; y++){
                for(int x = 0; x < worldDimension.x; x++){
                    GameObject chunk = Instantiate(chunkPrefab);
                    Vector3 position = new Vector3(x * chunkDimensions.x, y * chunkDimensions.y, z * chunkDimensions.z);
                    chunk.GetComponent<Chunk>().CreateChunk(chunkDimensions, position);
                    loadingBar.value++;
                    yield return null;
                }
            }
        }
        mainCamera.SetActive(false);
        float xpos = (worldDimension.x * chunkDimensions.x) / 2f;
        float zpos = (worldDimension.z * chunkDimensions.z) / 2f;
        Chunk c = chunkPrefab.GetComponent<Chunk>();
        float ypos = MeshUtils.fBM(xpos, zpos, c.octaves, c.fScale, c.aScale, c.heightOffset) + 10;
        fpc.transform.position = new Vector3(xpos, ypos, zpos);
        loadingBar.gameObject.SetActive(false);
        fpc.SetActive(true);
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
