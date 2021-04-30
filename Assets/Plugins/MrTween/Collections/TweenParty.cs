using UnityEngine;
using System;
using System.Collections.Generic;


namespace Mara.MrTween {
    /// <summary>
    /// helper class for managing a series of simultaneous tweens. An important item to note here is that the delay,
    /// loop values and ease/animation curve should be set on the TweenParty and that the sub-tweens must have the
    /// same duration as the TweenParty. TweenParty will force reset delay, loops, duration and ease type of all subtweens.
    /// 
    /// We piggyback on a FloatTween here and use the float value to tween all of our sub-tweens.
    /// </summary>
    public class TweenParty : FloatTween, ITweenTarget<float> {
        public int TotalTweens { get { return _tweenList.Count; } }
        public float CurrentElapsedTime { get; private set; }

        List<ITweenControl> _tweenList = new List<ITweenControl>();


        public TweenParty(float duration) {
            _target = this;
            _duration = duration;
            _toValue = duration;
        }


        #region ITweenTarget

        /// <summary>
        /// value will be an already eased float between 0 and duration. We can just manually apply this to all our
        /// sub-tweens
        /// </summary>
        /// <param name="value">Value.</param>
        public void SetTweenedValue(float value) {
            CurrentElapsedTime = value;
            for (var i = 0; i < _tweenList.Count; i++)
                _tweenList[i].JumpToElapsedTime(value);
        }


        public float GetTweenedValue() {
            return CurrentElapsedTime;
        }


        public new object GetTargetObject() {
            return null;
        }

        #endregion


        #region ITweenControl

        public override void Start() {
            if (_tweenState == TweenState.Complete) {
                _tweenState = TweenState.Running;

                // normalize all of our subtweens. this is gross but it helps alleviate user error
                for (var i = 0; i < _tweenList.Count; i++) {
                    if (_tweenList[i] is ITween<int>)
                        ((ITween<int>)_tweenList[i]).SetDelay(0).SetLoops(LoopType.None).SetDuration(_duration).SetEaseType(_easeType);
                    else if (_tweenList[i] is ITween<float>)
                        ((ITween<float>)_tweenList[i]).SetDelay(0).SetLoops(LoopType.None).SetDuration(_duration).SetEaseType(_easeType);
                    else if (_tweenList[i] is ITween<Vector2>)
                        ((ITween<Vector2>)_tweenList[i]).SetDelay(0).SetLoops(LoopType.None).SetDuration(_duration).SetEaseType(_easeType);
                    else if (_tweenList[i] is ITween<Vector3>)
                        ((ITween<Vector3>)_tweenList[i]).SetDelay(0).SetLoops(LoopType.None).SetDuration(_duration).SetEaseType(_easeType);
                    else if (_tweenList[i] is ITween<Vector4>)
                        ((ITween<Vector4>)_tweenList[i]).SetDelay(0).SetLoops(LoopType.None).SetDuration(_duration).SetEaseType(_easeType);
                    else if (_tweenList[i] is ITween<Quaternion>)
                        ((ITween<Quaternion>)_tweenList[i]).SetDelay(0).SetLoops(LoopType.None).SetDuration(_duration).SetEaseType(_easeType);
                    else if (_tweenList[i] is ITween<Color>)
                        ((ITween<Color>)_tweenList[i]).SetDelay(0).SetLoops(LoopType.None).SetDuration(_duration).SetEaseType(_easeType);
                    else if (_tweenList[i] is ITween<Color32>)
                        ((ITween<Color32>)_tweenList[i]).SetDelay(0).SetLoops(LoopType.None).SetDuration(_duration).SetEaseType(_easeType);

                    _tweenList[i].Start();
                }

                MrTween.Instance.AddTween(this);
            }
        }


        public override void RecycleSelf() {
            for (var i = 0; i < _tweenList.Count; i++)
                _tweenList[i].RecycleSelf();
            _tweenList.Clear();
        }

        #endregion


        #region TweenParty management

        public TweenParty AddTween(ITweenControl tween) {
            tween.Resume();
            _tweenList.Add(tween);

            return this;
        }


        /// <summary>
        /// Prepare TweenParty for reuse. This recycles sub-tweens so use setRecycleTween(false) on any sub-tweens you want to reuse.
        /// </summary>
        /// <param name="duration">Duration.</param>
        public TweenParty PrepareForReuse(float duration) {
            for (var i = 0; i < _tweenList.Count; i++)
                _tweenList[i].RecycleSelf();
            _tweenList.Clear();

            return (TweenParty)PrepareForReuse(0f, duration, duration);
        }

        #endregion

    }
}