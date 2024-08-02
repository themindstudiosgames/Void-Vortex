using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Utils.Editor
{
    public class UIScaler : EditorWindow
    {
        private GameObject rootGO;
        private float scaleFactor = 1.0f;

        [MenuItem("Tools/UI Scaler")]
        public static void ShowWindow()
        {
            GetWindow<UIScaler>("UI Scaler");
        }

        private void OnGUI()
        {
            GUILayout.Label("Scale UI Screen", EditorStyles.boldLabel);

            rootGO = (GameObject)EditorGUILayout.ObjectField("Root UI GameObject", rootGO, typeof(GameObject), true);
            scaleFactor = EditorGUILayout.FloatField("Scale Factor", scaleFactor);

            if (GUILayout.Button("Scale Down UI"))
            {
                if (rootGO != null && scaleFactor > 0)
                {
                    ScaleUI(rootGO, scaleFactor);
                }
            }
        }

        private void ScaleUI(GameObject root, float scaleFactor)
        {
            RectTransform[] rectTransforms = root.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform rt in rectTransforms)
            {
                ScaleRectTransform(rt, scaleFactor);
            }
        }

        private void ScaleRectTransform(RectTransform rectTransform, float scaleFactor)
        {
            rectTransform.anchoredPosition *= scaleFactor;
            rectTransform.sizeDelta *= scaleFactor;
            rectTransform.offsetMin *= scaleFactor;
            rectTransform.offsetMax *= scaleFactor;
        }
    }
}
#endif