﻿
#if UNITY_ANDROID
namespace DeadMosquito.AndroidGoodies
{
	using System.Collections.Generic;
	using Internal;
	using MiniJSON;
	using UnityEngine;

	/// <summary>
	///     Image that was picked
	/// </summary>
	public class ImagePickResult
	{
		/// <summary>
		/// Path to the processed file. This will always be a local path on the device.
		/// </summary>
		public string OriginalPath { get; private set; }

		/// <summary>
		/// Display name of the file
		/// </summary>
		public string DisplayName { get; private set; }

		/// <summary>
		/// Get the path to the thumbnail(big) of the image
		/// </summary>
		public string ThumbnailPath { get; private set; }

		/// <summary>
		/// Get the path to the thumbnail(small) of the image
		/// </summary>
		public string SmallThumbnailPath { get; private set; }

		/// <summary>
		/// Get the image width
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Get the image height
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Get the size of the processed file in bytes
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// Read the picked file and load image into <see cref="Texture2D"/>
		/// </summary>
		/// <returns>Picked image as <see cref="Texture2D"/></returns>
		public Texture2D LoadTexture2D()
		{
			return AGUtils.Texture2DFromFile(OriginalPath);
		}

		/// <summary>
		/// Read the picked file and load thumbnail image into <see cref="Texture2D"/>
		/// NOTE: Will be null if "generateThumbnails" param is set to false when picking image
		/// </summary>
		/// <returns>Picked image thumbnail as <see cref="Texture2D"/></returns>
		public Texture2D LoadThumbnailTexture2D()
		{
			return CommonUtils.TextureFromFile(ThumbnailPath);
		}

		/// <summary>
		/// Read the picked file and load small thumbnail image into <see cref="Texture2D"/>
		/// NOTE: Will be null if "generateThumbnails" param is set to false when picking image
		/// </summary>
		/// <returns>Picked small image thumbnail as <see cref="Texture2D"/></returns>
		public Texture2D LoadSmallThumbnailTexture2D()
		{
			return CommonUtils.TextureFromFile(SmallThumbnailPath);
		}

		public static ImagePickResult FromJson(string json)
		{
			var imageResult = new ImagePickResult();
			var dic = Json.Deserialize(json) as Dictionary<string, object>;

			imageResult.OriginalPath = dic.GetStr("originalPath");
			imageResult.ThumbnailPath = dic.GetStr("thumbnailPath");
			imageResult.SmallThumbnailPath = dic.GetStr("thumbnailSmallPath");
			imageResult.DisplayName = dic.GetStr("displayName");
			imageResult.Width = (int) (long) dic["width"];
			imageResult.Height = (int) (long) dic["height"];
			imageResult.Size = (int) (long) dic["size"];

			return imageResult;
		}
	}
}

#endif