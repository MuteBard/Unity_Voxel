using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector2>;

public static class MeshUtils
{
    public enum BlockType
    {
        GRASSTOP, DIRT, STONE, GLASS, ICE, SAND, WATER,
    }

    public static Vector2[] blockUVs = {
        /*GRASSTOP*/ new Vector2(4, 15),
        /*DIRT*/ new Vector2(2, 15),
        /*STONE*/  new Vector2(1, 15),
        /*GLASS*/ new Vector2(1, 12),
        /*ICE*/ new Vector2(3, 11),
        /*SAND*/ new Vector2(2, 14),
        /*WATER*/ new Vector2(16, 3)
    };

    public static Vector2 GetblockUVData(BlockType bType){
        Vector2 blockuv;
        
        switch(bType){
            case BlockType.GRASSTOP:
                blockuv = blockUVs[0];
                break;
            case BlockType.DIRT:
                blockuv = blockUVs[1];
                break;
            case BlockType.STONE:
                blockuv = blockUVs[2];
                break;
            case BlockType.GLASS:
                blockuv = blockUVs[3];
                break;
            case BlockType.ICE:
                blockuv = blockUVs[4];
                break;
            case BlockType.SAND:
                blockuv = blockUVs[5];
                break;
            case BlockType.WATER:
                blockuv = blockUVs[6];
                break;
            default:
                blockuv = new Vector2(16, 16);
            break;
        }

        return blockuv;
    }
    
    public static Mesh MergeMeshes(Mesh[] meshes){
        
        Mesh mesh = new Mesh();
        Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>();
        HashSet<VertexData> pointsHash = new HashSet<VertexData>();
        List<int> triangles = new List<int>();

        int pIndex = 0;
        for(int i = 0; i < meshes.Length; i++){
            if(!meshes[i]) continue;
            for(int j = 0; j < meshes[i].vertices.Length; j++){
                Vector3 v = meshes[i].vertices[j];
                Vector3 n = meshes[i].normals[j];
                Vector3 u = meshes[i].uv[j];
                VertexData p = new VertexData(v, n, u);
                if(!pointsHash.Contains(p)){
                    pointsOrder.Add(p, pIndex);
                    pointsHash.Add(p);
                    pIndex++;
                }
            }

            for (int t = 0; t < meshes[i].triangles.Length; t++){
                int triPoint = meshes[i].triangles[t];
                Vector3 v = meshes[i].vertices[triPoint];
                Vector3 n = meshes[i].normals[triPoint];
                Vector3 u = meshes[i].uv[triPoint];
                VertexData p = new VertexData(v, n, u);
                int index;
                pointsOrder.TryGetValue(p, out index);
                triangles.Add(index);
            }
            meshes[i] = null;
        }
        ExtractArrays(pointsOrder, mesh);
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static void ExtractArrays(Dictionary <VertexData, int> list, Mesh mesh){
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        foreach (VertexData v in list.Keys){
            vertices.Add(v.Item1);
            normals.Add(v.Item2);
            uvs.Add(v.Item3);
        }
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();

    }
}