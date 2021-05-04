using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OMGame : PersistentObject {
    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }

    [SerializeField] private ShapeFactory[] shapeFactories;
    [SerializeField] private PersistentStorage storage;
    [SerializeField] private Slider creationSpeedSlider;
    [SerializeField] private Slider destructionSpeedSlider;
    [SerializeField] private int levelCount;
    [SerializeField] private bool reseedOnLoad;

    private const int SAVE_VERSION = 6;
    private const string SCENE_LEVEL_PREFIX = "CC_OM_LEVEL";
    private int loadedLevelBuildIndex;

    private Random.State mainRandomState;

    private List<Shape> shapes;

    private float creationProgress, destructionProgress;

    // UNITY OVERRIDES --------------------------------------------------
    private void OnEnable() {
        if (shapeFactories[0].FactoryID != 0) {
            for (int i = 0; i < shapeFactories.Length; i++) {
                shapeFactories[i].FactoryID = i;
            }
        }
    }

    private void Start() {
        mainRandomState = Random.state;

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

    private void FixedUpdate() {
        for (int i = 0; i < shapes.Count; i++) {
            shapes[i].GameUpdate();
        }

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
        writer.Write(CreationSpeed);
        writer.Write(creationProgress);
        writer.Write(DestructionSpeed);
        writer.Write(destructionProgress);
        writer.Write(loadedLevelBuildIndex);
        GameLevel.Current.Save(writer);
        for (int i = 0; i < shapes.Count; i++) {
            writer.Write(shapes[i].OriginFactory.FactoryID);
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
        StartCoroutine(LoadGameRoutine(reader));
    }

    IEnumerator LoadGameRoutine(GameDataReader reader) {
        int version = reader.Version;
        int count = version <= 0 ? -version : reader.ReadInt();

        if (version >= 3) {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad) {
                Random.state = state;
            }
            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
            creationProgress = reader.ReadFloat();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat();
            destructionProgress = reader.ReadFloat();
        }

        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());
        if (version >= 3) {
            GameLevel.Current.Load(reader);
        }

        for (int i = 0; i < count; i++) {
            int factoryId = version >= 5 ? reader.ReadInt() : 0;
            int shapeID = version > 0 ? reader.ReadInt() : 0;
            int materialID = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactories[factoryId].GetShape(shapeID, materialID);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
    // ------------------------------------------------------------------

    private void CreateShape() {
        shapes.Add(GameLevel.Current.SpawnShape());
    }

    private void DestroyShape() {
        if (shapes.Count > 0) {
            int index = Random.Range(0, shapes.Count);
            shapes[index].Recycle();
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

        creationSpeedSlider.value = CreationSpeed = 0f;
        destructionSpeedSlider.value = DestructionSpeed = 0f;

        for (int i = 0; i < shapes.Count; i++) {
            shapes[i].Recycle();
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
        StartCoroutine(LoadLevel(loadedLevelBuildIndex));
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

    public void OnScene4Pressed() {
        BeginNewGame();
        StartCoroutine(LoadLevel(4));
    }
    // ------------------------------------------------------------------
}
