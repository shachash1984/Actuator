Shader "Custom/ApplyOutline"
{
    Properties
	{
		_DiffuseTexture("Diffuse Texture (RGB)", 2D) = "white" {} // Texture peoperty
	    _Color("Color", Color) = (1,1,1,1) // Color Property
		_BumpMap("Bump", 2D) = "bump" {}

		_OutlineTexture("Outline Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineWidth("Outline Width", Range(1.0,1.1)) = 1.1
	}

	SubShader
	{
		tags
		{
			"Queue" = "Transparent"
        }

		Pass
		{
			Name "Outline"

			ZWrite off

			CGPROGRAM // Allows talk between two languages: shader lab and NVidia C for graphics
			// Function Defines
			#pragma vertex vert // Define for the building function.

			#pragma fragment frag // Define for coloring funtion

			// Includes
			#include "UnityCG.cginc" // Built in shader functions

			// Structures
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// Imports
			float _OutlineWidth;
			float4 _OutlineColor;
			sampler2D _OutlineTexture;

			// Vertex funtion
			v2f vert(appdata IN)
			{
				IN.vertex.xyz *= _OutlineWidth;
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;

				return OUT;
			}

			// Fragment funtion
			fixed4 frag(v2f IN) : SV_Target
			{
				float4 texColor = tex2D(_OutlineTexture, IN.uv);
				return texColor * _OutlineColor;
			}

			ENDCG
        }

		Pass
		{
			Name "Object"
			CGPROGRAM // Allows talk between two languages: shader lab and NVidia C for graphics
            // Function Defines
            #pragma vertex vert // Define for the building function.

            #pragma fragment frag // Define for coloring funtion

			// Includes
            #include "UnityCG.cginc" // Built in shader functions

			// Structures
			struct appdata
		    {
			    float4 vertex : POSITION;
			    float2 uv : TEXCOORD;
				float4 tangent : TANGENT;
            };

		    struct v2f
		    {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalWorld : TEXCOORD1;
				float3 tangentWorld: TEXCOORD2;
				float3 binormalWorld: TEXTCOORD3;
			};

			// Imports
			float4 _Color;
			sampler2D _DiffuseTexture;
			uniform sampler2D _BumpMap;
			uniform float4 _BumpMap_ST;

			// Vertex funtion
			v2f vert(appdata IN)
			{
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				
				//OUT.tangentWorld = normalize(float3(mul(unity_ObjectToWorld, float4(float3(IN.tangent.xyz), 0.0)).xyz));
				//OUT.normalWorld = normalize(mul(float4(IN.normal.xyz, 0.0)._World2Object).xyz);
				//OUT.binormalWorld = normalize(cross(OUT.normalWorld, OUT.tangentWorld).xyz * IN.tangent.x);

				OUT.uv = IN.uv;

				return OUT;
			}

			// Fragment funtion
			fixed4 frag(v2f IN) : SV_Target
			{
				float4 texColor = tex2D(_DiffuseTexture, IN.uv);
				//float4 texN = tex2D(_BumpMap, BumpMap_ST.xy * INtex.xy + _BumpMap_ST.zw);
				//float3 localCoords = float3(2.0 * texN.ag - float2(1.0, 1.0).0.0);
				//localCoords.z = 1.0 - 0.5 * dot(localCoords, localCoords);
				return texColor * _Color;
			}

			ENDCG
        }
	}
}
