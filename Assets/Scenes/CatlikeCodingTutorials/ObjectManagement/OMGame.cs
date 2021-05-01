using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OMGame : PersistentObject {
    [SerializeField] private PersistentObject prefab;
    [SerializeField] private PersistentStorage storage;

    private List<PersistentObject> objects;

    public override void Save(GameDataWriter writer) {
        writer.Write(objects.Count);
        for (int i = 0; i < objects.Count; i++) {
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader) {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++) {
            PersistentObject o = Instantiate(prefab);
            o.Load(reader);
            objects.Add(o);
        }
    }

    private void Awake() {
        objects = new List<PersistentObject>();
    }

    private void CreateObject() {
        PersistentObject o = Instantiate(prefab);
        Transform t = o.gameObject.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.rotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        objects.Add(o);
    }

    private void BeginNewGame() {
        for (int i = 0; i < objects.Count; i++) {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    // Input Callbacks -----------------------------------------------
    public void OnCreatePressed(InputAction.CallbackContext context) {
        if (context.performed) {
            CreateObject();
        }
    }

    public void OnNewGamePressed(InputAction.CallbackContext context) {
        if (context.performed) {
            BeginNewGame();
        }
    }

    public void OnSavePressed(InputAction.CallbackContext context) {
        if (context.performed) {
            storage.Save(this);
        }
    }

    public void OnLoadPressed(InputAction.CallbackContext context) {
        if (context.performed) {
            BeginNewGame();
            storage.Load(this);
        }
    }
    // ---------------------------------------------------------------
}
