using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelPerfect : MonoBehaviour
{
	public int width = 145;
	
	Camera cam;
	int height;

	void Start()
	{
		cam = GetComponent<Camera>();
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}

	void Update()
	{
		float ratio = ((float)cam.pixelHeight / (float)cam.pixelWidth);
		height = Mathf.RoundToInt((width * ratio));
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		source.filterMode = FilterMode.Point;
		RenderTexture buffer = RenderTexture.GetTemporary(width, height, -1);
		buffer.filterMode = FilterMode.Point;
		Graphics.Blit(source, buffer);
		Graphics.Blit(buffer, destination);
		RenderTexture.ReleaseTemporary(buffer);
	}
}
