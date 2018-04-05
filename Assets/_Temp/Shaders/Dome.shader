// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New AmplifyShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Visibility("Visibility", Range( 0 , 1)) = 1
		_RimPower("RimPower", Float) = 12
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend One One
		BlendOp Add
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldNormal;
			float3 viewDir;
			float4 screenPos;
			float clampDepth;
		};

		uniform float _Visibility;
		uniform float4 _Color;
		uniform sampler2D _CameraDepthTexture;
		uniform float _RimPower;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.clampDepth = -UnityObjectToViewPos( v.vertex.xyz ).z * _ProjectionParams.w;
		}

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float dotResult52 = dot( ase_vertexNormal , i.viewDir );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float clampDepth22 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float smoothstepResult24 = smoothstep( 0 , ( _ProjectionParams.w * 0.5 ) , ( clampDepth22 - i.clampDepth ));
			float intersect43 = ( 1.0 - smoothstepResult24 );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float temp_output_31_0 = pow( max( ( 1.0 - ( abs( dotResult52 ) * 2 ) ) , intersect43 ) , ( _RimPower * ase_objectScale ).x );
			float4 lerpResult29 = lerp( _Color , float4( float3(1,1,1) , 0.0 ) , temp_output_31_0);
			o.Emission = ( ( _Visibility * ( _Color * _Color.a ) ) + ( _Visibility * lerpResult29 * temp_output_31_0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
916;225;1352;732;119.4962;811.6688;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;46;410.0166,-788.164;Float;False;1237.267;435.7777;Intersect;8;43;25;27;22;26;28;24;23;;1,1,1,1;0;0
Node;AmplifyShaderEditor.NormalVertexDataNode;49;-1805.187,154.858;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenDepthNode;22;468.928,-738.1641;Float;False;1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SurfaceDepthNode;25;436.0164,-644.3866;Float;False;1;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;51;-1844.188,306.8581;Float;True;World;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ProjectionParams;27;491.0163,-554.3868;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;26;726.8593,-695.3673;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;738.017,-553.3868;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;52;-1578.187,217.8582;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;53;-1428.187,219.8582;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;24;909.7972,-628.9082;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-1277.187,224.8582;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;23;1154.899,-605.5614;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-1154.238,461.3975;Float;False;Property;_RimPower;RimPower;3;0;Create;True;0;12;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;55;-1118.187,256.8581;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;-1165.263,355.8759;Float;False;43;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;1404.283,-615.5749;Float;False;intersect;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectScaleNode;58;-1180.238,545.3975;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-959.2375,449.9975;Float;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;42;-928.749,296.6901;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;56;-802.9982,121.3975;Float;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;31;-721.2122,287.9307;Float;True;2;0;FLOAT;0;False;1;FLOAT3;12.17,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-939.4127,-93.07445;Float;False;Property;_Color;Color;1;0;Create;True;0;1,1,1,1;0,0.7931032,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;29;-589.7339,51.80303;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-666.1258,-91.16597;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-766.4738,-203.4281;Float;False;Property;_Visibility;Visibility;2;0;Create;True;0;1;0.02081918;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-356.463,77.32003;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-376.373,-49.67438;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-167.642,19.39951;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;New AmplifyShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;2;0;False;0;0;False;0;Custom;0;True;False;0;True;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;False;4;One;One;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;48;477.685,-260.5518;Float;False;799.8185;257;Color;0;;1,1,1,1;0;0
WireConnection;26;0;22;0
WireConnection;26;1;25;0
WireConnection;28;0;27;4
WireConnection;52;0;49;0
WireConnection;52;1;51;0
WireConnection;53;0;52;0
WireConnection;24;0;26;0
WireConnection;24;2;28;0
WireConnection;54;0;53;0
WireConnection;23;0;24;0
WireConnection;55;0;54;0
WireConnection;43;0;23;0
WireConnection;61;0;57;0
WireConnection;61;1;58;0
WireConnection;42;0;55;0
WireConnection;42;1;45;0
WireConnection;31;0;42;0
WireConnection;31;1;61;0
WireConnection;29;0;1;0
WireConnection;29;1;56;0
WireConnection;29;2;31;0
WireConnection;47;0;1;0
WireConnection;47;1;1;4
WireConnection;40;0;2;0
WireConnection;40;1;29;0
WireConnection;40;2;31;0
WireConnection;3;0;2;0
WireConnection;3;1;47;0
WireConnection;21;0;3;0
WireConnection;21;1;40;0
WireConnection;0;2;21;0
ASEEND*/
//CHKSM=DB6721CAB905065E15F8838A02593DAEBD798A78