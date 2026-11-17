Shader "Custom/TilingNormalMappedSurface"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Strength", Range(0,2)) = 1.0
        _TileScale ("Tile Scale", Range(0.1, 50)) = 1.0
        _UseWorldSpace ("Use World Space", Float) = 1.0
        [Toggle] _UseTriplanar ("Use Triplanar Mapping", Float) = 0.0
        _TriplanarBlend ("Triplanar Blend", Range(0.1, 10)) = 1.0
        _Color ("Color Tint", Color) = (1,1,1,1)
        
        // PBR Properties
        _Glossiness ("Smoothness", Range(0,1)) = 0.2
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma shader_feature _USETRIPLANAR_ON
        #pragma target 3.0
        
        sampler2D _MainTex;
        sampler2D _BumpMap;
        float _BumpScale;
        float _TileScale;
        float _UseWorldSpace;
        float _UseTriplanar;
        float _TriplanarBlend;
        fixed4 _Color;
        
        // PBR Properties
        half _Glossiness;
        half _Metallic;
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };
        
        void vert (inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
        }
        
        fixed4 TriplanarSample(sampler2D tex, float3 worldPos, float3 worldNormal, float scale)
        {
            float2 uvX = worldPos.zy * scale;
            float2 uvY = worldPos.xz * scale;
            float2 uvZ = worldPos.xy * scale;
            
            fixed4 texX = tex2D(tex, uvX);
            fixed4 texY = tex2D(tex, uvY);
            fixed4 texZ = tex2D(tex, uvZ);
            
            float3 blend = pow(abs(worldNormal), _TriplanarBlend);
            blend = blend / (blend.x + blend.y + blend.z);
            
            return texX * blend.x + texY * blend.y + texZ * blend.z;
        }
        
        float3 TriplanarNormalSample(sampler2D normalMap, float3 worldPos, float3 worldNormal, float scale)
        {
            float3 normalX = UnpackNormal(tex2D(normalMap, worldPos.zy * scale));
            float3 normalY = UnpackNormal(tex2D(normalMap, worldPos.xz * scale));
            float3 normalZ = UnpackNormal(tex2D(normalMap, worldPos.xy * scale));
            
            normalX = float3(normalX.xy + worldNormal.zy, abs(normalX.z) * worldNormal.x);
            normalY = float3(normalY.xy + worldNormal.xz, abs(normalY.z) * worldNormal.y);
            normalZ = float3(normalZ.xy + worldNormal.xy, abs(normalZ.z) * worldNormal.z);
            
            float3 blend = pow(abs(worldNormal), _TriplanarBlend);
            blend = blend / (blend.x + blend.y + blend.z);
            
            return normalize(normalX * blend.x + normalY * blend.y + normalZ * blend.z);
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c;
            float3 n;
            
            if (_UseTriplanar > 0.5)
            {
                c = TriplanarSample(_MainTex, IN.worldPos, IN.worldNormal, _TileScale) * _Color;
                n = TriplanarNormalSample(_BumpMap, IN.worldPos, IN.worldNormal, _TileScale);
                n.xy *= _BumpScale;
            }
            else
            {
                float2 texCoords;
                
                if (_UseWorldSpace > 0.5)
                {
                    texCoords = IN.worldPos.xz * _TileScale;
                }
                else
                {
                    texCoords = IN.uv_MainTex * _TileScale;
                }
                
                c = tex2D(_MainTex, texCoords) * _Color;
                n = UnpackNormal(tex2D(_BumpMap, texCoords));
                n.xy *= _BumpScale;
            }
            
            o.Albedo = c.rgb;
            o.Normal = normalize(n);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}