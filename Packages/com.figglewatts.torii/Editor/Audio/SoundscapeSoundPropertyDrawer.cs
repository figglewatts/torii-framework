// using UnityEditor;
// using UnityEngine;
//
// namespace Torii.Audio
// {
//     [CustomPropertyDrawer(typeof(SoundscapeSound))]
//     public class SoundscapeSoundPropertyDrawer : PropertyDrawer
//     {
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             EditorGUI.BeginProperty(position, label, property);
//
//             position = EditorGUI.PrefixLabel(position, label);
//             EditorGUI.PropertyField(position, property.FindPropertyRelative("Sound"));
//             EditorGUI.MinMaxSlider(position, "Angle range", ref property.);
//
//             EditorGUI.EndProperty();
//         }
//     }
// }


