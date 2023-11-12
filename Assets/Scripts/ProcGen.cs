using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ProcGen : MonoBehaviour
{
    public static List<GameObject> chunks = new List<GameObject>();
    public int defaultSections;
    public int defaultSize;
    public float amp;
    public float scale;
    public int  rad;

    private void Start()  {
        var size = defaultSize;
        for (int x = -rad; x <= rad; x++) { 
            for (int z = -rad; z <= rad; z++) {
                generateTerrainChunk(x*size, z*size, defaultSections, defaultSize);
            }
        }
    }

    private void generateTerrainChunk(int offsetx, int offsetz, int sections, float size) {

        var go = new GameObject();
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[(sections + 1) * (sections + 1)];
        int[] tris = new int[(sections) * (sections) * 6];
        Vector2[] uvs = new Vector2[(sections + 1) * (sections + 1)];

        int i = 0;
        int triIndex = 0;
        int col = 0; // column; for triangle placement
        for (int x = 0; x <= sections; x++)
        {
            for (int z = 0; z <= sections; z++) {
                var xPos = x * (size / sections) + offsetx;
                var zPos = z * (size / sections) - offsetz;
                var y = amp * Mathf.PerlinNoise(
                        (float)xPos / scale,
                        (float)zPos / scale
                );

                vertices[i] = new Vector3(xPos, y, zPos);
                uvs[i] = new Vector2(x, z);
                i++;

                // new column
                if (x > 0 && z == 0) { 
                    col += sections + 1;
                } 

                // generate triangles
                if (x > 0 && x <= sections && z > 0 && z <= sections) { 
                    int v = col + z;
                    tris[triIndex]     = v - (sections + 1);     // + 0
                    tris[triIndex + 1] = v;                      // + 2
                    tris[triIndex + 2] = v - 1 - (sections + 1); // + 1
                    tris[triIndex + 3] = v - 1 - (sections + 1); // + 1
                    tris[triIndex + 4] = v;                      // + 2
                    tris[triIndex + 5] = v - 1;                  // + 3
                    triIndex += 6;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        chunks.Append(go);
    }
}
