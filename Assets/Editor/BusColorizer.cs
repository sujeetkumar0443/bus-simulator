#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// Builds real materials for the new buses (paint / glass / tyre / rim / nuts ...) and registers
// the paint + rim + nut slots in BusRandomColor so each bus gets a random colour at runtime.
public static class BusColorizer
{
    enum Role { None, Paint, Glass, Tyre, Rim, Nut, Chrome, Black, Dark, Lamp, Red, Amber }

    public static void Apply(GameObject inst, RCC_CarControllerV3 car, string busName, Transform chassis, float groundY, StringBuilder sb)
    {
        string gen = "Assets/NewBuses/" + busName + "/" + BusInstaller.GenDir;
        if (!AssetDatabase.IsValidFolder(gen)) { sb.AppendLine("  colorizer: no Gen folder"); return; }
        var mats = new Dictionary<Role, Material>();
        Color pc = RandPaint();
        mats[Role.Paint] = Mk(gen, "Paint", pc, 0.25f, 0.65f);
        mats[Role.Glass] = Mk(gen, "Glass", new Color(0.06f, 0.09f, 0.12f), 0.1f, 0.92f);
        mats[Role.Tyre] = Mk(gen, "Tyre", new Color(0.05f, 0.05f, 0.05f), 0f, 0.12f);
        mats[Role.Rim] = Mk(gen, "Rim", new Color(0.88f, 0.88f, 0.9f), 0.85f, 0.75f);
        mats[Role.Nut] = Mk(gen, "Nut", Color.white, 0.8f, 0.8f);
        mats[Role.Chrome] = Mk(gen, "Chrome", new Color(0.8f, 0.8f, 0.82f), 0.9f, 0.85f);
        mats[Role.Black] = Mk(gen, "Black", new Color(0.04f, 0.04f, 0.04f), 0.2f, 0.7f);
        mats[Role.Dark] = Mk(gen, "Dark", new Color(0.14f, 0.14f, 0.15f), 0.2f, 0.35f);
        mats[Role.Lamp] = Mk(gen, "Lamp", new Color(1f, 0.97f, 0.85f), 0f, 0.9f);
        mats[Role.Red] = Mk(gen, "RedLamp", new Color(0.8f, 0.03f, 0.03f), 0f, 0.9f);
        mats[Role.Amber] = Mk(gen, "AmberLamp", new Color(1f, 0.6f, 0.05f), 0f, 0.9f);

        var slots = new List<BusRandomColor.Slot>();
        int changed = 0;
        foreach (Renderer r in inst.GetComponentsInChildren<Renderer>(true))
        {
            MeshFilter mf = r.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;
            bool wheel = r.name.StartsWith("wheel_") || r.name.StartsWith("Pivot_wheel_");
            Material[] sm = r.sharedMaterials;
            Mesh mesh = mf.sharedMesh;
            // single-material model (no colour information): split the mesh into parts
            if (sm.Length == 1 && sm[0] != null && sm[0].name == "defaultMat" && mesh.subMeshCount == 1)
            {
                Mesh nm = wheel ? SplitWheelRadial(mesh, gen, busName + "_" + r.name + "_C")
                                : (r.enabled && mesh.vertexCount > 20000 ? SplitBodyParts(mesh, r.transform, chassis, groundY, gen, busName + "_bodyC", sb) : null);
                if (nm != null)
                {
                    mf.sharedMesh = nm; mesh = nm;
                    sm = new Material[nm.subMeshCount];
                    for (int i = 0; i < sm.Length; i++) sm[i] = new Material(Shader.Find("Standard")) { name = SubName(nm, i) };
                }
            }
            bool dirty = false;
            for (int i = 0; i < sm.Length; i++)
            {
                if (sm[i] == null) continue;
                Role role = RoleOf(sm[i].name, r.name, wheel, busName);
                if (role == Role.None) continue;
                sm[i] = mats[role]; dirty = true; changed++;
                if (role == Role.Paint || role == Role.Rim || role == Role.Nut)
                    slots.Add(new BusRandomColor.Slot { renderer = r, index = i, role = role == Role.Paint ? 0 : (role == Role.Rim ? 1 : 2) });
            }
            if (dirty) { r.sharedMaterials = sm; EditorUtility.SetDirty(r); }
        }
        var brc = car.GetComponent<BusRandomColor>();
        if (brc == null) brc = car.gameObject.AddComponent<BusRandomColor>();
        brc.slots = slots.ToArray();
        EditorUtility.SetDirty(brc);
        AssetDatabase.SaveAssets();
        sb.AppendLine("  colorizer: material slots recoloured=" + changed + " random-colour slots=" + slots.Count);
    }

    static Color RandPaint()
    {
        Color[] p = { new Color32(0xC6,0x28,0x28,255), new Color32(0x15,0x65,0xC0,255), new Color32(0x2E,0x7D,0x32,255), new Color32(0xF9,0xA8,0x25,255), new Color32(0x6A,0x1B,0x9A,255), new Color32(0x00,0x83,0x8F,255), new Color32(0xEF,0x6C,0x00,255) };
        return p[Random.Range(0, p.Length)];
    }

    static Material Mk(string gen, string name, Color c, float metal, float smooth)
    {
        string path = gen + "/" + name + ".mat";
        AssetDatabase.DeleteAsset(path);
        var m = new Material(Shader.Find("Standard")) { name = name };
        m.color = c; m.SetFloat("_Metallic", metal); m.SetFloat("_Glossiness", smooth);
        AssetDatabase.CreateAsset(m, path);
        return AssetDatabase.LoadAssetAtPath<Material>(path);
    }

    static string SubName(Mesh m, int i) { int k = SubIndex.ContainsKey(m) ? SubIndex[m][i] : i; return m.name.Contains("_bodyC") ? new[] { "gen_paint", "gen_glass", "gen_dark" }[k] : new[] { "gen_tyre", "gen_rim", "gen_nut" }[k]; }

    static Role RoleOf(string mat, string rend, bool wheel, string bus)
    {
        string m = mat.ToLower(), n = rend.ToLower();
        switch (m)
        {
            case "gen_paint": return Role.Paint; case "gen_glass": return Role.Glass; case "gen_dark": return Role.Dark;
            case "gen_tyre": return Role.Tyre; case "gen_rim": return Role.Rim; case "gen_nut": return Role.Nut;
        }
        if (m.Contains("kaca") || m.Contains("glass") || m.Contains("glasses")) return Role.Glass;
        if (bus == "Laksana")
        {
            if (wheel || m.Contains("8_ban")) { if (m.Contains("ban_hitam")) return Role.Tyre; if (m.Contains("ban_dop")) return Role.Nut; if (m.Contains("pelek")) return Role.Rim; return Role.None; }
            if (m.Contains("merah")) return Role.Red;
            if (m.Contains("kuning")) return Role.Amber;
            if (m.Contains("nyala") || m.Contains("lampu_putih")) return Role.Lamp;
            if (m.Contains("crome")) return Role.Chrome;
            if (m.Contains("putih_glos") && (m.Contains("krum") || m.Contains("bodi"))) return Role.Paint;
            if (m.Contains("bodi") && !m.Contains("hitam") && !m.Contains("abu") && !m.Contains("lampu")) return Role.Paint;
            if (m.Contains("hitam")) return Role.Black;
            return Role.None;
        }
        if (bus == "Irizar")
        {
            if (wheel || n.Contains("pelek") || n.Contains("ban")) { if (m == "bodi") return Role.Rim; if (m == "rim") return Role.Nut; if (m == "bsn") return Role.Tyre; return Role.None; }
            if (m == "bodi") return Role.Paint;
            if (m == "krum") return Role.Chrome;
            if (m.Contains("hitam gloss")) return Role.Black;
            return Role.None;
        }
        if (bus == "Zedone")
        {
            if (wheel) { if (m.StartsWith("tyre")) return Role.Tyre; if (m.StartsWith("bolt") || m == "wheel cap") return Role.Nut; if (m == "ametal1" || m == "seat_") return Role.Rim; return Role.None; }
            if (m == "zedone color") return Role.Paint;
            return Role.None;
        }
        return Role.None;
    }

    // ---- wheel: split by distance from the axle (axis = local X) -> tyre / rim / nuts
    static Mesh SplitWheelRadial(Mesh src, string gen, string name)
    {
        Vector3[] v = src.vertices; int[] t = src.triangles;
        float R = 0f; foreach (var p in v) R = Mathf.Max(R, new Vector2(p.y, p.z).magnitude);
        var tyre = new List<int>(); var rim = new List<int>(); var nut = new List<int>();
        for (int i = 0; i < t.Length; i += 3)
        {
            Vector3 c = (v[t[i]] + v[t[i + 1]] + v[t[i + 2]]) / 3f;
            float r = new Vector2(c.y, c.z).magnitude / R;
            (r > 0.72f ? tyre : (r > 0.38f ? rim : nut)).AddRange(new[] { t[i], t[i + 1], t[i + 2] });
        }
        return Rebuild(src, gen, name, new List<int>[] { tyre, rim, nut });
    }

    // ---- body: connected components -> paint / glass / dark
    static Mesh SplitBodyParts(Mesh src, Transform tr, Transform chassis, float groundY, string gen, string name, StringBuilder sb)
    {
        Vector3[] v = src.vertices; int[] t = src.triangles;
        var weld = new Dictionary<long, int>(); int[] w = new int[v.Length];
        for (int i = 0; i < v.Length; i++)
        {
            long k = ((long)Mathf.RoundToInt(v[i].x * 1000f) * 73856093L) ^ ((long)Mathf.RoundToInt(v[i].y * 1000f) * 19349663L) ^ ((long)Mathf.RoundToInt(v[i].z * 1000f) * 83492791L);
            int id; if (!weld.TryGetValue(k, out id)) { id = weld.Count; weld[k] = id; } w[i] = id;
        }
        int[] uf = Enumerable.Range(0, weld.Count).ToArray();
        System.Func<int, int> find = null; find = a => { while (uf[a] != a) { uf[a] = uf[uf[a]]; a = uf[a]; } return a; };
        for (int i = 0; i < t.Length; i += 3) { int a = find(w[t[i]]), b = find(w[t[i + 1]]), c = find(w[t[i + 2]]); uf[b] = a; uf[find(c)] = find(a); }
        Matrix4x4 m = chassis.worldToLocalMatrix * tr.localToWorldMatrix;
        var comp = new Dictionary<int, List<int>>();
        for (int i = 0; i < t.Length; i += 3) { int r = find(w[t[i]]); if (!comp.ContainsKey(r)) comp[r] = new List<int>(); comp[r].Add(i); }
        var paint = new List<int>(); var glass = new List<int>(); var dark = new List<int>();
        var log = new List<string>();
        var V = new List<Vector3>(v); var N = new List<Vector3>(src.normals); var UV = new List<Vector2>(src.uv);
        if (N.Count != v.Length) { N = Enumerable.Repeat(Vector3.up, v.Length).ToList(); }
        if (UV.Count != v.Length) { UV = Enumerable.Repeat(Vector2.zero, v.Length).ToList(); }
        float topY = float.MinValue; foreach (var p in v) topY = Mathf.Max(topY, m.MultiplyPoint3x4(p).y);
        float totalH = topY - groundY;
        foreach (var kv in comp.OrderByDescending(k => k.Value.Count))
        {
            Bounds b = new Bounds(m.MultiplyPoint3x4(v[t[kv.Value[0]]]), Vector3.zero);
            foreach (int ti in kv.Value) for (int j = 0; j < 3; j++) b.Encapsulate(m.MultiplyPoint3x4(v[t[ti + j]]));
            float[] s = { b.size.x, b.size.y, b.size.z }; System.Array.Sort(s);
            float h = b.center.y - groundY;
            string cls;
            if (kv.Value.Count < 150 || h < 0.45f) cls = "dark";
            else if (s[0] < 0.12f && s[1] > 0.35f && h > 0.9f) cls = "glass";
            else cls = "paint";
            if (log.Count < 28) log.Add(kv.Value.Count + " tris h=" + h.ToString("F1") + " size=" + b.size.ToString("F2") + " -> " + cls);
            if (cls == "paint" && kv.Value.Count > 3000)
            {
                // windows are part of the same shell: cut a clean window band (triangles are clipped at the band edges)
                float H = totalH;
                foreach (int ti in kv.Value)
                {
                    int[] id = { t[ti], t[ti + 1], t[ti + 2] };
                    Vector3 p0 = m.MultiplyPoint3x4(v[id[0]]), p1 = m.MultiplyPoint3x4(v[id[1]]), p2 = m.MultiplyPoint3x4(v[id[2]]);
                    Vector3 nn = Vector3.Cross(p1 - p0, p2 - p0).normalized;
                    float lo = 0, hi = 0; bool win = false;
                    if (Mathf.Abs(nn.x) > 0.8f) { lo = groundY + 0.33f * H; hi = groundY + 0.67f * H; win = true; }
                    else if (nn.z > 0.65f) { lo = groundY + 0.36f * H; hi = groundY + 0.74f * H; win = true; }
                    else if (nn.z < -0.65f) { lo = groundY + 0.42f * H; hi = groundY + 0.70f * H; win = true; }
                    if (!win) { paint.Add(id[0]); paint.Add(id[1]); paint.Add(id[2]); continue; }
                    var poly = new List<int>(id);
                    var below = Clip(poly, lo, false, V, N, UV, m); var rest = Clip(poly, lo, true, V, N, UV, m);
                    var mid = Clip(rest, hi, false, V, N, UV, m); var above = Clip(rest, hi, true, V, N, UV, m);
                    Fan(below, paint); Fan(above, paint); Fan(mid, glass);
                }
                continue;
            }
            var dst = cls == "paint" ? paint : (cls == "glass" ? glass : dark);
            foreach (int ti in kv.Value) { dst.Add(t[ti]); dst.Add(t[ti + 1]); dst.Add(t[ti + 2]); }
        }
        sb.AppendLine("  body parts of " + src.name + ": components=" + comp.Count + " paint=" + paint.Count / 3 + " glass=" + glass.Count / 3 + " dark=" + dark.Count / 3);
        foreach (string l in log) sb.AppendLine("     " + l);
        return RebuildV(name, gen, V, N, UV, new List<int>[] { paint, glass, dark });
    }

    // clip a polygon (list of vertex indices) against the horizontal plane y = py in chassis space; keep the part above or below
    static List<int> Clip(List<int> poly, float py, bool keepAbove, List<Vector3> V, List<Vector3> N, List<Vector2> UV, Matrix4x4 m)
    {
        var res = new List<int>();
        for (int i = 0; i < poly.Count; i++)
        {
            int a = poly[i], b = poly[(i + 1) % poly.Count];
            float ya = m.MultiplyPoint3x4(V[a]).y - py, yb = m.MultiplyPoint3x4(V[b]).y - py;
            bool ina = keepAbove ? ya >= 0 : ya <= 0, inb = keepAbove ? yb >= 0 : yb <= 0;
            if (ina) res.Add(a);
            if (ina != inb && Mathf.Abs(ya - yb) > 1e-6f)
            {
                float k = ya / (ya - yb);
                V.Add(Vector3.Lerp(V[a], V[b], k)); N.Add(Vector3.Lerp(N[a], N[b], k).normalized); UV.Add(Vector2.Lerp(UV[a], UV[b], k));
                res.Add(V.Count - 1);
            }
        }
        return res;
    }
    static void Fan(List<int> poly, List<int> dst) { for (int i = 1; i + 1 < poly.Count; i++) { dst.Add(poly[0]); dst.Add(poly[i]); dst.Add(poly[i + 1]); } }

    static Mesh RebuildV(string name, string gen, List<Vector3> V, List<Vector3> N, List<Vector2> UV, List<int>[] groups)
    {
        var keep = groups.Where(g => g.Count > 0).ToList();
        var nm = new Mesh { name = name, indexFormat = V.Count > 65000 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16 };
        nm.SetVertices(V); nm.SetNormals(N); nm.SetUVs(0, UV);
        nm.subMeshCount = keep.Count;
        for (int i = 0; i < keep.Count; i++) nm.SetTriangles(keep[i], i);
        nm.RecalculateBounds();
        string path = gen + "/" + name + ".asset";
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(nm, path);
        var saved = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        SubIndex[saved] = groups.Select((g, i) => g.Count > 0 ? i : -1).Where(i => i >= 0).ToArray();
        return saved;
    }

    static Mesh Rebuild(Mesh src, string gen, string name, List<int>[] groups)
    {
        var keep = groups.Where(g => g.Count > 0).ToList();
        var nm = new Mesh { name = name, indexFormat = src.vertexCount > 65000 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16 };
        nm.vertices = src.vertices; nm.normals = src.normals; nm.uv = src.uv;
        if (src.tangents.Length == src.vertexCount) nm.tangents = src.tangents;
        nm.subMeshCount = keep.Count;
        for (int i = 0; i < keep.Count; i++) nm.SetTriangles(keep[i], i);
        nm.RecalculateBounds();
        // sub-mesh names must stay positional: paint, glass, dark / tyre, rim, nut -> record which are present
        string path = gen + "/" + name + ".asset";
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(nm, path);
        var saved = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        // remember the original index of every kept group inside the mesh name-independent list
        saved.name = name;
        SubIndex[saved] = groups.Select((g, i) => g.Count > 0 ? i : -1).Where(i => i >= 0).ToArray();
        return saved;
    }
    public static Dictionary<Mesh, int[]> SubIndex = new Dictionary<Mesh, int[]>();
}
#endif
