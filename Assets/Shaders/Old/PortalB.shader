Shader "Custom/PortalB"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius", Float) = 0.5
        _HaloWidth ("Halo Width", Float) = 0.1  // Width of the halo
        _HaloColor ("Halo Color", Color) = (1, 0.65, 0, 1)  // Orange color
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Front 
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Radius;
            float _HaloWidth;
            half4 _HaloColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.position = TransformObjectToHClip(IN.position.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float dist = length((IN.uv - 0.5) * 1);  // Calculate distance from the center
                float innerEdge = _Radius - _HaloWidth;
                float outerEdge = _Radius;
                float alpha = smoothstep(innerEdge, outerEdge, dist);  // Transition for halo
                float circleAlpha = step(dist, innerEdge);  // Alpha for inner circle

                half4 haloColor = _HaloColor * (1.0 - alpha);  // Apply halo color
                half4 circleColor = half4(1, 1, 1, circleAlpha);  // White circle

                // Combine halo and circle
                half4 finalColor = lerp(haloColor, circleColor, circleAlpha);
                return finalColor;
            }
            ENDHLSL
        }
    }
}
