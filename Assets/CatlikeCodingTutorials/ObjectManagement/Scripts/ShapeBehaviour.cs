using UnityEngine;

public abstract class ShapeBehaviour
#if UNITY_EDITOR
    : ScriptableObject
#endif
{
    public abstract ShapeBehaviourType BehaviourType { get; }


    public abstract void GameUpdate(Shape shape);
    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);
    public abstract void Recycle();

#if UNITY_EDITOR
    public bool IsReclaimed { get; set; }
    private void OnEnable() {
        if (IsReclaimed) {
            Recycle();
        }
    }
#endif
}