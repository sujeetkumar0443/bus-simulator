using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BusSelect
{
    const string SelScene = "Assets/Scenes/VehicleSelection.unity";

    static string Path(Transform t, Transform root)
    {
        string s = t.name;
        for (var p = t.parent; p != null && p != root; p = p.parent) s = p.name + "/" + s;
        return s;
    }

    [MenuItem("Tools/Bus/Dump Selection Scene")]
    public static void Dump()
    {
        var sb = new StringBuilder();
        var scene = EditorSceneManager.OpenScene(SelScene, OpenSceneMode.Single);
        var ctrl = Object.FindObjectOfType<BusSimulator.VehicleSelectionController>(true);
        if (ctrl == null) { sb.AppendLine("no controller"); System.IO.File.WriteAllText("BusTest/selection.txt", sb.ToString()); return; }
        Transform v = ctrl.vehicle.transform;
        sb.AppendLine("vehicle root: " + Path(v, null) + " pos=" + v.position + " rot=" + v.eulerAngles + " scale=" + v.localScale + " children=" + v.childCount);
        for (int i = 0; i < v.childCount; i++)
        {
            Transform c = v.GetChild(i);
            sb.AppendLine("[" + i + "] " + c.name + " active=" + c.gameObject.activeSelf + " lpos=" + c.localPosition + " lrot=" + c.localEulerAngles + " lscale=" + c.localScale + " children=" + c.childCount + " comps=" + string.Join(",", c.GetComponents<Component>().Select(x => x == null ? "null" : x.GetType().Name)));
            var rs = c.GetComponentsInChildren<Renderer>(true);
            if (rs.Length > 0)
            {
                Bounds b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds);
                sb.AppendLine("     renderers=" + rs.Length + " world bounds center=" + b.center + " size=" + b.size);
            }
            foreach (Transform k in c) sb.AppendLine("     - " + k.name + " active=" + k.gameObject.activeSelf + " lpos=" + k.localPosition + " lrot=" + k.localEulerAngles + " lscale=" + k.localScale + " kids=" + k.childCount);
        }
        sb.AppendLine("prices: " + string.Join(",", ctrl.vehicleprices));
        sb.AppendLine("all scene roots:");
        foreach (var r in scene.GetRootGameObjects()) sb.AppendLine("  " + r.name + " active=" + r.activeSelf);
        System.IO.File.WriteAllText("BusTest/selection.txt", sb.ToString());
        Debug.Log("[Select] dump written");
    }
    [MenuItem("Tools/Bus/Scale Selection Buses 2.4")]
    public static void ScaleSel()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.path != SelScene) { Debug.LogError("[Select] open VehicleSelection first"); return; }
        var ctrl = Object.FindObjectOfType<BusSimulator.VehicleSelectionController>(true);
        for (int i = 1; i < ctrl.vehicle.transform.childCount; i++) ctrl.vehicle.transform.GetChild(i).localScale = Vector3.one * 2.4f;
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        Debug.Log("[Select] scaled to 2.4 and saved");
    }
    [MenuItem("Tools/Bus/Scale Selection Bus0 4.4")]
    public static void ScaleSel0()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.path != SelScene) { Debug.LogError("[Select] open VehicleSelection first"); return; }
        var ctrl = Object.FindObjectOfType<BusSimulator.VehicleSelectionController>(true);
        ctrl.vehicle.transform.GetChild(0).localScale = Vector3.one * 4.4f;
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        Debug.Log("[Select] bus0 scaled to 4.4 and saved");
    }
    [MenuItem("Tools/Bus/Dump Selection Layout")]
    public static void DumpLayout()
    {
        var sb = new StringBuilder();
        var scene = EditorSceneManager.OpenScene(SelScene, OpenSceneMode.Single);
        foreach (var cam in Object.FindObjectsOfType<Camera>(true))
            sb.AppendLine("CAM " + cam.name + " active=" + cam.gameObject.activeInHierarchy + " pos=" + cam.transform.position + " rot=" + cam.transform.eulerAngles + " fov=" + cam.fieldOfView + " ortho=" + cam.orthographic + " size=" + cam.orthographicSize + " depth=" + cam.depth + " clear=" + cam.clearFlags + " mask=" + cam.cullingMask + " parent=" + (cam.transform.parent ? cam.transform.parent.name : "-"));
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name.Contains("garage") || root.name == "BusShadow" || root.name.Contains("Others"))
            {
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                {
                    var r = t.GetComponent<Renderer>();
                    sb.AppendLine("  " + Path(t, null) + " act=" + t.gameObject.activeSelf + " pos=" + t.position + " rot=" + t.eulerAngles + " scale=" + t.lossyScale + (r != null ? " RENDER bounds c=" + r.bounds.center + " s=" + r.bounds.size : "") + " comps=" + string.Join(",", t.GetComponents<Component>().Select(c => c == null ? "null" : c.GetType().Name)));
                }
            }
        }
        var ctrl = Object.FindObjectOfType<BusSimulator.VehicleSelectionController>(true);
        Transform v = ctrl.vehicle.transform;
        for (int i = 0; i < v.childCount; i++)
        {
            Transform c = v.GetChild(i);
            bool was = c.gameObject.activeSelf; c.gameObject.SetActive(true);
            var rs = c.GetComponentsInChildren<Renderer>(false).Where(r => r.enabled && (r is MeshRenderer || r is SkinnedMeshRenderer) && !(r is SpriteRenderer) && r.GetComponent<ParticleSystem>() == null).ToArray();
            Bounds b = new Bounds(); bool any = false;
            foreach (var r in rs) { if (r.name.ToLower().Contains("shadow") || r.name.ToLower().Contains("mappoint")) continue; if (!any) { b = r.bounds; any = true; } else b.Encapsulate(r.bounds); }
            sb.AppendLine("BUS[" + i + "] " + c.name + " pos=" + c.position + " rot=" + c.eulerAngles + " scale=" + c.localScale + " renderers=" + rs.Length + " bounds center=" + b.center + " size=" + b.size + " min=" + b.min + " max=" + b.max);
            c.gameObject.SetActive(was);
        }
        System.IO.File.WriteAllText("BusTest/layout.txt", sb.ToString());
        Debug.Log("[Select] layout written");
    }

    static Camera SelCam()
    {
        return Object.FindObjectsOfType<Camera>(true).First(c => c.gameObject.activeInHierarchy && c.GetComponent("OrbitCamera") != null);
    }

    [MenuItem("Tools/Bus/Record Play Camera")]
    public static void RecPlayCam()
    {
        Camera cam = SelCam(); var t = cam.transform;
        var ci = System.Globalization.CultureInfo.InvariantCulture;
        System.IO.File.WriteAllText("BusTest/playcam.txt", string.Join(" ", new[] { t.position.x, t.position.y, t.position.z, t.rotation.x, t.rotation.y, t.rotation.z, t.rotation.w, cam.fieldOfView }.Select(f => f.ToString("R", ci)).ToArray()));
        Debug.Log("[Select] play cam recorded " + t.position + " " + t.eulerAngles + " fov " + cam.fieldOfView);
    }

    [MenuItem("Tools/Bus/Fit Selection Buses")]
    public static void FitAll()
    {
        EditorSceneManager.OpenScene(SelScene, OpenSceneMode.Single);
        var ctrl = Object.FindObjectOfType<BusSimulator.VehicleSelectionController>(true);
        Camera cam = SelCam();
        Vector3 oPos = cam.transform.position; Quaternion oRot = cam.transform.rotation; float oFov = cam.fieldOfView;
        if (System.IO.File.Exists("BusTest/playcam.txt"))
        {
            var ci = System.Globalization.CultureInfo.InvariantCulture;
            var f = System.IO.File.ReadAllText("BusTest/playcam.txt").Split(' ').Select(x => float.Parse(x, ci)).ToArray();
            cam.transform.position = new Vector3(f[0], f[1], f[2]); cam.transform.rotation = new Quaternion(f[3], f[4], f[5], f[6]); cam.fieldOfView = f[7];
        }
        var shadow = GameObject.Find("BusShadow");
        Vector3 C = shadow.transform.position;
        float floorY = C.y;
        cam.aspect = 2f;
        Vector3 fwd = cam.transform.forward; fwd.y = 0; fwd.Normalize();
        Vector3 right = cam.transform.right; right.y = 0; right.Normalize();
        var sb = new StringBuilder();
        sb.AppendLine("cam=" + cam.transform.position + " rot=" + cam.transform.eulerAngles + " fov=" + cam.fieldOfView);
        Transform v = ctrl.vehicle.transform;
        for (int i = 0; i < v.childCount; i++)
        {
            Transform c = v.GetChild(i);
            bool was = c.gameObject.activeSelf; c.gameObject.SetActive(true);
            float lo = 0.3f, hi = 8f;
            for (int it = 0; it < 16; it++)
            {
                float s = (lo + hi) * 0.5f;
                Rect r = PlaceView(c, s, C, floorY, cam, fwd, right);
                bool fits = r.yMax <= (i == 1 || i == 2 ? 0.62f : TopMax) && r.xMin >= 0.17f && r.xMax <= 0.83f;
                if (fits) lo = s; else hi = s;
            }
            Rect rr = PlaceView(c, lo, C, floorY, cam, fwd, right);
            sb.AppendLine("BUS[" + i + "] " + c.name + " scale=" + lo + " pos=" + c.position + " view x=" + rr.xMin + ".." + rr.xMax + " y=" + rr.yMin + ".." + rr.yMax);
            c.gameObject.SetActive(was);
        }
        cam.ResetAspect();
        cam.transform.position = oPos; cam.transform.rotation = oRot; cam.fieldOfView = oFov;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        System.IO.File.WriteAllText("BusTest/fit.txt", sb.ToString());
        Debug.Log("[Select] fit done");
    }

    const float TopMax = 0.76f, BottomTarget = 0.24f;

    static Rect PlaceView(Transform c, float s, Vector3 C, float floorY, Camera cam, Vector3 fwd, Vector3 right)
    {
        Place(c, s, C, floorY);
        Vector3 basePos = c.position;
        var pts = Pts(c);
        float fl = -30f, fh = 30f, lat = 0, m = 0;
        Rect r = ProjP(pts, Vector3.zero, cam);
        for (int outer = 0; outer < 3; outer++)
        {
            fl = -30f; fh = 30f;
            for (int k = 0; k < 14; k++)
            {
                m = (fl + fh) * 0.5f;
                r = ProjP(pts, fwd * m + right * lat, cam);
                if (r.yMin > BottomTarget) fh = m; else fl = m;
            }
            Vector3 pc = basePos + fwd * m + right * lat;
            float d = Vector3.Dot(pc - cam.transform.position, cam.transform.forward);
            float W = 2f * d * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2f;
            lat += (0.5f - r.center.x) * W;
        }
        m = (fl + fh) * 0.5f;
        c.position = basePos + fwd * m + right * lat;
        return ProjP(pts, fwd * m + right * lat, cam);
    }

    static Rect ProjP(System.Collections.Generic.List<Vector3> pts, Vector3 off, Camera cam)
    {
        var xs = new float[pts.Count]; var ys = new float[pts.Count];
        for (int i = 0; i < pts.Count; i++) { Vector3 q = cam.WorldToViewportPoint(pts[i] + off); xs[i] = q.x; ys[i] = q.y; }
        System.Array.Sort(xs); System.Array.Sort(ys);
        int lo = (int)(0), hi = xs.Length - 1;
        return Rect.MinMaxRect(xs[lo], ys[lo], xs[hi], ys[hi]);
    }

    static Bounds B(Transform c)
    {
        Bounds b = new Bounds(); bool any = false;
        foreach (var p in Pts(c)) { if (!any) { b = new Bounds(p, Vector3.zero); any = true; } else b.Encapsulate(p); }
        return b;
    }

    static System.Collections.Generic.List<Vector3> Pts(Transform c)
    {
        var l = new System.Collections.Generic.List<Vector3>();
        foreach (var r in c.GetComponentsInChildren<Renderer>(false))
        {
            if (!r.enabled || !(r is MeshRenderer || r is SkinnedMeshRenderer)) continue;
            string n = r.name.ToLower(); if (n.Contains("shadow") || n.Contains("mappoint") || n.Contains("minimap")) continue;
            var mf = r.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null && mf.sharedMesh.isReadable)
            {
                var vs = mf.sharedMesh.vertices; int st = Mathf.Max(1, vs.Length / 3000);
                for (int i = 0; i < vs.Length; i += st) l.Add(r.transform.TransformPoint(vs[i]));
            }
            else if (mf != null && mf.sharedMesh != null)
            {
                Bounds bb = mf.sharedMesh.bounds;
                for (int k = 0; k < 8; k++) l.Add(r.transform.TransformPoint(bb.center + Vector3.Scale(bb.extents, new Vector3((k & 1) == 0 ? -1 : 1, (k & 2) == 0 ? -1 : 1, (k & 4) == 0 ? -1 : 1))));
            }
            else
            {
                Bounds bb = r.bounds;
                for (int k = 0; k < 8; k++) l.Add(bb.center + Vector3.Scale(bb.extents, new Vector3((k & 1) == 0 ? -1 : 1, (k & 2) == 0 ? -1 : 1, (k & 4) == 0 ? -1 : 1)));
            }
        }
        return l;
    }

    static void Place(Transform c, float s, Vector3 C, float floorY)
    {
        c.localScale = new Vector3(s, s, s);
        Bounds b = B(c);
        Vector3 d = new Vector3(C.x - b.center.x, floorY + 0.05f - b.min.y, C.z - b.center.z);
        c.position += d;
    }

    static Rect Proj(Transform c, Camera cam)
    {
        var xs = new System.Collections.Generic.List<float>(); var ys = new System.Collections.Generic.List<float>();
        foreach (var p in Pts(c)) { Vector3 q = cam.WorldToViewportPoint(p); xs.Add(q.x); ys.Add(q.y); }
        xs.Sort(); ys.Sort();
        int lo = (int)(xs.Count * 0.02f), hi = Mathf.Min(xs.Count - 1, (int)(xs.Count * 0.98f));
        return Rect.MinMaxRect(xs[lo], ys[lo], xs[hi], ys[hi]);
    }

    [MenuItem("Tools/Bus/Report Active View")]
    public static void ReportView()
    {
        Camera cam = SelCam();
        var ctrl = Object.FindObjectOfType<BusSimulator.VehicleSelectionController>(true);
        var sb = new StringBuilder();
        sb.AppendLine("cam=" + cam.transform.position + " rot=" + cam.transform.rotation.eulerAngles + " fov=" + cam.fieldOfView + " aspect=" + cam.aspect + " pixel=" + cam.pixelWidth + "x" + cam.pixelHeight);
        Transform v = ctrl.vehicle.transform;
        for (int i = 0; i < v.childCount; i++)
        {
            Transform c = v.GetChild(i); if (!c.gameObject.activeSelf) continue;
            var pts = Pts(c);
            Rect r = ProjP(pts, Vector3.zero, cam);
            foreach (var rr in c.GetComponentsInChildren<Renderer>(false)) { var mf = rr.GetComponent<MeshFilter>(); sb.AppendLine("   R " + rr.name + " " + rr.GetType().Name + " en=" + rr.enabled + " mesh=" + (mf != null && mf.sharedMesh != null ? mf.sharedMesh.vertexCount + (mf.sharedMesh.isReadable ? "r" : "nr") : "none") + " bmax.y=" + rr.bounds.max.y + " bmin.y=" + rr.bounds.min.y + " size=" + rr.bounds.size); }
            sb.AppendLine(c.name + " pos=" + c.position + " rot=" + c.eulerAngles + " scale=" + c.localScale + " view x=" + r.xMin + ".." + r.xMax + " y=" + r.yMin + ".." + r.yMax + " pts=" + pts.Count);
        }
        System.IO.File.WriteAllText("BusTest/view.txt", sb.ToString());
        Debug.Log("[Select] view reported");
    }
}
