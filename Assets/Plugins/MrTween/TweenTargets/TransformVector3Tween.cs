using UnityEngine;
using System.Collections.Generic;


namespace Mara.MrTween {
    /// <summary>
    /// useful enum for any transform related property tweens
    /// </summary>
    public enum TransformTargetType {
        Position,
        LocalPosition,
        LocalScale,
        EulerAngles,
        LocalEulerAngles
    }

    /// <summary>
    /// this is a special case since Transforms are by far the most tweened object. we encapsulate the Tween and the ITweenTarget
    /// in a single, cacheable class
    /// </summary>
    public class TransformVector3Tween : Vector3Tween, ITweenTarget<Vector3> {
        Transform _transform;
        TransformTargetType _targetType;


        public void SetTweenedValue(Vector3 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_transform)
                return;

            switch (_targetType) {
                case TransformTargetType.Position:
                    _transform.position = value;
                    break;
                case TransformTargetType.LocalPosition:
                    _transform.localPosition = value;
                    break;
                case TransformTargetType.LocalScale:
                    _transform.localScale = value;
                    break;
                case TransformTargetType.EulerAngles:
                    _transform.eulerAngles = value;
                    break;
                case TransformTargetType.LocalEulerAngles:
                    _transform.localEulerAngles = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public Vector3 GetTweenedValue() {
            switch (_targetType) {
                case TransformTargetType.Position:
                    return _transform.position;
                case TransformTargetType.LocalPosition:
                    return _transform.localPosition;
                case TransformTargetType.LocalScale:
                    return _transform.localScale;
                case TransformTargetType.EulerAngles:
                    return _transform.eulerAngles;
                case TransformTargetType.LocalEulerAngles:
                    return _transform.localEulerAngles;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public new object GetTargetObject() {
            return _transform;
        }


        public void SetTargetAndType(Transform transform, TransformTargetType targetType) {
            _transform = transform;
            _targetType = targetType;
        }


        protected override void UpdateValue() {
            // special case for non-relative angle lerps so that they take the shortest possible rotation
            if ((_targetType == TransformTargetType.EulerAngles || _targetType == TransformTargetType.LocalEulerAngles) && !_isRelative) {
                if (_animationCurve != null)
                    SetTweenedValue(TweenMath.EaseAngle(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
                else
                    SetTweenedValue(TweenMath.EaseAngle(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
            } else {
                if (_animationCurve != null)
                    SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
                else
                    SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
            }
        }


        public override void RecycleSelf() {
            if (_shouldRecycleTween) {
                _target = null;
                _nextTween = null;
                _transform = null;
                QuickCache<TransformVector3Tween>.Push(this);
            }
        }
    }
}