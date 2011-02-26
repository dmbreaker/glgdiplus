using System;
using OpenTK.Graphics.OpenGL;


namespace GLGDIPlus
{
    public class VBO
    {
		internal Vertex[] Vertices;     // Image vertices
		internal TexCoord[] Texcoords;  // Image texture coordinates

		internal int VboID;
		internal int TexID;

		internal bool IsVBOSupported = false;


        /// <summary>
        /// Builds VBO for vertices.
        /// </summary>
		internal void BuildVertices()
        {
            //if (GL.SupportsExtension("VERSION_1_5"))
			if( IsVBOSupported )
            {
                // Delete old VBO
                if (VboID != 0) GL.DeleteBuffers(1, ref VboID);

                // VBO for vertices
                GL.GenBuffers(1, out VboID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VboID);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * 2 * sizeof(float)), Vertices, BufferUsageHint.StaticDraw);
            }
        }


        /// <summary>
        /// Builds VBO for texcoords.
        /// </summary>
        internal void BuildTex()
        {
            //if (GL.SupportsExtension("VERSION_1_5"))
			if (IsVBOSupported)
            {
                // Delete old VBO
                if (TexID != 0) GL.DeleteBuffers(1, ref TexID);

                // VBO for texcoords
                GL.GenBuffers(1, out TexID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, TexID);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Texcoords.Length * 2 * sizeof(float)), Texcoords, BufferUsageHint.StaticDraw);
            }
        }


        /// <summary>
        /// Draws VBO.
        /// </summary>
        internal void Draw()
        {
            Draw(Vertices.Length, BeginMode.Quads);
        }


        /// <summary>
        /// Draws VBO.
        /// </summary>
        /// <param name="mode">Mode used for drawing.</param>
        internal void Draw(BeginMode mode)
        {
            Draw(Vertices.Length, mode);
        }


        /// <summary>
        /// Draws VBO.
        /// </summary>
        /// <param name="lenght">Number of vertices to be drawn from array.</param>
        internal void Draw(int lenght)
        {
            Draw(lenght, BeginMode.Quads);
        }


        /// <summary>
        /// Draws VBO.
        /// </summary>
        /// <param name="lenght">Number of vertices to be drawn from array.</param>
        /// <param name="mode">Mode used for drawing.</param>
        internal void Draw(int length, BeginMode mode)
        {
            // Use VBOs if they are supported
            //if (GL.SupportsExtension("VERSION_1_5"))
			if (IsVBOSupported)
            {
				GL.EnableClientState(ArrayCap.VertexArray);
				GL.EnableClientState(ArrayCap.TextureCoordArray);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VboID);
				GL.VertexPointer(2, VertexPointerType.Float, 0, IntPtr.Zero);

                GL.BindBuffer(BufferTarget.ArrayBuffer, TexID);
				GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero);

                GL.DrawArrays(mode, 0, length);

				GL.DisableClientState(ArrayCap.VertexArray);
				GL.DisableClientState(ArrayCap.TextureCoordArray);
            }
            // Use immediate mode
            else
            {
                GL.Begin(mode);

                for (int i = 0; i < length; i++)
                {
                    GL.TexCoord2(Texcoords[i].u, Texcoords[i].v);
                    GL.Vertex2(Vertices[i].x, Vertices[i].y);
                }

                GL.End();
            }
        }
		// ============================================================
    }
}
