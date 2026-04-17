using UnityEngine;
using System.Collections.Generic;
using SwarmCore2D.Simulation;
using SwarmCore2D.ScriptableObjects;

namespace SwarmCore2D.Rendering
{
    public class SwarmRenderer : MonoBehaviour
    {
        public Mesh mesh;

        RenderData renderData;
        InstancedBatcher batcher;

        SwarmState state;
        SwarmRenderProfile renderProfile;

        // Pre-allocated buffers — no GC per frame
        Matrix4x4[] matrices;
        float[] frameBuffer;
        float[] flipBuffer;
        Vector4[] tintBuffer;
        MaterialPropertyBlock props;

        Dictionary<Material, List<int>> groups;

        public void Initialize(SwarmState state, SwarmRenderProfile profile = null)
        {
            this.state = state;
            this.renderProfile = profile;

            int batchSize = profile != null ? profile.batchSize : 1023;
            int maxCap = profile != null ? profile.maxVisibleEntities : SwarmCore2D.Core.SwarmConstants.MaxEntities;

            renderData = new RenderData();
            batcher = new InstancedBatcher(mesh, batchSize);

            matrices    = new Matrix4x4[maxCap];
            frameBuffer = new float[maxCap];
            flipBuffer  = new float[maxCap];
            tintBuffer  = new Vector4[maxCap];
            props = new MaterialPropertyBlock();

            groups = new Dictionary<Material, List<int>>();
        }

        void LateUpdate()
        {
            if (state == null)
                return;

            int maxVisible = renderProfile != null ? renderProfile.maxVisibleEntities : int.MaxValue;
            bool cull = renderProfile != null && renderProfile.useFrustumCulling;
            Rect bounds = cull ? GetCameraBounds() : default;

            renderData.Build(state, maxVisible, cull, bounds);

            GroupByMaterial();
            DrawGroups();
        }

        Rect GetCameraBounds(float padding = 1.5f)
        {
            var cam = Camera.main;
            if (cam == null) return default;

            float halfH = cam.orthographicSize + padding;
            float halfW = halfH * cam.aspect + padding;
            Vector2 camPos = cam.transform.position;

            return new Rect(camPos.x - halfW, camPos.y - halfH, halfW * 2f, halfH * 2f);
        }

        void GroupByMaterial()
        {
            groups.Clear();

            for (int i = 0; i < renderData.count; i++)
            {
                int type = renderData.enemyType[i];
                var data = EnemyDatabase.Instance.Get(type);

                if (data == null || data.material == null)
                    continue;

                if (!groups.ContainsKey(data.material))
                    groups[data.material] = new List<int>();

                groups[data.material].Add(i);
            }
        }

        void DrawGroups()
        {
            foreach (var pair in groups)
            {
                Material mat = pair.Key;
                var list = pair.Value;
                int count = list.Count;

                for (int i = 0; i < count; i++)
                {
                    int idx = list[i];
                    matrices[i]    = renderData.matrices[idx];  // includes position + scale
                    frameBuffer[i] = renderData.frames[idx];
                    flipBuffer[i]  = renderData.flips[idx];
                    tintBuffer[i]  = renderData.tint[idx];
                }

                batcher.Draw(mat, matrices, frameBuffer, flipBuffer, tintBuffer, count, props);
            }
        }
    }
}
