Shader "Swarm/SpriteInstanced"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FrameCount ("Frame Count", Float) = 7
        _Tint ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _FrameCount;

            UNITY_INSTANCING_BUFFER_START(Props)

                UNITY_DEFINE_INSTANCED_PROP(float, _Frame)
                UNITY_DEFINE_INSTANCED_PROP(float, _Flip)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Tint)

            UNITY_INSTANCING_BUFFER_END(Props)

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float frame = UNITY_ACCESS_INSTANCED_PROP(Props, _Frame);
                float flip = UNITY_ACCESS_INSTANCED_PROP(Props, _Flip);
                float4 tint = UNITY_ACCESS_INSTANCED_PROP(Props, _Tint);

                float2 uv = IN.uv;

                if (flip > 0.5)
                    uv.x = 1 - uv.x;

                uv.x = (uv.x + frame) / _FrameCount;

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                return col * tint;
            }

            ENDHLSL
        }
    }
}