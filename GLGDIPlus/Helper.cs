using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Collections.Generic;
namespace Engine
{
    public class Helper
    {
        /// <summary>
        /// Checks collision between 2 quads.
        /// </summary>
        /// <param name="x1">X of 1st object.</param>
        /// <param name="y1">Y of 1st object.</param>
        /// <param name="w1">W of 1st object.</param>
        /// <param name="h1">H of 1st object.</param>
        /// <param name="x2">X of 2nd object.</param>
        /// <param name="y2">Y of 2nd object.</param>
        /// <param name="w2">W of 2nd object.</param>
        /// <param name="h2">H of 2nd object.</param>
        /// <returns>True if objects collide.</returns>
        public bool IsCollision(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2)
        {
            if (x1 + w1 >= x2 && x1 <= x2 + w2 &&
                y1 + h1 >= y2 && y1 <= y2 + h2)
                return true;

            return false;
        }
		// ============================================================
		public static void DrawLine(System.Drawing.Color c, int x1, int y1, int x2, int y2)
		{
			GL.Disable(EnableCap.Texture2D);
			GL.Enable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Blend);
			GL.Color4(c);

			GL.Begin(BeginMode.Lines);

			GL.Vertex2((double)x1, (double)y1);
			GL.Vertex2((double)x2, (double)y2);

			GL.End();

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Texture2D);
		}
		// ============================================================
		public static void FillRectangle(System.Drawing.Color c, Rectangle r)
		{
			FillRectangle(c, r.X, r.Y, r.Width, r.Height);
		}
		// ============================================================
		public static void FillRectangle(System.Drawing.Color c, int x, int y, int w, int h)
		{
			GL.Disable(EnableCap.Texture2D);
			GL.Enable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Blend);
			GL.Color4(c);

			GL.Begin(BeginMode.Quads);

			GL.Vertex2((double)x, (double)y);
			GL.Vertex2((double)x+w, (double)y);
			GL.Vertex2((double)x+w, (double)y+h);
			GL.Vertex2((double)x, (double)y + h);

			GL.End();

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Texture2D);
		}
		// ============================================================
		public static void DrawRectangle(System.Drawing.Color c, Rectangle r)
		{
			DrawRectangle(c, r.X, r.Y, r.Width, r.Height);
		}
		// ============================================================
		public static void DrawRectangle(System.Drawing.Color c, int x, int y, int w, int h)
		{
			GL.Disable(EnableCap.Texture2D);
			GL.Enable(EnableCap.ColorArray);
			GL.Enable(EnableCap.Blend);
			GL.Color4(c);

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
		public static void DrawPoint(Color c, Point pnt, float size)
		{
			DrawPointF( c, new PointF(pnt.X,pnt.Y), size );
		}
		// ============================================================
		// ============================================================
		public static void DrawPointF(Color c, PointF pnt, float size)
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
		public static void DrawPointsF( System.Drawing.Color c, List<PointF> points, float size )
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
		public static void SetLinearBlend(bool value)
		{
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
    }
}
