﻿using UnityEngine;
using System.Collections;
using System;


namespace Mara.MrTween {
    /// <summary>
    /// AbstractTweenable serves as a base for any custom classes you might want to make that can be ticked. These differ from
    /// ITweens in that they dont implement the ITweenT interface. What does that mean? It just says that an AbstractTweenable
    /// is not just moving a value from start to finish. It can do anything at all that requires a tick each frame.
    /// 
    /// The TweenChain is one example of AbstractTweenable for reference.
    /// </summary>
    public abstract class AbstractTweenable : ITweenable {
        protected bool _isPaused;

        /// <summary>
        /// AbstractTweenable are often kept around after they complete. This flag lets them know internally if they are currently
        /// being tweened by MrTween so that they can readd themselves if necessary.
        /// </summary>
        protected bool _isCurrentlyManagedByMrTween;


        #region ITweenable

        public abstract bool Tick();


        public virtual void RecycleSelf() { }


        public bool IsRunning() {
            return _isCurrentlyManagedByMrTween && !_isPaused;
        }


        public virtual void Start() {
            // dont add ourself twice!
            if (_isCurrentlyManagedByMrTween) {
                _isPaused = false;
                return;
            }

            MrTween.Instance.AddTween(this);
            _isCurrentlyManagedByMrTween = true;
            _isPaused = false;
        }


        public void Pause() {
            _isPaused = true;
        }


        public void Resume() {
            _isPaused = false;
        }


        public virtual void Stop(bool bringToCompletion = false, bool bringToCompletionImmediately = false) {
            MrTween.Instance.RemoveTween(this);
            _isCurrentlyManagedByMrTween = false;
            _isPaused = true;
        }

        #endregion

    }
}