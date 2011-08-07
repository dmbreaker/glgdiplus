using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;


namespace GLGDIPlus
{
    public class GLImage : GLImageBase
    {
        private Bitmap bitmap;          // Used to load image
        private int TextureIndex;            // Holds image data
        private VBO vbo = new VBO();

        private bool rebuild = true;


        /// <summary>
        /// Creates 4 vertices and texcoords for quad.
        /// </summary>
        public GLImage()
        {
            vbo.Vertices = new Vertex[4];    // Create 4 vertices for quad
            vbo.Texcoords = new TexCoord[4]; // Texture coordinates for quad
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
                if (vbo.Texcoords[0].u != u1 || vbo.Texcoords[1].u != u2 || vbo.Texcoords[2].v != v1 || vbo.Texcoords[0].v != v2)
                {
                    // Update texcoords for all vertices
                    BuildTexcoords(u1, u2, v1, v2);
                }

                // Check if position coordinates have changed
                if (vbo.Vertices[0].x != x || vbo.Vertices[2].y != y || vbo.Vertices[0].y != y + h || vbo.Vertices[1].x != x + w)
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
            vbo.Texcoords[0].u = u1;
            vbo.Texcoords[0].v = v2;
            vbo.Texcoords[1].u = u2;
            vbo.Texcoords[1].v = v2;
            vbo.Texcoords[2].u = u2;
            vbo.Texcoords[2].v = v1;
            vbo.Texcoords[3].u = u1;
            vbo.Texcoords[3].v = v1;

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
			vbo.Vertices[0].x = x;
			vbo.Vertices[0].y = y + h;
			vbo.Vertices[1].x = x + w;
			vbo.Vertices[1].y = y + h;
			vbo.Vertices[2].x = x + w;
			vbo.Vertices[2].y = y;
			vbo.Vertices[3].x = x;
			vbo.Vertices[3].y = y;

            vbo.BuildVertices();
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
