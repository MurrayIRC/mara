using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OMGame : MonoBehaviour {
    [SerializeField] private GameObject prefab;

    private List<Transform> objects;
    private string savePath;

    private void Awake() {
        objects = new List<Transform>();
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    private void CreateObject() {
        Transform t = Instantiate(prefab).transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.rotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        objects.Add(t);
    }

    private void BeginNewGame() {
        for (int i = 0; i < objects.Count; i++) {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    private void Save() {
        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))) {
            writer.Write(objects.Count);

            Debug.Log("Done saving. Saved file to: " + savePath);
        }

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
            Save();
        }
    }
    // ---------------------------------------------------------------
}
