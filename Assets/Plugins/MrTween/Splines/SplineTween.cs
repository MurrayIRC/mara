using UnityEngine;
using System.Collections;


namespace Mara.MrTween {
    public class SplineTween : Tween<Vector3>, ITweenTarget<Vector3> {
        Transform _transform;
        Spline _spline;
        bool _isRelativeTween;


        public SplineTween(Transform transform, Spline spline, float duration) {
            _transform = transform;
            _spline = spline;
            _spline.BuildPath();

            Initialize(this, Vector3.zero, duration);
        }


        public void SetTweenedValue(Vector3 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_transform)
                return;

            _transform.position = value;
        }


        public Vector3 GetTweenedValue() {
            return _transform.position;
        }


        public override ITween<Vector3> SetIsRelative() {
            _isRelativeTween = true;
            return this;
        }


        protected override void UpdateValue() {
            var easedTime = EaseHelper.Ease(_easeType, _elapsedTime, _duration);
            var position = _spline.GetPointOnPath(easedTime);

            // if this is a relative tween we use the fromValue (initial position) as a base and add the spline to it
            if (_isRelativeTween)
                position += _fromValue;

            SetTweenedValue(position);
        }


        public new object GetTargetObject() {
            return _transform;
        }
    }
}