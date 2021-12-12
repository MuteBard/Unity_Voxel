using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Mesh mesh;
    MeshUtils.BlockType blockType;
    public Block(Vector3 offset, MeshUtils.BlockType blockType)
    {
        Vector2 blockUVData = MeshUtils.GetblockUVData(blockType);
        Quad[] quads = new Quad[6];
        quads[0] = new Quad(MeshUtils.BlockSide.BOTTOM, offset, blockUVData);
        quads[1] = new Quad(MeshUtils.BlockSide.TOP, offset, blockUVData);
        quads[2] = new Quad(MeshUtils.BlockSide.LEFT, offset, blockUVData);
        quads[3] = new Quad(MeshUtils.BlockSide.RIGHT, offset, blockUVData);
        quads[4] = new Quad(MeshUtils.BlockSide.FRONT, offset, blockUVData);
        quads[5] = new Quad(MeshUtils.BlockSide.BACK, offset, blockUVData);

        Mesh[] sideMeshes = new Mesh[6];
        sideMeshes[0] = quads[0].mesh;
        sideMeshes[1] = quads[1].mesh;
        sideMeshes[2] = quads[2].mesh;
        sideMeshes[3] = quads[3].mesh;
        sideMeshes[4] = quads[4].mesh;
        sideMeshes[5] = quads[5].mesh;

        mesh = MeshUtils.MergeMeshes(sideMeshes);
        mesh.name = $"Cube_{0}_{0}_{0}";
    }
}
