using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Mara.MrTween {
    /// <summary>
    /// this class adds extension methods for the most commonly used tweens
    /// </summary>
    public static class MrTweenExtensions {
        #region Transform tweens

        /// <summary>
        /// transform.position tween
        /// </summary>
        /// <returns>The kposition to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Vector3> PositionTo(this Transform self, Vector3 to, float duration = 0.3f) {
            var tween = QuickCache<TransformVector3Tween>.Pop();
            tween.SetTargetAndType(self, TransformTargetType.Position);
            tween.Initialize(tween, to, duration);

            return tween;
        }


        /// <summary>
        /// transform.localPosition tween
        /// </summary>
        /// <returns>The klocal position to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Vector3> LocalPositionTo(this Transform self, Vector3 to, float duration = 0.3f) {
            var tween = QuickCache<TransformVector3Tween>.Pop();
            tween.SetTargetAndType(self, TransformTargetType.LocalPosition);
            tween.Initialize(tween, to, duration);

            return tween;
        }


        /// <summary>
        /// transform.localScale tween
        /// </summary>
        /// <returns>The klocal scale to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Vector3> LocalScaleTo(this Transform self, Vector3 to, float duration = 0.3f) {
            var tween = QuickCache<TransformVector3Tween>.Pop();
            tween.SetTargetAndType(self, TransformTargetType.LocalScale);
            tween.Initialize(tween, to, duration);

            return tween;
        }


        /// <summary>
        /// transform.eulers tween
        /// </summary>
        /// <returns>The keulers to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Vector3> EulersTo(this Transform self, Vector3 to, float duration = 0.3f) {
            var tween = QuickCache<TransformVector3Tween>.Pop();
            tween.SetTargetAndType(self, TransformTargetType.EulerAngles);
            tween.Initialize(tween, to, duration);

            return tween;
        }


        /// <summary>
        /// transform.localEulers tween
        /// </summary>
        /// <returns>The klocal eulers to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Vector3> LocalEulersTo(this Transform self, Vector3 to, float duration = 0.3f) {
            var tween = QuickCache<TransformVector3Tween>.Pop();
            tween.SetTargetAndType(self, TransformTargetType.LocalEulerAngles);
            tween.Initialize(tween, to, duration);

            return tween;
        }


        /// <summary>
        /// transform.rotation tween
        /// </summary>
        /// <returns>The krotation to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Quaternion> RotationTo(this Transform self, Quaternion to, float duration = 0.3f) {
            var tweenTarget = new TransformRotationTarget(self, TransformRotationTarget.TransformRotationType.Rotation);
            var tween = new QuaternionTween(tweenTarget, self.rotation, to, duration);

            return tween;
        }


        /// <summary>
        /// transform.localRotation tween
        /// </summary>
        /// <returns>The klocal rotation to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Quaternion> LocalRotationTo(this Transform self, Quaternion to, float duration = 0.3f) {
            var tweenTarget = new TransformRotationTarget(self, TransformRotationTarget.TransformRotationType.LocalRotation);
            var tween = new QuaternionTween(tweenTarget, self.localRotation, to, duration);

            return tween;
        }

        #endregion


        #region SpriteRenderer tweens

        /// <summary>
        /// tweens any SpriteRenderer Color property
        /// </summary>
        /// <returns>The kcolor to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="propertyName">Property name.</param>
        public static ITween<Color> ColorTo(this SpriteRenderer self, Color to, float duration = 0.3f) {
            var tweenTarget = new SpriteRendererColorTarget(self);
            var tween = ColorTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens the alpha value of any SpriteRenderer Color property
        /// </summary>
        /// <returns>The kalpha to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="propertyName">Property name.</param>
        public static ITween<float> AlphaTo(this SpriteRenderer self, float to, float duration = 0.3f) {
            var tweenTarget = new SpriteRendererAlphaTarget(self);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion


        #region Material tweens

        /// <summary>
        /// tweens any Material Color property
        /// </summary>
        /// <returns>The kcolor to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="propertyName">Property name.</param>
        public static ITween<Color> ColorTo(this Material self, Color to, float duration = 0.3f, string propertyName = "_Color") {
            var tweenTarget = new MaterialColorTarget(self, propertyName);
            var tween = ColorTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens the alpha value of any Material Color property
        /// </summary>
        /// <returns>The kalpha to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="propertyName">Property name.</param>
        public static ITween<float> AlphaTo(this Material self, float to, float duration = 0.3f, string propertyName = "_Color") {
            var tweenTarget = new MaterialAlphaTarget(self, propertyName);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens any Material float property
        /// </summary>
        /// <returns>The kfloat to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="propertyName">Property name.</param>
        public static ITween<float> FloatTo(this Material self, float to, float duration = 0.3f, string propertyName = "_Color") {
            var tweenTarget = new MaterialFloatTarget(self, propertyName);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens any Material Vector4 property
        /// </summary>
        /// <returns>The vector4 to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="propertyName">Property name.</param>
        public static ITween<Vector4> Vector4To(this Material self, Vector4 to, float duration, string propertyName) {
            var tweenTarget = new MaterialVector4Target(self, propertyName);
            var tween = Vector4Tween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens the Materials texture offset
        /// </summary>
        /// <returns>The ktexture offset to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="propertyName">Property name.</param>
        public static ITween<Vector2> TextureOffsetTo(this Material self, Vector2 to, float duration, string propertyName = "_MainTex") {
            var tweenTarget = new MaterialTextureOffsetTarget(self, propertyName);
            var tween = Vector2Tween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens the Materials texture scale
        /// </summary>
        /// <returns>The ktexture scale to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="propertyName">Property name.</param>
        public static ITween<Vector2> TextureScaleTo(this Material self, Vector2 to, float duration, string propertyName = "_MainTex") {
            var tweenTarget = new MaterialTextureScaleTarget(self, propertyName);
            var tween = Vector2Tween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion


        #region AudioSource tweens

        /// <summary>
        /// tweens an AudioSource volume property
        /// </summary>
        /// <returns>The kvolume to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> VolumeTo(this AudioSource self, float to, float duration = 0.3f) {
            var tweenTarget = new AudioSourceFloatTarget(self, AudioSourceFloatTarget.AudioSourceFloatType.Volume);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens an AudioSource pitch property
        /// </summary>
        /// <returns>The kpitch to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> PitchTo(this AudioSource self, float to, float duration = 0.3f) {
            var tweenTarget = new AudioSourceFloatTarget(self, AudioSourceFloatTarget.AudioSourceFloatType.Pitch);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens an AudioSource panStereo property
        /// </summary>
        /// <returns>The kpan stereo to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> PanStereoTo(this AudioSource self, float to, float duration = 0.3f) {
            var tweenTarget = new AudioSourceFloatTarget(self, AudioSourceFloatTarget.AudioSourceFloatType.PanStereo);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion


        #region Camera tweens

        /// <summary>
        /// tweens the Cameras fieldOfView
        /// </summary>
        /// <returns>The kfield of view to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> FieldOfViewTo(this Camera self, float to, float duration = 0.3f) {
            var tweenTarget = new CameraFloatTarget(self, CameraFloatTarget.CameraTargetType.FieldOfView);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens the Cameras orthographicSize
        /// </summary>
        /// <returns>The korthographic size to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> OrthographicSizeTo(this Camera self, float to, float duration = 0.3f) {
            var tweenTarget = new CameraFloatTarget(self, CameraFloatTarget.CameraTargetType.OrthographicSize);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens the Cameras backgroundColor property
        /// </summary>
        /// <returns>The kbackground color to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Color> BackgroundColorTo(this Camera self, Color to, float duration = 0.3f) {
            var tweenTarget = new CameraBackgroundColorTarget(self);
            var tween = ColorTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens the Cameras rect property
        /// </summary>
        /// <returns>The krect to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Rect> RectTo(this Camera self, Rect to, float duration = 0.3f) {
            var tweenTarget = new CameraRectTarget(self, CameraRectTarget.CameraTargetType.Rect);
            var tween = RectTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens the Cameras pixelRect property
        /// </summary>
        /// <returns>The krect to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Rect> PixelRectTo(this Camera self, Rect to, float duration = 0.3f) {
            var tweenTarget = new CameraRectTarget(self, CameraRectTarget.CameraTargetType.PixelRect);
            var tween = RectTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion


        #region CanvasGroup tweens

        /// <summary>
        /// tweens the CanvasGroup alpha property
        /// </summary>
        /// <returns>The kalpha to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> AlphaTo(this CanvasGroup self, float to, float duration = 0.3f) {
            var tweenTarget = new CanvasGroupAlphaTarget(self);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion


        #region Image tweens

        /// <summary>
        /// tweens an Images alpha property
        /// </summary>
        /// <returns>The kalpha to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> AlphaTo(this Image self, float to, float duration = 0.3f) {
            var tweenTarget = new ImageFloatTarget(self, ImageFloatTarget.ImageTargetType.Alpha);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens an Images fillAmount property
        /// </summary>
        /// <returns>The kfill amount to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> FillAmountTo(this Image self, float to, float duration = 0.3f) {
            var tweenTarget = new ImageFloatTarget(self, ImageFloatTarget.ImageTargetType.FillAmount);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens an Images color property
        /// </summary>
        /// <returns>The kcolor to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Color> ColorTo(this Image self, Color to, float duration = 0.3f) {
            var tweenTarget = new ImageColorTarget(self);
            var tween = ColorTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion


        #region RectTransform tweens

        /// <summary>
        /// tweens the RectTransforms anchoredPosition property
        /// </summary>
        /// <returns>The kanchored position to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Vector2> AnchoredPositionTo(this RectTransform self, Vector2 to, float duration = 0.3f) {
            var tweenTarget = new RectTransformAnchoredPositionTarget(self);
            var tween = Vector2Tween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens the RectTransforms anchoredPosition3D property
        /// </summary>
        /// <returns>The kanchored position3 D to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Vector3> AnchoredPosition3DTo(this RectTransform self, Vector3 to, float duration = 0.3f) {
            var tweenTarget = new RectTransformAnchoredPosition3DTarget(self);
            var tween = Vector3Tween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
		/// tweens the RectTransforms sizeDelta property
		/// </summary>
		/// <returns>The sizeDelta to.</returns>
		/// <param name="self">Self.</param>
		/// <param name="to">To.</param>
		/// <param name="duration">Duration.</param>
		public static ITween<Vector2> SizeDeltaTo(this RectTransform self, Vector2 to, float duration = 0.3f) {
            var tweenTarget = new RectTransformSizeDeltaTarget(self);
            var tween = Vector2Tween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion


        #region ScrollRect tweens

        /// <summary>
        /// tweens the ScrollRects normalizedPosition (scroll position)
        /// </summary>
        /// <returns>The knormalized position to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Vector2> NormalizedPositionTo(this ScrollRect self, Vector2 to, float duration = 0.3f) {
            var tweenTarget = new ScrollRectNormalizedPositionTarget(self);
            var tween = Vector2Tween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion


        #region Light tweens

        /// <summary>
        /// tweens a Lights intensity property
        /// </summary>
        /// <returns>The kintensity to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> IntensityTo(this Light self, float to, float duration = 0.3f) {
            var tweenTarget = new LightFloatTarget(self, LightFloatTarget.LightTargetType.Intensity);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens a Lights range property
        /// </summary>
        /// <returns>The krange to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> RangeTo(this Light self, float to, float duration = 0.3f) {
            var tweenTarget = new LightFloatTarget(self, LightFloatTarget.LightTargetType.Range);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens a Lights spotAngle property
        /// </summary>
        /// <returns>The kspot angle to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<float> SpotAngleTo(this Light self, float to, float duration = 0.3f) {
            var tweenTarget = new LightFloatTarget(self, LightFloatTarget.LightTargetType.SpotAngle);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }


        /// <summary>
        /// tweens a Lights color property
        /// </summary>
        /// <returns>The kcolor to.</returns>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        public static ITween<Color> ColorTo(this Light self, Color to, float duration = 0.3f) {
            var tweenTarget = new LightColorTarget(self);
            var tween = ColorTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion

        #region Text tweens

        /// <summary>
        /// tweens a Text color property
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <returns></returns>
        public static ITween<Color> ColorTo(this Text self, Color to, float duration = 0.3f) {
            var tweenTarget = new TextColorTarget(self);
            var tween = ColorTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        /// <summary>
        /// tweens a Text color alpha property
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="to">To.</param>
        /// <param name="duration">Duration.</param>
        /// <returns></returns>
	    public static ITween<float> AlphaTo(this Text self, float to, float duration = 0.3f) {
            var tweenTarget = new TextAlphaTarget(self);
            var tween = FloatTween.Create();
            tween.Initialize(tweenTarget, to, duration);

            return tween;
        }

        #endregion
    }
}