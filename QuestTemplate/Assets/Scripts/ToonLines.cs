/*
 * ToonLines by James Bellian 
 * 04/17/2019
 * 
 * Made in Unity 2018.2.0b2
 * (Ask James for Unity 2019 version)
 * 
 * Generates a toonline for it by inflating then flipping the source mesh.
 * Currently not compatible with LWRP, but you can easily fix this by
 * changing the shader in line 94 to a LWRP friendly shader.
 * 
 * Aknowledgements: Special thanks to Clayton Stamper for the flip normals code
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToonLines : MonoBehaviour
{
    private MeshFilter sourceMesh;
    private GameObject toonline;

    public Color color;
    public float width = 0.01f;

    // Claytons Flip normal function
    private void Flip(Mesh mesh)
    {
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -1 * normals[i];
        }

        mesh.normals = normals;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            for (int j = 0; j < tris.Length; j += 3)
            {
                //swap order of tri vertices
                int temp = tris[j];
                tris[j] = tris[j + 1];
                tris[j + 1] = temp;
            }
            mesh.SetTriangles(tris, i);
        }
    }

    //James's Inflate Mesh function
    private Mesh Inflate(Mesh mesh, float amt)
    {
        Mesh newMesh = new Mesh();

        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;

        int numVerts = verts.Length;

        Vector3 [] newVerts = new Vector3[numVerts];

        for (int i = 0; i < numVerts; i++)
        {
            newVerts[i] = verts[i] + normals[i] * amt;
        }
       
        newMesh.vertices = newVerts;
        newMesh.triangles = mesh.triangles;

        return newMesh;
    }

    // Together they form ToonLine Generator!
    [ContextMenu("Generate ToonLines")]
    private void GenerateToonLines()
    {
        //delete existing toonLine
        Transform oldLine = gameObject.transform.Find("toonLine");
        sourceMesh = GetComponent<MeshFilter>();

        if (sourceMesh == null)
        {
            Debug.Log("Error: Source mesh could not be found. Ensure there is a Mesh Filter component above!");
            return;
        }

        Mesh oldMesh = Instantiate(sourceMesh.sharedMesh);
        RecalculateNormals(oldMesh, 180f);

        if (oldLine != null)
        {
            DestroyImmediate(oldLine.gameObject);
        }

        toonline = new GameObject("toonLine");

        var meshRenderer = toonline.AddComponent<MeshRenderer>();
        var meshFilter = toonline.AddComponent<MeshFilter>();

        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = color;

        meshRenderer.sharedMaterial = mat;

        var newMesh = Inflate(oldMesh, width);

        Flip(newMesh);

        meshFilter.sharedMesh = newMesh;

        toonline.transform.rotation = transform.rotation;
        toonline.transform.localScale = transform.localScale;
        toonline.transform.parent = transform;
        toonline.transform.localPosition = Vector3.zero;

        DestroyImmediate(oldMesh);
    }

    [ContextMenu("Clear ToonLine")]
    private void ClearToonLines()
    {
        DestroyImmediate(toonline);
    }

    public void EnableLine()
    {
        toonline.SetActive(true);
    }

    public void DisableLine()
    {
        toonline.SetActive(false);
    }

    #region Recalculate Normals
    /// <summary>
    ///     Recalculate the normals of a mesh based on an angle threshold. This takes
    ///     into account distinct vertices that have the same position.
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="angle">
    ///     The smoothing angle. Note that triangles that already share
    ///     the same vertex will be smooth regardless of the angle! 
    /// </param>
    public static void RecalculateNormals(Mesh mesh, float angle)
        {
            var cosineThreshold = Mathf.Cos(angle * Mathf.Deg2Rad);

            var vertices = mesh.vertices;
            var normals = new Vector3[vertices.Length];

            // Holds the normal of each triangle in each sub mesh.
            var triNormals = new Vector3[mesh.subMeshCount][];

            var dictionary = new Dictionary<VertexKey, List<VertexEntry>>(vertices.Length);

            for (var subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; ++subMeshIndex)
            {

                var triangles = mesh.GetTriangles(subMeshIndex);

                triNormals[subMeshIndex] = new Vector3[triangles.Length / 3];

                for (var i = 0; i < triangles.Length; i += 3)
                {
                    int i1 = triangles[i];
                    int i2 = triangles[i + 1];
                    int i3 = triangles[i + 2];

                    // Calculate the normal of the triangle
                    Vector3 p1 = vertices[i2] - vertices[i1];
                    Vector3 p2 = vertices[i3] - vertices[i1];
                    Vector3 normal = Vector3.Cross(p1, p2).normalized;
                    int triIndex = i / 3;
                    triNormals[subMeshIndex][triIndex] = normal;

                    List<VertexEntry> entry;
                    VertexKey key;

                    if (!dictionary.TryGetValue(key = new VertexKey(vertices[i1]), out entry))
                    {
                        entry = new List<VertexEntry>(4);
                        dictionary.Add(key, entry);
                    }
                    entry.Add(new VertexEntry(subMeshIndex, triIndex, i1));

                    if (!dictionary.TryGetValue(key = new VertexKey(vertices[i2]), out entry))
                    {
                        entry = new List<VertexEntry>();
                        dictionary.Add(key, entry);
                    }
                    entry.Add(new VertexEntry(subMeshIndex, triIndex, i2));

                    if (!dictionary.TryGetValue(key = new VertexKey(vertices[i3]), out entry))
                    {
                        entry = new List<VertexEntry>();
                        dictionary.Add(key, entry);
                    }
                    entry.Add(new VertexEntry(subMeshIndex, triIndex, i3));
                }
            }

            // Each entry in the dictionary represents a unique vertex position.

            foreach (var vertList in dictionary.Values)
            {
                for (var i = 0; i < vertList.Count; ++i)
                {

                    var sum = new Vector3();
                    var lhsEntry = vertList[i];

                    for (var j = 0; j < vertList.Count; ++j)
                    {
                        var rhsEntry = vertList[j];

                        if (lhsEntry.VertexIndex == rhsEntry.VertexIndex)
                        {
                            sum += triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex];
                        }
                        else
                        {
                            // The dot product is the cosine of the angle between the two triangles.
                            // A larger cosine means a smaller angle.
                            var dot = Vector3.Dot(
                                triNormals[lhsEntry.MeshIndex][lhsEntry.TriangleIndex],
                                triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex]);
                            if (dot >= cosineThreshold)
                            {
                                sum += triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex];
                            }
                        }
                    }

                    normals[lhsEntry.VertexIndex] = sum.normalized;
                }
            }

            mesh.normals = normals;
        }

        private struct VertexKey
        {
            private readonly long _x;
            private readonly long _y;
            private readonly long _z;

            // Change this if you require a different precision.
            private const int Tolerance = 100000;

            // Magic FNV values. Do not change these.
            private const long FNV32Init = 0x811c9dc5;
            private const long FNV32Prime = 0x01000193;

            public VertexKey(Vector3 position)
            {
                _x = (long)(Mathf.Round(position.x * Tolerance));
                _y = (long)(Mathf.Round(position.y * Tolerance));
                _z = (long)(Mathf.Round(position.z * Tolerance));
            }

            public override bool Equals(object obj)
            {
                var key = (VertexKey)obj;
                return _x == key._x && _y == key._y && _z == key._z;
            }

            public override int GetHashCode()
            {
                long rv = FNV32Init;
                rv ^= _x;
                rv *= FNV32Prime;
                rv ^= _y;
                rv *= FNV32Prime;
                rv ^= _z;
                rv *= FNV32Prime;

                return rv.GetHashCode();
            }
        }

        private struct VertexEntry
        {
            public int MeshIndex;
            public int TriangleIndex;
            public int VertexIndex;

            public VertexEntry(int meshIndex, int triIndex, int vertIndex)
            {
                MeshIndex = meshIndex;
                TriangleIndex = triIndex;
                VertexIndex = vertIndex;
            }
        }
    }
    #endregion