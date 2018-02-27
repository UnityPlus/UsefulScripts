using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
 
public class EditorControls
{
    private static EditorWindow _mouseOverWindow;
 
    [MenuItem("Editor Controls/Toggle Lock %`")]
    static void ToggleInspectorLock()
    {
        if (_mouseOverWindow == null)
        {
            if (!EditorPrefs.HasKey("LockableInspectorIndex"))
                EditorPrefs.SetInt("LockableInspectorIndex", 0);
            int i = EditorPrefs.GetInt("LockableInspectorIndex");
 
            Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
            Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
            _mouseOverWindow = (EditorWindow)findObjectsOfTypeAll[i];
        }
 
        if (_mouseOverWindow != null && _mouseOverWindow.GetType().Name == "InspectorWindow")
        {
            Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
            PropertyInfo propertyInfo = type.GetProperty("isLocked");
            bool value = (bool)propertyInfo.GetValue(_mouseOverWindow, null);
            propertyInfo.SetValue(_mouseOverWindow, !value, null);
            _mouseOverWindow.Repaint();
        }
    }
 
    [MenuItem("Editor Controls/Clear Console %&c")]
    static void ClearConsole()
    {
        Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditorInternal.LogEntries");
        type.GetMethod("Clear").Invoke(null,null);
    }

    [MenuItem("Editor Controls/New Child GameObject %&n")]
    static void NewChildGameObject()
    {
        if (Selection.activeTransform) 
        {
            GameObject newObject = new GameObject ();
            Undo.RegisterCreatedObjectUndo(newObject, "New Child Created");
            newObject.transform.parent = Selection.activeTransform;
            newObject.transform.position = Selection.activeTransform.position;
        }
    }
    
    [MenuItem("Editor Controls/TurnOffRenderersAndColliders")]
    static void TurnOffRenderersAndColliders()
    {
        foreach (Transform transform in Selection.transforms)
            Utilities.ToggleChildrenRenderersAndColliders(transform, false);
    }

    [MenuItem("Editor Controls/TurnOnRenderersAndColliders")]
    static void TurnOnRenderersAndColliders()
    {
        foreach (Transform transform in Selection.transforms)
            Utilities.ToggleChildrenRenderersAndColliders(transform, true);
    }
    
    #region Zero Object Pos/Rot
    [MenuItem("Editor Controls/ZeroObjectsRotation %j")]
    static void ZeroObjectsRotation()
    {
        EditorControls.ZeroSelectedObjectsRotation();
    }

    [MenuItem("Editor Controls/ZeroObjectsPosition %h")]
    static void ZeroObjectsPosition()
    {
        EditorControls.ZeroSelectedObjectsPosition();
    }

    static void ZeroSelectedObjectsPosition()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Zero Object Position");
            obj.transform.localPosition = Vector3.zero;
        }
    }

    static void ZeroSelectedObjectsRotation()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Zero Object Rotation");
            obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
    }
    #endregion
    
    #region Group/Ungroup Objects
    [MenuItem("Editor Controls/GroupObjects %g")]
    static void GroupObjects()
    {
        EditorControls.GroupSelectedObjects();
    }
    
    [MenuItem("Editor Controls/RemoveParents %#g")]
    static void RemoveParents()
    {
        EditorControls.RemoveParentsOfSelectedObjects();
    }
    
    static void GroupSelectedObjects()
    {
        if (Selection.gameObjects.Length >= 1)
        {

            GameObject parentObject = new GameObject("Group - " + Selection.gameObjects[0].name);
            Undo.RegisterCreatedObjectUndo(parentObject, "Group Parent");

            if(Selection.gameObjects.Length > 1)
            {
                parentObject.transform.position = GetAveragePosition();
                parentObject.transform.rotation = new Quaternion(0,0,0,0);
            }
            else
            {
                parentObject.transform.position = Selection.gameObjects[0].transform.position;
                parentObject.transform.rotation = Selection.gameObjects[0].transform.rotation;
            }

            parentObject.transform.parent = Selection.gameObjects[0].transform.parent;

            foreach (GameObject obj in Selection.gameObjects)
            {
                Undo.SetTransformParent(obj.transform, parentObject.transform, "Grouped");
            }

            Selection.activeGameObject = parentObject;
        }
    }

    static Vector3 GetAveragePosition()
    {
        Vector3 centroid = Vector3.zero;

        float x = 0f;
        float y = 0f;
        float z = 0f;
        foreach (GameObject go in Selection.gameObjects)
        {
            x += go.transform.position.x;
            y += go.transform.position.y;
            z += go.transform.position.z;
            centroid += go.transform.position;
        }
        return (centroid /= Selection.gameObjects.Length);
    }

    static void RemoveParentsOfSelectedObjects()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.SetTransformParent(obj.transform, null, "Remove Parents");

            Transform[] childrenOfObj = obj.transform.GetComponentsInChildren<Transform>();

            foreach (Transform child in childrenOfObj)
            {
                Undo.SetTransformParent(child, null, "Remove Parent");
            }
        }
    }
    #endregion
    
    #region RevertSelection
    [MenuItem("Editor Controls/RevertSelection %#r")]
    static void RevertSelection()
    {
        if (Selection.activeGameObject)
        {
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                RegisterAllComponentsInChildren(gameObject.transform);
                PrefabUtility.RevertPrefabInstance(gameObject);
            }
        }
    }

    static void RegisterAllComponents(Transform target)
    {
        foreach (Component component in target.GetComponents<Component>())
            Undo.RecordObject(component, "Reverted");
    }

    static void RegisterAllComponentsInChildren(Transform target)
    {
        if (target)
        {
            RegisterAllComponents(target);
            for (int index = 0; index < target.childCount; index++) 
                RegisterAllComponentsInChildren(target.GetChild(index));
        }
    }
    #endregion
}

