using System;
using OpenTK.Graphics.OpenGL;


namespace Engine
{
    public class VBO
    {
        public Vertex[] vertices;     // Image vertices
        public TexCoord[] texcoords;  // Image texture coordinates

        int vboID;
        int texID;


        /// <summary>
        /// Builds VBO for vertices.
        /// </summary>
        public void Build()
        {
            //if (GL.SupportsExtension("VERSION_1_5"))
			string verion = GL.GetString(StringName.Version);
			if( true )
            {
                // Delete old VBO
                if (vboID != 0) GL.DeleteBuffers(1, ref vboID);

                // VBO for vertices
                GL.GenBuffers(1, out vboID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * 2 * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
            }
        }


        /// <summary>
        /// Builds VBO for texcoords.
        /// </summary>
        public void BuildTex()
        {
            //if (GL.SupportsExtension("VERSION_1_5"))
			string verion = GL.GetString(StringName.Version);
			if (true)
            {
                // Delete old VBO
                if (texID != 0) GL.DeleteBuffers(1, ref texID);

                // VBO for texcoords
                GL.GenBuffers(1, out texID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, texID);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(texcoords.Length * 2 * sizeof(float)), texcoords, BufferUsageHint.StaticDraw);
            }
        }


        /// <summary>
        /// Draws VBO.
        /// </summary>
        public void Draw()
        {
            Draw(vertices.Length, BeginMode.Quads);
        }


        /// <summary>
        /// Draws VBO.
        /// </summary>
        /// <param name="mode">Mode used for drawing.</param>
        public void Draw(BeginMode mode)
        {
            Draw(vertices.Length, mode);
        }


        /// <summary>
        /// Draws VBO.
        /// </summary>
        /// <param name="lenght">Number of vertices to be drawn from array.</param>
        public void Draw(int lenght)
        {
            Draw(lenght, BeginMode.Quads);
        }


        /// <summary>
        /// Draws VBO.
        /// </summary>
        /// <param name="lenght">Number of vertices to be drawn from array.</param>
        /// <param name="mode">Mode used for drawing.</param>
        public void Draw(int lenght, BeginMode mode)
        {
            // Use VBOs if they are supported
            //if (GL.SupportsExtension("VERSION_1_5"))
			string verion = GL.GetString(StringName.Version);
			if (true)
            {
				GL.EnableClientState(ArrayCap.VertexArray | ArrayCap.TextureCoordArray);
                //GL.EnableClientState(EnableCap.VertexArray);
                //GL.EnableClientState(EnableCap.TextureCoordArray);

                GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
				GL.VertexPointer(2, VertexPointerType.Float, 0, IntPtr.Zero);

                GL.BindBuffer(BufferTarget.ArrayBuffer, texID);
				GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero);

                GL.DrawArrays(mode, 0, lenght);

				GL.DisableClientState(ArrayCap.VertexArray | ArrayCap.TextureCoordArray);
				//GL.DisableClientState(EnableCap.VertexArray);
				//GL.DisableClientState(EnableCap.TextureCoordArray);
            }
            // Use immediate mode
            else
            {
                GL.Begin(mode);

                for (int i = 0; i < lenght; i++)
                {
                    GL.TexCoord2(texcoords[i].u, texcoords[i].v);
                    GL.Vertex2(vertices[i].x, vertices[i].y);
                }

                GL.End();
            }
        }
    }
}
