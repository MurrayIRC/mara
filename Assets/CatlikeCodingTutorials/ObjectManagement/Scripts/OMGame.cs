using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OMGame : PersistentObject {
    public static OMGame Instance { get; private set; }

    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }
    public SpawnZone SpawnZoneOfCurrentLevel { get; set; }

    [SerializeField] private ShapeFactory shapeFactory;
    [SerializeField] private PersistentStorage storage;
    [SerializeField] private int levelCount;
    [SerializeField] private bool reseedOnLoad;

    private const int SAVE_VERSION = 3;
    private const string SCENE_LEVEL_PREFIX = "CC_OM_LEVEL";
    private int loadedLevelBuildIndex;

    private Random.State mainRandomState;

    private List<Shape> shapes;

    private float creationProgress, destructionProgress;

    // UNITY OVERRIDES --------------------------------------------------
    private void Start() {
        mainRandomState = Random.state;
        Instance = this;

        shapes = new List<Shape>();

        if (Application.isEditor) {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains(SCENE_LEVEL_PREFIX)) {
                    SceneManager.SetActiveScene(loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }

        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }

    private void OnEnable() {
        Instance = this;
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
    // ------------------------------------------------------------------

    // PERSISTENT OBJECT OVERRIDES --------------------------------------
    public override void Save(GameDataWriter writer) {
        writer.Write(shapes.Count);
        writer.Write(Random.state);
        writer.Write(loadedLevelBuildIndex);
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

        if (version >= 3) {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad) {
                Random.state = state;
            }
        }

        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
        for (int i = 0; i < count; i++) {
            int shapeID = version > 0 ? reader.ReadInt() : 0;
            int materialID = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.GetShape(shapeID, materialID);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
    // ------------------------------------------------------------------

    private void CreateShape() {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = SpawnZoneOfCurrentLevel.SpawnPoint;
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
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState(seed);

        for (int i = 0; i < shapes.Count; i++) {
            shapeFactory.Reclaim(shapes[i]);
        }
        shapes.Clear();
    }

    private IEnumerator LoadLevel(int levelBuildIndex) {
        enabled = false;
        if (loadedLevelBuildIndex > 0) {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }

    // Input Callbacks --------------------------------------------------
    public void OnCreatePressed() {
        CreateShape();
    }

    public void OnDestroyPressed() {
        DestroyShape();
    }

    public void OnNewGamePressed() {
        BeginNewGame();
    }

    public void OnSavePressed() {
        storage.Save(this, SAVE_VERSION);
    }

    public void OnLoadPressed() {
        BeginNewGame();
        storage.Load(this);
    }

    public void OnScene1Pressed() {
        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }

    public void OnScene2Pressed() {
        BeginNewGame();
        StartCoroutine(LoadLevel(2));
    }

    public void OnScene3Pressed() {
        BeginNewGame();
        StartCoroutine(LoadLevel(3));
    }
    // ------------------------------------------------------------------
}
