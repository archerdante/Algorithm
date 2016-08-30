using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//八叉树
public class Octree {
    public Octree[] child = new Octree[8];  //子节点
    public List<SceneObject> objList = new List<SceneObject>();  //数据节点
    private Bounds nodeBound;  //用于计算边界
    public Vector3 center;  //中心点
    public float radius;   //半径
    public bool isleaf = false;  //是否是叶子节点

    public Octree(float _radius, Vector3 _center)
    {
        radius = _radius;
        center = _center;
        nodeBound = new Bounds(_center, new Vector3(_radius*2, _radius * 2, _radius * 2));
    }


    /// <summary>
    /// 建立空树
    /// </summary>
    /// <param name="minRadius">最小区块的半径</param>
    public void buildTree(float minRadius)
    {
        //使用类似BFS的方法。利用队列进行迭代建树
        //根据最小半径计算队列需要的最大长度，提前申请空间提高性能
        int p = (int)Math.Log((int)((radius / minRadius)),2);
        int quemaxCount = (int)(Math.Pow(8 , p));
        Queue<Octree> myQue = new Queue<Octree>(quemaxCount);
        myQue.Enqueue(this);
        int layerCount = 1;
        float tempRadius = radius * 0.5f;
        Octree tempNode;
        while (tempRadius >= minRadius)
        {
            while (layerCount > 0)
            {
                tempNode = myQue.Dequeue();
                for (int i = 0; i < 8; i++)
                {
                    //八个子节点，使用bit标记更容易分配
                    Vector3 _center = new Vector3();
                    if ((i & 1) == 1) _center.x = tempNode.center.x + tempRadius;
                    else _center.x = tempNode.center.x - tempRadius;
                    if (((i >> 1) & 1) == 1) _center.y = tempNode.center.y + tempRadius;
                    else _center.y = tempNode.center.y - tempRadius;
                    if (((i >> 2) & 1) == 1) _center.z = tempNode.center.z + tempRadius;
                    else _center.z = tempNode.center.z - tempRadius;
                    tempNode.child[i] = new Octree(tempRadius, _center);
                    myQue.Enqueue(tempNode.child[i]);
                }
                layerCount--;
            }
            layerCount = myQue.Count;
            tempRadius = tempRadius * 0.5f;
        }
        foreach (var node in myQue)
        {
            node.isleaf = true;
        }
    }

    //插入数据
    public void insertObj(SceneObject obj)
    {
        if (isleaf)
        {
            objList.Add(obj);
            obj.OctreeParent = this;
            obj.isLoaded = true;
            return;
        }
        int index = 0;
        if (obj.m_aBounds.center.x > center.x) index |= 1;
        if (obj.m_aBounds.center.y > center.y) index |= 2;
        if (obj.m_aBounds.center.z > center.z) index |= 4;
        child[index].insertObj(obj);
    }
    ////删除数据
    public void removeObj(SceneObject obj)
    {
        try
        {
            obj.OctreeParent.objList.Remove(obj);
            obj.isLoaded = false;
        }
        catch(Exception e)
        {
            Debug.LogError("remove obj error");
        }
    }
    //使用类似DFS的方法递归查找区域内物体
    public void GetObjectList(Bounds _aBound, List<SceneObject> lstSceneObject)
    {
        // TODO
        if (isleaf)  //如果到达叶子节点，则直接扫描叶子节点中的所有obj，判断是否与给定Bound相交
        {
            //Debug.Log("find the small aria");
            //Debug.Log(nodeBound);
            foreach (var obj in objList)
            {
                if (_aBound.Intersects(obj.m_aBounds))
                    lstSceneObject.Add(obj);
            }
            return;
        }
        //未到达叶子节点，深度搜索与Bound相交的区块
        foreach (var cube in child)
        {
            if (cube.nodeBound.Intersects(_aBound))
            {
                //Debug.Log(cube.nodeBound);
                cube.GetObjectList(_aBound, lstSceneObject);
            }
            //else
            //{
               // Debug.Log("culling this");
               // Debug.Log(cube.nodeBound);
            //}
        }
    }
}
