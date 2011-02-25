using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GLGDIPlus
{
	public class STri
	{
		public Vertex[] vert = new Vertex[3];
		public TexCoord[] tex = new TexCoord[3];

		// ============================================================
		// ============================================================
		public STri( float x0, float y0, float x1, float y1, float x2, float y2,
						float u0, float v0, float u1, float v1, float u2, float v2)
		{
			vert[0].x = x0;
			vert[0].y = y0;
			vert[1].x = x1;
			vert[1].y = y1;
			vert[2].x = x2;
			vert[2].y = y2;

			tex[0].u = u0;
			tex[0].v = v0;
			tex[1].u = u1;
			tex[1].v = v1;
			tex[2].u = u2;
			tex[2].v = v2;
		}
		// ============================================================
		// ============================================================
		public STri(PointF p0, PointF p1, PointF p2,
						PointF t0, PointF t1, PointF t2)
		{
			vert[0].x = p0.X;
			vert[0].y = p0.Y;
			vert[1].x = p1.X;
			vert[1].y = p1.Y;
			vert[2].x = p2.X;
			vert[2].y = p2.Y;

			tex[0].u = t0.X;
			tex[0].v = t0.Y;
			tex[1].u = t1.X;
			tex[1].v = t1.Y;
			tex[2].u = t2.X;
			tex[2].v = t2.Y;
		}
		// ============================================================
	}
}
