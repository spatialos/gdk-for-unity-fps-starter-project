// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32737,y:32553,varname:node_3138,prsc:2|emission-9971-OUT;n:type:ShaderForge.SFN_Tex2d,id:7641,x:31420,y:32383,ptovrint:False,ptlb:T_Main,ptin:_T_Main,varname:_T_Main,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e671c4ec40be85c48af3dd89b1687db4,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9971,x:32388,y:32600,varname:node_9971,prsc:2|A-8612-RGB,B-380-OUT,C-8612-A;n:type:ShaderForge.SFN_Vector3,id:6815,x:31572,y:32822,varname:node_6815,prsc:0,v1:0.3,v2:10,v3:50;n:type:ShaderForge.SFN_Multiply,id:5574,x:31809,y:32717,varname:node_5574,prsc:2|A-7641-RGB,B-6815-OUT;n:type:ShaderForge.SFN_Power,id:521,x:31636,y:32619,varname:node_521,prsc:2|VAL-7641-RGB,EXP-5015-OUT;n:type:ShaderForge.SFN_Vector1,id:5015,x:31452,y:32653,varname:node_5015,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:380,x:32136,y:32727,varname:node_380,prsc:2|A-2710-R,B-2710-G,C-2710-B;n:type:ShaderForge.SFN_ComponentMask,id:2710,x:31973,y:32717,varname:node_2710,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-5574-OUT;n:type:ShaderForge.SFN_VertexColor,id:8612,x:31894,y:32507,varname:node_8612,prsc:2;proporder:7641;pass:END;sub:END;*/

Shader "Spatial/Spatial_Sprite_Spark_Master" {
    Properties {
        _T_Main ("T_Main", 2D) = "white" {}
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
            uniform sampler2D _T_Main; uniform float4 _T_Main_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                fixed4 _T_Main_var = tex2D(_T_Main,TRANSFORM_TEX(i.uv0, _T_Main));
                float3 node_2710 = (_T_Main_var.rgb*fixed3(0.3,10,50)).rgb;
                float3 emissive = (i.vertexColor.rgb*(node_2710.r+node_2710.g+node_2710.b)*i.vertexColor.a);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
