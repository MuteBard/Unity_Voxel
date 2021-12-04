using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector2>;

public static class MeshUtils
{
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