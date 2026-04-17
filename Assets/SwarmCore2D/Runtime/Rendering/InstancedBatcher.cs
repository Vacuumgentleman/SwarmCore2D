using UnityEngine;

public class InstancedBatcher
{
    readonly int batchSize;

    Mesh mesh;

    Matrix4x4[] batchMatrices;
    float[] batchFrames;
    float[] batchFlips;
    Vector4[] batchTint;

    public InstancedBatcher(Mesh mesh, int batchSize = 1023)
    {
        this.mesh = mesh;
        this.batchSize = Mathf.Clamp(batchSize, 1, 1023);

        batchMatrices = new Matrix4x4[this.batchSize];
        batchFrames = new float[this.batchSize];
        batchFlips = new float[this.batchSize];
        batchTint = new Vector4[this.batchSize];
    }

    public void Draw(
        Material material,
        Matrix4x4[] matrices,
        float[] frames,
        float[] flips,
        Vector4[] tint,
        int count,
        MaterialPropertyBlock props
    )
    {
        int index = 0;

        while (index < count)
        {
            int batchCount = Mathf.Min(batchSize, count - index);

            for (int i = 0; i < batchCount; i++)
            {
                batchMatrices[i] = matrices[index + i];
                batchFrames[i] = frames[index + i];
                batchFlips[i] = flips[index + i];
                batchTint[i] = tint[index + i];
            }

            props.Clear();

            props.SetFloatArray("_Frame", batchFrames);
            props.SetFloatArray("_Flip", batchFlips);
            props.SetVectorArray("_Tint", batchTint);

            Graphics.DrawMeshInstanced(
                mesh,
                0,
                material,
                batchMatrices,
                batchCount,
                props
            );

            index += batchCount;
        }
    }
}
