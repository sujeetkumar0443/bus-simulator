using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;

public static class MeasureNewBuses
{
    static readonly string[] Paths = {
        "Assets/NewBuses/JetSet/BUSJETSET.obj",
        "Assets/NewBuses/Ecoline/BUSECOLINE.obj",
        "Assets/NewBuses/Laksana/LAKSANASR2.obj",
        "Assets/NewBuses/Zedone/Zedone.fbx",
        "Assets/NewBuses/Irizar/irizar_I6.fbx" };

    [MenuItem("Tools/Bus/Measure New Buses")]
    public static void Run()
    {
        var sb = new StringBuilder("[MeasureNewBuses]\n");
        foreach (var p in Paths)
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
            if (go == null) { sb.AppendLine(p + " NOT LOADED"); continue; }
            var mfs = go.GetComponentsInChildren<MeshFilter>(true);
            var smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            bool first = true; Bounds b = new Bounds();
            int verts = 0; int mats = 0;
            foreach (var mf in mfs)
            {
                if (mf.sharedMesh == null) continue;
                verts += mf.sharedMesh.vertexCount;
                var m = mf.transform.localToWorldMatrix;
                var mb = mf.sharedMesh.bounds;
                for (int i = 0; i < 8; i++)
                {
                    Vector3 c = mb.center + Vector3.Scale(mb.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                    Vector3 w = m.MultiplyPoint3x4(c);
                    if (first) { b = new Bounds(w, Vector3.zero); first = false; } else b.Encapsulate(w);
                }
            }
            foreach (var r in go.GetComponentsInChildren<Renderer>(true)) mats += r.sharedMaterials.Length;
            sb.AppendLine("== " + p + " meshes=" + mfs.Length + " skinned=" + smrs.Length + " verts=" + verts + " mats=" + mats);
            sb.AppendLine("  bounds min=" + b.min.ToString("F2") + " max=" + b.max.ToString("F2") + " rootScale=" + go.transform.localScale + " rootRot=" + go.transform.eulerAngles);
            foreach (var mf in mfs.Take(70))
            {
                if (mf.sharedMesh == null) continue;
                var m = mf.transform.localToWorldMatrix; var mb = mf.sharedMesh.bounds;
                Vector3 c = m.MultiplyPoint3x4(mb.center);
                sb.AppendLine("   " + mf.name + " v=" + mf.sharedMesh.vertexCount + " c=" + c.ToString("F2") + " ext=" + Vector3.Scale(mb.extents, mf.transform.lossyScale).ToString("F2"));
            }
        }
        Debug.Log(sb.ToString());
    }
}
