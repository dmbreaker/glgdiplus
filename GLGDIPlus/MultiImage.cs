using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;


namespace GLGDIPlus
{
    public class GLMultiImage : IProperties
    {
        public Bitmap bitmap;          // Used to load image
        public int texture;            // Holds image data
        public VBO vbo = new VBO();

		// вектор вершин
		// вектор текстурных координат

        public bool rebuild = true;


        /// <summary>
        /// Creates 4 vertices and texcoords for quad.
        /// </summary>
		public GLMultiImage()
        {
            vbo.vertices = new Vertex[4];    // Create 4 vertices for quad
            vbo.texcoords = new TexCoord[4]; // Texture coordinates for quad
        }

		/// <summary>
		/// Set rectangles where you need draw image
		/// </summary>
		/// <param name="tiles"></param>
		public void SetImageTiles( List<RectangleF> tiles )
		{
			int totalC = tiles.Count;
			if (totalC != (vbo.vertices.Length * 4))
			{
				vbo.vertices = new Vertex[totalC*4];
				vbo.texcoords = new TexCoord[totalC*4];
			}

			for (int i = 0; i < totalC; i++)
			{
				int k = i * 4;
				RectangleF r = tiles[i];
				vbo.vertices[k + 0].x = r.X;
				vbo.vertices[k + 0].y = r.Y + r.Height;
				vbo.vertices[k + 1].x = r.X + r.Width;
				vbo.vertices[k + 1].y = r.Y + r.Height;
				vbo.vertices[k + 2].x = r.X + r.Width;
				vbo.vertices[k + 2].y = r.Y;
				vbo.vertices[k + 3].x = r.X;
				vbo.vertices[k + 3].y = r.Y;

				vbo.texcoords[k + 0].u = 0;
				vbo.texcoords[k + 0].v = 1;
				vbo.texcoords[k + 1].u = 1;
				vbo.texcoords[k + 1].v = 1;
				vbo.texcoords[k + 2].u = 1;
				vbo.texcoords[k + 2].v = 0;
				vbo.texcoords[k + 3].u = 0;
				vbo.texcoords[k + 3].v = 0;
			}

			vbo.BuildTex();

			vbo.Build();
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
            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);

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
			bitmap = src;

			// Generate texture
			GL.GenTextures(1, out texture);
			GL.BindTexture(TextureTarget.Texture2D, texture);

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
        /// Deletes texture from memory.
        /// </summary>
        public void Free()
        {
            GL.DeleteTextures(1, ref texture);
        }


        /// <summary>
        /// Draws image.
        /// </summary>
        /// <param name="x">X position of left-upper corner.</param>
        /// <param name="y">Y position of left-upper corner.</param>
        public void Draw(int x, int y)
        {
            Draw(x, y, Width, Height, 0, 0, Width, Height);
        }


        /// <summary>
        /// Draws a part of image.
        /// </summary>
        /// <param name="x">X position of left-upper corner.</param>
        /// <param name="y">Y position of left-upper corner.</param>
        /// <param name="imgX">X positon on image.</param>
        /// <param name="imgY">Y positon on image.</param>
        /// <param name="imgW">Width of image part to be drawn.</param>
        /// <param name="imgH">Height of image part to be drawn.</param>
        public void Draw(int x, int y, int imgX, int imgY, int imgW, int imgH)
        {
            Draw(x, y, Width, Height, imgX, imgY, imgW, imgH);
        }


        /// <summary>
        /// Draws image with specified size.
        /// </summary>
        /// <param name="x">X position of left-upper corner.</param>
        /// <param name="y">Y position of left-upper corner.</param>
        /// <param name="w">Width of image.</param>
        /// <param name="h">Height of image.</param>
        internal void Draw(int x, int y, int w, int h)
        {
            Draw(x, y, w, h, 0, 0, this.Width, this.Height);
        }


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
        internal void Draw(int x, int y, int w, int h, int imgX, int imgY, int imgW, int imgH)
        {
            // Prepare drawing
            Begin(x, y, w, h);

            // Bind texture
            GL.BindTexture(TextureTarget.Texture2D, texture);

            // Draw VBO
			vbo.Draw();

            End();
        }


        /// <summary>
        /// Builds texcoords for quad.
        /// </summary>
        public void BuildTexcoords()
        {
            BuildTexcoords(0.0f, 1.0f, 0.0f, 1.0f);
        }



        /// <summary>
        /// Builds texcoords for quad.
        /// </summary>
        /// <param name="u1">U1.</param>
        /// <param name="u2">U2.</param>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        public void BuildTexcoords(float u1, float u2, float v1, float v2)
        {
            vbo.texcoords[0].u = u1;
            vbo.texcoords[0].v = v2;
            vbo.texcoords[1].u = u2;
            vbo.texcoords[1].v = v2;
            vbo.texcoords[2].u = u2;
            vbo.texcoords[2].v = v1;
            vbo.texcoords[3].u = u1;
            vbo.texcoords[3].v = v1;

            vbo.BuildTex();
        }


        /// <summary>
        /// Builds vertices for quad.
        /// </summary>
        /// <param name="x">X pos.</param>
        /// <param name="y">Y pos.</param>
        /// <param name="w">Width.</param>
        /// <param name="h">Height.</param>
        public void BuildVertices(int x, int y, int w, int h)
        {
			vbo.vertices[0].x = x;
			vbo.vertices[0].y = y + h;
			vbo.vertices[1].x = x + w;
			vbo.vertices[1].y = y + h;
			vbo.vertices[2].x = x + w;
			vbo.vertices[2].y = y;
			vbo.vertices[3].x = x;
			vbo.vertices[3].y = y;

            vbo.Build();
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
