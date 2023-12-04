using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ProceduralGen {
    public static class Tools {
        public static Tuple<int, int> chunkIdFromVal(float posX, float posZ, float size) {
            return new Tuple<int, int>(
                Mathf.FloorToInt(posX / size),
                Mathf.FloorToInt(posZ / size)
            );
        } 

         public static Tuple<int, int> chunkIdFromVal(Transform t, float size) {
            return chunkIdFromVal(t.position.x, t.position.z, size);
         }

        public static Tuple<int, int> chunkIdFromVal(GameObject go, float size) {
            return chunkIdFromVal(go.transform, size);
        }
    }

    [System.Serializable]
    public class ProcGenSettings {
        public int defaultSections;         // = 10;
        public int defaultSize;             // = 100;
        public int mainChunkRadius;         // = 1;
        public int simulatedChunksRadius;   // = 3;
        public float maxViewingDistance;    // = 2000;
    }

    [System.Serializable]
    public class TerrainSamplerSettings {
        public float amp;
        public float scale;
    }
}
