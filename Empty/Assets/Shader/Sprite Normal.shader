Shader "Bubble/Sprite Normal"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1) // �⺻ ������ ������� ����
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {} // ��� �� �ؽ�ó
        _NormalStrength ("Normal Strength", Range(0.0, 2.0)) = 1.0 // ��� �� ���� ����
        _LightIntensity ("Light Intensity", Range(0.0, 2.0)) = 1.0 // ��ü���� �� ���� ����
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" } // ���� ������ ����

        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off                     
        Cull Off                       

        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc" // Unity �⺻ ��ƿ��Ƽ �Լ� ����
            #include "Lighting.cginc" // ���� ���� ��ƿ��Ƽ �Լ� ���� (��: _LightColor0)

            #pragma vertex vert
            #pragma fragment frag

            // ���̴� ������Ƽ ���� ����
            sampler2D _MainTex;
            float4 _MainTex_ST; 

            sampler2D _NormalMap;
            float _NormalStrength;
            float _LightIntensity;
            float4 _Color;

            // �� ������ (�޽����� ���ؽ� ���̴��� �Է�)
            struct appdata
            {
                float4 vertex : POSITION;   // ���ؽ� ��ġ
                float2 uv : TEXCOORD0;      // UV ��ǥ
                float3 normal : NORMAL;     // ���ؽ� ���
                float4 tangent : TANGENT;   // ���ؽ� ź��Ʈ (��� �ʿ� �ʼ�)
                fixed4 color : COLOR;      // ���ؽ� ����
            };

            // ���ؽ� ���̴����� �����׸�Ʈ ���̴��� ���
            struct v2f
            {
                float4 position : SV_POSITION;  // Ŭ�� ���� ��ġ
                float2 uv : TEXCOORD0;          // UV ��ǥ
                fixed4 color : COLOR;           // ���ؽ� ����
                float3 worldNormal : TEXCOORD1; // ���� ���� ��� (���̽ý� ��ȯ��)
                float3 worldTangent : TEXCOORD2; // ���� ���� ź��Ʈ (���̽ý� ��ȯ��)
                float3 worldBinormal : TEXCOORD3; // ���� ���� ���̳�� (���̽ý� ��ȯ��)
                float3 worldPos : TEXCOORD4;    // ���� ���� ��ġ (���� ��꿡 ���)
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex); // ������Ʈ ���� -> Ŭ�� ���� ��ȯ
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);       // UV ��ȯ

                o.color = v.color; // ���ؽ� �÷� ����

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * v.tangent.w;

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // ���� ���� ��ġ
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. ���� �ؽ�ó �� ���� ����
                fixed4 mainTexColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = mainTexColor * _Color * i.color;

                // 2. ��� �� ���ø� �� ���� (ź��Ʈ ���� ��� ���)
                fixed4 normalMapSample = tex2D(_NormalMap, i.uv);
                float3 tangentSpaceNormal = UnpackNormal(normalMapSample);

                // 3. ��� �� ���� ���� �� ����ȭ
                tangentSpaceNormal.xy *= _NormalStrength;
                tangentSpaceNormal = normalize(tangentSpaceNormal);

                // 4. ź��Ʈ ���� ����� ���� ���� ��ַ� ��ȯ
                // ���ؽ� ���̴����� ����� ���̽ý� ����(���� ���, ź��Ʈ, ���̳��)�� ���
                float3 worldNormalFromMap = normalize(
                    i.worldTangent * tangentSpaceNormal.x +
                    i.worldBinormal * tangentSpaceNormal.y +
                    i.worldNormal * tangentSpaceNormal.z
                );

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz); // ���⼺ ������ ����
                
                // saturate�� ���� 0~1 ���̷� �����Ͽ� ���� ���� ���� �ʵ��� �մϴ�.
                float diffuse = saturate(dot(worldNormalFromMap, lightDir));

                // ���� ����� ������ ����
                float3 lightContribution = _LightIntensity;
                float3 lightContribution2 = _LightIntensity * _LightColor0.rgb * diffuse;

                // 6. ���� ���� ���� ���� ����
                finalColor.rgb *= (lightContribution + lightContribution2); // ���� ���� ���� ������ ����
                finalColor.a = i.color.a * _Color.a * mainTexColor.a;
                return finalColor;
            }
            ENDCG
        }
    }
}
