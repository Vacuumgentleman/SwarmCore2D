using UnityEngine;

namespace SwarmCore2D.Rendering
{
    public class InstancedBatcher
    {
        const int BatchSize = 1023;

        Mesh mesh;
        Material material;

        MaterialPropertyBlock props;

        public InstancedBatcher(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;

            props = new MaterialPropertyBlock();
        }

        public void Draw(
            Matrix4x4[] matrices,
            float[] frames,
            float[] flips,
            Vector4[] tint,
            int count)
        {
            int index = 0;

            while (index < count)
            {
                int batch = Mathf.Min(BatchSize, count - index);

                props.Clear();

                props.SetFloatArray("_Frame", frames);
                props.SetFloatArray("_Flip", flips);
                props.SetVectorArray("_Tint", tint);

                Graphics.DrawMeshInstanced(
                    mesh,
                    0,
                    material,
                    matrices,
                    batch,
                    props,
                    UnityEngine.Rendering.ShadowCastingMode.Off,
                    false,
                    0,
                    null,
                    UnityEngine.Rendering.LightProbeUsage.Off,
                    null
                );

                index += batch;
            }
        }
    }
}