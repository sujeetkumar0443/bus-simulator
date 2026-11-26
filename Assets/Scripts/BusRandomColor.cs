using UnityEngine;

// Gives a new-model bus a random paint / rim / wheel-nut colour every time it is spawned.
public class BusRandomColor : MonoBehaviour
{
    [System.Serializable]
    public class Slot { public Renderer renderer; public int index; public int role; } // role 0 = paint, 1 = rim, 2 = nuts

    public Slot[] slots;

    static readonly Color[] Paint = {
        new Color32(0xC6,0x28,0x28,255), new Color32(0x15,0x65,0xC0,255), new Color32(0x2E,0x7D,0x32,255), new Color32(0xF9,0xA8,0x25,255), new Color32(0x6A,0x1B,0x9A,255),
        new Color32(0xE5,0x39,0x35,255), new Color32(0x03,0xA9,0xF4,255), new Color32(0x66,0xBB,0x6A,255), new Color32(0xFF,0xCA,0x28,255), new Color32(0xAB,0x47,0xBC,255),
        new Color32(0x8E,0x00,0x00,255), new Color32(0x0D,0x47,0xA1,255), new Color32(0x1B,0x5E,0x20,255), new Color32(0xFF,0x8F,0x00,255), new Color32(0x4A,0x14,0x8C,255) };
    static readonly Color[] Rim = {
        new Color32(0xFF,0xD5,0x4F,255), new Color32(0xE0,0xE0,0xE0,255), new Color32(0xFF,0x52,0x52,255),
        new Color32(0x40,0xC4,0xFF,255), new Color32(0xB2,0xFF,0x59,255), new Color32(0xFF,0x8A,0x65,255) };
    static readonly Color[] Nut = {
        new Color32(0xFF,0xFF,0xFF,255), new Color32(0xFF,0xD6,0x00,255), new Color32(0xFF,0x17,0x44,255),
        new Color32(0x00,0xE5,0xFF,255), new Color32(0x76,0xFF,0x03,255), new Color32(0xD5,0x00,0xF9,255) };

    void Start() { Apply(); }

    public void Apply()
    {
        if (slots == null) return;
        // every bus gets its own colour family: buses use disjoint palette entries (group = bus index)
        int g = transform.GetSiblingIndex() % 5;
        Color paint = Paint[(g + 5 * Random.Range(0, 3)) % Paint.Length];
        Color rim = Rim[(g + Random.Range(0, 2) * 5) % Rim.Length];
        Color nut = Nut[g % Nut.Length];
        foreach (var s in slots)
        {
            if (s == null || s.renderer == null) continue;
            Material[] ms = s.renderer.materials;
            if (s.index < 0 || s.index >= ms.Length) continue;
            Color c = s.role == 0 ? paint : (s.role == 1 ? rim : nut);
            if (ms[s.index].HasProperty("_Color")) ms[s.index].color = c;
            if (ms[s.index].HasProperty("_BaseColor")) ms[s.index].SetColor("_BaseColor", c);
            s.renderer.materials = ms;
        }
    }
}
