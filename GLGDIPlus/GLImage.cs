using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;


namespace GLGDIPlus
{
    public class GLImage : GLImageBase
    {
		List<SImage> mImageParts = new List<SImage>();

        /// <summary>
        /// Creates 4 vertices and texcoords for quad.
        /// </summary>
        public GLImage()
        {
        }

        /// <summary>
        /// Loads image from harddisk into memory.
        /// </summary>
        /// <param name="path">Image path.</param>
        public void Load(string path)
        {
			SImage img = new SImage();
			img.Load(path);

			Width = img.Width;
			Height = img.Height;

			Free();
			mImageParts.Add(img);
        }

		/// <summary>
		/// Loads image from harddisk into memory.
		/// </summary>
		/// <param name="path">Image path.</param>
		public void FromBitmap(Bitmap src)
		{
			SImage img = new SImage();
			img.FromBitmap(src);
			
			Width = img.Width;
			Height = img.Height;

			Free();
			mImageParts.Add(img);
		}


        /// <summary>
        /// Deletes texture from memory.
        /// </summary>
        public void Free()
        {
			foreach (var img in mImageParts)
			{
				img.Free();
			}
			mImageParts.Clear();
        }


        /// <summary>
        /// Draws image.
        /// </summary>
        /// <param name="x">X position of left-upper corner.</param>
        /// <param name="y">Y position of left-upper corner.</param>
        internal void Draw(int x, int y)
        {
			foreach (var img in mImageParts)
			{
				img.Draw(x, y);	//@
			}
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
			foreach (var img in mImageParts)
			{
				if (imgW == 0 || imgH == 0)
					continue;
				img.Draw(x, y, imgX, imgY, imgW, imgH);	//@
			}
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
            //Draw(x, y, w, h, 0, 0, this.Width, this.Height);
			foreach (var img in mImageParts)
			{
				if (w == 0 || h == 0)
					continue;
				img.Draw(x, y, w, h);	//@
			}
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
			//SetBlending();	// всегда включаем блендинг, чтобы прозрачность рисовать
			//// Texture coordinates
			//float u1 = 0.0f, u2 = 0.0f, v1 = 0.0f, v2 = 0.0f;
			//// Calculate coordinates, prevent dividing by zero
			//if (imgX != 0) u1 = 1.0f / ((float)this.Width / (float)imgX);
			//if (imgW != 0) u2 = 1.0f / ((float)this.Width / (float)(imgX+imgW));
			//if (imgY != 0) v1 = 1.0f / ((float)this.Height / (float)imgY);
			//if (imgH != 0) v2 = 1.0f / ((float)this.Height / (float)(imgY+imgH));

			//if (rebuild)
			//{
			//    // Check if texture coordinates have changed
			//    if (vbo.Texcoords[0].u != u1 || vbo.Texcoords[1].u != u2 || vbo.Texcoords[2].v != v1 || vbo.Texcoords[0].v != v2)
			//    {
			//        // Update texcoords for all vertices
			//        BuildTexcoords(u1, u2, v1, v2);
			//    }
			//    // Check if position coordinates have changed
			//    if (vbo.Vertices[0].x != x || vbo.Vertices[2].y != y || vbo.Vertices[0].y != y + h || vbo.Vertices[1].x != x + w)
			//    {
			//        BuildVertices(x, y, w, h);
			//    }
			//}

			//// Prepare drawing
			//Begin(x, y, w, h);
			//// Bind texture
			//GL.BindTexture(TextureTarget.Texture2D, TextureIndex);
			//// Draw VBO
			//vbo.Draw();
			//End();

			foreach (var img in mImageParts)
			{
				if( w == 0 || h == 0 || imgW == 0 || imgH == 0 )
					continue;
				img.Draw(x, y, w, h, imgX, imgY, imgW, imgH);	//@
			}
        }


        /// <summary>
        /// Builds texcoords for quad.
        /// </summary>
        public void BuildTexcoords()
        {
            //BuildTexcoords(0.0f, 1.0f, 0.0f, 1.0f);
			foreach (var img in mImageParts)
			{
				img.BuildTexcoords(0.0f, 1.0f, 0.0f, 1.0f);	//@
			}
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
			//vbo.Texcoords[0].u = u1;
			//vbo.Texcoords[0].v = v2;
			//vbo.Texcoords[1].u = u2;
			//vbo.Texcoords[1].v = v2;
			//vbo.Texcoords[2].u = u2;
			//vbo.Texcoords[2].v = v1;
			//vbo.Texcoords[3].u = u1;
			//vbo.Texcoords[3].v = v1;

			//vbo.BuildTex();

			foreach (var img in mImageParts)
			{
				img.BuildTexcoords(u1, u2, v1, v2);	//@
			}
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
			//vbo.Vertices[0].x = x;
			//vbo.Vertices[0].y = y + h;
			//vbo.Vertices[1].x = x + w;
			//vbo.Vertices[1].y = y + h;
			//vbo.Vertices[2].x = x + w;
			//vbo.Vertices[2].y = y;
			//vbo.Vertices[3].x = x;
			//vbo.Vertices[3].y = y;

			//vbo.BuildVertices();

			foreach (var img in mImageParts)
			{
				img.BuildVertices(x, y, w, h);	//@
			}
        }
		// ============================================================
		public bool IsVBOSupported
		{
			get
			{
				if (mImageParts.Count > 0)
					return mImageParts[0].IsVBOSupported;
				else
					return false;
			}
			set
			{
				foreach (var img in mImageParts)
				{
					img.IsVBOSupported = value;
				}
			}
		}
		// ============================================================
    }
}
