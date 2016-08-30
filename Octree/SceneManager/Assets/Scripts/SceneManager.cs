using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// 场景管理器
public class SceneManager
{
    //List<SceneObject> loadedObjectList;
    Octree myTree;
    public SceneManager()
    {
        myTree = new Octree(1000f,Vector3.zero);
        myTree.buildTree(1000/32);
    }
    // 添加一个场景物体
    public void AddSceneObject(SceneObject _aObject)
    {
        // TODO
        if (!_aObject.isLoaded)
        {
            //加载该物体
            myTree.insertObj(_aObject);
        }
    }

    // 移除一个场景物体
    public void RemoveSceneObject(SceneObject _aObject)
    {
        // TODO
        myTree.removeObj(_aObject);
    }

    // 获取区域范围内所有场景物体
    public List<SceneObject> GetSceneObjectList(Bounds _aBound)
    {

        // TODO
        List<SceneObject> lstSceneObject = new List<SceneObject>();
        myTree.GetObjectList(_aBound, lstSceneObject);
        //Debug.Log(lstSceneObject.Count);
        return lstSceneObject;
    }
}
