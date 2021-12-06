using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Quad
{
    public Mesh mesh;
    Vector3[] vertices = new Vector3[4];
    Vector3[] normals = new Vector3[4];
    Vector2[] uvs = new Vector2[4];
    int[] triangles = new int[6];

    public Quad(Block.BlockSide side, Vector3 offset, Vector2 blockUVData)
    {
        mesh = new Mesh();
        mesh.name = "ScriptedQuad";
        mesh = GetFace(side, mesh, offset, blockUVData);
        mesh.RecalculateBounds();
    }

    private Vector2 getUVFromAtlus( int x, int y, int uvx, int uvy){
        float atlusSquareSize = 0.0625f;

        if(uvx != 0 && uvx != 1){
            throw new System.Exception("uvx is out of bounds");
        }
        if(uvy != 0 && uvy != 1){
            throw new System.Exception("uvy is out of bounds");
        }

        // Debug.Log($"{atlusSquareSize * (x + uvx)}, {atlusSquareSize * (y + uvy)}");
        return new Vector2(atlusSquareSize * (x + uvx), atlusSquareSize * (y + uvy));
    }

    private void setUVs(Vector2 blockUVData){
        Vector2 uv00 = getUVFromAtlus((int) blockUVData.x, (int) blockUVData.y, 0, 0);
        Vector2 uv10 = getUVFromAtlus((int) blockUVData.x, (int) blockUVData.y, 1, 0);
        Vector2 uv01 = getUVFromAtlus((int) blockUVData.x, (int) blockUVData.y, 0, 1);
        Vector2 uv11 = getUVFromAtlus((int) blockUVData.x, (int) blockUVData.y, 1, 1);

        uvs = new Vector2[] {
            uv00,
            uv10,
            uv01,
            uv11
        };
    }

    private void setVerticesandNormals(Block.BlockSide side, Vector3 offset){

        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f) + offset;
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f) + offset;
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f) + offset;
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f) + offset;
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f) + offset;
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f) + offset;
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f) + offset;
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f) + offset;

        switch(side){
            case Block.BlockSide.FRONT:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                break;
            case Block.BlockSide.BACK:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                break;
            case Block.BlockSide.TOP:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                break;
            case Block.BlockSide.BOTTOM:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                break;
            case Block.BlockSide.LEFT:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                break;
            case Block.BlockSide.RIGHT:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                break;
        }
    }

    private void setTriangles(){
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };
    }


    private Mesh GetFace(Block.BlockSide side, Mesh mesh, Vector3 offset, Vector2 blockUVData){        
        
        setUVs(blockUVData);
        setVerticesandNormals(side, offset);
        setTriangles();

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        return mesh;
    }
}
