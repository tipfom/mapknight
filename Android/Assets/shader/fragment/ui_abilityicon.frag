precision mediump float;

uniform sampler2D u_baseTexture;
uniform sampler2D u_ampTexture;

varying vec2 v_baseTexcoord;
varying vec2 v_ampTexcoord;

void main()
{
	vec4 baseColor = texture2D(u_baseTexture, v_baseTexcoord);
    vec4 ampColor = texture2D(u_ampTexture, v_ampTexcoord);
    
    if (ampColor.a > 0.0) {
        float grayScale = ampColor.r * (0.2126 * baseColor.r + 0.7152 * baseColor.g + 0.0722 * baseColor.b);    
        baseColor = vec4(grayScale, grayScale, grayScale, baseColor.a);
    }

    gl_FragColor = baseColor;
}
