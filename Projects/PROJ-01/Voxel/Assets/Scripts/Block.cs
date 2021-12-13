using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Mesh mesh;
    MeshUtils.BlockType blockType;
    Chunk parentChunk;
    public Block(Vector3 offset, MeshUtils.BlockType blockType, Chunk chunk)
    {
        parentChunk = chunk;
        Vector3 blockLocalPos = offset - chunk.location;

        if(blockType == MeshUtils.BlockType.AIR) return;

        Vector2 blockUVData = MeshUtils.GetblockUVData(blockType);
        List<Quad> quads = new List<Quad>();
        if(!HasSolidNeighbor( (int) blockLocalPos.x, (int) blockLocalPos.y - 1, (int) blockLocalPos.z ))
            quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, blockUVData));
        if(!HasSolidNeighbor( (int) blockLocalPos.x, (int) blockLocalPos.y + 1, (int) blockLocalPos.z ))
            quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, blockUVData));
        if(!HasSolidNeighbor( (int) blockLocalPos.x - 1, (int) blockLocalPos.y, (int) blockLocalPos.z ))
            quads.Add(new Quad(MeshUtils.BlockSide.LEFT, offset, blockUVData));
        if(!HasSolidNeighbor( (int) blockLocalPos.x + 1, (int) blockLocalPos.y, (int) blockLocalPos.z ))
            quads.Add(new Quad(MeshUtils.BlockSide.RIGHT, offset, blockUVData));
        if(!HasSolidNeighbor( (int) blockLocalPos.x, (int) blockLocalPos.y, (int) blockLocalPos.z + 1 ))
            quads.Add( new Quad(MeshUtils.BlockSide.FRONT, offset, blockUVData));
        if(!HasSolidNeighbor( (int) blockLocalPos.x, (int) blockLocalPos.y, (int) blockLocalPos.z - 1 ))
            quads.Add(new Quad(MeshUtils.BlockSide.BACK, offset, blockUVData));

        if(quads.Count == 0) return;


        Mesh[] sideMeshes = new Mesh[quads.Count];
        int m = 0;
        foreach(Quad q in quads){
            sideMeshes[m] = q.mesh;
            m++;
        }

        mesh = MeshUtils.MergeMeshes(sideMeshes);
        mesh.name = $"Cube_{blockLocalPos.x}_{blockLocalPos.y}_{blockLocalPos.z}";
    }


    public bool HasSolidNeighbor(int x, int y, int z){
        if(
            x < 0 || x >= parentChunk.width ||
            y < 0 || y >= parentChunk.height ||
            z < 0 || z >= parentChunk.depth
        ){
            return false;
        }
        if(
            parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.AIR ||
            parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.WATER
        ){
            return false;
        }
        return true;
    }
}
