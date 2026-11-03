Shader "UI/VideoGlitch"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Intensity ("Intensidad", Range(0,1)) = 0.5
        _Bands ("Cantidad de bandas", Float) = 30
        _Aberration ("Separacion RGB", Range(0,0.05)) = 0.01
        _T ("Tiempo (lo setea el script)", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Intensity, _Bands, _Aberration, _T;

            float hash(float n) { return frac(sin(n * 127.1) * 43758.5453); }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = floor(_T * 12.0);                  
                float band = floor(i.uv.y * _Bands);         
                float r = hash(band + t * 17.0);           
                float glitchOn = step(1.0 - _Intensity * 0.5, r); 
                float shift = (hash(band * 3.1 + t) - 0.5) * 0.2 * _Intensity * glitchOn;

                float2 uv = i.uv;
                uv.x = frac(uv.x + shift);                   

                float ab = _Aberration * _Intensity * (1.0 + glitchOn * 3.0);
                fixed r_ = tex2D(_MainTex, uv + float2(ab, 0)).r;
                fixed4 g_ = tex2D(_MainTex, uv);
                fixed b_ = tex2D(_MainTex, uv - float2(ab, 0)).b;

                return fixed4(r_, g_.g, b_, g_.a) * i.color;
            }
            ENDCG
        }
    }
}
