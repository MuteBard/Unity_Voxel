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

    [SerializeField] BlockSide side;

    void Start()
    {
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();

        Quad q = new Quad();
        Vector3 origin = new Vector3(1,1,1);
        mf.mesh = q.Build(side, origin);
    }
}
