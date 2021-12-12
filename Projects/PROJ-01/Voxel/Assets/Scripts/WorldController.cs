using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    [SerializeField] GameObject block;
    [SerializeField] int width = 10;
    [SerializeField] int height = 5;
    [SerializeField] int depth = 10;
    [SerializeField] [Range(0, 100)] int randomFactor = 5;
    [SerializeField] int destroyTopLayers = 2;

    void Start(){
        destroyTopLayers = destroyTopLayers > height ? height : destroyTopLayers;
        StartCoroutine(BuildWorld(width, height, depth));
    }
    void Update(){

    }

    public IEnumerator BuildWorld(int width, int height, int depth){
        for(int z = 0; z < depth; z++){
            for(int y = 0; y < height; y++){
                for(int x = 0; x < width; x++){
                    if(y >= height - destroyTopLayers && Randomness()){
                        continue;
                    }else{
                        createCube(x, y, z);
                    }
                }
            }
            yield return null;
        }
    }

    public bool Randomness(){
        var value = UnityEngine.Random.Range(1, 100);
        return value > randomFactor;
    }

    public void createCube(int x, int y, int z){
        Vector3 pos = new Vector3(x,y,z);
        GameObject cube = GameObject.Instantiate(block, pos, Quaternion.identity, this.transform);
        cube.name = $"{x}_{y}_{z}";
    }
}
