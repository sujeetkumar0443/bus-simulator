using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public static class FixLevelSel
{
    [MenuItem("Tools/Bus/Fix LevelSelection Start Button")]
    static void Run()
    {
        var sc = EditorSceneManager.OpenScene("Assets/Scenes/LevelSelection.unity", OpenSceneMode.Single);
        string log = "";
        foreach (var root in sc.GetRootGameObjects())
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if ((t.name == "Start" || t.name == "Back") && t.parent != null && t.parent.name.Contains("UI"))
                {
                    t.SetAsLastSibling();
                    log += "moved " + t.name + " under " + t.parent.name + "\n";
                }
            }
        }
        EditorSceneManager.MarkSceneDirty(sc);
        EditorSceneManager.SaveScene(sc);
        Directory.CreateDirectory("BusTest");
        File.WriteAllText("BusTest/fixlevel.txt", log);
    }
}
