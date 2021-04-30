using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Mara.MrTween {
    public class TransformRotationTarget : AbstractTweenTarget<Transform, Quaternion> {
        public enum TransformRotationType {
            Rotation,
            LocalRotation
        }

        TransformRotationType _targetType;


        public override void SetTweenedValue(Quaternion value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            switch (_targetType) {
                case TransformRotationType.Rotation:
                    _target.rotation = value;
                    break;
                case TransformRotationType.LocalRotation:
                    _target.localRotation = value;
                    break;
            }
        }


        public override Quaternion GetTweenedValue() {
            switch (_targetType) {
                case TransformRotationType.Rotation:
                    return _target.rotation;
                case TransformRotationType.LocalRotation:
                    return _target.localRotation;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public TransformRotationTarget(Transform transform, TransformRotationType targetType) {
            _target = transform;
            _targetType = targetType;
        }
    }


    #region SpriteRenderer target


    public abstract class AbstractSpriteRendererTarget {
        protected SpriteRenderer _spriteRenderer;


        public void PrepareForUse(SpriteRenderer spriteRenderer) {
            _spriteRenderer = spriteRenderer;
        }


        public object GetTargetObject() {
            return _spriteRenderer;
        }
    }


    public class SpriteRendererColorTarget : AbstractSpriteRendererTarget, ITweenTarget<Color> {
        public SpriteRendererColorTarget(SpriteRenderer spriteRenderer) {
            PrepareForUse(spriteRenderer);
        }


        public void SetTweenedValue(Color value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_spriteRenderer)
                return;

            _spriteRenderer.color = value;
        }


        public Color GetTweenedValue() {
            return _spriteRenderer.color;
        }
    }


    public class SpriteRendererAlphaTarget : AbstractSpriteRendererTarget, ITweenTarget<float> {
        public SpriteRendererAlphaTarget(SpriteRenderer spriteRenderer) {
            PrepareForUse(spriteRenderer);
        }


        public void SetTweenedValue(float value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_spriteRenderer)
                return;

            var color = _spriteRenderer.color;
            color.a = value;
            _spriteRenderer.color = color;
        }


        public float GetTweenedValue() {
            return _spriteRenderer.color.a;
        }
    }


    #endregion


    #region Text targets

    public abstract class AbstractTextTarget {
        protected Text _text;

        public void PrepareForUse(Text text) {
            _text = text;
        }

        public object GetTargetObject() {
            return _text;
        }
    }

    public class TextColorTarget : AbstractTextTarget, ITweenTarget<Color> {
        public TextColorTarget(Text text) {
            PrepareForUse(text);
        }

        public void SetTweenedValue(Color value) {
            if (MrTween.EnableBabysitter && !_text)
                return;

            _text.color = value;
        }

        public Color GetTweenedValue() {
            return _text.color;
        }
    }

    public class TextAlphaTarget : AbstractTextTarget, ITweenTarget<float> {
        public TextAlphaTarget(Text text) {
            PrepareForUse(text);
        }

        public void SetTweenedValue(float value) {
            if (MrTween.EnableBabysitter && !_text)
                return;

            var color = _text.color;
            color.a = value;
            _text.color = color;
        }

        public float GetTweenedValue() {
            return _text.color.a;
        }
    }

    #endregion


    #region Material targets

    public abstract class AbstractMaterialTarget {
        protected Material _material;
        protected int _materialNameId;


        public void PrepareForUse(Material material, string propertyName) {
            _material = material;
            _materialNameId = Shader.PropertyToID(propertyName);
        }


        public object GetTargetObject() {
            return _material;
        }
    }


    public class MaterialColorTarget : AbstractMaterialTarget, ITweenTarget<Color> {
        public MaterialColorTarget(Material material, string propertyName) {
            PrepareForUse(material, propertyName);
        }


        public void SetTweenedValue(Color value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_material)
                return;

            _material.SetColor(_materialNameId, value);
        }


        public Color GetTweenedValue() {
            return _material.GetColor(_materialNameId);
        }
    }


    public class MaterialAlphaTarget : AbstractMaterialTarget, ITweenTarget<float> {
        public MaterialAlphaTarget(Material material, string propertyName) {
            PrepareForUse(material, propertyName);
        }


        public void SetTweenedValue(float value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_material)
                return;

            var color = _material.GetColor(_materialNameId);
            color.a = value;
            _material.SetColor(_materialNameId, color);
        }


        public float GetTweenedValue() {
            return _material.GetColor(_materialNameId).a;
        }
    }


    public class MaterialFloatTarget : AbstractMaterialTarget, ITweenTarget<float> {
        public MaterialFloatTarget(Material material, string propertyName) {
            PrepareForUse(material, propertyName);
        }


        public void SetTweenedValue(float value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_material)
                return;

            _material.SetFloat(_materialNameId, value);
        }


        public float GetTweenedValue() {
            return _material.GetFloat(_materialNameId);
        }
    }


    public class MaterialVector4Target : AbstractMaterialTarget, ITweenTarget<Vector4> {
        public MaterialVector4Target(Material material, string propertyName) {
            PrepareForUse(material, propertyName);
        }


        public void SetTweenedValue(Vector4 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_material)
                return;

            _material.SetVector(_materialNameId, value);
        }


        public Vector4 GetTweenedValue() {
            return _material.GetVector(_materialNameId);
        }
    }


    public class MaterialTextureOffsetTarget : AbstractMaterialTarget, ITweenTarget<Vector2> {
        string _propertyName;

        public MaterialTextureOffsetTarget(Material material, string propertyName) {
            PrepareForUse(material, propertyName);
            _propertyName = propertyName;
        }


        public void SetTweenedValue(Vector2 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_material)
                return;

            _material.SetTextureOffset(_propertyName, value);
        }


        public Vector2 GetTweenedValue() {
            return _material.GetTextureOffset(_propertyName);
        }
    }


    public class MaterialTextureScaleTarget : AbstractMaterialTarget, ITweenTarget<Vector2> {
        string _propertyName;

        public MaterialTextureScaleTarget(Material material, string propertyName) {
            PrepareForUse(material, propertyName);
            _propertyName = propertyName;
        }


        public void SetTweenedValue(Vector2 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !_material)
                return;

            _material.SetTextureScale(_propertyName, value);
        }


        public Vector2 GetTweenedValue() {
            return _material.GetTextureScale(_propertyName);
        }
    }

    #endregion


    public class AudioSourceFloatTarget : AbstractTweenTarget<AudioSource, float> {
        public enum AudioSourceFloatType {
            Volume,
            Pitch,
            PanStereo
        }

        AudioSourceFloatType _tweenType;


        public override void SetTweenedValue(float value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            switch (_tweenType) {
                case AudioSourceFloatType.Volume:
                    _target.volume = value;
                    break;
                case AudioSourceFloatType.Pitch:
                    _target.pitch = value;
                    break;
                case AudioSourceFloatType.PanStereo:
                    _target.panStereo = value;
                    break;
            }
        }


        public override float GetTweenedValue() {
            switch (_tweenType) {
                case AudioSourceFloatType.Volume:
                    return _target.volume;
                case AudioSourceFloatType.Pitch:
                    return _target.pitch;
                case AudioSourceFloatType.PanStereo:
                    return _target.panStereo;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public AudioSourceFloatTarget(AudioSource audioSource, AudioSourceFloatType targetType) {
            _target = audioSource;
            _tweenType = targetType;
        }
    }


    public class CameraFloatTarget : AbstractTweenTarget<Camera, float> {
        public enum CameraTargetType {
            OrthographicSize,
            FieldOfView
        }

        CameraTargetType _targetType;


        public override void SetTweenedValue(float value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            switch (_targetType) {
                case CameraTargetType.OrthographicSize:
                    _target.orthographicSize = value;
                    break;
                case CameraTargetType.FieldOfView:
                    _target.fieldOfView = value;
                    break;
            }
        }


        public override float GetTweenedValue() {
            switch (_targetType) {
                case CameraTargetType.OrthographicSize:
                    return _target.orthographicSize;
                case CameraTargetType.FieldOfView:
                    return _target.fieldOfView;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public CameraFloatTarget(Camera camera, CameraTargetType targetType = CameraTargetType.OrthographicSize) {
            _target = camera;
            _targetType = targetType;
        }
    }


    public class CameraBackgroundColorTarget : AbstractTweenTarget<Camera, Color> {
        public override void SetTweenedValue(Color value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            _target.backgroundColor = value;
        }


        public override Color GetTweenedValue() {
            return _target.backgroundColor;
        }


        public CameraBackgroundColorTarget(Camera camera) {
            _target = camera;
        }
    }


    public class CameraRectTarget : AbstractTweenTarget<Camera, Rect> {
        public enum CameraTargetType {
            Rect,
            PixelRect
        }

        CameraTargetType _targetType;


        public override void SetTweenedValue(Rect value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            switch (_targetType) {
                case CameraTargetType.Rect:
                    _target.rect = value;
                    break;
                case CameraTargetType.PixelRect:
                    _target.pixelRect = value;
                    break;
            }
        }


        public override Rect GetTweenedValue() {
            switch (_targetType) {
                case CameraTargetType.Rect:
                    return _target.rect;
                case CameraTargetType.PixelRect:
                    return _target.pixelRect;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public CameraRectTarget(Camera camera, CameraTargetType targetType = CameraTargetType.Rect) {
            _target = camera;
            _targetType = targetType;
        }
    }


    public class CanvasGroupAlphaTarget : AbstractTweenTarget<CanvasGroup, float> {
        public override void SetTweenedValue(float value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            _target.alpha = value;
        }


        public override float GetTweenedValue() {
            return _target.alpha;
        }


        public CanvasGroupAlphaTarget(CanvasGroup canvasGroup) {
            _target = canvasGroup;
        }
    }


    public class ImageFloatTarget : AbstractTweenTarget<Image, float> {
        public enum ImageTargetType {
            Alpha,
            FillAmount
        }

        ImageTargetType _targetType;


        public ImageFloatTarget(Image image, ImageTargetType targetType = ImageTargetType.Alpha) {
            _target = image;
            _targetType = targetType;
        }


        public override void SetTweenedValue(float value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            switch (_targetType) {
                case ImageTargetType.Alpha:
                    var color = _target.color;
                    color.a = value;
                    _target.color = color;
                    break;
                case ImageTargetType.FillAmount:
                    _target.fillAmount = value;
                    break;
            }
        }


        public override float GetTweenedValue() {
            switch (_targetType) {
                case ImageTargetType.Alpha:
                    return _target.color.a;
                case ImageTargetType.FillAmount:
                    return _target.fillAmount;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public void SetTweenedValue(Color value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            _target.color = value;
        }
    }


    public class ImageColorTarget : AbstractTweenTarget<Image, Color> {
        public ImageColorTarget(Image image) {
            _target = image;
        }


        public override void SetTweenedValue(Color value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            _target.color = value;
        }


        public override Color GetTweenedValue() {
            return _target.color;
        }
    }


    public class RectTransformAnchoredPositionTarget : AbstractTweenTarget<RectTransform, Vector2> {
        public override void SetTweenedValue(Vector2 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            _target.anchoredPosition = value;
        }


        public override Vector2 GetTweenedValue() {
            return _target.anchoredPosition;
        }


        public RectTransformAnchoredPositionTarget(RectTransform rectTransform) {
            _target = rectTransform;
        }
    }


    public class RectTransformAnchoredPosition3DTarget : AbstractTweenTarget<RectTransform, Vector3> {
        public override void SetTweenedValue(Vector3 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            _target.anchoredPosition3D = value;
        }


        public override Vector3 GetTweenedValue() {
            return _target.anchoredPosition3D;
        }


        public RectTransformAnchoredPosition3DTarget(RectTransform rectTransform) {
            _target = rectTransform;
        }
    }


    public class RectTransformSizeDeltaTarget : AbstractTweenTarget<RectTransform, Vector2> {
        public override void SetTweenedValue(Vector2 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            _target.sizeDelta = value;
        }


        public override Vector2 GetTweenedValue() {
            return _target.sizeDelta;
        }


        public RectTransformSizeDeltaTarget(RectTransform rectTransform) {
            _target = rectTransform;
        }
    }


    public class ScrollRectNormalizedPositionTarget : AbstractTweenTarget<ScrollRect, Vector2> {
        public override void SetTweenedValue(Vector2 value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            _target.normalizedPosition = value;
        }


        public override Vector2 GetTweenedValue() {
            return _target.normalizedPosition;
        }


        public ScrollRectNormalizedPositionTarget(ScrollRect scrollRect) {
            _target = scrollRect;
        }
    }


    public class LightColorTarget : AbstractTweenTarget<Light, Color> {
        public override void SetTweenedValue(Color value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            _target.color = value;
        }


        public override Color GetTweenedValue() {
            return _target.color;
        }


        public LightColorTarget(Light light) {
            _target = light;
        }
    }


    public class LightFloatTarget : AbstractTweenTarget<Light, float> {
        public enum LightTargetType {
            Intensity,
            Range,
            SpotAngle
        }

        LightTargetType _targetType;


        public override void SetTweenedValue(float value) {
            // if the babysitter is enabled and we dont validate just silently do nothing
            if (MrTween.EnableBabysitter && !ValidateTarget())
                return;

            switch (_targetType) {
                case LightTargetType.Intensity:
                    _target.intensity = value;
                    break;
                case LightTargetType.Range:
                    _target.range = value;
                    break;
                case LightTargetType.SpotAngle:
                    _target.spotAngle = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public override float GetTweenedValue() {
            switch (_targetType) {
                case LightTargetType.Intensity:
                    return _target.intensity;
                case LightTargetType.Range:
                    return _target.range;
                case LightTargetType.SpotAngle:
                    return _target.spotAngle;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }


        public LightFloatTarget(Light light, LightTargetType targetType = LightTargetType.Intensity) {
            _target = light;
            _targetType = targetType;
        }
    }

}
