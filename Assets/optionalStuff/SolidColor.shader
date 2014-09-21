Shader "Custom/Solid Color" {
 
    Properties {
        _Color("Color", Color) = (0, 0, 0, 1)
    }
    SubShader {
        Lighting Off
        ZWrite Off
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        Tags {"Queue" = "Transparent"}
        Color[_Color]
        Pass {}
    } 
    FallBack "Unlit/Transparent"
}