using UnityEngine;

public class GameLevel : MonoBehaviour {
    [SerializeField] private SpawnZone spawnZone;

    private void Start() {
        OMGame.Instance.SpawnZoneOfCurrentLevel = spawnZone;
    }
}
