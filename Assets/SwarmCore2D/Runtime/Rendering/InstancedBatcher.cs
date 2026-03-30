using UnityEngine;

public class InstancedBatcher
{
    const int BATCH_SIZE = 1023;

    Mesh mesh;

    Matrix4x4[] batchMatrices;
    float[] batchFrames;
    float[] batchFlips;
    Vector4[] batchTint;

    public InstancedBatcher(Mesh mesh)
    {
        this.mesh = mesh;

        batchMatrices = new Matrix4x4[BATCH_SIZE];
        batchFrames = new float[BATCH_SIZE];
        batchFlips = new float[BATCH_SIZE];
        batchTint = new Vector4[BATCH_SIZE];
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
            int batchCount = Mathf.Min(BATCH_SIZE, count - index);

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