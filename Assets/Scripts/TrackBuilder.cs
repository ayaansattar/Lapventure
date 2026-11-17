using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

// Rebuilds the Track spline from a list of points.
// In the editor, with the game stopped: Track Builder's ⋮ menu → "Build Track".
// Default layout: inspired by the Kerpen karting circuit, rotated to portrait.
[RequireComponent(typeof(SplineContainer))]
public class TrackBuilder : MonoBehaviour
{
    public Vector2[] points = {
        new(0.69f, -4.48f), new(3.66f, -6.64f), new(4.83f, -8.31f), new(4.29f, -9.61f),
        new(1.95f, -9.16f), new(0.24f, -8.71f), new(-2.19f, -7.36f), new(-5.43f, -5.65f),
        new(-5.88f, -3.40f), new(-5.52f, -1.78f), new(-5.97f, 0.24f), new(-5.70f, 2.72f),
        new(-6.06f, 4.97f), new(-4.75f, 6.41f), new(-2.73f, 7.31f), new(-0.03f, 7.31f),
        new(1.77f, 6.86f), new(2.22f, 5.42f), new(2.40f, 4.16f), new(3.66f, 3.80f),
        new(4.29f, 3.08f), new(3.93f, 2.09f), new(4.56f, 1.46f), new(5.73f, 0.47f),
        new(5.19f, -0.97f), new(3.48f, -0.70f), new(2.04f, 0.02f), new(1.05f, -0.34f),
        new(-0.21f, -0.70f), new(-0.12f, 1.73f), new(-0.66f, 4.07f), new(-2.01f, 4.34f),
        new(-3.09f, 3.80f), new(-3.00f, 0.47f), new(-2.73f, -2.77f)
    };
    public Vector2 stretch = new Vector2(1.3f, 1f);

    [ContextMenu("Build Track")]
    public void Build()
    {
        var container = GetComponent<SplineContainer>();
#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(container, "Build Track");
#endif
        var spline = container.Spline;
        spline.Clear();
        foreach (var p in points)
            spline.Add(new BezierKnot(new float3(p.x * stretch.x, p.y * stretch.y, 0f)), TangentMode.AutoSmooth); 
        spline.Closed = true;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(container);
#endif
    }
}
