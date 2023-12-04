using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

// Chunk types:
// Main chunk: Rendered, simulated, hitboxes
// Simulated chunk: Rendered, simulated

namespace ProceduralGen 
{
    // Manage data regarding chunks:
    // Creating / destroying / reusing meshes
    // Chunk pools, Resource loading and management
    public class ChunkManager 
    {
        private static Dictionary<Tuple<int, int>, GameObject> mainChunks = new Dictionary<Tuple<int, int>, GameObject>();
        private static Dictionary<Tuple<int, int>, GameObject> simulatedChunks = new Dictionary<Tuple<int, int>, GameObject>();

        private ProcGenSettings settings;
        private Material baseChunkMat;
        private TerrainSampler sampler;

        private const int maxVerts = 65535;
        private const int maxTris = 255;

        public ChunkManager(Material bm, TerrainSampler s, ProcGenSettings se) {
            baseChunkMat = bm;
            sampler = s;
            settings = se;
        }

        public void simulateChunks() { 
        }

        public void loadMainChunks(List<Tuple<int, int>> ids) {
            foreach (var cid in ids) {
                mainChunks[cid] = generateStdTerrainChunk(
                    cid.Item1 * settings.defaultSize,
                    cid.Item2 * settings.defaultSize,
                    settings.defaultSections,
                    settings.defaultSize,
                    true
                );
            }
        }

        public void loadSecondaryChunks(List<Tuple<int, int>> ids) {
            foreach (var cid in ids) {
                simulatedChunks[cid] = generateStdTerrainChunk(
                    cid.Item1 * settings.defaultSize,
                    cid.Item2 * settings.defaultSize,
                    settings.defaultSections,
                    settings.defaultSize,
                    false
                );
            }
        }

        public void updateVisualAroundOrigin(Tuple<int, int> origin) { 
        }

        private GameObject generateStdTerrainChunk(int offsetX, int offsetZ, int sections, float size, bool hitBox = false) {
            int noTris = (sections) * (sections);
            int noVerts = (sections + 1) * (sections + 1);
            int noUvs = (sections + 1) * (sections + 1);

            if (noVerts >= maxVerts) {
                Debug.LogWarning($"Creating a chunk mesh with too many verts: {noVerts}/{maxVerts}");
            }

            if (hitBox && noTris >= maxTris) {
                Debug.LogWarning($"Creating a chunk hitbox with too many tris: {noTris}/{maxTris}");
            }

            var go = new GameObject();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = baseChunkMat;
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[noVerts];
            int[] tris = new int[noTris * 6];
            Vector2[] uvs = new Vector2[noUvs];

            int i = 0;
            int triIndex = 0;
            int col = 0; // column; for triangle placement
            for (int x = 0; x <= sections; x++) {
                for (int z = 0; z <= sections; z++) {
                    var xPos = x * (size / sections) + offsetX;
                    var zPos = z * (size / sections) - offsetZ;
                    var y = sampler.Sample(xPos, zPos);

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
                        tris[triIndex] = v - (sections + 1);     // + 0
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

            if (hitBox) {
                var meshColl = go.AddComponent<MeshCollider>();
                meshColl.sharedMesh = mesh;
            }

            return go;
        }
    }
}
