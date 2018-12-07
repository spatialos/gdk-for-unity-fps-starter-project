// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32779,y:32448,varname:node_3138,prsc:2|emission-3096-OUT;n:type:ShaderForge.SFN_TexCoord,id:9015,x:30298,y:32668,varname:node_9015,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:6615,x:30554,y:32668,varname:node_6615,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-9015-UVOUT;n:type:ShaderForge.SFN_Abs,id:9495,x:30740,y:32668,varname:node_9495,prsc:2|IN-6615-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3446,x:31069,y:32668,varname:node_3446,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-3537-OUT;n:type:ShaderForge.SFN_OneMinus,id:3537,x:30902,y:32668,varname:node_3537,prsc:2|IN-9495-OUT;n:type:ShaderForge.SFN_RemapRange,id:8474,x:31238,y:32668,varname:node_8474,prsc:2,frmn:0,frmx:0.005,tomn:0,tomx:1|IN-3446-R;n:type:ShaderForge.SFN_Clamp01,id:281,x:31410,y:32668,varname:node_281,prsc:2|IN-8474-OUT;n:type:ShaderForge.SFN_Clamp01,id:7235,x:31410,y:32870,varname:node_7235,prsc:2|IN-8179-OUT;n:type:ShaderForge.SFN_RemapRange,id:8179,x:31238,y:32870,varname:node_8179,prsc:2,frmn:0,frmx:0.99,tomn:0,tomx:1|IN-3446-G;n:type:ShaderForge.SFN_Power,id:6710,x:31692,y:32671,varname:node_6710,prsc:2|VAL-281-OUT,EXP-9686-OUT;n:type:ShaderForge.SFN_Vector1,id:9686,x:31410,y:32790,varname:node_9686,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:570,x:31979,y:32865,varname:node_570,prsc:2|A-6710-OUT,B-9766-OUT,C-5075-OUT;n:type:ShaderForge.SFN_Power,id:9766,x:31692,y:32873,varname:node_9766,prsc:2|VAL-7235-OUT,EXP-1943-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:8668,x:30265,y:33182,varname:node_8668,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:3462,x:30265,y:33315,varname:node_3462,prsc:2;n:type:ShaderForge.SFN_Distance,id:2065,x:30490,y:33250,varname:node_2065,prsc:2|A-8668-XYZ,B-3462-XYZ;n:type:ShaderForge.SFN_RemapRange,id:749,x:30686,y:33250,varname:node_749,prsc:2,frmn:5,frmx:40,tomn:0,tomx:1|IN-2065-OUT;n:type:ShaderForge.SFN_Clamp01,id:6609,x:30869,y:33250,varname:node_6609,prsc:2|IN-749-OUT;n:type:ShaderForge.SFN_Power,id:8808,x:31053,y:33250,varname:node_8808,prsc:2|VAL-6609-OUT,EXP-536-OUT;n:type:ShaderForge.SFN_Vector1,id:536,x:30869,y:33390,varname:node_536,prsc:2,v1:3;n:type:ShaderForge.SFN_Clamp01,id:9677,x:31228,y:33250,varname:node_9677,prsc:0|IN-8808-OUT;n:type:ShaderForge.SFN_Lerp,id:1943,x:31428,y:33073,varname:node_1943,prsc:2|A-4307-OUT,B-4478-OUT,T-9677-OUT;n:type:ShaderForge.SFN_Vector1,id:4307,x:31228,y:33041,varname:node_4307,prsc:2,v1:8;n:type:ShaderForge.SFN_Vector1,id:4478,x:31228,y:33097,varname:node_4478,prsc:2,v1:1;n:type:ShaderForge.SFN_RemapRange,id:5075,x:31428,y:33250,varname:node_5075,prsc:2,frmn:0,frmx:1,tomn:10,tomx:0.1|IN-9677-OUT;n:type:ShaderForge.SFN_Power,id:3445,x:31979,y:32715,varname:node_3445,prsc:2|VAL-7235-OUT,EXP-8238-OUT;n:type:ShaderForge.SFN_Vector1,id:8238,x:31769,y:32787,varname:node_8238,prsc:2,v1:200;n:type:ShaderForge.SFN_VertexColor,id:9571,x:31843,y:32404,varname:node_9571,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3096,x:32391,y:32545,varname:node_3096,prsc:2|A-9876-OUT,B-9571-A,C-570-OUT,D-7377-OUT;n:type:ShaderForge.SFN_Lerp,id:9876,x:32225,y:32479,varname:node_9876,prsc:2|A-2936-OUT,B-1937-OUT,T-3445-OUT;n:type:ShaderForge.SFN_Vector1,id:1937,x:31974,y:32557,varname:node_1937,prsc:0,v1:1;n:type:ShaderForge.SFN_Multiply,id:2936,x:32053,y:32327,varname:node_2936,prsc:2|A-5999-RGB,B-9571-RGB;n:type:ShaderForge.SFN_Color,id:5999,x:31843,y:32208,ptovrint:False,ptlb:Colour,ptin:_Colour,varname:_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.2,c3:0.1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:7377,x:32180,y:32661,ptovrint:False,ptlb:Brightness,ptin:_Brightness,varname:_Brightness,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:5999-7377;pass:END;sub:END;*/

Shader "Spatial/Spatial_Bullet_Tracer_Master" {
    Properties {
        _Colour ("Colour", Color) = (1,0.2,0.1,1)
        _Brightness ("Brightness", Float ) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 
            #pragma target 3.0
            uniform fixed4 _Colour;
            uniform fixed _Brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                fixed node_1937 = 1.0;
                float2 node_3446 = (1.0 - abs((i.uv0*2.0+-1.0))).rg;
                float node_7235 = saturate((node_3446.g*1.010101+0.0));
                fixed node_9677 = saturate(pow(saturate((distance(i.posWorld.rgb,_WorldSpaceCameraPos)*0.02857143+-0.1428571)),3.0));
                float3 emissive = (lerp((_Colour.rgb*i.vertexColor.rgb),float3(node_1937,node_1937,node_1937),pow(node_7235,200.0))*i.vertexColor.a*(pow(saturate((node_3446.r*200.0+0.0)),2.0)*pow(node_7235,lerp(8.0,1.0,node_9677))*(node_9677*-9.9+10.0))*_Brightness);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
