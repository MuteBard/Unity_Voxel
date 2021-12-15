using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class Chunk : MonoBehaviour
{
    
    [Header("Blocks")]
    [SerializeField] Material atlas;
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] public int depth;
    [SerializeField] [Range(0, 100)] int randomFactor = 5;

    public Block[,,] blocks;

    [Header("BlockType")]
    [SerializeField] MeshUtils.BlockType blockType1;
    [SerializeField] MeshUtils.BlockType blockType2;
    [SerializeField] MeshUtils.BlockType blockType3;
    [SerializeField] MeshUtils.BlockType blockType4;

    public Vector3 location;
    //Flat [ * (y + depth * z)] = Original[x,y,z]
    //x = i % width
    //y = (i / width) % height
    //z = i / (width * height)
    public MeshUtils.BlockType[] chunkData;

    void BuildChunk(){
        int blockCount = width * depth * height;
        chunkData = new MeshUtils.BlockType[blockCount];
        for(int i = 0; i < blockCount; i++){
            int x = i % width + (int) location.x;
            int y = (i / width) % height + (int) location.y;
            int z = i / (width * height) + (int) location.z;
            int surfaceHeight = (int) MeshUtils.fBM(x, z, World.surfaceSettings.octaves, World.surfaceSettings.fScale, World.surfaceSettings.aScale, World.surfaceSettings.heightOffset);
            int stoneHeight = (int) MeshUtils.fBM(x, z, World.stoneSettings.octaves, World.stoneSettings.fScale, World.stoneSettings.aScale, World.stoneSettings.heightOffset);
            int caveTopHeight = (int) MeshUtils.fBM(x, z, World.caveTopSettings.octaves, World.caveTopSettings.fScale, World.caveTopSettings.aScale, World.caveTopSettings.heightOffset);
            int caveBottomHeight = (int) MeshUtils.fBM(x, z, World.caveBottomSettings.octaves, World.caveBottomSettings.fScale, World.caveBottomSettings.aScale, World.caveBottomSettings.heightOffset);
            int urGoldHeight = (int) MeshUtils.fBM(x, z, World.urGoldSettings.octaves, World.urGoldSettings.fScale, World.urGoldSettings.aScale, World.urGoldSettings.heightOffset);
            int cave = (int) MeshUtils.fBM3D(x, y, z, World.caveSettings.octaves, World.caveSettings.fScale, World.caveSettings.aScale, World.caveSettings.heightOffset);


            if(surfaceHeight == y){
                chunkData[i] = blockType1;
            }else if(y < stoneHeight && y > caveBottomHeight - 5 && !(y < caveTopHeight && y > caveBottomHeight) && UnityEngine.Random.Range(0.0f, 1.0f) <= World.urGoldSettings.probability){
                chunkData[i] = blockType4;
            }
            else if(y < caveTopHeight && y > caveBottomHeight){
                chunkData[i] = MeshUtils.BlockType.AIR;
            }
            else if(y < stoneHeight){
                chunkData[i] = blockType3;
            }else if(y < surfaceHeight){
                chunkData[i] = blockType2;
            }else{
                chunkData[i] = MeshUtils.BlockType.AIR;
            }

            if(cave < World.caveSettings.probability && y < surfaceHeight - 5){
                chunkData[i] = MeshUtils.BlockType.AIR;
            }
        }
    }
    public bool Randomness(){
        var value = UnityEngine.Random.Range(1, 100);
        return value > randomFactor;
    }

    void Start(){

    }
    public void CreateChunk(Vector3 dimensions, Vector3 position)
    {
        location = position;
        width = (int) dimensions.x;
        height = (int) dimensions.y;
        depth = (int) dimensions.z;
        // Debug.Log($"width({width}), height({height}), depth({depth})");

        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        mr.material = atlas;
        blocks = new Block[width, height, depth];
        BuildChunk();

        var inputMeshes = new List<Mesh>();
        int vertexStart = 0;
        int triStart = 0;
        int meshCount = width * height * depth;
        int m = 0;
        var jobs = new ProcessMeshDataJob();
        jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        jobs.triStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

         for(int z = 0; z < depth; z++){
            for(int y = 0; y < height; y++){
                for(int x = 0; x < width; x++){
                    blocks[x, y, z] = new Block(new Vector3(x, y, z) + location, chunkData[x + width * (y + depth * z)], this);
                    if(blocks[x, y, z].mesh != null){
                        inputMeshes.Add(blocks[x, y, z].mesh);
                        var vertexCount = blocks[x, y, z].mesh.vertexCount;
                        var indexCount = (int)blocks[x, y, z].mesh.GetIndexCount(0);
                        jobs.vertexStart[m] = vertexStart;
                        jobs.triStart[m] = triStart;
                        vertexStart += vertexCount;
                        triStart += indexCount;
                        m++;
                    }
                } 
            }
        }

        jobs.meshData = Mesh.AcquireReadOnlyMeshData(inputMeshes);
        var outputMeshData = Mesh.AllocateWritableMeshData(1);
        jobs.outputMesh = outputMeshData[0];
        jobs.outputMesh.SetIndexBufferParams(triStart, IndexFormat.UInt32);
        jobs.outputMesh.SetVertexBufferParams(vertexStart,
            new VertexAttributeDescriptor(VertexAttribute.Position),
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 2)
        );

        var handle = jobs.Schedule(inputMeshes.Count, 4);
        var newMesh = new Mesh();
        newMesh.name = $"Chunk_{location.x}_{location.y}_{location.z}";
        var sm = new SubMeshDescriptor(0, triStart, MeshTopology.Triangles);
        sm.firstVertex = 0;
        sm.vertexCount = vertexStart;

        handle.Complete();

        jobs.outputMesh.subMeshCount = 1;
        jobs.outputMesh.SetSubMesh(0, sm);

        Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, new[] { newMesh });

        jobs.meshData.Dispose();
        jobs.vertexStart.Dispose();
        jobs.triStart.Dispose();
        newMesh.RecalculateBounds();

        mf.mesh = newMesh;
        MeshCollider collider = this.gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = mf.mesh;
    }

    [BurstCompile]
    struct ProcessMeshDataJob: IJobParallelFor{
        [ReadOnly] public Mesh.MeshDataArray meshData;
        public Mesh.MeshData outputMesh;
        public NativeArray<int> vertexStart;
        public NativeArray<int> triStart;

        public void Execute(int index){
            var data = meshData[index];
            var vCount = data.vertexCount;
            var vStart = vertexStart[index];

            var verts = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(verts.Reinterpret<Vector3>());
            var normals = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(normals.Reinterpret<Vector3>());
            var uvs = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(0, uvs.Reinterpret<Vector3>());

            var outputVerts = outputMesh.GetVertexData<Vector3>();
            var outputNormals = outputMesh.GetVertexData<Vector3>(stream: 1);
            var outputUVs = outputMesh.GetVertexData<Vector3>(stream: 2);

            for(int i = 0; i < vCount; i++){
                outputVerts[i + vStart] = verts[i];
                outputNormals[i + vStart] = normals[i];
                outputUVs[i + vStart] = uvs[i];
            }

            verts.Dispose();
            normals.Dispose();
            uvs.Dispose();

            var tStart = triStart[index];
            var tCount = data.GetSubMesh(0).indexCount;
            var outputTris = outputMesh.GetIndexData<int>();

            if(data.indexFormat == IndexFormat.UInt16){
                var tris = data.GetIndexData<ushort>();
                for(int i = 0; i < tCount; ++i){
                    int idx = tris[i];
                    outputTris[i + tStart] = vStart + idx;
                }
            }else{
                var tris = data.GetIndexData<int>();
                for(int i = 0; i < tCount; ++i){
                    int idx = tris[i];
                    outputTris[i + tStart] = vStart + idx;
                }
            }
        }
    }
}