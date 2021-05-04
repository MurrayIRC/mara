public enum ShapeBehaviourType {
    Movement,
    Rotation,
    Oscillation
}

public static class ShapeBehaviorTypeMethods {
    public static ShapeBehaviour GetInstance(this ShapeBehaviourType type) {
        switch (type) {
            case ShapeBehaviourType.Movement:
                return ShapeBehaviorPool<MovementShapeBehaviour>.Get();
            case ShapeBehaviourType.Rotation:
                return ShapeBehaviorPool<RotationShapeBehaviour>.Get();
            case ShapeBehaviourType.Oscillation:
                return ShapeBehaviorPool<OscillationShapeBehaviour>.Get();
        }
        UnityEngine.Debug.Log("Forgot to support " + type);
        return null;
    }
}