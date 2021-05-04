using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject {
    public int FactoryID {
        get {
            return factoryID;
        }
        set {
            if (factoryID == int.MinValue && value != int.MinValue) {
                factoryID = value;
            } else {
                Debug.Log("Not allowed to change factoryId.");
            }
        }
    }

    [System.NonSerialized] private int factoryID = int.MinValue;

    [SerializeField] private bool recycle;
    [SerializeField] private Shape[] prefabs;
    [SerializeField] private Material[] materials;

    private List<Shape>[] pools;
    private Scene poolScene;

    public Shape GetShape(int shapeID = 0, int materialID = 0) {
        Shape instance;
        if (recycle) {
            if (pools == null) {
                CreatePools();
            }
            List<Shape> pool = pools[shapeID];
            int lastIndex = pool.Count - 1;
            if (lastIndex >= 0) {
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            } else {
                instance = Instantiate(prefabs[shapeID]);
                instance.OriginFactory = this;
                instance.ShapeID = shapeID;
                SceneManager.MoveGameObjectToScene(instance.gameObject, poolScene);
            }
        } else {
            instance = Instantiate(prefabs[shapeID]);
            instance.ShapeID = shapeID;
        }

        instance.SetMaterial(materials[materialID], materialID);
        return instance;
    }

    public Shape GetRandom() {
        return GetShape(Random.Range(0, prefabs.Length), Random.Range(0, materials.Length));
    }

    public void Reclaim(Shape shapeToRecycle) {
        if (shapeToRecycle.OriginFactory != this) {
            Debug.LogError("Tried to reclaim shape with wrong factory.");
            return;
        }

        if (recycle) {
            if (pools == null) {
                CreatePools();
            }
            pools[shapeToRecycle.ShapeID].Add(shapeToRecycle);
            shapeToRecycle.gameObject.SetActive(false);
        } else {
            Destroy(shapeToRecycle.gameObject);
        }
    }

    private void CreatePools() {
        pools = new List<Shape>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++) {
            pools[i] = new List<Shape>();
        }

        if (Application.isEditor) {
            poolScene = SceneManager.GetSceneByName(name);
            if (poolScene.isLoaded) {
                GameObject[] rootObjects = poolScene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++) {
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();
                    if (!pooledShape.gameObject.activeSelf) {
                        pools[pooledShape.ShapeID].Add(pooledShape);
                    }
                }
                return;
            }
        }

        poolScene = SceneManager.CreateScene(name);
    }
}
