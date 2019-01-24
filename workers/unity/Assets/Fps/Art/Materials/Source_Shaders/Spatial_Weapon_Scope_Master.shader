// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-4736-OUT,alpha-760-G;n:type:ShaderForge.SFN_Tex2d,id:760,x:31813,y:32660,ptovrint:False,ptlb:T_Mask,ptin:_T_Mask,varname:_T_Mask,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4549623964cf55a48a35835550177076,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Power,id:7071,x:32037,y:32761,varname:node_7071,prsc:2|VAL-760-R,EXP-9319-OUT;n:type:ShaderForge.SFN_Multiply,id:4736,x:32216,y:32853,varname:node_4736,prsc:2|A-7071-OUT,B-6412-OUT,C-4808-RGB;n:type:ShaderForge.SFN_ValueProperty,id:9319,x:31813,y:32908,ptovrint:False,ptlb:Emissive_Power,ptin:_Emissive_Power,varname:_Emissive_Power,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Color,id:4808,x:31828,y:33176,ptovrint:False,ptlb:Emissive_Colour,ptin:_Emissive_Colour,varname:_Emissive_Colour,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.2,c3:0.1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:6412,x:31813,y:33044,ptovrint:False,ptlb:Emissive_Brightness,ptin:_Emissive_Brightness,varname:_Emissive_Brightness,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Vector1,id:2392,x:32376,y:32947,varname:node_2392,prsc:2,v1:1;proporder:760-9319-4808-6412;pass:END;sub:END;*/

Shader "Spatial/Spatial_Weapon_Scope_Master" {
    Properties {
        _T_Mask ("T_Mask", 2D) = "white" {}
        _Emissive_Power ("Emissive_Power", Float ) = 2
        _Emissive_Colour ("Emissive_Colour", Color) = (1,0.2,0.1,1)
        _Emissive_Brightness ("Emissive_Brightness", Float ) = 5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _T_Mask; uniform float4 _T_Mask_ST;
            uniform fixed _Emissive_Power;
            uniform fixed4 _Emissive_Colour;
            uniform fixed _Emissive_Brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                fixed4 _T_Mask_var = tex2D(_T_Mask,TRANSFORM_TEX(i.uv0, _T_Mask));
                float3 emissive = (pow(_T_Mask_var.r,_Emissive_Power)*_Emissive_Brightness*_Emissive_Colour.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,_T_Mask_var.g);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
