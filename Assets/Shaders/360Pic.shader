Shader "Custom/360Pic"
{
    Properties
    {
        _MainTex ("360 Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }
        
        Pass
        {
            Cull Front // Render inside of sphere
            
            Stencil {
                Ref 1
                Comp Equal
                Pass Keep
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
            };

            sampler2D _MainTex;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionOS = IN.positionOS.xyz;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Convert position to spherical coordinates
                float3 dir = normalize(IN.positionOS);
                float2 uv;
                uv.x = atan2(dir.z, dir.x) / (2.0 * PI) + 0.5;
                uv.y = asin(dir.y) / PI + 0.5;
                
                return tex2D(_MainTex, uv);
            }
            ENDHLSL
        }
    }
}