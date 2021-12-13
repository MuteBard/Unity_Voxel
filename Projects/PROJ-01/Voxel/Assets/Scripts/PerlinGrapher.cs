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
    [SerializeField] float height = 1;
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
            float y = MeshUtils.fBM(x, z, octaves, fScale, aScale, height);
            positions[x] = new Vector3(x, y, z);
        }
        lr.SetPositions(positions);
    }

    void OnValidate(){
        Graph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
