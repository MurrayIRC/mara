using UnityEngine;
using System.Collections;


namespace Mara.MrTween {
    public class TransformSpringTween : AbstractTweenable {
        public TransformTargetType TargetType { get { return _targetType; } }

        Transform _transform;
        TransformTargetType _targetType;
        Vector3 _targetValue;
        Vector3 _velocity;

        // configuration of dampingRatio and angularFrequency are public for easier value tweaking at design time

        /// <summary>
        /// lower values are less damped and higher values are more damped resulting in less springiness.
        /// should be between 0.01f, 1f to avoid unstable systems.
        /// </summary>
        public float DampingRatio = 0.23f;

        /// <summary>
        /// An angular frequency of 2pi (radians per second) means the oscillation completes one
        /// full period over one second, i.e. 1Hz. should be less than 35 or so to remain stableThe angular frequency.
        /// </summary>
        public float AngularFrequency = 25;


        /// <summary>
        /// Initializes a new instance of the <see cref="Mara.MrTween.TransformSpringTween"/> class.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="dampingRatio">lower values are less damped and higher values are more damped resulting in less springiness.
        /// should be between 0.01f, 1f to avoid unstable systems.</param>
        /// <param name="angularFrequency">An angular frequency of 2pi (radians per second) means the oscillation completes one
        /// full period over one second, i.e. 1Hz. should be less than 35 or so to remain stable</param>
        public TransformSpringTween(Transform transform, TransformTargetType targetType, Vector3 targetValue) {
            _transform = transform;
            _targetType = targetType;
            SetTargetValue(targetValue);
        }


        /// <summary>
        /// you can call setTargetValue at any time to reset the target value to a new Vector3. If you have not called start to add the
        /// spring tween to MrTween it will be called for you.
        /// </summary>
        /// <param name="targetValue">Target value.</param>
        public void SetTargetValue(Vector3 targetValue) {
            _velocity = Vector3.zero;
            _targetValue = targetValue;

            if (!_isCurrentlyManagedByMrTween)
                Start();
        }


        /// <summary>
        /// lambda should be the desired duration when the oscillation magnitude is reduced by 50%
        /// </summary>
        /// <param name="lambda">Lambda.</param>
        public void UpdateDampingRatioWithHalfLife(float lambda) {
            DampingRatio = (-lambda / AngularFrequency) * Mathf.Log(0.5f);
        }


        #region AbstractTweenable

        public override bool Tick() {
            if (!_isPaused)
                SetTweenedValue(TweenMath.FastSpring(GetCurrentValueOfTweenedTargetType(), _targetValue, ref _velocity, DampingRatio, AngularFrequency));

            return false;
        }

        #endregion


        void SetTweenedValue(Vector3 value) {
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


        Vector3 GetCurrentValueOfTweenedTargetType() {
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
            }

            return Vector3.zero;
        }
    }
}