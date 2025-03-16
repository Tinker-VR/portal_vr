Shader "Custom/OldPortal360"
{
    Properties
    {
        _MainTex ("360 Texture (Equirectangular)", 2D) = "white" {}
        _MaskRadius ("Mask Radius", Float) = 0.9   // Adjust up to 1.0 to fill the quad
        _Feather ("Feather", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #ifndef UNITY_PI
            #define UNITY_PI 3.14159265359
            #endif

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // We'll output the world position for each vertex as well as a remapped UV for masking.
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0; // For computing view direction
                float2 uvLocal    : TEXCOORD1; // For circular mask
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _MaskRadius;
            float _Feather;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.vertex.xyz);
                // Compute the world position of the vertex.
                OUT.worldPos = TransformObjectToWorld(IN.vertex.xyz);
                // Remap the mesh UV from [0,1] to [-1,1]. (Assumes the quad UV covers the entire window.)
                OUT.uvLocal = IN.uv * 2.0 - 1.0;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // --- Circular Mask ---
                float mask = 1.0 - smoothstep(_MaskRadius - _Feather, _MaskRadius, length(IN.uvLocal));
                clip(mask - 0.01);

                // --- Compute View Direction ---
                // The direction from the camera to this fragment (in world space) naturally reflects the portal’s scale.
                float3 viewDir = normalize(IN.worldPos - _WorldSpaceCameraPos);

                // --- Spherical Mapping ---
                // Convert the view direction into equirectangular (spherical) UV coordinates.
                float u = (atan2(viewDir.z, viewDir.x) + UNITY_PI) / (2.0 * UNITY_PI);
                float v = (asin(viewDir.y) + (UNITY_PI / 2.0)) / UNITY_PI;
                float2 sphereUV = float2(u, v);

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sphereUV);
                col.a *= mask;
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Transparent/Diffuse"
}
