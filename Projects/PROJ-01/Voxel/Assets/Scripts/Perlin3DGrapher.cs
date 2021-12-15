using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Perlin3DGrapher : MonoBehaviour
{
    Vector3 dimensions = new Vector3(5,5,5);
    [SerializeField] bool on = false;
    [SerializeField] [Range(0f, 4f)] public float aScale = 2f;
    [SerializeField] [Range(0f, 0.5f)] public float fScale = 0.5f;
    [SerializeField] public int octaves = 1;
    [SerializeField] public float heightOffset = 1;
    [SerializeField] [Range(0, 10f)] public float DrawCutOff = 1;

    void CreateCubes(){
        for(int z = 0; z < dimensions.z; z++){
            for(int y = 0; y < dimensions.y; y++){
                for(int x = 0; x < dimensions.x; x++){
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "perlin_cube";
                    cube.transform.parent = this.transform;
                    cube.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    void Graph(){
        //destroy existing cubes
        MeshRenderer[] cubes = this.GetComponentsInChildren<MeshRenderer>();
        if(cubes.Length == 0){
            CreateCubes();
        }
        if(cubes.Length == 0) return;
        for(int z = 0; z < dimensions.z; z++){
            for(int y = 0; y < dimensions.y; y++){
                for(int x = 0; x < dimensions.x; x++){
                    float p3d = MeshUtils.fBM3D(x, y, z, octaves, fScale, aScale, heightOffset);
                    if(p3d < DrawCutOff){
                        cubes[flatten(x, y, z, dimensions.x, dimensions.z)].enabled = false;
                    }else{
                         cubes[flatten(x, y, z, dimensions.x, dimensions.z)].enabled = true;
                    }
                }
            }
        }
    }

    private int flatten(int x, int y, int z, float width, float depth){
        return x + (int) width * (y + (int) depth * z);
    }

    void OnValidate(){
        if(on) Graph();
    }
}
