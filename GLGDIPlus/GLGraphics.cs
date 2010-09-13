using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using opengl = OpenTK.Graphics;

using System.Drawing;

namespace GLGDIPlus
{
	public class GLGraphics
	{
		// ============================================================
		private bool IsVBOSupported = false;
		private bool IsLinearFiltering = false;

		private bool mIsFilteringSet = false;

		private BlendingValues mNoColorChange = new BlendingValues(255);
		// ============================================================
		/// <summary>
		/// Initializes 2D mode.
		/// </summary>
		public void Init()
		{
			int[] viewPort = new int[4];

			GL.GetInteger(GetPName.Viewport, viewPort);

			GL.MatrixMode(MatrixMode.Projection);
			GL.PushMatrix();
			GL.LoadIdentity();

			GL.Ortho(viewPort[0], viewPort[0] + viewPort[2], viewPort[1] + viewPort[3], viewPort[1], -1, 1);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.PushMatrix();
			GL.LoadIdentity();
			//GL.Translate(0.375, 0.375, 0.0);
			GL.Translate(0.0, 0.0, 0.0);

			GL.PushAttrib(AttribMask.DepthBufferBit);
			GL.Disable(EnableCap.DepthTest);

			GL.Enable(EnableCap.Texture2D);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			IsVBOSupported = IsHigherOrEqualVersion(1, 5);

			SetLinearFiltering(true);
		}


		/// <summary>
		/// Resizes viewport.
		/// </summary>
		/// <param name="windowW">New width of window.</param>
		/// <param name="windowH">New height of window.</param>
		public void Resize(int windowW, int windowH)
		{
			GL.Viewport(0, 0, windowW, windowH);
		}


		/// <summary>
		/// Sets color to be cleared on Clear() call.
		/// </summary>
		/// <param name="r">Red intensity.</param>
		/// <param name="r">Green intensity.</param>
		/// <param name="r">Blue intensity.</param>
		public void SetClearColor(byte r, byte g, byte b)
		{
			GL.ClearColor((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, 1.0f);
		}
		// ============================================================
		public void SetClearColor(Color c)
		{
			GL.ClearColor((float)c.R / 255.0f, (float)c.G / 255.0f, (float)c.B / 255.0f, 1.0f);
		}


		/// <summary>
		/// Clears the background.
		/// </summary>
		public void Clear()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}
		// ============================================================
		public void Clear( Color c )
		{
			SetClearColor(c);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}


		/// <summary>
		/// Sets drawing color.
		/// </summary>
		/// <param name="r">Red intensity.</param>
		/// <param name="g">Green intensity.</param>
		/// <param name="b">Blue intensity.</param>
		public void Color(byte r, byte g, byte b)
		{
			GL.Color3(r, g, b);
		}

		/// <summary>
		/// Sets drawing color.
		/// </summary>
		/// <param name="r">Red intensity.</param>
		/// <param name="g">Green intensity.</param>
		/// <param name="b">Blue intensity.</param>
		public void Color(System.Drawing.Color color)
		{
			GL.Color3(color);
		}


		/// <summary>
		/// Sets drawing color.
		/// </summary>
		/// <param name="r">Red intensity.</param>
		/// <param name="g">Green intensity.</param>
		/// <param name="b">Blue intensity.</param>
		/// <param name="a">Alpha intensity.</param>
		public void Color(byte r, byte g, byte b, byte a)
		{
			GL.Color4(r, g, b, a);
		}


		/// <summary>
		/// Translates.
		/// </summary>
		/// <param name="x">X position.</param>
		/// <param name="y">Y position.</param>
		public void Translate(int x, int y)
		{
			GL.Translate(x, y, 0.0f);
		}


		/// <summary>
		/// Rotates.
		/// </summary>
		/// <param name="angle">Angle of rotation.</param>
		/// <param name="originX">X position of origin to be rotated around.</param>
		/// <param name="originY">Y position of origin to be rotated around.</param>
		public void Rotate(float angle, int originX, int originY)
		{
			GL.Translate(originX, originY, 0.0);
			GL.Rotate(angle, 0.0f, 0.0f, 1.0f);
			GL.Translate(-originX, -originY, 0.0);
		}


		/// <summary>
		/// Scales.
		/// </summary>
		/// <param name="x">X scale factor.</param>
		/// <param name="y">Y scale factor.</param>
		public void Scale(float x, float y)
		{
			GL.Scale(x, y, 0.0f);
		}


		/// <summary>
		/// Resets color to white and loads identity.
		/// </summary>
		public void Reset()
		{
			GL.Color4((byte)255, (byte)255, (byte)255, (byte)255);

			GL.LoadIdentity();
			//GL.Translate(0.375, 0.375, 0.0);    // Move slightly for pixel precise drawing
			GL.Translate(0, 0, 0.0);
		}

		/// <summary>
		/// Set filtering to LINEAR if value is true, 
		/// otherwise to NEAREST
		/// </summary>
		/// <param name="value"></param>
		public void SetLinearFiltering(bool value)
		{
			IsLinearFiltering = value;
			if (value)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			}
			else
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			}
		}
		// ============================================================
		//public void StartDraw()
		//{
		//    SetLinearFiltering(IsLinearFiltering);
		//}
		// ============================================================
		internal bool IsHigherOrEqualVersion(int major, int minor)
		{
			string version = GL.GetString(StringName.Version);
			int index = version.IndexOf('.');
			int vMajor = 0;
			int vMinor = 0;
			string tmp = "";
			if (index >= 0)
			{
				tmp = version.Substring(0, index);
				int.TryParse(tmp, out vMajor);

				version = version.Substring(index + 1);
				index = version.IndexOf('.');
				if (index < 0 && version.Length > 0)
				{
					int.TryParse(version, out vMinor);
				}
				else
				{
					tmp = version.Substring(0, index);
					int.TryParse(tmp, out vMinor);
				}

				if (vMajor > major || (vMajor == major && vMinor >= minor) )
					return true;
				else
					return false;
			}
			else
				return false;
		}

		// ============================================================
		// ============================================================
		// ============================================================
		public void DrawLine(Color c, int x1, int y1, int x2, int y2)
		{
			DrawLine(c, x1, y1, x2, y2, 1f);
		}
		// ============================================================
		public void DrawLine(Color c, int x1, int y1, int x2, int y2, float line_width)
		{
			GL.Disable(EnableCap.Texture2D);
			GL.Enable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Blend);
			GL.Color4(c);
			GL.LineWidth(line_width);

			GL.Begin(BeginMode.Lines);

			GL.Vertex2((double)x1, (double)y1);
			GL.Vertex2((double)x2, (double)y2);

			GL.End();

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Texture2D);
		}
		// ============================================================
		public void FillRectangle(Color c, Rectangle r)
		{
			FillRectangle(c, r.X, r.Y, r.Width, r.Height);
		}
		// ============================================================
		public void FillRectangle(Color c, int x, int y, int w, int h)
		{
			GL.Disable(EnableCap.Texture2D);
			GL.Enable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Blend);
			GL.Color4(c);

			GL.Begin(BeginMode.Quads);

			GL.Vertex2((double)x, (double)y);
			GL.Vertex2((double)x + w, (double)y);
			GL.Vertex2((double)x + w, (double)y + h);
			GL.Vertex2((double)x, (double)y + h);

			GL.End();

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Texture2D);
		}
		// ============================================================
		public void DrawRectangle(Color c, Rectangle r)
		{
			DrawRectangle(c, r.X, r.Y, r.Width, r.Height);
		}
		// ============================================================
		public void DrawRectangle(Color c, int x, int y, int w, int h)
		{
			DrawRectangle(c, x, y, w, h, 1f);
		}
		// ============================================================
		public void DrawRectangle(Color c, int x, int y, int w, int h, float line_width)
		{
			GL.Disable(EnableCap.Texture2D);
			GL.Enable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Blend);
			GL.Color4(c);
			GL.LineWidth(line_width);

			GL.Begin(BeginMode.LineStrip);

			GL.Vertex2((double)x, (double)y);
			GL.Vertex2((double)x + w, (double)y);
			GL.Vertex2((double)x + w, (double)y + h);
			GL.Vertex2((double)x, (double)y + h);
			GL.Vertex2((double)x, (double)y);

			GL.End();

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Texture2D);
		}
		// ============================================================
		public void DrawPoint(Color c, Point pnt, float size)
		{
			DrawPointF(c, new PointF(pnt.X, pnt.Y), size);
		}
		// ============================================================
		// ============================================================
		public void DrawPointF(Color c, PointF pnt, float size)
		{
			GL.Disable(EnableCap.Texture2D);
			GL.Enable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Blend);
			GL.PointSize(size);
			GL.Color4(c);

			GL.Begin(BeginMode.Points);

			GL.Vertex2(pnt.X, pnt.Y);

			GL.End();

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Texture2D);
		}
		// ============================================================
		public void DrawPointsF(Color c, List<PointF> points, float size)
		{
			GL.Disable(EnableCap.Texture2D);
			GL.Enable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Blend);
			GL.PointSize(size);
			GL.Color4(c);

			GL.Begin(BeginMode.Points);

			foreach (var pnt in points)
			{
				GL.Vertex2(pnt.X, pnt.Y);
			}

			GL.End();

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Texture2D);
		}
		// ============================================================
		private void EnsureFiltering()
		{
			//if (!mIsFilteringSet)
			//{
			//    mIsFilteringSet = true;
			//    SetLinearFiltering(IsLinearFiltering);
			//}

			SetLinearFiltering(IsLinearFiltering);
		}
		// ============================================================
		public void DrawImage( GLImage img, int x, int y )
		{
			DrawImage(img, x, y, mNoColorChange);
		}
		// ============================================================
		public void DrawImage(GLImage img, int x, int y, BlendingValues b)
		{
			EnsureFiltering();

			img.IsVBOSupported = IsVBOSupported;
			img.SetBlending(b.R, b.G, b.B, b.A);
			img.Draw(x, y);

			img.SetBlending(255, 255, 255, 255);
		}
		// ============================================================
		public void DrawImage(GLImage img, int x, int y, int destWidth, int destHeight)
		{
			DrawImage(img, x, y, destWidth, destHeight, mNoColorChange);
		}
		// ============================================================
		public void DrawImage(GLImage img, int x, int y, int destWidth, int destHeight, BlendingValues b)
		{
			EnsureFiltering();

			img.IsVBOSupported = IsVBOSupported;
			img.SetBlending(b.R, b.G, b.B, b.A);
			img.Draw(x, y, destWidth, destHeight);

			img.SetBlending(255, 255, 255, 255);
		}
		// ============================================================
		public void DrawImage(GLImage img, int x, int y, int imgX, int imgY, int imgW, int imgH)
		{
			DrawImage(img, x, y, imgX, imgY, imgW, imgH, mNoColorChange);
		}
		// ============================================================
		public void DrawImage(GLImage img, int x, int y, int imgX, int imgY, int imgW, int imgH, BlendingValues b)
		{
			EnsureFiltering();

			img.IsVBOSupported = IsVBOSupported;
			img.SetBlending(b.R, b.G, b.B, b.A);
			img.Draw(x, y, imgX, imgY, imgW, imgH);

			img.SetBlending(255, 255, 255, 255);
		}
		// ============================================================
		public void DrawImage(GLImage img, int x, int y, int w, int h, int imgX, int imgY, int imgW, int imgH)
		{
			DrawImage(img, x, y, w, h, imgX, imgY, imgW, imgH, mNoColorChange);
		}
		// ============================================================
		public void DrawImage(GLImage img, int x, int y, int w, int h, int imgX, int imgY, int imgW, int imgH, BlendingValues b)
		{
			EnsureFiltering();

			img.IsVBOSupported = IsVBOSupported;
			img.SetBlending(b.R, b.G, b.B, b.A);
			img.Draw(x, y, w, h, imgX, imgY, imgW, imgH);

			img.SetBlending(255, 255, 255, 255);
		}
		// ============================================================
		public void DrawImage(GLImage img, Rectangle dest, Rectangle src)
		{
			DrawImage(img, dest, src, mNoColorChange);
		}
		// ============================================================
		public void DrawImage(GLImage img, Rectangle dest, Rectangle src, BlendingValues b)
		{
			EnsureFiltering();

			img.IsVBOSupported = IsVBOSupported;
			img.SetBlending(b.R, b.G, b.B, b.A);
			img.Draw(dest.X, dest.Y, dest.Width, dest.Height,
						src.X, src.Y, src.Width, src.Height);

			img.SetBlending(255,255,255,255);
		}
		// ============================================================
		private opengl.TextPrinter mTPrinterHigh = new opengl.TextPrinter(opengl.TextQuality.High);
		private opengl.TextPrinter mTPrinterMedium = new opengl.TextPrinter(opengl.TextQuality.Medium);
		private opengl.TextPrinter mTPrinterLow = new opengl.TextPrinter(opengl.TextQuality.Low);

		public enum TextQuality
		{
			High,
			Medium,
			Low
		}
		// ============================================================
		private void DrawString(opengl.TextPrinter printer, string text, Font font, Color c,
								RectangleF r)
		{
			printer.Begin();
			printer.Print(text, font, c, r);
			printer.End();
		}
		// ============================================================
		private void DrawString(opengl.TextPrinter printer, string text, Font font, Color c,
								float x, float y)
		{
			printer.Begin();

			GL.PushMatrix();
			GL.Translate(x, y, 0);
			
			printer.Print(text, font, c);

			GL.PopMatrix();

			printer.End();
		}
		// ============================================================
		public void DrawString(string text, Font font, Color c, float x, float y)
		{
			DrawString(mTPrinterHigh, text, font, c, x, y);
		}
		// ============================================================
		public void DrawString(string text, Font font, Color c, float x, float y,
								TextQuality q)
		{
			opengl.TextPrinter printer = null;
			switch (q)
			{
				case TextQuality.High:
					printer = mTPrinterHigh;
					break;

				case TextQuality.Medium:
					printer = mTPrinterMedium;
					break;

				case TextQuality.Low:
					printer = mTPrinterLow;
					break;
			}

			DrawString(printer, text, font, c, x, y);
		}
		// ============================================================
		private void DrawString(string text, Font font, Color c, RectangleF r)
		{
			DrawString(mTPrinterHigh, text, font, c, r);
		}
		// ============================================================
		public void DrawString(string text, Font font, Color c, RectangleF r, TextQuality q)
		{
			opengl.TextPrinter printer = null;
			switch(q)
			{
				case TextQuality.High:
					printer = mTPrinterHigh;
					break;

				case TextQuality.Medium:
					printer = mTPrinterMedium;
					break;

				case TextQuality.Low:
					printer = mTPrinterLow;
					break;
			}

			DrawString(printer, text, font, c, r);
		}
		// ============================================================
		public void DrawMultiImage(GLMultiImage img)
		{
			EnsureFiltering();

			img.IsVBOSupported = IsVBOSupported;
			img.Draw();
		}
		// ============================================================
		public void DrawMultiImage(GLMultiImage img, BlendingValues b)
		{
			EnsureFiltering();

			img.SetBlending(b.R, b.G, b.B, b.A);
			img.IsVBOSupported = IsVBOSupported;
			img.Draw();
		}
		// ============================================================
	}
}
