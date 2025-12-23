//
// OutlineHighlight.cs
// Combined QuickOutline + Highlight Controller
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class ObjectHighlight : MonoBehaviour
{
    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    [Header("Outline Settings")]
    public Mode outlineMode = Mode.OutlineAll;
    public Color outlineColor = Color.yellow;
    [Range(0f, 10f)] public float outlineWidth = 3f;

    [Header("Bake Settings")]
    public bool precomputeOutline = false;

    private Renderer[] renderers;
    private Material outlineMaskMaterial;
    private Material outlineFillMaterial;

    private List<Mesh> bakeKeys = new List<Mesh>();
    private List<ListVector3> bakeValues = new List<ListVector3>();

    private bool needsUpdate = false;

    [Serializable]
    private class ListVector3
    {
        public List<Vector3> data;
    }

    private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

    // ----------------------------------------------------------
    // UNITY LIFECYCLE
    // ----------------------------------------------------------
    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        outlineMaskMaterial = Instantiate(Resources.Load<Material>("OutlineMask"));
        outlineFillMaterial = Instantiate(Resources.Load<Material>("OutlineFill"));

        outlineMaskMaterial.name = "OutlineMask (Instance)";
        outlineFillMaterial.name = "OutlineFill (Instance)";

        LoadSmoothNormals();
        needsUpdate = true;
    }

    void OnEnable()
    {
        foreach (var r in renderers)
        {
            var mats = r.sharedMaterials.ToList();
            mats.Add(outlineMaskMaterial);
            mats.Add(outlineFillMaterial);
            r.materials = mats.ToArray();
        }
    }

    void OnDisable()
    {
        foreach (var r in renderers)
        {
            var mats = r.sharedMaterials.ToList();
            mats.Remove(outlineMaskMaterial);
            mats.Remove(outlineFillMaterial);
            r.materials = mats.ToArray();
        }
    }

    void OnDestroy()
    {
        Destroy(outlineMaskMaterial);
        Destroy(outlineFillMaterial);
    }

    void Update()
    {
        if (needsUpdate)
        {
            needsUpdate = false;
            UpdateMaterialProperties();
        }
    }

    // ----------------------------------------------------------
    // HIGHLIGHT API (dipanggil dari raycast)
    // ----------------------------------------------------------
    public void HighlightOn()
    {
        enabled = true;
    }

    public void HighlightOff()
    {
        enabled = false;
    }

    // ----------------------------------------------------------
    // INTERNAL OUTLINE FUNCTIONS (QuickOutline original)
    // ----------------------------------------------------------
    void UpdateMaterialProperties()
    {
        outlineFillMaterial.SetColor("_OutlineColor", outlineColor);

        switch (outlineMode)
        {
            case Mode.OutlineAll:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.OutlineVisible:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.OutlineHidden:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.OutlineAndSilhouette:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.SilhouetteOnly:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
                break;
        }
    }

    void LoadSmoothNormals()
    {
        foreach (var mf in GetComponentsInChildren<MeshFilter>())
        {
            if (!registeredMeshes.Add(mf.sharedMesh))
                continue;

            int index = bakeKeys.IndexOf(mf.sharedMesh);
            var normals = (index >= 0) ? bakeValues[index].data : SmoothNormals(mf.sharedMesh);

            mf.sharedMesh.SetUVs(3, normals);
            CombineSubmeshes(mf.sharedMesh, mf.GetComponent<Renderer>().sharedMaterials);
        }

        // Skinned meshes
        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (!registeredMeshes.Add(smr.sharedMesh))
                continue;

            smr.sharedMesh.uv4 = new Vector2[smr.sharedMesh.vertexCount];
            CombineSubmeshes(smr.sharedMesh, smr.sharedMaterials);
        }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {
        var groups = mesh.vertices
                         .Select((v, i) => new KeyValuePair<Vector3, int>(v, i))
                         .GroupBy(p => p.Key);

        var smooth = new List<Vector3>(mesh.normals);

        foreach (var g in groups)
        {
            if (g.Count() == 1) continue;

            Vector3 avg = Vector3.zero;
            foreach (var p in g) avg += smooth[p.Value];

            avg.Normalize();

            foreach (var p in g) smooth[p.Value] = avg;
        }

        return smooth;
    }

    void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
        if (mesh.subMeshCount == 1) return;
        if (mesh.subMeshCount > materials.Length) return;

        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }
}
