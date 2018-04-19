// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Oction/Holographic/Cast"
{
	Properties
	{
		_Color0("Color 0", Color) = (0,0.8758621,1,1)
		_Color1("Color 1", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _Color1;
		uniform float4 _Color0;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float smoothstepResult21 = smoothstep( 0.75 , 0.95 , ( 1.0 - ase_vertex3Pos.y ));
			float temp_output_7_0 = saturate( smoothstepResult21 );
			float4 lerpResult12 = lerp( _Color1 , _Color0 , temp_output_7_0);
			o.Emission = lerpResult12.rgb;
			o.Alpha = temp_output_7_0;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
1107;230;1352;732;1137.223;176.398;1;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;6;-998.2234,85.60196;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;9;-766.2234,114.602;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;21;-554.2234,135.602;Float;True;3;0;FLOAT;0;False;1;FLOAT;0.75;False;2;FLOAT;0.95;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;7;-269.2234,229.602;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-682.2234,-130.398;Float;False;Property;_Color0;Color 0;1;0;Create;True;0;0,0.8758621,1,1;0,0.8758621,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-644.2234,-267.398;Float;False;Property;_Color1;Color 1;2;0;Create;True;0;0,0,0,0;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;12;-305.2234,-162.398;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;5;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Oction/Holographic/Cast;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;Off;0;0;False;0;0;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;6;2
WireConnection;21;0;9;0
WireConnection;7;0;21;0
WireConnection;12;0;13;0
WireConnection;12;1;2;0
WireConnection;12;2;7;0
WireConnection;5;2;12;0
WireConnection;5;9;7;0
ASEEND*/
//CHKSM=C866A3E443F2A2BED209969AB082158E54E86144