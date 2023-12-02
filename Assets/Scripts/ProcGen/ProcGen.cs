using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralGen  
{
    // Entry point for procedural generation system.
    // Manage loading/deloading logic
    public class ProcGen : MonoBehaviour 
    {
        public GameObject trackedObject;
        public Material baseMat;
        public int defaultSections;
        public int defaultSize;
        public int mainChunksRadius;

        private ProceduralGen.TerrainSampler sampler = new TerrainSampler();
        private ProceduralGen.ChunkManager chunkManager;

        private Vector2 trackedOffset; // x, z

        private void Start() {
            if (baseMat == null) {
                baseMat = new Material(Shader.Find("Standard"));
            }

            if (trackedObject == null) {
                Debug.LogWarning("No object has been registered for tracking");
            }

            chunkManager = new ChunkManager(baseMat, sampler, mainChunksRadius);

            trackedOffset = new Vector2(
                Mathf.FloorToInt(trackedObject.transform.position.x / defaultSize),
                Mathf.FloorToInt(trackedObject.transform.position.z / defaultSize) 
            );

            chunkManager.generateStdTerrainChunk(0, 0, defaultSections, defaultSize, true);
            chunkManager.generateStdTerrainChunk(0, 1 * defaultSize, defaultSections, defaultSize, true);
            chunkManager.generateStdTerrainChunk(0, 2 * defaultSize, defaultSections, defaultSize, true);
        }

        private void Update() {
            if (trackedObject == null) {
                return;
            }

            int currChunkX = Mathf.FloorToInt(trackedObject.transform.position.x / defaultSize);
            int currChunkZ = Mathf.FloorToInt(trackedObject.transform.position.z / defaultSize);

            if (currChunkX != trackedOffset.x || currChunkZ != trackedOffset.y) {
                trackedOffset.x = currChunkX;
                trackedOffset.y = currChunkZ;
                updateHitboxChunks();
            }
        }

        private void updateHitboxChunks() {
            Debug.Log(trackedOffset);
        }
    }
}
