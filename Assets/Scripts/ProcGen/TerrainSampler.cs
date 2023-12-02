using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Support: Various maps that affect height / terrain color

namespace ProceduralGen
{
    // Responsible for all procedural world data
    // E.g. terrain height, nature, material 
    public class TerrainSampler
    {
        public float amp = 1f;
        public float scale = 10.001f;

        public float Sample(float x, float z)
        {
            return amp * Mathf.PerlinNoise(
                (float)x / scale,
                (float)z / scale
            );
        }
    }
}
