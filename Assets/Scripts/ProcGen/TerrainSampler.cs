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
        private ProceduralGen.TerrainSamplerSettings settings;

        public TerrainSampler(TerrainSamplerSettings s) {
            settings = s;
        }   

        public float Sample(float x, float z)
        {
            return settings.amp * Mathf.PerlinNoise(
                (float)x / settings.scale,
                (float)z / settings.scale
            );
        }
    }
}
