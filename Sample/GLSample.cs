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
		// ============================================================
		public GLSample()
		{
			InitializeComponent();
		}
		// ============================================================
		protected override void OnPaint(PaintEventArgs e)
		{
			if (!mIsLoaded)		// чтобы отследить, что OpenGL контекст еще не создан
				return;

			MakeCurrent();

			GLGraphics g = mGraphics;
			g.Reset();
			g.Clear();

			SwapBuffers();
		}
		// ============================================================
		private void GLSample_Load(object sender, EventArgs e)
		{
			mIsLoaded = true;	// OpenGL контекст уже должен быть создан

			if (this.IsHandleCreated)
			{
				mGraphics.Init();
				mGraphics.Clear();
				mGraphics.Reset();
				mGraphics.SetLinearFiltering(false);	// чтобы картинки были четкими (но при скейлинге они будут пикселизированы)
				mGraphics.SetClearColor(SystemColors.ActiveCaption);
			}
		}
		// ============================================================
	}
}
