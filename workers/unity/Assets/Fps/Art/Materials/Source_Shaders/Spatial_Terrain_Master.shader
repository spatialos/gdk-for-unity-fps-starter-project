// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:35025,y:32908,varname:node_4013,prsc:2|diff-4543-OUT,spec-3356-OUT,gloss-8471-OUT,normal-6239-OUT,emission-9539-OUT,amdfl-322-RGB,amspl-322-RGB,clip-8012-OUT;n:type:ShaderForge.SFN_Tex2d,id:8520,x:31981,y:32940,ptovrint:False,ptlb:T_Main,ptin:_T_Main,cmnt:Unpack channels,varname:_T_Main,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:44c4899883df42548bfbcc4119521f8d,ntxv:0,isnm:False|UVIN-883-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3862,x:31981,y:33162,ptovrint:False,ptlb:T_Normal,ptin:_T_Normal,varname:_T_Normal,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:344e82c3f8c38814493a68ebdd13a446,ntxv:3,isnm:True|UVIN-883-UVOUT;n:type:ShaderForge.SFN_FragmentPosition,id:5923,x:30437,y:32918,varname:node_5923,prsc:2;n:type:ShaderForge.SFN_Append,id:2160,x:30626,y:32937,varname:node_2160,prsc:2|A-5923-X,B-5923-Z;n:type:ShaderForge.SFN_Multiply,id:4191,x:30907,y:32937,varname:node_4191,prsc:0|A-2160-OUT,B-3181-OUT;n:type:ShaderForge.SFN_Vector1,id:3181,x:30746,y:32971,varname:node_3181,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Add,id:926,x:31108,y:32965,cmnt:UVs projected down Y axis,varname:node_926,prsc:0|A-4191-OUT,B-3436-OUT;n:type:ShaderForge.SFN_Tex2d,id:9061,x:31981,y:32698,ptovrint:False,ptlb:T_Alpha,ptin:_T_Alpha,varname:_T_Alpha,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:65559dfd0c0534545bb0a56b26efd2d0,ntxv:3,isnm:False|UVIN-8489-OUT;n:type:ShaderForge.SFN_Divide,id:8489,x:31787,y:32698,varname:node_8489,prsc:0|A-3597-OUT,B-1579-OUT;n:type:ShaderForge.SFN_Vector1,id:1579,x:31613,y:32731,varname:node_1579,prsc:2,v1:9;n:type:ShaderForge.SFN_Add,id:3597,x:31468,y:32697,cmnt:UVs for pattern alpha,varname:node_3597,prsc:2|A-9399-OUT,B-8631-OUT;n:type:ShaderForge.SFN_Vector2,id:8631,x:31280,y:32727,varname:node_8631,prsc:0,v1:-0.5,v2:-0.5;n:type:ShaderForge.SFN_ValueProperty,id:509,x:30485,y:33215,ptovrint:False,ptlb:Proj_Offset_X,ptin:_Proj_Offset_X,varname:_Proj_Offset_X,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:1412,x:30485,y:33340,ptovrint:False,ptlb:Proj_Offset_Y,ptin:_Proj_Offset_Y,varname:_Proj_Offset_Y,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Append,id:3436,x:30781,y:33164,varname:node_3436,prsc:0|A-509-OUT,B-1412-OUT;n:type:ShaderForge.SFN_Lerp,id:5384,x:33075,y:32539,varname:node_5384,prsc:2|A-6783-RGB,B-3802-RGB,T-9061-R;n:type:ShaderForge.SFN_Color,id:6783,x:32760,y:32223,ptovrint:False,ptlb:Colour_Tint_Primary,ptin:_Colour_Tint_Primary,varname:_Colour_Tint_Primary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3,c2:0.3,c3:0.3,c4:1;n:type:ShaderForge.SFN_Color,id:3802,x:32760,y:32433,ptovrint:False,ptlb:Colour_Tint_Secondary,ptin:_Colour_Tint_Secondary,varname:_Colour_Tint_Secondary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.45,c2:0.45,c3:0.45,c4:1;n:type:ShaderForge.SFN_Multiply,id:4543,x:33970,y:32728,varname:node_4543,prsc:2|A-5384-OUT,B-8545-R;n:type:ShaderForge.SFN_Vector1,id:3356,x:33970,y:33112,varname:node_3356,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:3288,x:34108,y:33175,varname:node_3288,prsc:2|A-9865-OUT,B-1469-OUT;n:type:ShaderForge.SFN_AmbientLight,id:322,x:33970,y:32973,varname:node_322,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:5514,x:30916,y:33661,varname:node_5514,prsc:2;n:type:ShaderForge.SFN_Append,id:8628,x:31099,y:33682,varname:node_8628,prsc:2|A-5514-X,B-5514-Z;n:type:ShaderForge.SFN_Multiply,id:1175,x:31380,y:33682,varname:node_1175,prsc:0|A-8628-OUT,B-7574-OUT;n:type:ShaderForge.SFN_Vector1,id:7574,x:31219,y:33716,varname:node_7574,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Add,id:7142,x:31551,y:33666,varname:node_7142,prsc:0|A-3436-OUT,B-1175-OUT;n:type:ShaderForge.SFN_Add,id:9514,x:31458,y:33464,varname:node_9514,prsc:2|A-9399-OUT,B-1129-OUT;n:type:ShaderForge.SFN_Vector1,id:1129,x:31301,y:33528,varname:node_1129,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Round,id:8950,x:31758,y:33495,varname:node_8950,prsc:2|IN-9514-OUT;n:type:ShaderForge.SFN_Multiply,id:6610,x:31982,y:33495,varname:node_6610,prsc:2|A-8950-OUT,B-479-OUT;n:type:ShaderForge.SFN_Vector1,id:479,x:31836,y:33548,varname:node_479,prsc:2,v1:4;n:type:ShaderForge.SFN_Subtract,id:5488,x:32256,y:33495,varname:node_5488,prsc:0|A-6610-OUT,B-252-OUT;n:type:ShaderForge.SFN_Vector1,id:252,x:32090,y:33548,varname:node_252,prsc:2,v1:2;n:type:ShaderForge.SFN_Distance,id:8146,x:32447,y:33651,cmnt:Rounded radial distance,varname:node_8146,prsc:2|A-5488-OUT,B-5169-OUT;n:type:ShaderForge.SFN_Multiply,id:599,x:32627,y:33488,varname:node_599,prsc:2|A-1390-OUT,B-8146-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1390,x:32466,y:33430,ptovrint:False,ptlb:Step_On_Radius,ptin:_Step_On_Radius,varname:_Step_On_Radius,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:7876,x:32977,y:33488,varname:node_7876,prsc:2|A-3173-OUT,B-3694-OUT;n:type:ShaderForge.SFN_Vector1,id:3694,x:32719,y:33660,varname:node_3694,prsc:2,v1:4;n:type:ShaderForge.SFN_OneMinus,id:3173,x:32802,y:33488,varname:node_3173,prsc:2|IN-599-OUT;n:type:ShaderForge.SFN_Clamp01,id:1469,x:33133,y:33488,varname:node_1469,prsc:2|IN-7876-OUT;n:type:ShaderForge.SFN_Vector1,id:7934,x:31836,y:33726,varname:node_7934,prsc:2,v1:4;n:type:ShaderForge.SFN_Multiply,id:5169,x:31982,y:33673,varname:node_5169,prsc:2|A-7142-OUT,B-7934-OUT;n:type:ShaderForge.SFN_Color,id:8326,x:33073,y:32727,ptovrint:False,ptlb:Emissive_Colour_Primary,ptin:_Emissive_Colour_Primary,varname:_Emissive_Colour_Primary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:9992,x:33073,y:32935,ptovrint:False,ptlb:Emissive_Colour_Secondary,ptin:_Emissive_Colour_Secondary,varname:_Emissive_Colour_Secondary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:2956,x:33320,y:32805,varname:node_2956,prsc:2|A-8326-RGB,B-9992-RGB,T-9061-R;n:type:ShaderForge.SFN_Multiply,id:9865,x:33673,y:33072,varname:node_9865,prsc:2|A-2956-OUT,B-8545-B;n:type:ShaderForge.SFN_Lerp,id:8975,x:33684,y:32874,varname:node_8975,prsc:2|A-2870-OUT,B-8228-OUT,T-9061-R;n:type:ShaderForge.SFN_Power,id:8471,x:33970,y:32844,varname:node_8471,prsc:2|VAL-8545-G,EXP-8975-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2870,x:33481,y:32874,ptovrint:False,ptlb:Gloss_Pow_Primary,ptin:_Gloss_Pow_Primary,varname:_Gloss_Pow_Primary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:8228,x:33481,y:32983,ptovrint:False,ptlb:Gloss_Pow_Secondary,ptin:_Gloss_Pow_Secondary,varname:_Gloss_Pow_Secondary,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_TexCoord,id:883,x:31499,y:32949,varname:node_883,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:9399,x:31300,y:33004,varname:node_9399,prsc:2|A-926-OUT,B-2921-OUT;n:type:ShaderForge.SFN_NormalVector,id:5140,x:30723,y:33496,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:2921,x:31169,y:33278,varname:node_2921,prsc:2|A-7377-OUT,B-3178-OUT;n:type:ShaderForge.SFN_Vector1,id:3178,x:31006,y:33580,varname:node_3178,prsc:2,v1:-0.1;n:type:ShaderForge.SFN_ComponentMask,id:7377,x:30938,y:33461,varname:node_7377,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-5140-OUT;n:type:ShaderForge.SFN_Lerp,id:6239,x:32613,y:33156,varname:node_6239,prsc:2|A-3862-RGB,B-2428-RGB,T-590-OUT;n:type:ShaderForge.SFN_Tex2d,id:5925,x:32208,y:33012,ptovrint:False,ptlb:T_Sides,ptin:_T_Sides,cmnt:Unpack channels,varname:_T_Sides,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:44c4899883df42548bfbcc4119521f8d,ntxv:0,isnm:False|UVIN-863-OUT;n:type:ShaderForge.SFN_Tex2d,id:2428,x:32208,y:33243,ptovrint:False,ptlb:T_Sides_Normal,ptin:_T_Sides_Normal,varname:_T_Sides_Normal,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:344e82c3f8c38814493a68ebdd13a446,ntxv:3,isnm:True|UVIN-863-OUT;n:type:ShaderForge.SFN_Lerp,id:8417,x:32483,y:32823,varname:node_8417,prsc:2|A-8520-RGB,B-5925-RGB,T-590-OUT;n:type:ShaderForge.SFN_Sign,id:7398,x:32160,y:32818,varname:node_7398,prsc:2|IN-883-V;n:type:ShaderForge.SFN_ComponentMask,id:8545,x:32647,y:32823,varname:node_8545,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-8417-OUT;n:type:ShaderForge.SFN_Clamp01,id:590,x:32304,y:32788,varname:node_590,prsc:0|IN-7398-OUT;n:type:ShaderForge.SFN_Multiply,id:1352,x:31640,y:33208,varname:node_1352,prsc:0|A-883-UVOUT,B-4410-OUT;n:type:ShaderForge.SFN_Vector2,id:4410,x:31485,y:33253,varname:node_4410,prsc:0,v1:1,v2:0.5;n:type:ShaderForge.SFN_Add,id:863,x:31823,y:33246,varname:node_863,prsc:0|A-1352-OUT,B-601-OUT;n:type:ShaderForge.SFN_Vector2,id:601,x:31600,y:33341,varname:node_601,prsc:0,v1:0,v2:0.5;n:type:ShaderForge.SFN_FragmentPosition,id:7454,x:32877,y:33739,varname:node_7454,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:9970,x:32877,y:33858,varname:node_9970,prsc:2;n:type:ShaderForge.SFN_Distance,id:2382,x:33281,y:33835,varname:node_2382,prsc:2|A-4840-OUT,B-2824-OUT;n:type:ShaderForge.SFN_Append,id:4840,x:33115,y:33749,varname:node_4840,prsc:2|A-7454-X,B-7454-Z;n:type:ShaderForge.SFN_Append,id:2824,x:33115,y:33874,varname:node_2824,prsc:2|A-9970-X,B-9970-Z;n:type:ShaderForge.SFN_RemapRange,id:3823,x:34138,y:33910,varname:node_3823,prsc:2,frmn:0.98,frmx:1,tomn:1,tomx:0|IN-5231-OUT;n:type:ShaderForge.SFN_Clamp01,id:8012,x:34290,y:33910,varname:node_8012,prsc:0|IN-3823-OUT;n:type:ShaderForge.SFN_Divide,id:2135,x:33653,y:34027,cmnt:Clip objects by radial distance,varname:node_2135,prsc:0|A-9749-OUT,B-6190-OUT;n:type:ShaderForge.SFN_RemapRange,id:2989,x:34138,y:33720,varname:node_2989,prsc:2,frmn:0.92,frmx:0.98,tomn:0,tomx:1|IN-5231-OUT;n:type:ShaderForge.SFN_Clamp01,id:2109,x:34290,y:33720,varname:node_2109,prsc:0|IN-2989-OUT;n:type:ShaderForge.SFN_Add,id:5231,x:33984,y:33898,varname:node_5231,prsc:0|A-8734-OUT,B-2135-OUT;n:type:ShaderForge.SFN_RemapRange,id:8734,x:33833,y:33847,varname:node_8734,prsc:2,frmn:0,frmx:1,tomn:0.1,tomx:-0.1|IN-8545-G;n:type:ShaderForge.SFN_ToggleProperty,id:1866,x:33193,y:34375,ptovrint:False,ptlb:OverrideClipDistanceToGlobals,ptin:_OverrideClipDistanceToGlobals,varname:_OverrideClipDistanceToGlobals,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_ValueProperty,id:3474,x:32952,y:34097,ptovrint:False,ptlb:LocalClipDistance,ptin:_LocalClipDistance,varname:_LocalClipDistance,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:200;n:type:ShaderForge.SFN_ValueProperty,id:2647,x:32774,y:34269,ptovrint:False,ptlb:GlobalClipDistance,ptin:_GlobalClipDistance,varname:_GlobalClipDistance,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Lerp,id:6190,x:33342,y:34166,varname:node_6190,prsc:2|A-3474-OUT,B-2081-OUT,T-1866-OUT;n:type:ShaderForge.SFN_Add,id:9749,x:33475,y:33921,varname:node_9749,prsc:2|A-2382-OUT,B-7016-OUT;n:type:ShaderForge.SFN_Vector1,id:7016,x:33195,y:34017,varname:node_7016,prsc:2,v1:21;n:type:ShaderForge.SFN_Subtract,id:2081,x:32952,y:34299,cmnt:Subtracting bypasses clip if clip distance is zero,varname:node_2081,prsc:2|A-2647-OUT,B-1153-OUT;n:type:ShaderForge.SFN_Vector1,id:1153,x:32774,y:34384,varname:node_1153,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Multiply,id:3777,x:34459,y:33662,varname:node_3777,prsc:2|A-6972-OUT,B-2109-OUT,C-2109-OUT,D-2109-OUT;n:type:ShaderForge.SFN_Vector3,id:6972,x:34268,y:33595,varname:node_6972,prsc:0,v1:0.2,v2:0.5,v3:1;n:type:ShaderForge.SFN_Add,id:9539,x:34585,y:33260,varname:node_9539,prsc:2|A-3288-OUT,B-3777-OUT;proporder:8520-9061-509-1412-6783-3802-3862-1390-8326-9992-2870-8228-5925-2428-1866-3474;pass:END;sub:END;*/

Shader "Spatial/Spatial_Terrain_Master" {
    Properties {
        _T_Main ("T_Main", 2D) = "white" {}
        _T_Alpha ("T_Alpha", 2D) = "bump" {}
        _Proj_Offset_X ("Proj_Offset_X", Float ) = 0.5
        _Proj_Offset_Y ("Proj_Offset_Y", Float ) = 0.5
        _Colour_Tint_Primary ("Colour_Tint_Primary", Color) = (0.3,0.3,0.3,1)
        _Colour_Tint_Secondary ("Colour_Tint_Secondary", Color) = (0.45,0.45,0.45,1)
        _T_Normal ("T_Normal", 2D) = "bump" {}
        _Step_On_Radius ("Step_On_Radius", Float ) = 1
        _Emissive_Colour_Primary ("Emissive_Colour_Primary", Color) = (0.5,0.5,0.5,1)
        _Emissive_Colour_Secondary ("Emissive_Colour_Secondary", Color) = (0.5,0.5,0.5,1)
        _Gloss_Pow_Primary ("Gloss_Pow_Primary", Float ) = 1
        _Gloss_Pow_Secondary ("Gloss_Pow_Secondary", Float ) = 2
        _T_Sides ("T_Sides", 2D) = "white" {}
        _T_Sides_Normal ("T_Sides_Normal", 2D) = "bump" {}
        [MaterialToggle] _OverrideClipDistanceToGlobals ("OverrideClipDistanceToGlobals", Float ) = 0
        _LocalClipDistance ("LocalClipDistance", Float ) = 200
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 
            #pragma target 3.0
            uniform sampler2D _T_Main; uniform float4 _T_Main_ST;
            uniform sampler2D _T_Normal; uniform float4 _T_Normal_ST;
            uniform sampler2D _T_Alpha; uniform float4 _T_Alpha_ST;
            uniform fixed _Proj_Offset_X;
            uniform fixed _Proj_Offset_Y;
            uniform fixed4 _Colour_Tint_Primary;
            uniform fixed4 _Colour_Tint_Secondary;
            uniform fixed _Step_On_Radius;
            uniform fixed4 _Emissive_Colour_Primary;
            uniform fixed4 _Emissive_Colour_Secondary;
            uniform fixed _Gloss_Pow_Primary;
            uniform fixed _Gloss_Pow_Secondary;
            uniform sampler2D _T_Sides; uniform float4 _T_Sides_ST;
            uniform sampler2D _T_Sides_Normal; uniform float4 _T_Sides_Normal_ST;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed3 _T_Normal_var = UnpackNormal(tex2D(_T_Normal,TRANSFORM_TEX(i.uv0, _T_Normal)));
                fixed2 node_863 = ((i.uv0*fixed2(1,0.5))+fixed2(0,0.5));
                fixed3 _T_Sides_Normal_var = UnpackNormal(tex2D(_T_Sides_Normal,TRANSFORM_TEX(node_863, _T_Sides_Normal)));
                fixed node_590 = saturate(sign(i.uv0.g));
                float3 normalLocal = lerp(_T_Normal_var.rgb,_T_Sides_Normal_var.rgb,node_590);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                fixed4 _T_Main_var = tex2D(_T_Main,TRANSFORM_TEX(i.uv0, _T_Main)); // Unpack channels
                fixed4 _T_Sides_var = tex2D(_T_Sides,TRANSFORM_TEX(node_863, _T_Sides)); // Unpack channels
                fixed3 node_8545 = lerp(_T_Main_var.rgb,_T_Sides_var.rgb,node_590).rgb;
                fixed node_5231 = ((node_8545.g*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                clip(saturate((node_5231*-50.00005+50.00005)) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                fixed2 node_3436 = float2(_Proj_Offset_X,_Proj_Offset_Y);
                float2 node_9399 = (((float2(i.posWorld.r,i.posWorld.b)*0.25)+node_3436)+(i.normalDir.rb*(-0.1)));
                fixed2 node_8489 = ((node_9399+fixed2(-0.5,-0.5))/9.0);
                fixed4 _T_Alpha_var = tex2D(_T_Alpha,TRANSFORM_TEX(node_8489, _T_Alpha));
                float gloss = pow(node_8545.g,lerp(_Gloss_Pow_Primary,_Gloss_Pow_Secondary,_T_Alpha_var.r));
                float perceptualRoughness = 1.0 - pow(node_8545.g,lerp(_Gloss_Pow_Primary,_Gloss_Pow_Secondary,_T_Alpha_var.r));
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float3 diffuseColor = (lerp(_Colour_Tint_Primary.rgb,_Colour_Tint_Secondary.rgb,_T_Alpha_var.r)*node_8545.r); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (0 + UNITY_LIGHTMODEL_AMBIENT.rgb);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Diffuse Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                fixed node_2109 = saturate((node_5231*16.66667+-15.33333));
                float3 emissive = (((lerp(_Emissive_Colour_Primary.rgb,_Emissive_Colour_Secondary.rgb,_T_Alpha_var.r)*node_8545.b)*saturate(((1.0 - (_Step_On_Radius*distance(((round((node_9399+0.5))*4.0)-2.0),((node_3436+(float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b)*0.25))*4.0))))+4.0)))+(fixed3(0.2,0.5,1)*node_2109*node_2109*node_2109));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 
            #pragma target 3.0
            uniform sampler2D _T_Main; uniform float4 _T_Main_ST;
            uniform sampler2D _T_Normal; uniform float4 _T_Normal_ST;
            uniform sampler2D _T_Alpha; uniform float4 _T_Alpha_ST;
            uniform fixed _Proj_Offset_X;
            uniform fixed _Proj_Offset_Y;
            uniform fixed4 _Colour_Tint_Primary;
            uniform fixed4 _Colour_Tint_Secondary;
            uniform fixed _Step_On_Radius;
            uniform fixed4 _Emissive_Colour_Primary;
            uniform fixed4 _Emissive_Colour_Secondary;
            uniform fixed _Gloss_Pow_Primary;
            uniform fixed _Gloss_Pow_Secondary;
            uniform sampler2D _T_Sides; uniform float4 _T_Sides_ST;
            uniform sampler2D _T_Sides_Normal; uniform float4 _T_Sides_Normal_ST;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed3 _T_Normal_var = UnpackNormal(tex2D(_T_Normal,TRANSFORM_TEX(i.uv0, _T_Normal)));
                fixed2 node_863 = ((i.uv0*fixed2(1,0.5))+fixed2(0,0.5));
                fixed3 _T_Sides_Normal_var = UnpackNormal(tex2D(_T_Sides_Normal,TRANSFORM_TEX(node_863, _T_Sides_Normal)));
                fixed node_590 = saturate(sign(i.uv0.g));
                float3 normalLocal = lerp(_T_Normal_var.rgb,_T_Sides_Normal_var.rgb,node_590);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                fixed4 _T_Main_var = tex2D(_T_Main,TRANSFORM_TEX(i.uv0, _T_Main)); // Unpack channels
                fixed4 _T_Sides_var = tex2D(_T_Sides,TRANSFORM_TEX(node_863, _T_Sides)); // Unpack channels
                fixed3 node_8545 = lerp(_T_Main_var.rgb,_T_Sides_var.rgb,node_590).rgb;
                fixed node_5231 = ((node_8545.g*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                clip(saturate((node_5231*-50.00005+50.00005)) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                fixed2 node_3436 = float2(_Proj_Offset_X,_Proj_Offset_Y);
                float2 node_9399 = (((float2(i.posWorld.r,i.posWorld.b)*0.25)+node_3436)+(i.normalDir.rb*(-0.1)));
                fixed2 node_8489 = ((node_9399+fixed2(-0.5,-0.5))/9.0);
                fixed4 _T_Alpha_var = tex2D(_T_Alpha,TRANSFORM_TEX(node_8489, _T_Alpha));
                float gloss = pow(node_8545.g,lerp(_Gloss_Pow_Primary,_Gloss_Pow_Secondary,_T_Alpha_var.r));
                float perceptualRoughness = 1.0 - pow(node_8545.g,lerp(_Gloss_Pow_Primary,_Gloss_Pow_Secondary,_T_Alpha_var.r));
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float3 diffuseColor = (lerp(_Colour_Tint_Primary.rgb,_Colour_Tint_Secondary.rgb,_T_Alpha_var.r)*node_8545.r); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 
            #pragma target 3.0
            uniform sampler2D _T_Main; uniform float4 _T_Main_ST;
            uniform sampler2D _T_Sides; uniform float4 _T_Sides_ST;
            uniform fixed _OverrideClipDistanceToGlobals;
            uniform fixed _LocalClipDistance;
            uniform fixed _GlobalClipDistance;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                fixed4 _T_Main_var = tex2D(_T_Main,TRANSFORM_TEX(i.uv0, _T_Main)); // Unpack channels
                fixed2 node_863 = ((i.uv0*fixed2(1,0.5))+fixed2(0,0.5));
                fixed4 _T_Sides_var = tex2D(_T_Sides,TRANSFORM_TEX(node_863, _T_Sides)); // Unpack channels
                fixed node_590 = saturate(sign(i.uv0.g));
                fixed3 node_8545 = lerp(_T_Main_var.rgb,_T_Sides_var.rgb,node_590).rgb;
                fixed node_5231 = ((node_8545.g*-0.2+0.1)+((distance(float2(i.posWorld.r,i.posWorld.b),float2(_WorldSpaceCameraPos.r,_WorldSpaceCameraPos.b))+21.0)/lerp(_LocalClipDistance,(_GlobalClipDistance-0.01),_OverrideClipDistanceToGlobals)));
                clip(saturate((node_5231*-50.00005+50.00005)) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
