#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Installs the new imported buses (Assets/NewBuses/...) into vehicle slots 1..5 of DayGameplay.
// Open Assets/Scenes/DayGameplay.unity (NOT in Play mode) and use Tools > Bus > Install ...
public static class BusInstaller
{
    class BusDef { public int slot; public string name; public string path; public float frontSign = 1f; public float camFwd = 0f; public bool keep = false; public int rearAxle = 1; }

    static readonly BusDef[] Defs =
    {
        new BusDef { slot = 1, name = "JetSet",  path = "Assets/NewBuses/JetSet/BUSJETSET.obj" },
        new BusDef { slot = 2, name = "Ecoline", path = "Assets/NewBuses/Ecoline/BUSECOLINE.obj" },
        new BusDef { slot = 3, name = "Laksana", path = "Assets/NewBuses/Laksana/LAKSANASR2.obj" },
        new BusDef { slot = 4, name = "Zedone",  path = "Assets/NewBuses/Zedone/Zedone.fbx" },
        new BusDef { slot = 5, name = "Irizar",  path = "Assets/NewBuses/Irizar/irizar_I6.fbx" },
        // textured models: keep their own materials (no random paint) - slots are created automatically
        new BusDef { slot = 6,  name = "ScaniaL94UB",   path = "Assets/NewBuses/ScaniaL94UB/ScaniaL94UB.fbx",     keep = true },
        new BusDef { slot = 7,  name = "ScaniaTouring", path = "Assets/NewBuses/ScaniaTouring/ScaniaTouring.fbx", keep = true, rearAxle = 2 },
        new BusDef { slot = 8,  name = "TownBus",       path = "Assets/NewBuses/TownBus/TownBus.fbx",             keep = true },
        new BusDef { slot = 9,  name = "CityBus",       path = "Assets/NewBuses/CityBus/CityBus.fbx",             keep = true },
        new BusDef { slot = 10, name = "TorinoBYD",     path = "Assets/NewBuses/TorinoBYD/TorinoBYD.fbx",         keep = true },
        new BusDef { slot = 11, name = "ManLions",      path = "Assets/NewBuses/ManLions/ManLions.fbx",           keep = true },
    };

    const string ScenePath = "Assets/Scenes/DayGameplay.unity";
    const string SelPath = "Assets/Scenes/VehicleSelection.unity";
    const string SelBackupPath = "Assets/Scenes/VehicleSelection_backup_before_newbus.unity";
    public static string GenDir = "Gen";
    static Transform VehiclesRoot;
    const string BackupPath = "Assets/Scenes/DayGameplay_backup_before_allbuses.unity";

    [MenuItem("Tools/Bus/Install Slot 1 (JetSet)")] static void I1() { Run(1); }
    [MenuItem("Tools/Bus/Install Slot 2 (Ecoline)")] static void I2() { Run(2); }
    [MenuItem("Tools/Bus/Install Slot 3 (Laksana)")] static void I3() { Run(3); }
    [MenuItem("Tools/Bus/Install Slot 4 (Zedone)")] static void I4() { Run(4); }
    [MenuItem("Tools/Bus/Install Slot 5 (Irizar)")] static void I5() { Run(5); }
    [MenuItem("Tools/Bus/Install ALL New Buses")] static void IAll() { Run(-1); }
    [MenuItem("Tools/Bus/Extract Textures (new buses)")]
    static void ExtractNewTex()
    {
        foreach (var d in Defs)
        {
            if (!d.keep) continue;
            var mi = AssetImporter.GetAtPath(d.path) as ModelImporter;
            if (mi == null) continue;
            string dir = "Assets/NewBuses/" + d.name + "/Tex";
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets/NewBuses/" + d.name, "Tex");
            try { mi.ExtractTextures(dir); } catch (System.Exception e) { Debug.LogWarning("[BusInstaller] extract " + d.name + ": " + e.Message); }
            AssetDatabase.ImportAsset(d.path, ImportAssetOptions.ForceUpdate);
            Debug.Log("[BusInstaller] textures extracted for " + d.name);
        }
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Bus/Install Slot 7 (Touring)")] static void I7() { Run(7); }
    [MenuItem("Tools/Bus/Add NEW Buses (slots 6-11)")] static void IAdd() { Run(-2); }

    [MenuItem("Tools/Bus/Test Next Vehicle (cycles 0-11)")]
    static void NextVehicle()
    {
        int v = (PlayerPrefs.GetInt("vehicle", 0) + 1) % 12;
        PlayerPrefs.SetInt("vehicle", v);
        PlayerPrefs.Save();
        Debug.Log("[BusInstaller] PlayerPrefs vehicle = " + v + " (press Play)");
    }

    static void Run(int slot)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) { Debug.LogError("[BusInstaller] Stop Play mode first."); return; }
        Scene scene = SceneManager.GetActiveScene();
        bool sel = scene.path == SelPath;
        if (scene.path != ScenePath && !sel) { Debug.LogError("[BusInstaller] Open DayGameplay.unity (or VehicleSelection.unity) first."); return; }
        GenDir = sel ? "GenSel" : "Gen";
        if (sel)
        {
            var ctrl = Object.FindObjectOfType<BusSimulator.VehicleSelectionController>(true);
            if (ctrl == null || ctrl.vehicle == null) { Debug.LogError("[BusInstaller] no VehicleSelectionController"); return; }
            VehiclesRoot = ctrl.vehicle.transform;
            if (!File.Exists(SelBackupPath)) { EditorSceneManager.SaveScene(scene); AssetDatabase.CopyAsset(SelPath, SelBackupPath); }
        }
        else
        {
            GameObject vg = GameObject.Find("Vehicles"); VehiclesRoot = vg != null ? vg.transform : null;
            if (!File.Exists(BackupPath)) { EditorSceneManager.SaveScene(scene); AssetDatabase.CopyAsset(ScenePath, BackupPath); }
        }
        var sb = new StringBuilder("[BusInstaller]\n");
        EnsureSlots(slot);
        foreach (var d in Defs)
        {
            if (slot == -2 ? d.slot < 6 : (slot != -1 && d.slot != slot)) continue;
            try { sb.AppendLine(Install(d, scene)); }
            catch (System.Exception e) { sb.AppendLine("ERROR " + d.name + ": " + e); }
        }
        if (sel) FixSelectionArrays(sb);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        System.IO.Directory.CreateDirectory("BusTest"); System.IO.File.WriteAllText(sel ? "BusTest/install_log_sel.txt" : "BusTest/install_log.txt", sb.ToString());
        Debug.Log(sb.ToString());
    }

    // ---------------------------------------------------------------- new slots
    // creates extra vehicle objects (copies of slot 1) so slots 6..11 exist in this scene; lock/unlock-all already loop over childCount
    static void EnsureSlots(int slot)
    {
        if (VehiclesRoot == null) return;
        int need = Defs.Where(d => slot == -1 || slot == -2 || d.slot == slot).Max(d => d.slot) + 1;
        while (VehiclesRoot.childCount < need && VehiclesRoot.childCount > 1)
        {
            GameObject c = Object.Instantiate(VehiclesRoot.GetChild(1).gameObject, VehiclesRoot);
            c.name = "Vehicle_" + (VehiclesRoot.childCount - 1);
            c.transform.SetAsLastSibling();
            c.SetActive(false);
        }
    }

    // price / stat-bar arrays of the selection screen must have one entry per vehicle
    static void FixSelectionArrays(StringBuilder sb)
    {
        var ctrl = Object.FindObjectOfType<BusSimulator.VehicleSelectionController>(true);
        if (ctrl == null) return;
        int n = VehiclesRoot.childCount;
        ctrl.vehicleprices = Grow(ctrl.vehicleprices, n, i => 50000 + 5000 * (i - 5));
        ctrl.SpeedBarValue = Grow(ctrl.SpeedBarValue, n, i => 0.55f + 0.06f * ((i - 6) % 6));
        ctrl.EngineBarValue = Grow(ctrl.EngineBarValue, n, i => 0.6f + 0.05f * ((i - 6) % 6));
        ctrl.SteerBarValue = Grow(ctrl.SteerBarValue, n, i => 0.7f + 0.04f * ((i - 6) % 6));
        ctrl.PowerBarValue = Grow(ctrl.PowerBarValue, n, i => 0.5f + 0.07f * ((i - 6) % 6));
        EditorUtility.SetDirty(ctrl);
        sb.AppendLine("selection arrays resized to " + n + " vehicles; prices: " + string.Join(",", ctrl.vehicleprices.Select(x => x.ToString()).ToArray()));
    }
    static T[] Grow<T>(T[] a, int n, System.Func<int, T> fill)
    {
        if (a == null) a = new T[0];
        if (a.Length >= n) return a;
        var r = new T[n]; System.Array.Copy(a, r, a.Length);
        for (int i = a.Length; i < n; i++) r[i] = fill(i);
        return r;
    }

    // ---------------------------------------------------------------- install
    static string Install(BusDef d, Scene scene)
    {
        var sb = new StringBuilder("== " + d.name + " (slot " + d.slot + ")");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(d.path);
        if (prefab == null) return sb + "\n  model not found " + d.path;
        if (VehiclesRoot == null || VehiclesRoot.childCount <= d.slot) return sb + "\n  slot not found";

        Transform bus = VehiclesRoot.GetChild(d.slot);
        bool wasActive = bus.gameObject.activeSelf;
        if (GenDir != "Gen") bus.gameObject.SetActive(true);
        try { return InstallInner(d, scene, sb, prefab, bus); }
        finally { if (GenDir != "Gen") bus.gameObject.SetActive(wasActive); }
    }

    static string InstallInner(BusDef d, Scene scene, StringBuilder sb, GameObject prefab, Transform bus)
    {
        RCC_CarControllerV3 car = bus.GetComponent<RCC_CarControllerV3>();
        if (car == null) car = bus.GetComponentInChildren<RCC_CarControllerV3>(true);
        if (car == null) return sb + "\n  no RCC on " + bus.name;
        Transform chassis = car.transform.Find("Chassis");
        if (chassis == null) chassis = car.transform;

        RCC_WheelCollider[] cols = { car.FrontLeftWheelCollider, car.FrontRightWheelCollider, car.RearLeftWheelCollider, car.RearRightWheelCollider };
        if (cols.Any(c => c == null)) return sb + "\n  RCC wheel colliders missing";

        // remove earlier installs
        foreach (Transform t in chassis.Cast<Transform>().Where(t => t.name.StartsWith("NewBus_")).ToList())
            Object.DestroyImmediate(t.gameObject);

        // reference = ORIGINAL collider positions (kept in empties so re-runs do not drift); scale.x = original radius
        string[] refNames = { "_Ref_FL", "_Ref_FR", "_Ref_RL", "_Ref_RR" };
        Vector3[] refLocal = new Vector3[4];
        float oldRadius = 0.5f;
        for (int i = 0; i < 4; i++)
        {
            Transform r = car.transform.Find(refNames[i]);
            if (r == null)
            {
                r = new GameObject(refNames[i]).transform;
                r.SetParent(car.transform, false);
                r.position = cols[i].transform.position;
                WheelCollider wcol = cols[i].GetComponent<WheelCollider>();
                float rad = wcol != null ? wcol.radius : 0.5f;
                r.localScale = new Vector3(rad, rad, rad);
            }
            refLocal[i] = chassis.InverseTransformPoint(r.position);
            if (i == 0) oldRadius = r.localScale.x;
        }
        WheelCollider fl = cols[0].GetComponent<WheelCollider>();
        float susp = fl != null ? fl.suspensionDistance : 0.2f;

        // --- instantiate at the origin and split the wheels out of the model
        GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        PrefabUtility.UnpackPrefabInstance(inst, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        inst.name = "NewBus_" + d.name;
        inst.transform.SetParent(null);
        inst.transform.position = Vector3.zero; inst.transform.rotation = Quaternion.identity; inst.transform.localScale = Vector3.one;

        Transform[] wheels = SplitWheels(inst, d, sb);   // FL FR RL RR or null
        if (wheels == null) { sb.AppendLine("\n  FAILED: could not find 4 wheels - model left unplaced (deleted)"); Object.DestroyImmediate(inst); return sb.ToString(); }

        if (d.frontSign < 0) inst.transform.rotation = Quaternion.Euler(0, 180, 0);

        // --- place under chassis so new wheel bottoms/axles line up with the old ones
        float newR = WorldBounds(wheels[0].GetComponent<Renderer>()).extents.y;
        Vector3 wcMean = Vector3.zero, refMean = Vector3.zero;
        for (int i = 0; i < 4; i++) { wcMean += wheels[i].position; refMean += refLocal[i]; }
        wcMean /= 4f; refMean /= 4f;
        float shiftY = (refMean.y - 0.5f * susp - oldRadius) - (wcMean.y - newR);
        inst.transform.SetParent(chassis, false);
        inst.transform.localRotation = d.frontSign < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        inst.transform.localScale = Vector3.one;
        Vector3 wcLocalMean = Vector3.zero;
        for (int i = 0; i < 4; i++) wcLocalMean += chassis.InverseTransformPoint(wheels[i].position);
        wcLocalMean /= 4f;
        inst.transform.localPosition = new Vector3(refMean.x - wcLocalMean.x, shiftY, refMean.z - wcLocalMean.z);

        foreach (Collider c in inst.GetComponentsInChildren<Collider>(true)) c.enabled = false;
        foreach (Rigidbody rb in inst.GetComponentsInChildren<Rigidbody>(true)) Object.DestroyImmediate(rb);

        // --- hide old visuals
        int hidden = 0;
        foreach (Renderer r in car.GetComponentsInChildren<Renderer>(true))
        {
            if (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer)) continue;
            if (r.transform.IsChildOf(inst.transform)) continue;
            if (KeepVisible(r.transform, car.transform)) continue;
            r.enabled = false; hidden++;
        }
        sb.AppendLine("\n  hidden old meshes: " + hidden);

        // --- wheels -> RCC
        AssignWheel(car, car.FrontLeftWheelCollider, ref car.FrontLeftWheelTransform, wheels[0], susp);
        AssignWheel(car, car.FrontRightWheelCollider, ref car.FrontRightWheelTransform, wheels[1], susp);
        AssignWheel(car, car.RearLeftWheelCollider, ref car.RearLeftWheelTransform, wheels[2], susp);
        AssignWheel(car, car.RearRightWheelCollider, ref car.RearRightWheelTransform, wheels[3], susp);
        car.ExtraRearWheelsTransform = new Transform[0];
        car.ExtraRearWheelsCollider = new RCC_WheelCollider[0];
        int off = 0;
        foreach (RCC_WheelCollider rc in car.GetComponentsInChildren<RCC_WheelCollider>(true))
            if (!cols.Contains(rc) && rc.gameObject.activeSelf) { rc.gameObject.SetActive(false); off++; }
        sb.AppendLine("  extra wheel colliders switched off: " + off);
        car.useDamage = false;

        // --- body bounds (chassis space), excluding wheels and far-away stray parts
        bool any = false; Bounds BB = new Bounds();
        float midX = 0f; for (int i = 0; i < 4; i++) midX += chassis.InverseTransformPoint(wheels[i].position).x; midX /= 4f;
        var stray = new List<string>();
        foreach (Renderer r in inst.GetComponentsInChildren<Renderer>(true))
        {
            if (r.transform.parent == inst.transform && r.name.StartsWith("wheel_")) continue;
            Mesh m = MeshOf(r); if (m == null) continue;
            if (Mathf.Abs(chassis.InverseTransformPoint(r.transform.TransformPoint(m.bounds.center)).x - midX) > 2.2f) { stray.Add(r.name); r.enabled = false; continue; }
            Bounds lb = m.bounds; Matrix4x4 mat = r.transform.localToWorldMatrix;
            for (int i = 0; i < 8; i++)
            {
                Vector3 c = lb.center + Vector3.Scale(lb.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                Vector3 p = chassis.InverseTransformPoint(mat.MultiplyPoint3x4(c));
                if (!any) { BB = new Bounds(p, Vector3.zero); any = true; } else BB.Encapsulate(p);
            }
        }
        if (stray.Count > 0) sb.AppendLine("  stray parts excluded from size: " + string.Join(", ", stray.ToArray()));
        float halfW = BB.extents.x;
        Vector3[] wl = wheels.Select(w => chassis.InverseTransformPoint(WorldBounds(w.GetComponent<Renderer>()).center)).ToArray();
        float groundY = wl[0].y - newR;
        float frontZ = (wl[0].z + wl[1].z) * 0.5f, rearZ = (wl[2].z + wl[3].z) * 0.5f;
        sb.AppendLine("  body local min=" + BB.min.ToString("F2") + " max=" + BB.max.ToString("F2") + " groundY=" + groundY.ToString("F2") + " frontAxleZ=" + frontZ.ToString("F2") + " rearAxleZ=" + rearZ.ToString("F2") + " wheelR=" + newR.ToString("F2"));

        // --- body collider
        BoxCollider bc = car.GetComponentsInChildren<BoxCollider>(true).FirstOrDefault(c => c.name == "Collider" && !c.transform.IsChildOf(inst.transform));
        if (bc == null) bc = car.GetComponentsInChildren<BoxCollider>(true).Where(c => !c.isTrigger && !c.transform.IsChildOf(inst.transform)).OrderByDescending(c => c.size.x * c.size.y * c.size.z).FirstOrDefault();
        if (bc != null && any)
        {
            Bounds nb = new Bounds(); bool a2 = false;
            for (int i = 0; i < 8; i++)
            {
                Vector3 c = BB.center + Vector3.Scale(BB.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                Vector3 p = bc.transform.InverseTransformPoint(chassis.TransformPoint(c));
                if (!a2) { nb = new Bounds(p, Vector3.zero); a2 = true; } else nb.Encapsulate(p);
            }
            float minY = bc.transform.InverseTransformPoint(chassis.TransformPoint(new Vector3(0, wl[0].y, 0))).y;
            Vector3 mn = nb.min, mx = nb.max; if (mn.y < minY) mn.y = minY;
            nb.SetMinMax(mn, mx);
            bc.center = nb.center; bc.size = nb.size;
            sb.AppendLine("  body collider size " + nb.size.ToString("F1"));
        }
        else sb.AppendLine("  WARNING: no BoxCollider named Collider found. Colliders: " + string.Join(", ", car.GetComponentsInChildren<Collider>(true).Where(c => !c.transform.IsChildOf(inst.transform)).Take(12).Select(c => c.name + ":" + c.GetType().Name).ToArray()));

        // --- COM
        if (car.COM != null)
            car.COM.position = chassis.TransformPoint(new Vector3(BB.center.x, wl[0].y - 0.15f, (frontZ + rearZ) * 0.5f));

        // --- lights (placed on the real front/rear surface of the model, measured from its vertices)
        int moved = 0;
        var pts = new List<Vector3>();
        foreach (Renderer r in inst.GetComponentsInChildren<Renderer>(true))
        {
            if (!r.enabled || (r.transform.parent == inst.transform && r.name.StartsWith("wheel_"))) continue;
            Mesh m = MeshOf(r); if (m == null) continue;
            Vector3[] mv = m.vertices; Matrix4x4 mat = chassis.worldToLocalMatrix * r.transform.localToWorldMatrix;
            int step = Mathf.Max(1, mv.Length / 40000);
            for (int i = 0; i < mv.Length; i += step)
            {
                Vector3 p = mat.MultiplyPoint3x4(mv[i]);
                if (p.y > groundY + 0.35f && p.y < groundY + 1.35f && Mathf.Abs(p.x - BB.center.x) < halfW + 0.2f) pts.Add(p);
            }
        }
        float fzMax = BB.max.z - 0.08f, rzMin = BB.min.z + 0.08f, fCx = BB.center.x, fHalf = halfW, rCx = BB.center.x, rHalf = halfW;
        if (pts.Count > 100)
        {
            var zs = pts.Select(p => p.z).OrderBy(z => z).ToArray();
            fzMax = zs[(int)(zs.Length * 0.995f)]; rzMin = zs[(int)(zs.Length * 0.005f)];
            var fx = pts.Where(p => p.z > fzMax - 0.5f).Select(p => p.x).OrderBy(x => x).ToArray();
            var rx = pts.Where(p => p.z < rzMin + 0.5f).Select(p => p.x).OrderBy(x => x).ToArray();
            if (fx.Length > 10) { float a = fx[(int)(fx.Length * 0.03f)], b = fx[(int)(fx.Length * 0.97f)]; fCx = (a + b) * 0.5f; fHalf = (b - a) * 0.5f; }
            if (rx.Length > 10) { float a = rx[(int)(rx.Length * 0.03f)], b = rx[(int)(rx.Length * 0.97f)]; rCx = (a + b) * 0.5f; rHalf = (b - a) * 0.5f; }
        }
        sb.AppendLine("  light surfaces: frontZ=" + fzMax.ToString("F2") + " halfW=" + fHalf.ToString("F2") + " rearZ=" + rzMin.ToString("F2") + " halfW=" + rHalf.ToString("F2") + " samples=" + pts.Count);
        foreach (RCC_Light l in car.GetComponentsInChildren<RCC_Light>(true))
        {
            string type = LightType(l).ToLower();
            Vector3 oldL = chassis.InverseTransformPoint(l.transform.position);
            float side = oldL.x >= 0f ? 1f : -1f;
            float zF = fzMax - 0.12f, zR = rzMin + 0.12f;
            Vector3 p;
            if (type.Contains("head") || type.Contains("high")) p = new Vector3(fCx + 0.58f * fHalf * side, groundY + 0.8f, zF);
            else if (type.Contains("brake")) p = new Vector3(rCx + 0.7f * rHalf * side, groundY + 0.95f, zR);
            else if (type.Contains("reverse")) p = new Vector3(rCx + 0.5f * rHalf * side, groundY + 0.75f, zR);
            else if (type.Contains("indicator") || type.Contains("signal")) { bool fr = oldL.z > BB.center.z; p = new Vector3((fr ? fCx : rCx) + 0.82f * (fr ? fHalf : rHalf) * side, groundY + (fr ? 0.7f : 1.05f), fr ? zF : zR); }
            else continue;
            {   // use the body width at this light's own height, so it never sits outside the bumper
                bool frL = p.z > BB.center.z; float zRef = frL ? fzMax : rzMin; float ty = p.y;
                var lx = pts.Where(q => Mathf.Abs(q.y - ty) < 0.2f && Mathf.Abs(q.z - zRef) < 0.6f).Select(q => q.x).OrderBy(x => x).ToArray();
                if (lx.Length > 10)
                {
                    float a = lx[(int)(lx.Length * 0.05f)], b = lx[(int)(lx.Length * 0.95f)];
                    float cx = (a + b) * 0.5f, hw = (b - a) * 0.5f;
                    float f = (type.Contains("indicator") || type.Contains("signal")) ? 0.72f : (type.Contains("head") || type.Contains("high")) ? 0.55f : type.Contains("brake") ? 0.62f : 0.42f;
                    p.x = cx + f * hw * side;
                }
            }
            l.transform.position = chassis.TransformPoint(p);
            moved++;
        }
        sb.AppendLine("  lights moved: " + moved);
        if (!d.keep) BusColorizer.Apply(inst, car, d.name, chassis, groundY, sb);
        else
        {
            foreach (BusRandomColor rc in car.GetComponentsInChildren<BusRandomColor>(true)) Object.DestroyImmediate(rc);
            sb.AppendLine("  keeping the model's own textures/materials");
        }

        // --- driver camera(s)
        Vector3 eye = new Vector3(BB.center.x - 0.5f * halfW, groundY + 2.1f, Mathf.Min(frontZ + 1.2f, BB.max.z - 1.0f) + d.camFwd);
        int cams = 0;
        foreach (Transform c in chassis)
            if (c.name.StartsWith("HoodCamera")) { c.position = chassis.TransformPoint(eye); c.localRotation = Quaternion.Euler(3f, 0f, 0f); cams++; }
        sb.AppendLine("  hood cameras: " + cams + " eye(chassis)=" + eye.ToString("F2"));

        Transform steer = inst.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name.ToLower().Contains("steering") || t.name.ToLower().Contains("setir"));
        if (steer != null) { car.SteeringWheel = steer; sb.AppendLine("  steering wheel: " + steer.name); }

        EditorUtility.SetDirty(car);
        PlayerPrefs.SetInt("vehicle", d.slot);
        PlayerPrefs.Save();
        return sb.ToString();
    }

    // ---------------------------------------------------------------- wheel splitting
    class MI
    {
        public MeshFilter mf; public Mesh mesh; public Vector3[] v; public Matrix4x4 toRoot;
        public int[] w; public int[] uf; public int[][] tris;
        public Dictionary<int, Bounds> cb = new Dictionary<int, Bounds>();
    }
    class Grp { public int side; public Bounds b; public bool init; public float z { get { return b.center.z; } } }
    class WB
    {
        public List<Vector3> v = new List<Vector3>(), n = new List<Vector3>(); public List<Vector2> uv = new List<Vector2>();
        public List<Material> mats = new List<Material>(); public List<List<int>> sub = new List<List<int>>();
    }

    static int Find(int[] uf, int x) { while (uf[x] != x) { uf[x] = uf[uf[x]]; x = uf[x]; } return x; }
    static void Union(int[] uf, int a, int b) { a = Find(uf, a); b = Find(uf, b); if (a != b) uf[a] = b; }

    static Transform[] SplitWheels(GameObject inst, BusDef d, StringBuilder log)
    {
        var infos = new List<MI>();
        Bounds all = new Bounds(); bool anyB = false;
        foreach (MeshFilter mf in inst.GetComponentsInChildren<MeshFilter>(true))
        {
            Mesh mesh = mf.sharedMesh; if (mesh == null || mf.GetComponent<MeshRenderer>() == null) continue;
            var mi = new MI { mf = mf, mesh = mesh, v = mesh.vertices };
            mi.toRoot = inst.transform.worldToLocalMatrix * mf.transform.localToWorldMatrix;
            var map = new Dictionary<Vector3Int, int>(); mi.w = new int[mi.v.Length];
            for (int i = 0; i < mi.v.Length; i++)
            {
                var key = new Vector3Int(Mathf.RoundToInt(mi.v[i].x * 1000f), Mathf.RoundToInt(mi.v[i].y * 1000f), Mathf.RoundToInt(mi.v[i].z * 1000f));
                int id; if (!map.TryGetValue(key, out id)) { id = map.Count; map[key] = id; }
                mi.w[i] = id;
            }
            mi.uf = Enumerable.Range(0, map.Count).ToArray();
            mi.tris = new int[mesh.subMeshCount][];
            for (int s = 0; s < mesh.subMeshCount; s++)
            {
                int[] t = mesh.GetTriangles(s); mi.tris[s] = t;
                for (int k = 0; k + 2 < t.Length; k += 3) { Union(mi.uf, mi.w[t[k]], mi.w[t[k + 1]]); Union(mi.uf, mi.w[t[k]], mi.w[t[k + 2]]); }
            }
            for (int s = 0; s < mi.tris.Length; s++)
            {
                int[] t = mi.tris[s];
                for (int k = 0; k + 2 < t.Length; k += 3)
                {
                    int c = Find(mi.uf, mi.w[t[k]]);
                    for (int q = 0; q < 3; q++)
                    {
                        Vector3 p = mi.toRoot.MultiplyPoint3x4(mi.v[t[k + q]]);
                        Bounds b;
                        if (!mi.cb.TryGetValue(c, out b)) mi.cb[c] = new Bounds(p, Vector3.zero); else { b.Encapsulate(p); mi.cb[c] = b; }
                        if (!anyB) { all = new Bounds(p, Vector3.zero); anyB = true; } else all.Encapsulate(p);
                    }
                }
            }
            infos.Add(mi);
        }
        if (!anyB) return null;
        float cx = all.center.x, minY = all.min.y;
        log.AppendLine("\n  model bounds min=" + all.min.ToString("F2") + " max=" + all.max.ToString("F2") + " meshes=" + infos.Count);

        // seeds: tyre-like components (round in the YZ plane, thin in X, near the ground)
        var groups = new List<Grp>(); var seedKeys = new List<KeyValuePair<long, Grp>>();
        var seeds = new List<KeyValuePair<long, Bounds>>();
        for (int ii = 0; ii < infos.Count; ii++)
            foreach (var kv in infos[ii].cb)
            {
                Vector3 sz = kv.Value.size;
                if (sz.y > 0.6f && sz.y < 1.5f && sz.z > 0.6f && sz.z < 1.5f && Mathf.Abs(sz.y - sz.z) < 0.2f * Mathf.Max(sz.y, sz.z) && sz.x > 0.02f && sz.x < 0.7f && kv.Value.min.y < minY + 0.7f)
                    seeds.Add(new KeyValuePair<long, Bounds>(((long)ii << 32) | (uint)kv.Key, kv.Value));
            }
        if (seeds.Count > 0) cx = seeds.Average(q => q.Value.center.x);
        foreach (var s in seeds.OrderByDescending(s => s.Value.center.z))
        {
            int side = s.Value.center.x >= cx ? 1 : -1;
            Grp g = groups.FirstOrDefault(q => q.side == side && Mathf.Abs(q.z - s.Value.center.z) < 0.5f);
            if (g == null) { g = new Grp { side = side, b = s.Value, init = true }; groups.Add(g); }
            else g.b.Encapsulate(s.Value.min); g.b.Encapsulate(s.Value.max);
            seedKeys.Add(new KeyValuePair<long, Grp>(s.Key, g));
        }
        groups.RemoveAll(g => g.b.min.y > minY + 0.15f || g.b.size.y < 0.6f);
        log.AppendLine("  tyre pieces=" + seeds.Count + " wheel groups=" + groups.Count + ": " + string.Join(" | ", groups.Select(g => "side" + g.side + " z=" + g.z.ToString("F2") + " c=" + g.b.center.ToString("F2") + " size=" + g.b.size.ToString("F2")).ToArray()));

        // axles = groups with the same z on both sides
        float fs = d.frontSign;
        var axles = new List<List<Grp>>();
        foreach (Grp g in groups.OrderByDescending(g => g.z * fs))
        {
            var ax = axles.FirstOrDefault(a => Mathf.Abs(a[0].z - g.z) < 0.6f);
            if (ax == null) axles.Add(new List<Grp> { g }); else ax.Add(g);
        }
        axles = axles.Where(a => a.Any(g => g.side == 1) && a.Any(g => g.side == -1)).ToList();
        if (axles.Count < 2) return null;
        Grp[] chosen = new Grp[4];
        Grp Left(List<Grp> a) { return a.Where(g => g.side * fs < 0).OrderBy(g => g.b.center.x * fs).First(); }
        Grp Right(List<Grp> a) { return a.Where(g => g.side * fs > 0).OrderByDescending(g => g.b.center.x * fs).First(); }
        if (!axles[0].Any(g => g.side * fs < 0) || !axles[0].Any(g => g.side * fs > 0) || !axles[1].Any(g => g.side * fs < 0) || !axles[1].Any(g => g.side * fs > 0)) return null;
        var rax = axles[Mathf.Clamp(d.rearAxle, 1, axles.Count - 1)];
        chosen[0] = Left(axles[0]); chosen[1] = Right(axles[0]); chosen[2] = Left(rax); chosen[3] = Right(rax);

        // every component fully inside a chosen wheel's bounds belongs to that wheel (rims, hubs, nuts)
        var assign = new Dictionary<long, int>();
        for (int wi = 0; wi < 4; wi++)
        {
            Bounds E = chosen[wi].b; E.Expand(0.06f);
            for (int ii = 0; ii < infos.Count; ii++)
                foreach (var kv in infos[ii].cb)
                {
                    long key = ((long)ii << 32) | (uint)kv.Key;
                    if (assign.ContainsKey(key)) continue;
                    if (E.Contains(kv.Value.min) && E.Contains(kv.Value.max)) assign[key] = wi;
                }
        }
        log.AppendLine("  pieces moved into wheels: " + assign.Count);

        string gen = "Assets/NewBuses/" + d.name + "/" + GenDir;
        if (!AssetDatabase.IsValidFolder(gen)) AssetDatabase.CreateFolder("Assets/NewBuses/" + d.name, GenDir);

        var builders = new WB[4]; var maps = new Dictionary<int, int>[4];
        for (int i = 0; i < 4; i++) builders[i] = new WB();
        Vector3[] pivot = chosen.Select(g => g.b.center).ToArray();
        string[] wn = { "FL", "FR", "RL", "RR" };

        for (int ii = 0; ii < infos.Count; ii++)
        {
            MI mi = infos[ii];
            Material[] mats = mi.mf.GetComponent<MeshRenderer>().sharedMaterials;
            Vector3[] nor = mi.mesh.normals; Vector2[] uv = mi.mesh.uv;
            var rem = new List<int>[mi.tris.Length]; for (int s = 0; s < rem.Length; s++) rem[s] = new List<int>();
            for (int i = 0; i < 4; i++) maps[i] = new Dictionary<int, int>();
            bool removed = false;
            for (int s = 0; s < mi.tris.Length; s++)
            {
                int[] t = mi.tris[s];
                for (int k = 0; k + 2 < t.Length; k += 3)
                {
                    int c = Find(mi.uf, mi.w[t[k]]); int wi;
                    if (!assign.TryGetValue(((long)ii << 32) | (uint)c, out wi)) { rem[s].Add(t[k]); rem[s].Add(t[k + 1]); rem[s].Add(t[k + 2]); continue; }
                    removed = true;
                    WB b = builders[wi];
                    Material m = s < mats.Length ? mats[s] : null;
                    int mi2 = b.mats.IndexOf(m); if (mi2 < 0) { b.mats.Add(m); b.sub.Add(new List<int>()); mi2 = b.mats.Count - 1; }
                    for (int q = 0; q < 3; q++)
                    {
                        int old = t[k + q]; int nv;
                        if (!maps[wi].TryGetValue(old, out nv))
                        {
                            nv = b.v.Count; maps[wi][old] = nv;
                            b.v.Add(mi.toRoot.MultiplyPoint3x4(mi.v[old]) - pivot[wi]);
                            b.n.Add(nor.Length > old ? mi.toRoot.MultiplyVector(nor[old]).normalized : Vector3.up);
                            b.uv.Add(uv.Length > old ? uv[old] : Vector2.zero);
                        }
                        b.sub[mi2].Add(nv);
                    }
                }
            }
            if (!removed) continue;
            // body mesh without the wheel triangles
            var nm = new Mesh { name = mi.mesh.name + "_nowheels" };
            if (mi.v.Length > 65535) nm.indexFormat = IndexFormat.UInt32;
            nm.vertices = mi.v; if (nor.Length == mi.v.Length) nm.normals = nor; if (uv.Length == mi.v.Length) nm.uv = uv;
            Vector4[] tan = mi.mesh.tangents; if (tan.Length == mi.v.Length) nm.tangents = tan;
            Color[] col = mi.mesh.colors; if (col.Length == mi.v.Length) nm.colors = col;
            nm.subMeshCount = mi.tris.Length;
            for (int s = 0; s < rem.Length; s++) nm.SetTriangles(rem[s], s);
            nm.RecalculateBounds();
            string p = gen + "/" + d.name + "_body" + ii + ".asset";
            AssetDatabase.DeleteAsset(p); AssetDatabase.CreateAsset(nm, p);
            mi.mf.sharedMesh = nm;
        }

        var result = new Transform[4];
        for (int wi = 0; wi < 4; wi++)
        {
            WB b = builders[wi];
            if (b.v.Count == 0) return null;
            var go = new GameObject("wheel_" + wn[wi]);
            go.transform.SetParent(inst.transform, false);
            go.transform.localPosition = pivot[wi];
            var wm = new Mesh { name = d.name + "_wheel_" + wn[wi] };
            if (b.v.Count > 65535) wm.indexFormat = IndexFormat.UInt32;
            wm.SetVertices(b.v); wm.SetNormals(b.n); wm.SetUVs(0, b.uv);
            wm.subMeshCount = b.sub.Count;
            for (int s = 0; s < b.sub.Count; s++) wm.SetTriangles(b.sub[s], s);
            wm.RecalculateBounds();
            string p = gen + "/" + d.name + "_wheel_" + wn[wi] + ".asset";
            AssetDatabase.DeleteAsset(p); AssetDatabase.CreateAsset(wm, p);
            go.AddComponent<MeshFilter>().sharedMesh = wm;
            go.AddComponent<MeshRenderer>().sharedMaterials = b.mats.ToArray();
            result[wi] = go.transform;
            log.AppendLine("  wheel " + wn[wi] + " verts=" + b.v.Count + " at " + pivot[wi].ToString("F2"));
        }
        return result;
    }

    // ---------------------------------------------------------------- helpers
    static string LightType(RCC_Light l)
    {
        var so = new SerializedObject(l);
        var p = so.FindProperty("lightType");
        if (p != null && p.propertyType == SerializedPropertyType.Enum && p.enumValueIndex >= 0 && p.enumValueIndex < p.enumNames.Length)
            return p.enumNames[p.enumValueIndex] + " " + l.name;
        return l.name;
    }

    static Mesh MeshOf(Renderer r)
    {
        MeshFilter mf = r.GetComponent<MeshFilter>();
        if (mf != null) return mf.sharedMesh;
        SkinnedMeshRenderer s = r as SkinnedMeshRenderer;
        return s != null ? s.sharedMesh : null;
    }

    static Bounds WorldBounds(Renderer r)
    {
        Mesh m = MeshOf(r);
        if (m == null) return r.bounds;
        Bounds lb = m.bounds; Matrix4x4 mat = r.transform.localToWorldMatrix;
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
            if (n.Contains("passenger") || n.Contains("mappoint") || n.Contains("minimap")) return true;
            if (p.GetComponent<RCC_Light>() != null) return true;
        }
        return false;
    }

    static void AssignWheel(RCC_CarControllerV3 car, RCC_WheelCollider wc, ref Transform field, Transform wheel, float susp)
    {
        field = wheel;
        wc.wheelModel = wheel;
        Bounds b = WorldBounds(wheel.GetComponent<Renderer>());
        WheelCollider col = wc.GetComponent<WheelCollider>();
        wc.transform.position = b.center + Vector3.up * (col != null ? col.suspensionDistance * 0.5f : susp * 0.5f);
        if (col != null) { col.radius = Mathf.Max(0.2f, b.extents.y); EditorUtility.SetDirty(col); }
        EditorUtility.SetDirty(wc); EditorUtility.SetDirty(wc.transform);
    }
}
#endif
