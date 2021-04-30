using UnityEngine;
using System.Collections;


namespace Mara.MrTween {
    /// <summary>
    /// helper base class to make creating custom ITweenTargets as trivial as possible
    /// </summary>
    public abstract class AbstractTweenTarget<U, T> : ITweenTarget<T> where T : struct {
        protected U _target;

        abstract public void SetTweenedValue(T value);
        abstract public T GetTweenedValue();


        public AbstractTweenTarget<U, T> SetTarget(U target) {
            _target = target;
            return this;
        }


        public bool ValidateTarget() {
            return !_target.Equals(null);
        }


        public object GetTargetObject() {
            return _target;
        }
    }
}