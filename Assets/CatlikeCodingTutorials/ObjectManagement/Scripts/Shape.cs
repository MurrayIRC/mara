using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistentObject {
    static int COLOR_PROPERTY_ID = Shader.PropertyToID("_BaseColor");
    static MaterialPropertyBlock SHARED_PROPERTY_BLOCK;

    public int MaterialID { get; private set; }
    public float Age { get; private set; }

    [SerializeField] private MeshRenderer[] meshRenderers;

    public ShapeFactory OriginFactory {
        get {
            return originFactory;
        }
        set {
            if (originFactory == null) {
                originFactory = value;
            } else {
                Debug.LogError("Not allowed to change origin factory.");
            }
        }
    }
    public int ShapeID {
        get {
            return shapeID;
        }
        set {
            if (shapeID == int.MinValue && value != int.MinValue) {
                shapeID = value;
            } else {
                Debug.LogError("Not allowed to change shapeID.");
            }
        }
    }
    public int ColorCount {
        get {
            return colors.Length;
        }
    }

    private ShapeFactory originFactory;
    private int shapeID = int.MinValue;
    private Color[] colors;
    private List<ShapeBehaviour> behaviorList = new List<ShapeBehaviour>();

    void Awake() {
        colors = new Color[meshRenderers.Length];
    }

    public void GameUpdate() {
        Age += Time.deltaTime;
        for (int i = 0; i < behaviorList.Count; i++) {
            behaviorList[i].GameUpdate(this);
        }
    }

    public void SetMaterial(Material material, int materialID) {
        for (int i = 0; i < meshRenderers.Length; i++) {
            meshRenderers[i].material = material;
        }
        MaterialID = materialID;
    }

    public void SetColor(Color color) {
        if (SHARED_PROPERTY_BLOCK == null) {
            SHARED_PROPERTY_BLOCK = new MaterialPropertyBlock();
        }
        SHARED_PROPERTY_BLOCK.SetColor(COLOR_PROPERTY_ID, color);
        for (int i = 0; i < meshRenderers.Length; i++) {
            colors[i] = color;
            meshRenderers[i].SetPropertyBlock(SHARED_PROPERTY_BLOCK);
        }
    }

    public void SetColor(Color color, int index) {
        if (SHARED_PROPERTY_BLOCK == null) {
            SHARED_PROPERTY_BLOCK = new MaterialPropertyBlock();
        }
        SHARED_PROPERTY_BLOCK.SetColor(COLOR_PROPERTY_ID, color);
        colors[index] = color;
        meshRenderers[index].SetPropertyBlock(SHARED_PROPERTY_BLOCK);
    }

    public override void Save(GameDataWriter writer) {
        base.Save(writer);
        writer.Write(colors.Length);
        for (int i = 0; i < colors.Length; i++) {
            writer.Write(colors[i]);
        }
        writer.Write(Age);
        writer.Write(behaviorList.Count);
        for (int i = 0; i < behaviorList.Count; i++) {
            writer.Write((int)behaviorList[i].BehaviourType);
            behaviorList[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader) {
        base.Load(reader);
        if (reader.Version >= 5) {
            LoadColors(reader);
        } else {
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
        if (reader.Version >= 6) {
            Age = reader.ReadFloat();
            int behaviorCount = reader.ReadInt();
            for (int i = 0; i < behaviorCount; i++) {
                ShapeBehaviour behavior =
                    ((ShapeBehaviourType)reader.ReadInt()).GetInstance();
                behaviorList.Add(behavior);
                behavior.Load(reader);
            }
        } else if (reader.Version >= 4) {
            AddBehavior<RotationShapeBehaviour>().AngularVelocity =
                reader.ReadVector3();
            AddBehavior<MovementShapeBehaviour>().Velocity = reader.ReadVector3();
        }
    }

    private void LoadColors(GameDataReader reader) {
        int count = reader.ReadInt();
        int max = count <= colors.Length ? count : colors.Length;
        int i = 0;
        for (; i < max; i++) {
            SetColor(reader.ReadColor(), i);
        }
        if (count > colors.Length) {
            for (; i < count; i++) {
                reader.ReadColor();
            }
        } else if (count < colors.Length) {
            for (; i < colors.Length; i++) {
                SetColor(Color.white, i);
            }
        }
    }

    public T AddBehavior<T>() where T : ShapeBehaviour, new() {
        T behavior = ShapeBehaviorPool<T>.Get();
        behaviorList.Add(behavior);
        return behavior;
    }

    private ShapeBehaviour AddBehavior(ShapeBehaviourType type) {
        switch (type) {
            case ShapeBehaviourType.Movement:
                return AddBehavior<MovementShapeBehaviour>();
            case ShapeBehaviourType.Rotation:
                return AddBehavior<RotationShapeBehaviour>();
        }
        Debug.LogError("Forgot to support " + type);
        return null;
    }

    public void Recycle() {
        Age = 0f;
        for (int i = 0; i < behaviorList.Count; i++) {
            behaviorList[i].Recycle();
        }
        behaviorList.Clear();
        OriginFactory.Reclaim(this);
    }
}
