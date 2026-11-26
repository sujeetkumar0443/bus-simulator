#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Menu: Tools > Bus > Swap Vehicle 0 (Bus5) With New Bus
// Open Assets/Scenes/DayGameplay.unity first, then click the menu once.
public static class SwapBusModel
{
    const string PrefabPath = "Assets/mb_371R_pbr/prefab/mb_371R_pbr.prefab";
    const string ScenePath = "Assets/Scenes/DayGameplay.unity";
    const string BackupPath = "Assets/Scenes/DayGameplay_backup_before_newbus.unity";
    const string MarkerName = "NewBus_mb371R";
    const float BusScale = 1.2f; // new model is 9.5 m long; this makes it ~11.4 m like the other buses

    [MenuItem("Tools/Bus/Swap Vehicle 0 (Bus5) With New Bus")]
    public static void Swap()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath)
        {
            EditorUtility.DisplayDialog("Swap Bus", "Open Assets/Scenes/DayGameplay.unity first, then run this again.", "OK");
            return;
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Swap Bus", "Prefab not found:\n" + PrefabPath, "OK");
            return;
        }

        GameObject vehicles = GameObject.Find("Vehicles");
        if (vehicles == null || vehicles.transform.childCount == 0)
        {
            EditorUtility.DisplayDialog("Swap Bus", "'Vehicles' object not found in the scene.", "OK");
            return;
        }

        Transform bus = vehicles.transform.GetChild(0);
        // Re-run support: remove a previously installed copy first.
        Transform old = bus.Find("Chassis/" + MarkerName);
        if (old == null) old = bus.Find(MarkerName);
        if (old != null) Undo.DestroyObjectImmediate(old.gameObject);

        RCC_CarControllerV3 car = bus.GetComponent<RCC_CarControllerV3>();
        if (car == null) car = bus.GetComponentInChildren<RCC_CarControllerV3>(true);
        if (car == null)
        {
            EditorUtility.DisplayDialog("Swap Bus", "RCC_CarControllerV3 not found on " + bus.name, "OK");
            return;
        }

        if (!EditorUtility.DisplayDialog("Swap Bus",
            "This will replace the look of '" + bus.name + "' (vehicle 0) with the new bus.\n\nA scene backup is saved first:\n" + BackupPath + "\n\nContinue?",
            "Yes", "Cancel"))
            return;

        // 1) Backup
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.DeleteAsset(BackupPath);
        if (!AssetDatabase.CopyAsset(ScenePath, BackupPath))
        {
            EditorUtility.DisplayDialog("Swap Bus", "Could not create backup. Aborting.", "OK");
            return;
        }

        Undo.SetCurrentGroupName("Swap Bus With New Bus");
        int group = Undo.GetCurrentGroup();

        // 2) Instantiate new bus under the chassis (or bus root)
        Transform parent = car.transform.Find("Chassis");
        if (parent == null) parent = car.transform;

        GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        Undo.RegisterCreatedObjectUndo(inst, "Create new bus");
        inst.name = MarkerName;
        // Unpack so we are allowed to re-parent the wheel hub caps under their wheels.
        PrefabUtility.UnpackPrefabInstance(inst, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        inst.transform.SetParent(parent, false);
        inst.transform.localPosition = Vector3.zero;
        inst.transform.localRotation = Quaternion.identity;
        inst.transform.localScale = Vector3.one * BusScale;

        // New model must not collide on its own - RCC already has colliders.
        foreach (Collider c in inst.GetComponentsInChildren<Collider>(true))
            c.enabled = false;
        foreach (Rigidbody rb in inst.GetComponentsInChildren<Rigidbody>(true))
            Object.DestroyImmediate(rb);

        // 3) Hide old visuals (keep logic objects, doors animators, passengers, lights)
        int hidden = 0;
        foreach (Renderer r in car.GetComponentsInChildren<Renderer>(true))
        {
            if (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer)) continue;
            if (r.transform.IsChildOf(inst.transform)) continue;
            if (KeepVisible(r.transform, car.transform)) continue;
            Undo.RecordObject(r, "Hide old bus mesh");
            r.enabled = false;
            hidden++;
        }

        // Also switch off the old model groups outright.
        Transform oldRoot = car.transform.Find("Chassis/Bus5");
        if (oldRoot != null)
        {
            foreach (string nm in new[] { "Group001", "inside and glass", "neongg" })
            {
                Transform g = oldRoot.Find(nm);
                if (g != null && g.gameObject.activeSelf) { Undo.RecordObject(g.gameObject, "Hide old model"); g.gameObject.SetActive(false); hidden++; }
            }
        }
        PlayerPrefs.SetInt("vehicle", 0);

        // 4) Find new wheels by position
        List<Renderer> wheels = inst.GetComponentsInChildren<Renderer>(true)
            .Where(r => r.name.ToLower().StartsWith("wheel_") && WorldBounds(r).size.magnitude > 0.3f).ToList(); // skips the empty parent "wheel_front_left"
        string log = "";
        {
            int hubs = 0;
            foreach (Renderer hub in inst.GetComponentsInChildren<Renderer>(true).Where(r => r.name.ToLower().StartsWith("wheelcc")).ToList())
            {
                if (wheels.Count == 0) break;
                Vector3 hc = WorldBounds(hub).center;
                Renderer best = wheels.OrderBy(w => (WorldBounds(w).center - hc).sqrMagnitude).First();
                if (hub.transform.IsChildOf(best.transform)) continue;
                Undo.SetTransformParent(hub.transform, best.transform, "Parent hub cap to wheel");
                hubs++;
            }
            log += "Hub caps attached to wheels: " + hubs + "\n";
        }
        if (wheels.Count >= 4)
        {
            float midZ = wheels.Average(w => WorldBounds(w).center.z);
            // use bus-local coordinates
            System.Func<Renderer, Vector3> loc = w => car.transform.InverseTransformPoint(WorldBounds(w).center);
            var sorted = wheels.OrderByDescending(w => loc(w).z).ToList();
            var front = sorted.Take(2).ToList();
            var rest = sorted.Skip(2).ToList();

            Renderer fl = front.OrderBy(w => loc(w).x).First();      // lower x = left
            Renderer fr = front.OrderByDescending(w => loc(w).x).First();
            Renderer rl = rest.Where(w => loc(w).x < 0).OrderByDescending(w => loc(w).z).FirstOrDefault();
            Renderer rr = rest.Where(w => loc(w).x >= 0).OrderByDescending(w => loc(w).z).FirstOrDefault();

            if (rl != null && rr != null)
            {
                AssignWheel(car, car.FrontLeftWheelCollider, ref car.FrontLeftWheelTransform, fl);
                AssignWheel(car, car.FrontRightWheelCollider, ref car.FrontRightWheelTransform, fr);
                AssignWheel(car, car.RearLeftWheelCollider, ref car.RearLeftWheelTransform, rl);
                AssignWheel(car, car.RearRightWheelCollider, ref car.RearRightWheelTransform, rr);
                log += "Wheels assigned: FL=" + fl.name + " FR=" + fr.name + " RL=" + rl.name + " RR=" + rr.name;
            }
        }
        if (!log.Contains("Wheels assigned")) log += "WARNING: could not auto-detect 4 wheels. Hidden old wheels are still referenced - assign wheels manually.";

        // Fit the main body collider to the new bus size.
        if (oldRoot != null)
        {
            Transform colT = oldRoot.Find("Collider");
            BoxCollider bc = colT != null ? colT.GetComponent<BoxCollider>() : null;
            bool any = false; Bounds nb = new Bounds();
            if (bc != null)
            {
                foreach (Renderer r in inst.GetComponentsInChildren<Renderer>(true))
                {
                    if (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer)) continue;
                    if (r.name.ToLower().StartsWith("wheel")) continue;
                    MeshFilter mf = r.GetComponent<MeshFilter>();
                    Mesh m = mf != null ? mf.sharedMesh : (r is SkinnedMeshRenderer ? ((SkinnedMeshRenderer)r).sharedMesh : null);
                    if (m == null) continue;
                    Bounds lb = m.bounds; Matrix4x4 mat = r.transform.localToWorldMatrix;
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3 c = lb.center + Vector3.Scale(lb.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                        Vector3 p = bc.transform.InverseTransformPoint(mat.MultiplyPoint3x4(c)); // collider-local space
                        if (!any) { nb = new Bounds(p, Vector3.zero); any = true; } else nb.Encapsulate(p);
                    }
                }
            }
            if (bc != null && any)
            {
                Undo.RecordObject(bc, "Fit body collider");
                bc.center = nb.center;
                bc.size = nb.size;
                EditorUtility.SetDirty(bc);
                log += "\nBody collider fitted (local " + nb.size.x.ToString("F1") + " x " + nb.size.y.ToString("F1") + " x " + nb.size.z.ToString("F1") + ")";
            }
        }

        // RCC crash-damage reads mesh vertices of every mesh under the car; the new FBX meshes are not
        // Read/Write enabled (and are very heavy), which throws errors and stops RCC from starting.
        Undo.RecordObject(car, "Disable RCC damage");
        car.useDamage = false;

        // Move the RCC light objects (head/brake/reverse/indicator) onto the new bus' light positions.
        Transform chassisT = car.transform.Find("Chassis");
        Transform lightsT = chassisT != null ? chassisT.Find("Lights") : null;
        int movedLights = 0;
        if (lightsT != null)
        {
            foreach (Transform lt in lightsT)
            {
                string ln = lt.name.ToLower();
                Vector3 oldLocal = chassisT.InverseTransformPoint(lt.position);
                float side = oldLocal.x >= 0f ? 1f : -1f;
                Vector3 p;
                if (ln.StartsWith("headlight")) p = new Vector3(0.74f * side, 0.00f, 4.25f);
                else if (ln.StartsWith("brakelight")) p = new Vector3(0.84f * side, 0.75f, -4.64f);
                else if (ln.StartsWith("reverselight")) p = new Vector3(0.70f * side, 0.00f, -4.69f);
                else if (ln.StartsWith("indicatorlight")) p = oldLocal.z > 0f ? new Vector3(0.90f * side, 0.25f, 4.25f) : new Vector3(0.90f * side, 0.55f, -4.64f);
                else continue;
                Undo.RecordObject(lt, "Move light");
                lt.position = chassisT.TransformPoint(p * BusScale);
                movedLights++;
            }
        }
        log += "\nLights moved: " + movedLights;

        // 5) Steering wheel (cosmetic)
        Transform steer = inst.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name.ToLower() == "steering_wheel");
        if (steer != null)
        {
            Undo.RecordObject(car, "Assign steering wheel");
            car.SteeringWheel = steer;
        }

        // 6) Driver-view (hood) cameras -> driver's eye position of the new bus
        Transform camParent = inst.transform.parent;
        Vector3 eye = inst.transform.TransformPoint(new Vector3(-0.62f, 1.22f, 3.55f));
        int cams = 0;
        foreach (Transform c in camParent)
        {
            if (c.name.StartsWith("HoodCamera"))
            {
                Undo.RecordObject(c, "Move hood camera");
                c.position = eye;
                c.localRotation = Quaternion.Euler(3f, 0f, 0f);
                EditorUtility.SetDirty(c);
                cams++;
            }
        }
        log += "\nHood cameras moved: " + cams;

        EditorUtility.SetDirty(car);
        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[SwapBus] Done. Hidden old meshes: " + hidden + ". " + log);
        EditorUtility.DisplayDialog("Swap Bus", "Done.\nHidden old meshes: " + hidden + "\n" + log +
            "\n\nBackup: " + BackupPath + "\nNow press Play and test Level 1.", "OK");
    }

    // Renderer.bounds is empty when the object is inactive (Bus5 is inactive in the scene), so use the mesh.
    static Bounds WorldBounds(Renderer r)
    {
        MeshFilter mf = r.GetComponent<MeshFilter>();
        Mesh m = mf != null ? mf.sharedMesh : (r is SkinnedMeshRenderer ? ((SkinnedMeshRenderer)r).sharedMesh : null);
        if (m == null) return r.bounds;
        Bounds lb = m.bounds;
        Matrix4x4 mat = r.transform.localToWorldMatrix;
        Bounds wb = new Bounds(mat.MultiplyPoint3x4(lb.center), Vector3.zero);
        for (int i = 0; i < 8; i++)
        {
            Vector3 c = lb.center + Vector3.Scale(lb.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
            wb.Encapsulate(mat.MultiplyPoint3x4(c));
        }
        return wb;
    }

    static bool KeepVisible(Transform t, Transform root)
    {
        for (Transform p = t; p != null && p != root; p = p.parent)
        {
            string n = p.name.ToLower();
            if (n.Contains("passenger") || n.Contains("light") || n.Contains("mappoint") || n.Contains("minimap"))
                return true;
            if (p.GetComponent<RCC_Light>() != null) return true;
        }
        return false;
    }

    static void AssignWheel(RCC_CarControllerV3 car, RCC_WheelCollider wc, ref Transform field, Renderer wheel)
    {
        Undo.RecordObject(car, "Assign wheel");
        field = wheel.transform;
        if (wc == null) return;

        Undo.RecordObject(wc, "Assign wheel model");
        wc.wheelModel = wheel.transform;

        Bounds b = WorldBounds(wheel);
        Undo.RecordObject(wc.transform, "Move wheel collider");
        wc.transform.position = b.center;

        WheelCollider col = wc.GetComponent<WheelCollider>();
        if (col != null)
        {
            // RCC drops the wheel model below the collider by (1-compression)*suspensionDistance,
            // so lift the collider by half the travel to make the wheel rest at its real position.
            wc.transform.position = b.center + Vector3.up * (col.suspensionDistance * 0.5f);
            Undo.RecordObject(col, "Wheel radius");
            col.radius = Mathf.Max(0.2f, b.extents.y);
            EditorUtility.SetDirty(col);
        }
        EditorUtility.SetDirty(wc);
        EditorUtility.SetDirty(wc.transform);
    }
}
#endif
