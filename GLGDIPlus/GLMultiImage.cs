using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;


namespace GLGDIPlus
{
    public class GLMultiImage : GLImageBase
    {
        public Bitmap bitmap;          // Used to load image
        public int TextureIndex;            // Holds image data
        public VBO vbo = new VBO();

		private bool IsDataBuilded = false;

		// вектор вершин
		// вектор текстурных координат

        public bool rebuild = true;


        /// <summary>
        /// Creates 4 vertices and texcoords for quad.
        /// </summary>
		public GLMultiImage()
        {
            vbo.Vertices = new Vertex[4];    // Create 4 vertices for quad
            vbo.Texcoords = new TexCoord[4]; // Texture coordinates for quad
        }

		/// <summary>
		/// Set rectangles where you need draw image
		/// </summary>
		/// <param name="tiles"></param>
		public void SetImageTiles( List<RectangleF> tiles )
		{
			IsDataBuilded = false;

			int totalC = tiles.Count;
			if( totalC == 0 )
				return;
			if (totalC*4 != (vbo.Vertices.Length))
			{
				vbo.Vertices = new Vertex[totalC*4];
				vbo.Texcoords = new TexCoord[totalC*4];
			}

			for (int i = 0; i < totalC; i++)
			{
				int k = i * 4;
				RectangleF r = tiles[i];
				vbo.Vertices[k + 0].x = r.X;
				vbo.Vertices[k + 0].y = r.Y + r.Height;
				vbo.Vertices[k + 1].x = r.X + r.Width;
				vbo.Vertices[k + 1].y = r.Y + r.Height;
				vbo.Vertices[k + 2].x = r.X + r.Width;
				vbo.Vertices[k + 2].y = r.Y;
				vbo.Vertices[k + 3].x = r.X;
				vbo.Vertices[k + 3].y = r.Y;

				vbo.Texcoords[k + 0].u = 0;
				vbo.Texcoords[k + 0].v = 1;
				vbo.Texcoords[k + 1].u = 1;
				vbo.Texcoords[k + 1].v = 1;
				vbo.Texcoords[k + 2].u = 1;
				vbo.Texcoords[k + 2].v = 0;
				vbo.Texcoords[k + 3].u = 0;
				vbo.Texcoords[k + 3].v = 0;
			}
		}


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

		/// <summary>
		/// Loads image from harddisk into memory.
		/// </summary>
		/// <param name="path">Image path.</param>
		public void FromBitmap(Bitmap src)
		{
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
		}


        /// <summary>
        /// Deletes texture from memory.
        /// </summary>
        public void Free()
        {
            GL.DeleteTextures(1, ref TextureIndex);
        }


        /// <summary>
        /// Draws image.
        /// </summary>
        /// <param name="x">X position of left-upper corner.</param>
        /// <param name="y">Y position of left-upper corner.</param>
		//public void Draw(int x, int y)
		//{
		//    Draw(x, y, Width, Height, 0, 0, Width, Height);
		//}


        /// <summary>
        /// Draws a part of image.
        /// </summary>
        /// <param name="x">X position of left-upper corner.</param>
        /// <param name="y">Y position of left-upper corner.</param>
        /// <param name="imgX">X positon on image.</param>
        /// <param name="imgY">Y positon on image.</param>
        /// <param name="imgW">Width of image part to be drawn.</param>
        /// <param name="imgH">Height of image part to be drawn.</param>
		//public void Draw(int x, int y, int imgX, int imgY, int imgW, int imgH)
		//{
		//    Draw(x, y, Width, Height, imgX, imgY, imgW, imgH);
		//}


        /// <summary>
        /// Draws image with specified size.
        /// </summary>
        /// <param name="x">X position of left-upper corner.</param>
        /// <param name="y">Y position of left-upper corner.</param>
        /// <param name="w">Width of image.</param>
        /// <param name="h">Height of image.</param>
		//internal void Draw(int x, int y, int w, int h)
		//{
		//    Draw(x, y, w, h, 0, 0, this.Width, this.Height);
		//}


        /// <summary>
        /// Draws a part of image with specified size.
        /// </summary>
        /// <param name="x">X position of left-upper corner.</param>
        /// <param name="y">Y position of left-upper corner.</param>
        /// <param name="w">Width of image.</param>
        /// <param name="h">Height of image.</param>
        /// <param name="imgX">X positon on image.</param>
        /// <param name="imgY">Y positon on image.</param>
        /// <param name="imgW">Width of image part to be drawn.</param>
        /// <param name="imgH">Height of image part to be drawn.</param>
        //internal void Draw(int x, int y, int w, int h, int imgX, int imgY, int imgW, int imgH)
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
			vbo.Draw();

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
