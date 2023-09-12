using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Creams
{
    public class CopyObjectPathEditor : MonoBehaviour
    {
        [MenuItem("GameObject/Copy Object Path")]
        public static void CopyObjectPath()
        {
            string objectPath = "";
            Transform objectTransform = Selection.activeTransform;
            while (objectTransform != null)
            {
                objectPath = objectTransform.name + "/" + objectPath;
                objectTransform = objectTransform.parent;
            }
            objectPath = objectPath.TrimEnd('/');
            GUIUtility.systemCopyBuffer = objectPath;
            Debug.Log("Object path copied: " + objectPath);
        }
    }
}
#endif