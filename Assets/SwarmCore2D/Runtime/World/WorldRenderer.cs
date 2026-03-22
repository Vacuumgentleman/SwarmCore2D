using UnityEngine;
using SwarmCore2D.World;
using System.Collections.Generic;

namespace SwarmCore2D.Rendering
{
    public class WorldRenderer : MonoBehaviour
    {
        [Header("Ground")]
        public Mesh tileMesh;

        const int BatchSize = 1023;

        Matrix4x4[] batch = new Matrix4x4[BatchSize];

        WorldChunkSystem chunkSystem;
        Camera cam;

        public void Initialize(WorldChunkSystem system)
        {
            chunkSystem = system;
            cam = Camera.main;
        }

        void LateUpdate()
        {
            if (chunkSystem == null || cam == null)
                return;

            Rect view = GetCameraRect();
            view = ExpandRect(view, 2f);

            foreach (var chunk in chunkSystem.GetChunks())
            {
                DrawGround(chunk, view);
                DrawProps(chunk, view);
            }
        }

        Rect GetCameraRect()
        {
            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;

            Vector3 pos = cam.transform.position;

            return new Rect(
                pos.x - width * 0.5f,
                pos.y - height * 0.5f,
                width,
                height
            );
        }

        void DrawGround(WorldChunk chunk, Rect view)
        {
            foreach (var pair in chunk.groundBatches)
            {
                DrawVisible(tileMesh, pair.Key, pair.Value, view);
            }
        }

        void DrawProps(WorldChunk chunk, Rect view)
        {
            foreach (var pair in chunk.propBatches)
            {
                var key = pair.Key;

                if (key.mesh == null || key.material == null)
                    continue;

                DrawVisible(key.mesh, key.material, pair.Value, view);
            }
        }

        void DrawVisible(
            Mesh mesh,
            Material material,
            List<Matrix4x4> matrices,
            Rect view)
        {
            int visibleCount = 0;

            for (int i = 0; i < matrices.Count; i++)
            {
                Vector3 pos = matrices[i].GetColumn(3);

                if (!view.Contains(new Vector2(pos.x, pos.y)))
                    continue;

                batch[visibleCount++] = matrices[i];

                if (visibleCount == BatchSize)
                {
                    Graphics.DrawMeshInstanced(mesh, 0, material, batch, visibleCount);
                    visibleCount = 0;
                }
            }

            if (visibleCount > 0)
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, batch, visibleCount);
            }
        }

        Rect ExpandRect(Rect r, float padding)
        {
            r.xMin -= padding;
            r.xMax += padding;
            r.yMin -= padding;
            r.yMax += padding;
            return r;
        }
    }
}