Shader "Custom/LampShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _MaskTex ("Mask (A)", 2D) = "white" {}
        _MaskPos ("Mask Position", Vector) = (0,0,100,100)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha:blend

        sampler2D _MainTex;
        sampler2D _MaskTex;
        float4 _MaskPos; // Position et taille du masque

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            float2 maskUV = (IN.uv_MainTex - _MaskPos.xy) / _MaskPos.zw;
            fixed4 mask = tex2D(_MaskTex, maskUV);
            o.Albedo = c.rgb;
            o.Alpha = c.a * mask.a; // Inverser le masque pour rendre la zone éclairée transparente
        }
        ENDCG
    }
    FallBack "Diffuse"
}