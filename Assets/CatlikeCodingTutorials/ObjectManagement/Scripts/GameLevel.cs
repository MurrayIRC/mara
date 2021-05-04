using UnityEngine;

public class GameLevel : PersistentObject {
    public static GameLevel Current { get; private set; }

    [SerializeField] private SpawnZone spawnZone;
    public Vector3 SpawnPoint {
        get {
            return spawnZone.SpawnPoint;
        }
    }

    [SerializeField] private PersistentObject[] persistentObjects;

    private void OnEnable() {
        Current = this;
        if (persistentObjects == null) {
            persistentObjects = new PersistentObject[0];
        }
    }

    public Shape SpawnShape() {
        return spawnZone.SpawnShape();
    }

    public override void Save(GameDataWriter writer) {
        writer.Write(persistentObjects.Length);
        for (int i = 0; i < persistentObjects.Length; i++) {
            persistentObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader) {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++) {
            persistentObjects[i].Load(reader);
        }
    }
}
