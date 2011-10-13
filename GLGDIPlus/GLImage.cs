using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System;


namespace GLGDIPlus
{
    public class GLImage : GLImageBase
    {
		const int MAX_W = 512;
		const int MAX_H = 512;

		int Columns = 0;
		int Rows = 0;
		

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
		//public void Load(string path)
		//{
		//    SImage img = new SImage();
		//    img.Load(path);

		//    Width = img.Width;
		//    Height = img.Height;

		//    Free();
		//    mImageParts.Add(img);
		//}

		/// <summary>
		/// Loads image from harddisk into memory.
		/// </summary>
		/// <param name="path">Image path.</param>
		public void FromBitmap(Bitmap src)
		{
			//SImage img = new SImage();
			//img.FromBitmap(src);
			
			//Width = img.Width;
			//Height = img.Height;

			//Free();
			//mImageParts.Add(img);

			SetImage(src);
		}

		// ============================================================
		private void SetImage(Bitmap src)
		{
			Free();

			Width = src.Width;
			Height = src.Height;

			if (Width < MAX_W && Height < MAX_H)
			{
				Columns = 1;
				Rows = 1;

				SImage img = new SImage();
				img.FromBitmap(src);
				mImageParts.Add(img);
			}
			else
			{
				//if( Width > MAX_W )
				Columns = (Width + MAX_W - 1) / MAX_W;
				//if (Height > MAX_H)
				Rows = (Height + MAX_H - 1) / MAX_H;
				CreateTiles(src, Columns, Rows);
			}
		}
		// ============================================================
		private void CreateTiles(Bitmap src, int cols, int rows)
		{
			for (int j = 0; j < rows; j++)
			{
				for (int i = 0; i < cols; i++)
				{
					int xs = i * MAX_W;
					int ys = j * MAX_H;
					int w = Math.Min(MAX_W, src.Width - xs);
					int h = Math.Min(MAX_H, src.Height - ys);
					//bbox = (xs, ys, xs + w, ys + h)
					//tile = img.crop(bbox)
					//result.append(tile)
					Bitmap tile = new Bitmap(w, h);
					tile.SetResolution(src.HorizontalResolution, src.VerticalResolution);	// вот хрен поймешь зачем оно нужно. Долбаные DPI...
					using (Graphics g = Graphics.FromImage(tile))
					{
						Rectangle destR = new Rectangle(0, 0, w, h);
						Rectangle srcR = new Rectangle(xs, ys, w, h);
						g.DrawImage(src, destR, srcR, GraphicsUnit.Pixel);
					}
					SImage simage = new SImage();
					simage.FromBitmap(tile);
					mImageParts.Add(simage);
				}
			}
		}
		// ============================================================


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
			//foreach (var img in mImageParts)
			//{
			//    img.Draw(x, y);	//@
			//}

			for (int j = 0; j < Rows; j++)
			{
				for (int i = 0; i < Columns; i++)
				{
					int xs = i * MAX_W + x;
					int ys = j * MAX_H + y;
					SImage img = mImageParts[j * Columns + i];
					img.Draw(xs, ys);
				}
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
			//Draw(x, y, Width, Height, imgX, imgY, imgW, imgH);
			Draw(x, y, imgW, imgH, imgX, imgY, imgW, imgH);
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
			//foreach (var img in mImageParts)
			//{
			//    if (w == 0 || h == 0)
			//        continue;
			//    img.Draw(x, y, w, h);	//@
			//}
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
			if (w == 0 || h == 0 || imgW == 0 || imgH == 0)
				return;

			float scale_x = (float)w / (float)imgW;
			float scale_y = (float)h / (float)imgH;

			// тайлы, которые будут рисоваться:
			int begin_col = imgX / MAX_W;
			int end_col = (imgX + imgW + MAX_W - 1) / MAX_W;
			int begin_row = imgY / MAX_H;
			int end_row = (imgY + imgH + MAX_H - 1) / MAX_H;

			if (begin_row >= Rows || end_row > Rows ||
				begin_col >= Columns || end_col > Columns)
			{
				return;	// nothing to draw
			}

			float acc_w = 0;
			float acc_h = 0;
			for (int j = begin_row; j < end_row; j++)
			{
				acc_w = 0;
				float from_h = 0;
				for (int i = begin_col; i < end_col; i++)
				{
					int sx = i * MAX_W;
					int sy = j * MAX_H;
					int tile_w = Math.Min(Width - sx, MAX_W);
					int tile_h = Math.Min(Height - sy, MAX_H);

					int _sx = Math.Max(imgX - sx, 0);	// positive (start position in tile)
					int _sy = Math.Max(imgY - sy, 0);	// positive

					float from_w = (float)(tile_w - _sx);	// ready
					from_h = (float)(tile_h - _sy);	// ready
					if (acc_w + from_w > (float)imgW)
						from_w = (float)imgW - acc_w;
					if (acc_h + from_h > (float)imgH)
						from_h = (float)imgH - acc_h;

					int result_w = (int)(from_w * scale_x + 0.5f);
					int result_h = (int)(from_h * scale_y + 0.5f);

					float from_x = (float)_sx;		// ready
					float from_y = (float)_sy;		// ready
					int result_x = x + (int)(acc_w * scale_x + 0.5f);
					int result_y = y + (int)(acc_h * scale_y + 0.5f);

					acc_w += from_w;
					
					SImage img = mImageParts[j * Columns + i];
					img.Draw(result_x, result_y, result_w, result_h, from_x, from_y, from_w, from_h);
				}
				acc_h += from_h;
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
		public void ApplyBlendingValues()
		{
			foreach (var simg in mImageParts)
			{
				simg.SetBlending(RBlend, GBlend, BBlend, ABlend);
			}
		}
		// ============================================================
		public override void SetBlending()
		{
			base.SetBlending();
			ApplyBlendingValues();
		}
		// ============================================================
		public override void SetBlending(byte a)
		{
			base.SetBlending(a);
			ApplyBlendingValues();
		}
		// ============================================================
		public override void SetBlending(byte r, byte g, byte b, byte a)
		{
			base.SetBlending(r,g,b,a);
			ApplyBlendingValues();
		}
		// ============================================================
		public override void DisableBlending()
		{
			base.DisableBlending();
			ApplyBlendingValues();
		}
		// ============================================================
    }
}
