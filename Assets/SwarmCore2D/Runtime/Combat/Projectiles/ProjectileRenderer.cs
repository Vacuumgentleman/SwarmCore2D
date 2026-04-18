using UnityEngine;
using System.Collections.Generic;
using SwarmCore2D.Combat;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Rendering
{
    public class ProjectileRenderer : MonoBehaviour
    {
        Mesh quadMesh;

        const int MaxBatch = 1023;
        Matrix4x4[] matrices    = new Matrix4x4[MaxBatch];
        float[]     frameBuffer = new float[MaxBatch];
        MaterialPropertyBlock props;

        Dictionary<Material, List<int>> groups     = new Dictionary<Material, List<int>>();
        Dictionary<Material, float>     animTimers = new Dictionary<Material, float>();

        ProjectileSystem projectileSystem;

        public void Initialize(ProjectileSystem system)
        {
            projectileSystem = system;
            EnsureMesh();
        }

        void Start()
        {
            props = new MaterialPropertyBlock();
            EnsureMesh();

            if (projectileSystem == null)
            {
                var sim = FindFirstObjectByType<SwarmSimulationController>();
                if (sim != null)
                    Initialize(sim.ProjectileSystem);
            }
        }

        void EnsureMesh()
        {
            if (quadMesh != null) return;
            var temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quadMesh = temp.GetComponent<MeshFilter>().sharedMesh;
            Destroy(temp);
        }

        void LateUpdate()
        {
            if (projectileSystem == null || quadMesh == null) return;

            var state = projectileSystem.State;
            int count = state.count;
            if (count == 0) return;

            if (props == null) props = new MaterialPropertyBlock();

            // Group indices by material
            foreach (var list in groups.Values) list.Clear();

            for (int i = 0; i < count; i++)
            {
                var mat = state.material[i];
                if (mat == null) continue;

                if (!groups.TryGetValue(mat, out var list))
                {
                    list = new List<int>();
                    groups[mat] = list;
                }
                list.Add(i);
            }

            float dt = Time.deltaTime;

            foreach (var kvp in groups)
            {
                var mat = kvp.Key;
                var ids = kvp.Value;
                if (ids.Count == 0) continue;

                // Advance shared animation timer for this material
                if (!animTimers.TryGetValue(mat, out float t)) t = 0f;
                t += dt;
                animTimers[mat] = t;

                int processed = 0;
                while (processed < ids.Count)
                {
                    int batchCount = Mathf.Min(MaxBatch, ids.Count - processed);

                    for (int b = 0; b < batchCount; b++)
                    {
                        int i = ids[processed + b];

                        Vector2 pos   = state.position[i];
                        Vector2 dir   = state.direction[i];
                        float   angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        float   sz    = state.size[i];

                        matrices[b] = Matrix4x4.TRS(
                            new Vector3(pos.x, pos.y, -1f),
                            Quaternion.Euler(0f, 0f, angle),
                            Vector3.one * sz);

                        int fc = state.frameCount[i];
                        int frame = fc > 1
                            ? (int)(t * state.frameRate[i]) % fc
                            : 0;
                        frameBuffer[b] = frame;
                    }

                    props.SetFloatArray("_Frame", frameBuffer);
                    Graphics.DrawMeshInstanced(quadMesh, 0, mat, matrices, batchCount, props);

                    processed += batchCount;
                }
            }
        }
    }
}
