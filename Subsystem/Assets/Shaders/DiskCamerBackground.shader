Shader "Unlit/DiskCameraBackground"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Background" }
        Pass
        {
            ZWrite Off Cull Off ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
            v2f vert(uint id: SV_VertexID)
            {
                float2 verts[4] = { float2(-1,-1), float2(1,-1), float2(1,1), float2(-1,1) };
                float2 uvs[4] = { float2(0,0), float2(1,0), float2(1,1), float2(0,1) };
                v2f o; o.pos = float4(verts[id],0,1); o.uv = uvs[id]; return o;
            }
            fixed4 frag(v2f i) :SV_Target { return tex2D(_MainTex, i.uv); }
            ENDCG
        }
    }
        Fallback Off
}
