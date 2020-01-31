using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_test : MonoBehaviour
{
    public int currentVertex = 0;
    public Mesh myMesh;
    public Color[] colors;
    [Range(0.01f, 1f)]
    public float delay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        myMesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = myMesh.vertices;
        colors = new Color[vertices.Length];

        for (int i = 0; i < colors.Length; i ++)
        {
            colors[i] = Color.white;
        }

        StartCoroutine("Blend");
        Debug.Log("Vertex colors: " + myMesh.colors.Length.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 private IEnumerator Blend()
    {
        while (currentVertex < colors.Length)
        {
            colors[currentVertex] = Color.black;
            currentVertex++;
            // assign the array of colors to the Mesh.
            myMesh.colors = colors;
            yield return new WaitForSeconds(delay);
        }
    }
}
