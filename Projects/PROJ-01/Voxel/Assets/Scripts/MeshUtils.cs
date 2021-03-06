using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector2>;

public static class MeshUtils
{
    public enum BlockType
    {
        GRASSTOP, GRASSSIDE, DIRT, STONE, GLASS, ICE, SAND, WATER, AIR, UR_GOLD
    }
        public enum BlockSide {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    }


    public static Vector2[] blockUVs = {
        /*GRASSTOP*/ new Vector2(2, 6),
        /*GRASSSIDE*/ new Vector2(3, 15),
        /*DIRT*/ new Vector2(2, 15),
        /*STONE*/  new Vector2(1, 15),
        /*GLASS*/ new Vector2(1, 12),
        /*ICE*/ new Vector2(3, 11),
        /*SAND*/ new Vector2(2, 14),
        /*WATER*/ new Vector2(15, 3),
        /*UNREFINED_GOLD*/ new Vector2(0, 13)
    };

    public static Vector2 GetBlockUVData(BlockType bType){
        Vector2 blockuv;
        
        switch(bType){
            case BlockType.GRASSTOP:
                blockuv = blockUVs[0];
                break;
            case BlockType.GRASSSIDE:
                blockuv = blockUVs[1];
                break;
            case BlockType.DIRT:
                blockuv = blockUVs[2];
                break;
            case BlockType.STONE:
                blockuv = blockUVs[3];
                break;
            case BlockType.GLASS:
                blockuv = blockUVs[4];
                break;
            case BlockType.ICE:
                blockuv = blockUVs[5];
                break;
            case BlockType.SAND:
                blockuv = blockUVs[6];
                break;
            case BlockType.WATER:
                blockuv = blockUVs[7];
                break;
            case BlockType.UR_GOLD:
                blockuv = blockUVs[8];
                break;
            default:
                blockuv = new Vector2(16, 16);
            break;
        }

        return blockuv;
    }

    public static float fBM(float x, float z, int octaves, float fScale, float aScale, float heightOffset){
        float total = 0;
        float frequency = 1;
        for(int i = 0; i < octaves; i++){
            total += Mathf.PerlinNoise(x * fScale * frequency, z * fScale * frequency) * aScale;
            frequency *= 2;
        }
        return total + heightOffset;
    }

    public static float fBM3D(float x, float y, float z, int octaves, float fScale, float aScale, float heightOffset){
        float XY = fBM(x, y, octaves, fScale, aScale, heightOffset);
        float XZ = fBM(x, z, octaves, fScale, aScale, heightOffset);
        float YZ = fBM(y, z, octaves, fScale, aScale, heightOffset);
        float YX = fBM(y, x, octaves, fScale, aScale, heightOffset);
        float ZX = fBM(z, x, octaves, fScale, aScale, heightOffset);
        float ZY = fBM(z, y, octaves, fScale, aScale, heightOffset);

        return (XY + XZ + YZ + YX + ZX + ZY) / 6.0f;
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