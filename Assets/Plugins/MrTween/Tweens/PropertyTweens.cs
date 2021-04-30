using UnityEngine;
using System;
using System.Collections;
using System.Reflection;


namespace Mara.MrTween {
    /// <summary>
    /// helper class to fetch property delegates
    /// </summary>
    class PropertyTweenUtils {
        /// <summary>
        /// either returns a super fast Delegate to set the given property or null if it couldn't be found
        /// via reflection
        /// </summary>
        public static T SetterForProperty<T>(System.Object targetObject, string propertyName) {
            // first get the property
#if NETFX_CORE
			var propInfo = targetObject.GetType().GetRuntimeProperty( propertyName );
#else
            var propInfo = targetObject.GetType().GetProperty(propertyName);
#endif

            if (propInfo == null) {
                Debug.Log("could not find property with name: " + propertyName);
                return default(T);
            }

#if NETFX_CORE
			// Windows Phone/Store new API
			return (T)(object)propInfo.SetMethod.CreateDelegate( typeof( T ), targetObject );
#else
            return (T)(object)Delegate.CreateDelegate(typeof(T), targetObject, propInfo.GetSetMethod());
#endif
        }


        /// <summary>
        /// either returns a super fast Delegate to get the given property or null if it couldn't be found
        /// via reflection
        /// </summary>
        public static T GetterForProperty<T>(System.Object targetObject, string propertyName) {
            // first get the property
#if NETFX_CORE
			var propInfo = targetObject.GetType().GetRuntimeProperty( propertyName );
#else
            var propInfo = targetObject.GetType().GetProperty(propertyName);
#endif

            if (propInfo == null) {
                Debug.Log("could not find property with name: " + propertyName);
                return default(T);
            }

#if NETFX_CORE
			// Windows Phone/Store new API
			return (T)(object)propInfo.GetMethod.CreateDelegate( typeof( T ), targetObject );
#else
            return (T)(object)Delegate.CreateDelegate(typeof(T), targetObject, propInfo.GetGetMethod());
#endif
        }

    }


    public class PropertyTarget<T> : ITweenTarget<T> where T : struct {
        protected object _target;
        protected Action<T> _setter;
        protected Func<T> _getter;


        public void SetTweenedValue(T value) {
            _setter(value);
        }


        public T GetTweenedValue() {
            return _getter();
        }


        public PropertyTarget(object target, string propertyName) {
            _target = target;
            _setter = PropertyTweenUtils.SetterForProperty<Action<T>>(target, propertyName);
            _getter = PropertyTweenUtils.GetterForProperty<Func<T>>(target, propertyName);

            if (_setter == null)
                Debug.LogError("either the property (" + propertyName + ") setter or getter could not be found on the object " + target);
        }


        public object GetTargetObject() {
            return _target;
        }
    }


    public static class PropertyTweens {
        public static ITween<int> IntPropertyTo(object self, string propertyName, int to, float duration) {
            var tweenTarget = new PropertyTarget<int>(self, propertyName);
            var tween = MrTween.CacheIntTweens ? QuickCache<IntTween>.Pop() : new IntTween();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        public static ITween<float> FloatPropertyTo(object self, string propertyName, float to, float duration) {
            var tweenTarget = new PropertyTarget<float>(self, propertyName);

            var tween = MrTween.CacheFloatTweens ? QuickCache<FloatTween>.Pop() : new FloatTween();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        public static ITween<Vector2> Vector2PropertyTo(object self, string propertyName, Vector2 to, float duration) {
            var tweenTarget = new PropertyTarget<Vector2>(self, propertyName);
            var tween = MrTween.CacheVector2Tweens ? QuickCache<Vector2Tween>.Pop() : new Vector2Tween();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        public static ITween<Vector3> Vector3PropertyTo(object self, string propertyName, Vector3 to, float duration) {
            var tweenTarget = new PropertyTarget<Vector3>(self, propertyName);
            var tween = MrTween.CacheVector3Tweens ? QuickCache<Vector3Tween>.Pop() : new Vector3Tween();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        public static ITween<Vector4> Vector4PropertyTo(object self, string propertyName, Vector4 to, float duration) {
            var tweenTarget = new PropertyTarget<Vector4>(self, propertyName);
            var tween = MrTween.CacheVector4Tweens ? QuickCache<Vector4Tween>.Pop() : new Vector4Tween();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        public static ITween<Quaternion> QuaternionPropertyTo(object self, string propertyName, Quaternion to, float duration) {
            var tweenTarget = new PropertyTarget<Quaternion>(self, propertyName);
            var tween = MrTween.CacheQuaternionTweens ? QuickCache<QuaternionTween>.Pop() : new QuaternionTween();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        public static ITween<Color> ColorPropertyTo(object self, string propertyName, Color to, float duration) {
            var tweenTarget = new PropertyTarget<Color>(self, propertyName);
            var tween = MrTween.CacheColorTweens ? QuickCache<ColorTween>.Pop() : new ColorTween();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        public static ITween<Color32> Color32PropertyTo(object self, string propertyName, Color32 to, float duration) {
            var tweenTarget = new PropertyTarget<Color32>(self, propertyName);
            var tween = MrTween.CacheColor32Tweens ? QuickCache<Color32Tween>.Pop() : new Color32Tween();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }
    }

}
