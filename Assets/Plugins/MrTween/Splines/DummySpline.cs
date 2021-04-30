#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// place this script on any GameObject to enable route editing. note that it is not required at runtime! it is
/// only required to be in your scene while editing a path
/// </summary>
namespace Mara.MrTween {
    public class DummySpline : MonoBehaviour {
        public string PathName = string.Empty;
        public Color PathColor = Color.magenta; // color of the path if visible in the editor
        public List<Vector3> Nodes = new List<Vector3>() { Vector3.zero, Vector3.one };
        public bool UseStandardHandles = false;
        public bool ForceStraightLinePath = false;
        public bool UseBezier = false;
        public int PathResolution = 30;
        public bool IsInEditMode;
        public bool ClosePath;


        public bool IsMultiPointBezierSpline {
            get {
                return Nodes.Count >= 4 && !ForceStraightLinePath && UseBezier;
            }
        }


        public void OnDrawGizmosSelected() {
            // the editor will draw paths when force straight line is on
            if (!ForceStraightLinePath) {
                var spline = new Spline(Nodes, UseBezier, ForceStraightLinePath);
                if (ClosePath)
                    spline.ClosePath();

                Gizmos.color = PathColor;
                spline.DrawGizmos(PathResolution, IsInEditMode);
            }
        }

    }
}
#endif