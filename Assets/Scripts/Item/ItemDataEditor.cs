using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
#endif
using Enums;

//[System.Serializable]
//[CustomEditor(typeof(ItemData)), CanEditMultipleObjects]
//public class ItemDataEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        ItemData itemData = (ItemData)target;

//        EditorGUILayout.PropertyField(serializedObject.FindProperty("e_item_Type")); 
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("item_name"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("item_description"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("item_explain"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("item_sprite"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("item_sprite_Big"));

//        switch (itemData.e_item_Type)
//        {
//            case Enums.Item_Type.Passive:
//                EditorGUILayout.PropertyField(serializedObject.FindProperty("attackBonus"));
//                EditorGUILayout.PropertyField(serializedObject.FindProperty("armorBonus"));
//                EditorGUILayout.PropertyField(serializedObject.FindProperty("speedBonus"));
//                EditorGUILayout.PropertyField(serializedObject.FindProperty("fireRateBonus"));
//                break;
//            case Enums.Item_Type.Active:
//                break;
//            default:
//                break;
//        }

//        serializedObject.ApplyModifiedProperties();
//        if (GUI.changed)
//        { 
//            EditorUtility.SetDirty(target); 
//        }
//    }
//}
