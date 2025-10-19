using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SoSystem
{
    [CustomEditor(typeof(StringVariable))]
    public class StringVariableEditor : Editor
    {
        private SerializedProperty _value;

        private void OnEnable()
        {
            _value = serializedObject.FindProperty("_value");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.DelayedTextField(_value);

            var myTarget = (StringVariable)target;
            myTarget.Value = _value.stringValue;

            serializedObject.ApplyModifiedProperties();
        }
    }

}
