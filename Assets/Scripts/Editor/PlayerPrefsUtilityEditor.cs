using UnityEditor;
using UnityEngine;

public class PlayerPrefsUtilityEditor : Editor
{
    [MenuItem("Tools/PlayerPrefs/Delete All PlayerPrefs")]
    private static void DeleteAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Delete All PlayerPrefs",
            "Are you sure you want to delete all PlayerPrefs? This action cannot be undone.", "Delete", "Cancel"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("All PlayerPrefs have been deleted.");
        }
    }
}
