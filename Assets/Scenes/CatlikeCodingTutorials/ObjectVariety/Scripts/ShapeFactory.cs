using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject {
    [SerializeField] private bool recycle;
    [SerializeField] private Shape[] prefabs;
    [SerializeField] private Material[] materials;

    private List<Shape>[] pools;

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
                instance.ShapeID = shapeID;
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
    }
}
