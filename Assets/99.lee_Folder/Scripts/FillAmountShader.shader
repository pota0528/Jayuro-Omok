Shader "Custom/FillAmountShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _FillAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float angle = atan2(i.uv.y - center.y, i.uv.x - center.x);

                // 각도를 12시 방향 기준으로 조정
                angle = angle - (0.5 * 3.14159); // π/2를 뺌
                if (angle < 0.0)
                    angle += 2.0 * 3.14159; // 음수를 양수로 변환

                angle = angle / (2.0 * 3.14159); // 0~1로 정규화

                if ((1.0 - angle) > _FillAmount) // 시계 방향 기준
                    discard;

                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}