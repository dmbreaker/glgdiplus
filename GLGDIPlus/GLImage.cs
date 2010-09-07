using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;


namespace GLGDIPlus
{
    public class GLImage : IProperties
    {
        public Bitmap bitmap;          // Used to load image
        public int TextureIndex;            // Holds image data
        public VBO vbo = new VBO();

        public bool rebuild = true;


        /// <summary>
        /// Creates 4 vertices and texcoords for quad.
        /// </summary>
        public GLImage()
        {
            vbo.vertices = new Vertex[4];    // Create 4 vertices for quad
            vbo.texcoords = new TexCoord[4]; // Texture coordinates for quad
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
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

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
        internal void Draw(int x, int y)
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
		internal void Draw(int x, int y, int imgX, int imgY, int imgW, int imgH)
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
			SetBlending();	// всегда включаем блендинг, чтобы прозрачность рисовать

            // Texture coordinates
            float u1 = 0.0f, u2 = 0.0f, v1 = 0.0f, v2 = 0.0f;

            // Calculate coordinates, prevent dividing by zero
            if (imgX != 0) u1 = 1.0f / ((float)this.Width / (float)imgX);
            if (imgW != 0) u2 = 1.0f / ((float)this.Width / (float)(imgX+imgW));
            if (imgY != 0) v1 = 1.0f / ((float)this.Height / (float)imgY);
            if (imgH != 0) v2 = 1.0f / ((float)this.Height / (float)(imgY+imgH));

            if (rebuild)
            {
                // Check if texture coordinates have changed
                if (vbo.texcoords[0].u != u1 || vbo.texcoords[1].u != u2 || vbo.texcoords[2].v != v1 || vbo.texcoords[0].v != v2)
                {
                    // Update texcoords for all vertices
                    BuildTexcoords(u1, u2, v1, v2);
                }

                // Check if position coordinates have changed
                if (vbo.vertices[0].x != x || vbo.vertices[2].y != y || vbo.vertices[0].y != y + h || vbo.vertices[1].x != x + w)
                {
                    BuildVertices(x, y, w, h);
                }
            }

            // Prepare drawing
            Begin(x, y, w, h);

            // Bind texture
            GL.BindTexture(TextureTarget.Texture2D, TextureIndex);

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
