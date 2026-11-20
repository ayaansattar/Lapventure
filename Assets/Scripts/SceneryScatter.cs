using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

// Scatters trees around the circuit, keeping clear of the track and of any
// scenery you placed by hand. Run it in the editor (game stopped):
// ⋮ menu → "Scatter Trees". Run again for a new layout, or "Clear Trees".
public class SceneryScatter : MonoBehaviour {
    public SplineContainer track;
    public Transform keepClear;          // parent of your grandstands, paddock, etc.
    public Sprite[] treeSprites;
    public int count = 60;
    public float areaPadding = 6f;       // how far beyond the track to scatter
    public float trackClearance = 2.2f;  // min distance from the track centerline
    public float treeSpacing = 1.1f;     // min distance between trees
    public Vector2 sizeRange = new Vector2(0.9f, 1.5f);
    public int seed = 1;
    public int sortingOrder = -4;

    [ContextMenu("Scatter Trees")]
    public void Scatter() {
        Clear();
        var rng = new System.Random(seed);
        float R() => (float)rng.NextDouble();

        var trackPts = new List<Vector2>();
        var bounds = new Bounds((Vector3)track.EvaluatePosition(0f), Vector3.zero);
        for (int i = 0; i < 400; i++) {
            Vector3 p = (Vector3)track.EvaluatePosition(i / 400f);
            trackPts.Add(p);
            bounds.Encapsulate(p);
        }
        bounds.Expand(areaPadding * 2f);

        var blockers = new List<Bounds>();
        if (keepClear != null)
            foreach (var r in keepClear.GetComponentsInChildren<Renderer>()) {
                var b = r.bounds; b.Expand(0.6f); blockers.Add(b);
            }

        var placed = new List<Vector2>();
        int attempts = 0;
        while (placed.Count < count && attempts++ < count * 50) {
            var p = new Vector2(Mathf.Lerp(bounds.min.x, bounds.max.x, R()),
                                Mathf.Lerp(bounds.min.y, bounds.max.y, R()));
            if (TooClose(p, trackPts, trackClearance)) continue;
            if (TooClose(p, placed, treeSpacing)) continue;
            bool blocked = false;
            foreach (var b in blockers)
                if (b.Contains(new Vector3(p.x, p.y, b.center.z))) { blocked = true; break; }
            if (blocked) continue;

            var go = new GameObject("Tree");
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(p.x, p.y, 0f);
            float s = Mathf.Lerp(sizeRange.x, sizeRange.y, R());
            go.transform.localScale = new Vector3(s, s, 1f);
            go.transform.rotation = Quaternion.Euler(0, 0, R() * 360f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = treeSprites[rng.Next(treeSprites.Length)];
            sr.sortingOrder = sortingOrder;
            placed.Add(p);
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    [ContextMenu("Clear Trees")]
    public void Clear() {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    static bool TooClose(Vector2 p, List<Vector2> pts, float d) {
        float d2 = d * d;
        foreach (var q in pts) if ((p - q).sqrMagnitude < d2) return true;
        return false;
    }
}
