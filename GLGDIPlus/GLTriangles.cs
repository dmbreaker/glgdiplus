using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace GLGDIPlus
{
	public class GLTriangles : GLImageBase
	{
		public Bitmap bitmap;          // Used to load image
		public int TextureIndex;            // Holds image data
		public VBO vbo = new VBO();

		private bool IsDataBuilded = false;

		// вектор вершин
		// вектор текстурных координат

		public bool rebuild = true;

		// ============================================================
		// ============================================================
		public GLTriangles()
		{
			vbo.Vertices = new Vertex[3];    // Create 4 vertices for quad
			vbo.Texcoords = new TexCoord[3]; // Texture coordinates for quad
		}
		// ============================================================
		public void SetVertices(List<STri> tris)
		{
			IsDataBuilded = false;

			int totalTris = tris.Count;
			if (totalTris != vbo.Vertices.Length * 3)
			{
				vbo.Vertices = new Vertex[totalTris * 3];
				vbo.Texcoords = new TexCoord[totalTris * 3];
			}

			for (int i = 0; i < totalTris; i++)
			{
				int k = i * 3;
				vbo.Vertices[k + 0].x = tris[i].vert[0].x;
				vbo.Vertices[k + 0].y = tris[i].vert[0].y;
				vbo.Vertices[k + 1].x = tris[i].vert[1].x;
				vbo.Vertices[k + 1].y = tris[i].vert[1].y;
				vbo.Vertices[k + 2].x = tris[i].vert[2].x;
				vbo.Vertices[k + 2].y = tris[i].vert[2].y;

				vbo.Texcoords[k + 0].u = tris[i].tex[0].u;
				vbo.Texcoords[k + 0].v = tris[i].tex[0].v;
				vbo.Texcoords[k + 1].u = tris[i].tex[1].u;
				vbo.Texcoords[k + 1].v = tris[i].tex[1].v;
				vbo.Texcoords[k + 2].u = tris[i].tex[2].u;
				vbo.Texcoords[k + 2].v = tris[i].tex[2].v;
			}
		}
		// ============================================================
		/// <summary>
		/// Loads image from harddisk into memory.
		/// </summary>
		/// <param name="path">Image path.</param>
		public void Load(string path)
		{
			// Load image
			bitmap = new Bitmap(path);

			// Generate texture
			GL.GenTextures(1, out TextureIndex);
			GL.BindTexture(TextureTarget.Texture2D, TextureIndex);

			// Store texture size
			Width = bitmap.Width;
			Height = bitmap.Height;

			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			bitmap.UnlockBits(data);

			// Setup filtering
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		}
		// ============================================================
		/// <summary>
		/// Loads image from harddisk into memory.
		/// </summary>
		/// <param name="path">Image path.</param>
		public void FromBitmap(Bitmap src)
		{
			bitmap = src;

			// Generate texture
			GL.GenTextures(1, out TextureIndex);
			GL.BindTexture(TextureTarget.Texture2D, TextureIndex);

			// Store texture size
			Width = bitmap.Width;
			Height = bitmap.Height;

			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			bitmap.UnlockBits(data);

			// Setup filtering
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		}
		// ============================================================
		/// <summary>
		/// Deletes texture from memory.
		/// </summary>
		public void Free()
		{
			GL.DeleteTextures(1, ref TextureIndex);
		}

		// ============================================================
		internal void Draw()
		{
			SetBlending();

			if (!IsDataBuilded)
			{
				IsDataBuilded = true;
				vbo.BuildTex();
				vbo.BuildVertices();
			}

			// Prepare drawing
			Begin(0, 0, Width, Height);

			// Bind texture
			GL.BindTexture(TextureTarget.Texture2D, TextureIndex);

			// Draw VBO
			vbo.Draw(BeginMode.Triangles);

			End();
		}
		// ============================================================
		public bool IsVBOSupported
		{
			get { return vbo.IsVBOSupported; }
			set
			{
				vbo.IsVBOSupported = value;
			}
		}
		// ============================================================
	}
}
