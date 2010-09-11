namespace Sample
{
	partial class Sample1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.glSample1 = new Sample.GLSample();
			this.SuspendLayout();
			// 
			// glSample1
			// 
			this.glSample1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.glSample1.BackColor = System.Drawing.Color.Black;
			this.glSample1.Location = new System.Drawing.Point(13, 13);
			this.glSample1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.glSample1.Name = "glSample1";
			this.glSample1.Size = new System.Drawing.Size(509, 366);
			this.glSample1.TabIndex = 0;
			this.glSample1.VSync = false;
			// 
			// Sample1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(535, 392);
			this.Controls.Add(this.glSample1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Sample1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private GLSample glSample1;
	}
}

