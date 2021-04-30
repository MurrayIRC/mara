using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// concrete implementations of all tweenable types
/// </summary>
namespace Mara.MrTween {
    public class IntTween : Tween<int> {
        public static IntTween Create() {
            return MrTween.CacheIntTweens ? QuickCache<IntTween>.Pop() : new IntTween();
        }


        public IntTween() { }


        public IntTween(ITweenTarget<int> target, int to, float duration) {
            Initialize(target, to, duration);
        }


        public override ITween<int> SetIsRelative() {
            _isRelative = true;
            _toValue += _fromValue;
            return this;
        }


        protected override void UpdateValue() {
            if (_animationCurve != null)
                _target.SetTweenedValue((int)TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            else
                _target.SetTweenedValue((int)TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }


        public override void RecycleSelf() {
            base.RecycleSelf();

            if (_shouldRecycleTween && MrTween.CacheIntTweens)
                QuickCache<IntTween>.Push(this);
        }
    }


    public class FloatTween : Tween<float> {
        public static FloatTween Create() {
            return MrTween.CacheFloatTweens ? QuickCache<FloatTween>.Pop() : new FloatTween();
        }


        public FloatTween() { }


        public FloatTween(ITweenTarget<float> target, float from, float to, float duration) {
            Initialize(target, to, duration);
        }


        public override ITween<float> SetIsRelative() {
            _isRelative = true;
            _toValue += _fromValue;
            return this;
        }


        protected override void UpdateValue() {
            if (_animationCurve != null)
                _target.SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            else
                _target.SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }


        public override void RecycleSelf() {
            base.RecycleSelf();

            if (_shouldRecycleTween && MrTween.CacheFloatTweens)
                QuickCache<FloatTween>.Push(this);
        }
    }


    public class Vector2Tween : Tween<Vector2> {
        public static Vector2Tween Create() {
            return MrTween.CacheVector2Tweens ? QuickCache<Vector2Tween>.Pop() : new Vector2Tween();
        }


        public Vector2Tween() { }


        public Vector2Tween(ITweenTarget<Vector2> target, Vector2 from, Vector2 to, float duration) {
            Initialize(target, to, duration);
        }


        public override ITween<Vector2> SetIsRelative() {
            _isRelative = true;
            _toValue += _fromValue;
            return this;
        }


        protected override void UpdateValue() {
            if (_animationCurve != null)
                _target.SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            else
                _target.SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }


        public override void RecycleSelf() {
            base.RecycleSelf();

            if (_shouldRecycleTween && MrTween.CacheVector2Tweens)
                QuickCache<Vector2Tween>.Push(this);
        }
    }


    public class Vector3Tween : Tween<Vector3> {
        public static Vector3Tween Create() {
            return MrTween.CacheVector3Tweens ? QuickCache<Vector3Tween>.Pop() : new Vector3Tween();
        }


        public Vector3Tween() { }


        public Vector3Tween(ITweenTarget<Vector3> target, Vector3 from, Vector3 to, float duration) {
            Initialize(target, to, duration);
        }


        public override ITween<Vector3> SetIsRelative() {
            _isRelative = true;
            _toValue += _fromValue;
            return this;
        }


        protected override void UpdateValue() {
            if (_animationCurve != null)
                _target.SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            else
                _target.SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }


        public override void RecycleSelf() {
            base.RecycleSelf();

            if (_shouldRecycleTween && MrTween.CacheVector3Tweens)
                QuickCache<Vector3Tween>.Push(this);
        }
    }


    public class Vector4Tween : Tween<Vector4> {
        public static Vector4Tween Create() {
            return MrTween.CacheVector4Tweens ? QuickCache<Vector4Tween>.Pop() : new Vector4Tween();
        }


        public Vector4Tween() { }


        public Vector4Tween(ITweenTarget<Vector4> target, Vector4 from, Vector4 to, float duration) {
            Initialize(target, to, duration);
        }


        public override ITween<Vector4> SetIsRelative() {
            _isRelative = true;
            _toValue += _fromValue;
            return this;
        }


        protected override void UpdateValue() {
            if (_animationCurve != null)
                _target.SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            else
                _target.SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }


        public override void RecycleSelf() {
            base.RecycleSelf();

            if (_shouldRecycleTween && MrTween.CacheVector4Tweens)
                QuickCache<Vector4Tween>.Push(this);
        }
    }


    public class QuaternionTween : Tween<Quaternion> {
        public static QuaternionTween Create() {
            return MrTween.CacheQuaternionTweens ? QuickCache<QuaternionTween>.Pop() : new QuaternionTween();
        }


        public QuaternionTween() { }


        public QuaternionTween(ITweenTarget<Quaternion> target, Quaternion from, Quaternion to, float duration) {
            Initialize(target, to, duration);
        }


        public override ITween<Quaternion> SetIsRelative() {
            _isRelative = true;
            _toValue *= _fromValue;
            return this;
        }


        protected override void UpdateValue() {
            if (_animationCurve != null)
                _target.SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            else
                _target.SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }


        public override void RecycleSelf() {
            base.RecycleSelf();

            if (_shouldRecycleTween && MrTween.CacheQuaternionTweens)
                QuickCache<QuaternionTween>.Push(this);
        }
    }


    public class ColorTween : Tween<Color> {
        public static ColorTween Create() {
            return MrTween.CacheColorTweens ? QuickCache<ColorTween>.Pop() : new ColorTween();
        }


        public ColorTween() { }


        public ColorTween(ITweenTarget<Color> target, Color from, Color to, float duration) {
            Initialize(target, to, duration);
        }


        public override ITween<Color> SetIsRelative() {
            _isRelative = true;
            _toValue += _fromValue;
            return this;
        }


        protected override void UpdateValue() {
            if (_animationCurve != null)
                _target.SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            else
                _target.SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }


        public override void RecycleSelf() {
            base.RecycleSelf();

            if (_shouldRecycleTween && MrTween.CacheColorTweens)
                QuickCache<ColorTween>.Push(this);
        }
    }


    public class Color32Tween : Tween<Color32> {
        public static Color32Tween Create() {
            return MrTween.CacheColor32Tweens ? QuickCache<Color32Tween>.Pop() : new Color32Tween();
        }


        public Color32Tween() { }


        public Color32Tween(ITweenTarget<Color32> target, Color32 from, Color32 to, float duration) {
            Initialize(target, to, duration);
        }


        public override ITween<Color32> SetIsRelative() {
            _isRelative = true;
            _toValue = (Color)_toValue + (Color)_fromValue;
            return this;
        }


        protected override void UpdateValue() {
            if (_animationCurve != null)
                _target.SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            else
                _target.SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }


        public override void RecycleSelf() {
            base.RecycleSelf();

            if (_shouldRecycleTween && MrTween.CacheColor32Tweens)
                QuickCache<Color32Tween>.Push(this);
        }
    }


    public class RectTween : Tween<Rect> {
        public static RectTween Create() {
            return MrTween.CacheRectTweens ? QuickCache<RectTween>.Pop() : new RectTween();
        }


        public RectTween() { }


        public RectTween(ITweenTarget<Rect> target, Rect from, Rect to, float duration) {
            Initialize(target, to, duration);
        }


        public override ITween<Rect> SetIsRelative() {
            _isRelative = true;
            _toValue = new Rect
            (
                _toValue.x + _fromValue.x,
                _toValue.y + _fromValue.y,
                _toValue.width + _fromValue.width,
                _toValue.height + _fromValue.height
            );

            return this;
        }


        protected override void UpdateValue() {
            if (_animationCurve != null)
                _target.SetTweenedValue(TweenMath.Ease(_animationCurve, _fromValue, _toValue, _elapsedTime, _duration));
            else
                _target.SetTweenedValue(TweenMath.Ease(_easeType, _fromValue, _toValue, _elapsedTime, _duration));
        }


        public override void RecycleSelf() {
            base.RecycleSelf();

            if (_shouldRecycleTween && MrTween.CacheRectTweens)
                QuickCache<RectTween>.Push(this);
        }
    }

}
