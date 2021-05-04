using UnityEngine;

public class CompositeSpawnZone : SpawnZone {
    [SerializeField] private bool isSequential;
    [SerializeField] private SpawnZone[] spawnZones;
    [SerializeField] private bool overrideConfig;

    private int nextSequentialIndex;

    public override Vector3 SpawnPoint {
        get {
            int index;
            if (isSequential) {
                index = nextSequentialIndex++;
                if (nextSequentialIndex >= spawnZones.Length) {
                    nextSequentialIndex = 0;
                }
            } else {
                index = Random.Range(0, spawnZones.Length);
            }
            return spawnZones[index].SpawnPoint;
        }
    }

    public override Shape SpawnShape() {
        if (overrideConfig) {
            return base.SpawnShape();
        } else {
            int index;
            if (isSequential) {
                index = nextSequentialIndex++;
                if (nextSequentialIndex >= spawnZones.Length) {
                    nextSequentialIndex = 0;
                }
            } else {
                index = Random.Range(0, spawnZones.Length);
            }
            return spawnZones[index].SpawnShape();
        }
    }

    public override void Save(GameDataWriter writer) {
        writer.Write(nextSequentialIndex);
    }

    public override void Load(GameDataReader reader) {
        nextSequentialIndex = reader.ReadInt();
    }
}