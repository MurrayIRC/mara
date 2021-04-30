using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Mara.MrTween {
    [CreateAssetMenu(fileName = "New MrTweenSpline", menuName = "MrTween/Create New MrTweenSplineSettings Asset")]
    public class MrTweenSplineSettings : ScriptableObject {
        public List<Vector3> Nodes;

#if UNITY_EDITOR
        // From: https://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
        public static void CreateAsset(string path, List<Vector3> nodes) {
            MrTweenSplineSettings asset = AssetDatabase.LoadAssetAtPath(path.Replace(Application.dataPath, "Assets"), typeof(MrTweenSplineSettings)) as MrTweenSplineSettings;
            if (asset != null) {
                asset.Nodes = nodes;
            } else {
                asset = ScriptableObject.CreateInstance<MrTweenSplineSettings>();
                asset.Nodes = nodes;
                AssetDatabase.CreateAsset(asset, path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
#endif
    }
}
