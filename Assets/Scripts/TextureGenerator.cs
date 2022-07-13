using UnityEngine;
using System.Collections;

public static class TextureGenerator {

	public static Texture2D TextureFromColourMap(Color[] colourMap, int chunkSize) {
		int sizeUp = 4;

		Texture2D texture = new Texture2D(chunkSize * sizeUp, chunkSize * sizeUp);
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;

		Color[] sharperColorMap = new Color[colourMap.Length * sizeUp * sizeUp];
        for (int i = 0; i < colourMap.Length; i++) {
			int x = (i % chunkSize) * sizeUp;
			int y = i / chunkSize * sizeUp;
			int line = chunkSize * sizeUp;
			Color color = colourMap[i];

            for (int j = 0; j < sizeUp; j++) {
                for (int k = 0; k < sizeUp; k++) {
					sharperColorMap[(x + j) + ((y + k) * line)] = color;

                }
            }
		}

		texture.SetPixels(sharperColorMap);
		texture.Apply();
		return texture;
	}


	public static Texture2D TextureFromHeightMap(float[,] heightMap) {
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
			}
		}

		return TextureFromColourMap(colourMap, width);
	}

}