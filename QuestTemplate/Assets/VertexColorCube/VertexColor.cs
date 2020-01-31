using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexColor : MonoBehaviour
{
    [SerializeField] [ReadOnly]
    private string note = "Color only visible at runtime";

    public Color color;
    // Start is called before the first frame update
    void Start()
    {
        ColorAllVerts();
    }

    void ColorAllVerts()
    {
        var mesh = GetComponent<MeshFilter>().mesh;

        List<Color32> vertexColorList= new List<Color32>();
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                vertexColorList.Add(color);
            }
        mesh.SetColors(vertexColorList);
    }
}
