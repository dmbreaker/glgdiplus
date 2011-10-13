//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace GLGDIPlus
{
    public class GLImageBase
    {
        public int Width, Height, OriginX, OriginY;
        public float Rotation, ScaleX = 1.0f, ScaleY = 1.0f;
        public bool IsOriginChanged, IsBlending = true;
        public byte RBlend = 255, GBlend = 255, BBlend = 255, ABlend = 255; // Blending colors


        /// <summary>
        /// Sets new size.
        /// </summary>
        /// <param name="w">New width.</param>
        /// <param name="h">New height.</param>
        public void SetSize(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }


        /// <summary>
        /// Sets blending.
        /// </summary>
        public virtual void SetBlending()
        {
            IsBlending = true;
        }



        /// <summary>
        /// Sets blending.
        /// </summary>
        /// <param name="a">Alpha intensity.</param>
		public virtual void SetBlending(byte a)
        {
            IsBlending = true;

            this.ABlend = a;
        }


        /// <summary>
        /// Sets blending.
        /// </summary>
        /// <param name="r">Red intensity.</param>
        /// <param name="g">Green intensity.</param>
        /// <param name="b">Blue intensity.</param>
        /// <param name="a">Alpha intensity.</param>
		public virtual void SetBlending(byte r, byte g, byte b, byte a)
        {
            IsBlending = true;

            this.RBlend = r;
            this.GBlend = g;
            this.BBlend = b;
            this.ABlend = a;
        }


        /// <summary>
        /// Disables blending.
        /// </summary>
		public virtual void DisableBlending()
        {
            IsBlending = false;
        }


        /// <summary>
        /// Prepares drawing.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="w">Width of frame.</param>
        /// <param name="h">Height of frame.</param>
        protected void Begin(int x, int y, int w, int h)
        {
            // Enable blending if allowed
            if (IsBlending)
            {
                GL.Enable(EnableCap.Blend);
                GL.Color4(RBlend, GBlend, BBlend, ABlend);
            }

            // Rotate around user specified origin
            if (IsOriginChanged)
            {
                GL.Translate(OriginX, OriginY, 0.0);
                GL.Rotate(Rotation, 0.0f, 0.0f, 1.0f);
                GL.Translate(-OriginX, -OriginY, 0.0);
            }
            // Else use frame center as origin
            else
            {
                GL.Translate(x + w / 2, y + h / 2, 0.0);
                GL.Rotate(Rotation, 0.0f, 0.0f, 1.0f);
                GL.Translate(-(x + w / 2), -(y + h / 2), 0.0);
            }

            // Scale
            GL.Scale(ScaleX, ScaleY, 0.0f);
        }


        /// <summary>
        /// Ends drawing.
        /// </summary>
        protected void End()
        {
            // Translate
            GL.LoadIdentity();
            //GL.Translate(0.375, 0.375, 0.0);
			GL.Translate(0.0, 0.0, 0.0);

            // Disable blending
            if (IsBlending)
            {
                GL.Disable(EnableCap.Blend);
                
                // Set white color
                GL.Color4((byte)255, (byte)255, (byte)255, (byte)255);
            }
        }
    }
}
