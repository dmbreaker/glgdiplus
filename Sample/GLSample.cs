using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using GLGDIPlus;

namespace Sample
{
	public partial class GLSample : GLControl
	{
		// ============================================================
		bool mIsLoaded = false;

		GLGraphics mGraphics = new GLGraphics();
		GLImage mImage = new GLImage();
		GLMultiImage mMultiImage = new GLMultiImage();
		GLTriangles mTris = new GLTriangles();
		// ============================================================
		public GLSample()
		{
			InitializeComponent();
		}
		// ============================================================
		Font mFont = new Font("Arial", 10f); 
		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.DesignMode)	// в режиме дизайна просто закрашиваем контрол цветом
			{
				e.Graphics.Clear(this.BackColor);
				e.Graphics.Flush();
				return;
			}

			if (!mIsLoaded)		// если OpenGL контекст еще не создан
				return;

			MakeCurrent();

			GLGraphics g = mGraphics;

			g.Reset();
			g.Clear();
			g.DrawMultiImage(mMultiImage);

			g.DrawImage(mImage, 10, 120, mImage.Width/3, mImage.Height/3);	// downscaled image
			Rectangle destR = new Rectangle(10, 250,mImage.Width / 3, mImage.Height / 4);
			Rectangle srcR = new Rectangle(50,50,mImage.Width/2, mImage.Height);
			g.DrawImage(mImage, destR, srcR,  new BlendingValues(128,128,128));	// downscaled image
			g.FillRectangle(Color.Gray, (Width - 60) / 2, (Height - 40) / 2, 60, 40);
			g.DrawLine(Color.Green, 0, 0, Width, Height);
			g.DrawString("This is a text on control", mFont, Color.AliceBlue, 150, 0);

			g.DrawString("Уменьшенное изображение", mFont, Color.AliceBlue, 10, 120 + (mImage.Height / 3));
			g.DrawString("Блендинг + скейлинг части изображения", mFont, Color.AliceBlue, destR.X, destR .Y + (mImage.Height / 4));
			g.DrawString("Functions:\n DrawString\n DrawImage\n DrawLine\n DrawRectangle\n FillRectangle\n DrawPoint\n DrawPoints\n DrawMultiImage", mFont, Color.AliceBlue, 350, 0);

			g.DrawTris(mTris);

			SwapBuffers();
		}
		// ============================================================
		private void GLSample_Load(object sender, EventArgs e)
		{
			if (this.DesignMode)
				return;


			mIsLoaded = true;	// OpenGL контекст уже должен быть создан

			mGraphics.Init();
			mGraphics.Reset();
			mGraphics.Clear();
			mGraphics.SetClearColor(SystemColors.ActiveCaption);

			mImage.Load("../../res/mult.jpg");
			mMultiImage.Load("../../res/tile32.png");
			mMultiImage.SetImageTiles(new List<RectangleF>() {
			                                    new RectangleF(0,0,32,32),
			                                    new RectangleF(32,32,32,32),
			                                    new RectangleF(64,64,32,32),
			                                    new RectangleF(0,32,32,32),
			                                }
										);

			float sx = 280;
			float sy = 220;
			float k = 70;
			var tris = new List<STri>();
			tris.Add(new STri(sx, sy, sx + k, sy + k, sx + k, sy+20, 0, 0, 1, 1, 1, 0));
			tris.Add(new STri(sx, sy, sx, sy + k+20, sx + k, sy + k, 0, 0, 0, 1, 1, 1));
			//tris.Add(new STri(0, 0, 20, 0, 20, 20, 0, 0, 0, 1, 1, 1));
			mTris.Load("../../res/mult.jpg");
			mTris.SetVertices(tris);
		}

		private void GLSample_Resize(object sender, EventArgs e)
		{
			if (this.DesignMode)
			{
				return;
			}

			mGraphics.Resize(Width, Height);
		}
		// ============================================================
	}
}
