using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PerlinGrapher : MonoBehaviour
{
    [SerializeField] int chunkDepth = 11;
    [SerializeField] float aScale = 2f;
    [SerializeField] float fScale = 0.5f;
    [SerializeField] int octaves = 1;
    [SerializeField] float heightOffset = 1;
    [SerializeField] public LineRenderer lr;
    void Start()
    {
        lr = this.GetComponent<LineRenderer>();
        lr.positionCount = 100;
        Graph();
    }
    void Graph(){
        lr = this.GetComponent<LineRenderer>();
        lr.positionCount = 100;
        int z = chunkDepth;
        Vector3[] positions = new Vector3[lr.positionCount];
        for(int x = 0; x < lr.positionCount; x++ ){
            float y = MeshUtils.fBM(x, z, octaves, fScale, aScale, heightOffset);
            positions[x] = new Vector3(x, y, z);
        }
        lr.SetPositions(positions);
    }

    // public static float fBM(float x, float z, int octaves, float fScale, float aScale, float heightOffset){
    //     float total = 0;
    //     float frequency = 1;
    //     for(int i = 0; i < octaves; i++){
    //         total += Mathf.PerlinNoise(x * fScale * frequency, z * fScale * frequency) * aScale;
    //         frequency *= 2;
    //     }
    //     return total + heightOffset;
    // }
    

    void OnValidate(){
        Graph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
