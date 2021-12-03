# DEV-02, 3D Array and Data Management
#### Tags: [array]


## 3D Arrays

    In creating a voxel world, it makes sense to use the indicies in the order width, height and depth
    int [,,] world = new int[width, height, depth]; (x, y, z)

    for(int z = 0; z < depth; z++)
        for(int y = 0; y < height; y++)
            for(int x = 0; x < width; x++)

## Performance when creating
    When we go about our business of creating a very blocky world, we need to take into consideration the number of materials and the number of meshes that are in it. You cannot build a world out of cubes, performance will drop significantly.


![](../images/DEV-02/DEV-02-A.png)