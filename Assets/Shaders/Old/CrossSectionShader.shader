Shader "Custom/CrossSectionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SectionPlane ("Section Plane", Vector) = (0, 1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        float4 _SectionPlane;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float d = dot(float4(IN.worldPos, 1), _SectionPlane);
            if (d > 0)
            {
                discard;
            }

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
