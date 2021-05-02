using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OMGame : PersistentObject {
    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }

    [SerializeField] private ShapeFactory shapeFactory;
    [SerializeField] private PersistentStorage storage;

    const int SAVE_VERSION = 1;

    private List<Shape> shapes;

    private float creationProgress, destructionProgress;

    // UNITY OVERRIDES -------------
    private void Awake() {
        shapes = new List<Shape>();
    }

    private void Update() {
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f) {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f) {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }
    // -----------------------------

    public override void Save(GameDataWriter writer) {
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++) {
            writer.Write(shapes[i].ShapeID);
            writer.Write(shapes[i].MaterialID);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader) {
        int version = reader.Version;
        if (version > SAVE_VERSION) {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }
        int count = version <= 0 ? -version : reader.ReadInt();
        for (int i = 0; i < count; i++) {
            int shapeID = version > 0 ? reader.ReadInt() : 0;
            int materialID = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.GetShape(shapeID, materialID);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

    private void CreateShape() {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        instance.SetColor(Random.ColorHSV(
            hueMin: 0f, hueMax: 1f,
            saturationMin: 0.5f, saturationMax: 1f,
            valueMin: 0.25f, valueMax: 1f,
            alphaMin: 1f, alphaMax: 1f
        ));
        shapes.Add(instance);
    }

    private void DestroyShape() {
        if (shapes.Count > 0) {
            int index = Random.Range(0, shapes.Count);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }

    // TODO: make this an extension method of List<T>, it's great.
    // private void QuickRemoveAt(this List<T> list, int index) {
    //     int lastIndex = list.Count - 1;
    //     list[index] = list[lastIndex];
    //     list.RemoveAt(lastIndex);
    // }

    private void BeginNewGame() {
        for (int i = 0; i < shapes.Count; i++) {
            shapeFactory.Reclaim(shapes[i]);
        }
        shapes.Clear();
    }

    // Input Callbacks -----------------------------------------------
    public void OnCreatePressed(InputAction.CallbackContext context) {
        if (context.performed) {
            CreateShape();
        }
    }

    public void OnDestroyPressed(InputAction.CallbackContext context) {
        if (context.performed) {
            DestroyShape();
        }
    }

    public void OnNewGamePressed(InputAction.CallbackContext context) {
        if (context.performed) {
            BeginNewGame();
        }
    }

    public void OnSavePressed(InputAction.CallbackContext context) {
        if (context.performed) {
            storage.Save(this, SAVE_VERSION);
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
