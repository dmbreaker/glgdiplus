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
			g.FillRectangle(Color.Gray, (Width - 60) / 2, (Height - 40) / 2, 60, 40);
			g.DrawLine(Color.Green, 0, 0, Width, Height);
			g.DrawString("This is a text on control", mFont, Color.AliceBlue, 150, 0);

			g.DrawString("Уменьшенное изображение", mFont, Color.AliceBlue, 10, 120 + (mImage.Height / 3));
			g.DrawString("Functions:\n DrawString\n DrawImage\n DrawLine\n DrawRectangle\n FillRectangle\n DrawPoint\n DrawPoints\n DrawMultiImage", mFont, Color.AliceBlue, 350, 0);

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

			mImage.Load("./res/mult.jpg");
			mMultiImage.Load("./res/tile32.png");
			mMultiImage.SetImageTiles(new List<RectangleF>() {
			                                    new RectangleF(0,0,32,32),
			                                    new RectangleF(32,32,32,32),
			                                    new RectangleF(64,64,32,32),
			                                    new RectangleF(0,32,32,32),
			                                }
										);
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
