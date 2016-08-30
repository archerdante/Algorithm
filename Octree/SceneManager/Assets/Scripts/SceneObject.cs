using UnityEngine;
using System.Collections;


// 场景物体描述
public class SceneObject
{
    public GameObject m_aGameObject;        // 真实场景对象
    public Bounds m_aBounds;                // 场景对象的包围盒
    public bool isLoaded = false;  //是否被加载
    public Octree OctreeParent;  //八叉树中父亲节点
}
