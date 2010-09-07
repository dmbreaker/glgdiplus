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
		// ============================================================
		public GLSample()
		{
			InitializeComponent();
		}
		// ============================================================
		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.DesignMode)
			{
				e.Graphics.Clear(this.BackColor);
				e.Graphics.Flush();
				return;
			}

			if (!mIsLoaded)		// чтобы отследить, что OpenGL контекст еще не создан
				return;

			MakeCurrent();

			GLGraphics g = mGraphics;
			g.StartDraw();		// нужно вызывать, чтобы выполнить необходимые действия перед отрисовкой

			g.Reset();
			g.Clear();

			g.DrawImage(mImage, 0, 0, Width, Height);
			g.FillRectangle(Color.Blue, (Width - 60) / 2, (Height - 40) / 2, 60, 40);
			g.DrawLine(Color.Green, 0, 0, Width, Height);

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
			mGraphics.SetLinearFiltering(false);	// чтобы картинки были четкими (но при скейлинге они будут пикселизированы)
			mGraphics.SetClearColor(SystemColors.ActiveCaption);

			mImage.Load("./res/mult.jpg");
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
