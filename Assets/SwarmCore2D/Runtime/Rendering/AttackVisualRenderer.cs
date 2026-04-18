using UnityEngine;
using System.Collections.Generic;
using SwarmCore2D.Combat;

namespace SwarmCore2D.Rendering
{
    public class AttackVisualRenderer : MonoBehaviour
    {
        public Mesh mesh;

        AttackVisualSystem system;

        Matrix4x4[] matrices    = new Matrix4x4[AttackVisualState.Capacity];
        float[]     frameBuffer = new float[AttackVisualState.Capacity];
        MaterialPropertyBlock props;

        Dictionary<Material, List<int>> groups = new Dictionary<Material, List<int>>();

        public void Initialize(AttackVisualSystem sys)
        {
            system = sys;
            props = new MaterialPropertyBlock();
        }

        void Start()
        {
            props = new MaterialPropertyBlock();
        }

        void LateUpdate()
        {
            if (system == null)
            {
                var controller = FindFirstObjectByType<PlayerWeaponController>();
                if (controller?.AttackVisualSystem != null)
                    Initialize(controller.AttackVisualSystem);
                else
                    return;
            }

            if (mesh == null) return;

            var s = system.state;

            foreach (var list in groups.Values)
                list.Clear();

            for (int a = 0; a < s.activeCount; a++)
            {
                int id = s.activeList[a];
                var mat = s.material[id];
                if (mat == null) continue;

                if (!groups.TryGetValue(mat, out var list))
                {
                    list = new List<int>();
                    groups[mat] = list;
                }
                list.Add(id);
            }

            foreach (var kvp in groups)
            {
                var mat = kvp.Key;
                var ids = kvp.Value;
                int count = ids.Count;
                if (count == 0) continue;

                for (int i = 0; i < count; i++)
                {
                    int id = ids[i];
                    float scale = s.scales[id];
                    Quaternion rot = Quaternion.Euler(0f, 0f, s.angles[id]);
                    matrices[i] = Matrix4x4.TRS(
                        new Vector3(s.positions[id].x, s.positions[id].y, 0f),
                        rot,
                        new Vector3(scale, scale, scale));
                    frameBuffer[i] = s.frame[id];
                }

                props.SetFloatArray("_Frame", frameBuffer);
                Graphics.DrawMeshInstanced(mesh, 0, mat, matrices, count, props);
            }
        }
    }
}
