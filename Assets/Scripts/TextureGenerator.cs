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

	public static Texture2D TextureFromColourMapWithSmooth(Color[] colorMap, int chunkSize) {
		//int enlargeTimes = 1;
		int perAxisAddition = chunkSize - 1;
		int newSideSize = chunkSize + perAxisAddition;

		Texture2D texture = new Texture2D(newSideSize, newSideSize);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;

		Color[] smootherColorMap = new Color[newSideSize * newSideSize];

        

		// Step 1: create spaces between values
		for (int i = 0; i < colorMap.Length; i++) {
			int x = i % chunkSize;
			int y = i / chunkSize;
			int position = x * 2 + y * 2 * newSideSize;
			smootherColorMap[position] = colorMap[i];
		}

        // Step 2: fill spaces horizontally
        for (int i = 1; i < colorMap.Length; i++) {
			int x = i % chunkSize;
			int y = i / chunkSize;
			int position = x * 2 + y * 2 * newSideSize;
			if (x != 0)
				smootherColorMap[position - 1] = (smootherColorMap[position - 2] + smootherColorMap[position]) / 2;
		}

		// Step 3: fill spaces vertically
		for (int i = 1; i < smootherColorMap.Length; i++) {
			int x = i % newSideSize;
			int y = i / newSideSize;
			if (y % 2 != 0)
				smootherColorMap[x + y * newSideSize] = (smootherColorMap[x + (y - 1) * newSideSize] + smootherColorMap[x + (y + 1) * newSideSize]) / 2;
			//else
			//	i += newSideSize - 1;
		}

		texture.SetPixels(smootherColorMap);
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