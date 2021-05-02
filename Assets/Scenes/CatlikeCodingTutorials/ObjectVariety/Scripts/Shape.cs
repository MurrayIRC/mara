using UnityEngine;

public class Shape : PersistentObject {
    static int COLOR_PROPERTY_ID = Shader.PropertyToID("_BaseColor");
    static MaterialPropertyBlock SHARED_PROPERTY_BLOCK;

    public int MaterialID { get; private set; }

    private MeshRenderer meshRenderer;

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
    private int shapeID = int.MinValue;

    private Color color;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material, int materialID) {
        meshRenderer.material = material;
        MaterialID = materialID;
    }

    public void SetColor(Color color) {
        this.color = color;
        if (SHARED_PROPERTY_BLOCK == null) {
            SHARED_PROPERTY_BLOCK = new MaterialPropertyBlock();
        }
        SHARED_PROPERTY_BLOCK.SetColor(COLOR_PROPERTY_ID, color);
        meshRenderer.SetPropertyBlock(SHARED_PROPERTY_BLOCK);
    }

    public override void Save(GameDataWriter writer) {
        base.Save(writer);
        writer.Write(color);
    }

    public override void Load(GameDataReader reader) {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }
}
