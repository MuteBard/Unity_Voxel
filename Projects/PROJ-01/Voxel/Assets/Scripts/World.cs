using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public struct PerlinSettings {
    public float aScale;
    public float fScale;
    public int octaves;
    public float heightOffset;
    public float probability;

    public PerlinSettings(float aScale, float fScale, int octaves, float heightOffset, float probability){
        this.aScale = aScale;
        this.fScale = fScale;
        this.octaves = octaves;
        this.heightOffset = heightOffset;
        this.probability = probability;
    }
}

public class World : MonoBehaviour
{
    public static Vector3 worldDimension = new Vector3(10,20,10);
    public static Vector3 chunkDimensions = new Vector3(10,10,10);
    [SerializeField] GameObject chunkPrefab;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject fpc;
    [SerializeField] Slider loadingBar;

    public static PerlinSettings surfaceSettings;
    [SerializeField] PerlinGrapher surface;
    public static PerlinSettings stoneSettings;
    [SerializeField] PerlinGrapher stone;
    public static PerlinSettings caveTopSettings;
    [SerializeField] PerlinGrapher caveTop;
    public static PerlinSettings caveBottomSettings;
    [SerializeField] PerlinGrapher caveBottom;
    public static PerlinSettings urGoldSettings;
    [SerializeField] PerlinGrapher urGold;
    public static PerlinSettings caveSettings;
    [SerializeField] Perlin3DGrapher cave;

    
    void Start()
    {
        loadingBar.maxValue = worldDimension.x * worldDimension.y * worldDimension.z;
        surfaceSettings = new PerlinSettings(surface.aScale, surface.fScale, surface.octaves, surface.heightOffset, surface.probability);
        stoneSettings = new PerlinSettings(stone.aScale, stone.fScale, stone.octaves, stone.heightOffset, stone.probability);
        caveTopSettings = new PerlinSettings(caveTop.aScale, caveTop.fScale, caveTop.octaves, caveTop.heightOffset, caveTop.probability);
        caveBottomSettings = new PerlinSettings(caveBottom.aScale, caveBottom.fScale, caveBottom.octaves, caveBottom.heightOffset, caveBottom.probability);
        urGoldSettings = new PerlinSettings(urGold.aScale, urGold.fScale, urGold.octaves, urGold.heightOffset, urGold.probability);
        caveSettings = new PerlinSettings(cave.aScale, cave.fScale, cave.octaves, cave.heightOffset, cave.DrawCutOff);
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
        float ypos = MeshUtils.fBM(xpos, zpos, surfaceSettings.octaves, surfaceSettings.fScale, surfaceSettings.aScale, surfaceSettings.heightOffset) + 10;
        fpc.transform.position = new Vector3(xpos, ypos, zpos);
        loadingBar.gameObject.SetActive(false);
        fpc.SetActive(true);
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
