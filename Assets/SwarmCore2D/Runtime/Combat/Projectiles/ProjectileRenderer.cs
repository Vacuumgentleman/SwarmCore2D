using UnityEngine;
using SwarmCore2D.Combat;

namespace SwarmCore2D.Rendering
{
    public class ProjectileRenderer : MonoBehaviour
    {
        public Mesh quadMesh;
        public Material material;

        [Header("Animation")]
        public int frameCount = 4;
        public float frameRate = 12f;

        const int MaxBatch = 1023;

        Matrix4x4[] matrices = new Matrix4x4[MaxBatch];

        ProjectileSystem projectileSystem;

        float animTimer;

        public void Initialize(ProjectileSystem system)
        {
            projectileSystem = system;

            if (quadMesh == null)
            {
                GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quadMesh = temp.GetComponent<MeshFilter>().sharedMesh;
                Destroy(temp);
            }
        }

        void LateUpdate()
        {
            if (projectileSystem == null)
                return;

            if (material == null)
                return;

            var state = projectileSystem.State;

            int count = state.count;

            if (count == 0)
                return;

            animTimer += Time.deltaTime;

            int frame = 0;

            if (frameCount > 1)
            {
                frame = (int)(animTimer * frameRate) % frameCount;
            }

            material.SetFloat("_Frame", frame);

            int index = 0;

            while (index < count)
            {
                int batchCount = Mathf.Min(MaxBatch, count - index);

                for (int i = 0; i < batchCount; i++)
                {
                    int p = index + i;

                    Vector2 pos = state.position[p];
                    Vector2 dir = state.direction[p];

                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    matrices[i] =
                        Matrix4x4.TRS(
                            new Vector3(pos.x, pos.y, -1f),
                            Quaternion.Euler(0, 0, angle),
                            Vector3.one
                        );
                }

                Graphics.DrawMeshInstanced(
                    quadMesh,
                    0,
                    material,
                    matrices,
                    batchCount
                );

                index += batchCount;
            }
        }
    }
}