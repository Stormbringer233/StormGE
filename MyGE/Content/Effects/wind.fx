#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};
sampler2D mask = sampler_state { Texture = <maskTexture>; };
float time;
matrix projectionMatrix;
matrix viewMatrix;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color: COLOR0;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput Main_VS(float4 position : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(position, viewMatrix);
	output.Position = mul(output.Position, projectionMatrix);
	output.Color = color;
	output.TexCoord = texCoord;

	if (texCoord.y < 1)
		output.Position.x += sin(time) * (sin(time * 3) * 0.03);

	return output;
}

float4 Main_PS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(SpriteTextureSampler, input.TexCoord);
	float4 maskColor = tex2D(mask, input.TexCoord);
	//color.rb = color.g * 0.5f * sin(time);
	return color * maskColor;
}

technique SpriteDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL Main_VS();
		//PixelShader = compile PS_SHADERMODEL Main_PS();
	}
};