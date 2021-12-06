using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum BlockSide {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    }
    [SerializeField] Material atlas;
    [SerializeField] MeshUtils.BlockType blockType;
    void Start()
    {
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        mr.material = atlas;

        Vector2 blockUVData = MeshUtils.GetblockUVData(blockType);

        Quad[] quads = new Quad[6];
        quads[0] = new Quad(BlockSide.BOTTOM, new Vector3(0,0,0), blockUVData);
        quads[1] = new Quad(BlockSide.TOP, new Vector3(0,0,0), blockUVData);
        quads[2] = new Quad(BlockSide.LEFT, new Vector3(0,0,0), blockUVData);
        quads[3] = new Quad(BlockSide.RIGHT, new Vector3(0,0,0), blockUVData);
        quads[4] = new Quad(BlockSide.FRONT, new Vector3(0,0,0), blockUVData);
        quads[5] = new Quad(BlockSide.BACK, new Vector3(0,0,0), blockUVData);

        Mesh[] sideMeshes = new Mesh[6];
        sideMeshes[0] = quads[0].mesh;
        sideMeshes[1] = quads[1].mesh;
        sideMeshes[2] = quads[2].mesh;
        sideMeshes[3] = quads[3].mesh;
        sideMeshes[4] = quads[4].mesh;
        sideMeshes[5] = quads[5].mesh;

        mf.mesh = MeshUtils.MergeMeshes(sideMeshes);
        mf.mesh.name = $"Cube_{0}_{0}_{0}";
    }
}
