using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace ProceduralGen  
{
    // Entry point for procedural generation system.
    // Manage loading/deloading logic
    public class ProcGen : MonoBehaviour 
    {
        public ProceduralGen.ProcGenSettings settings = new ProcGenSettings();
        public ProceduralGen.TerrainSamplerSettings samplerSettings = new TerrainSamplerSettings();
        public GameObject trackedObject;
        public Material baseMat;

        private ProceduralGen.TerrainSampler sampler;
        private ProceduralGen.ChunkManager chunkManager;
        private Tuple<int, int> trackedId;

        private void Start() {
            if (baseMat == null) {
                baseMat = new Material(Shader.Find("Standard"));
            }

            if (trackedObject == null) {
                Debug.LogWarning("No object has been registered for tracking");
            }

            sampler = new TerrainSampler(samplerSettings);
            chunkManager = new ChunkManager(baseMat, sampler, settings);
            trackedId = ProceduralGen.Tools.chunkIdFromVal(trackedObject, settings.defaultSize);

            updateChunks();
        }

        private void Update() {
            chunkManager.simulateChunks();

            if (trackedObject == null) {
                return;
            }

            var curr = ProceduralGen.Tools.chunkIdFromVal(trackedObject, settings.defaultSize);
            if (!curr.Equals(trackedId)) {
                trackedId = curr;
                updateChunks();
            }
        }

        private void updateChunks() {
            Debug.Log(trackedId);

            List<Tuple<int, int>> main = new List<Tuple<int, int>>();
            List<Tuple<int, int>> secondary = new List<Tuple<int, int>>();

            for (int x = -settings.simulatedChunksRadius; x <= settings.simulatedChunksRadius; x++) {
                for (int z = -settings.simulatedChunksRadius; z <= settings.simulatedChunksRadius; z++) {
                    int trueX = x + trackedId.Item1;
                    int trueZ = z + trackedId.Item2; 

                    if (Mathf.Abs(x) <= settings.mainChunkRadius && Mathf.Abs(z) <= settings.mainChunkRadius) {
                        main.Add(new Tuple<int, int>(trueX, trueZ));
                    }
                    else { 
                        secondary.Add(new Tuple<int, int>(trueX, trueZ));
                    }
                }
            }

            Debug.Log(main.Count);
            Debug.Log(secondary.Count);

            chunkManager.loadMainChunks(main);
            chunkManager.loadSecondaryChunks(secondary);
            chunkManager.updateVisualAroundOrigin(trackedId);
        }
    }
}
