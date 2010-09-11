namespace GLGDIPlus
{
    public struct Vertex
    {
        public float x, y;
    }


	public struct TexCoord
    {
        public float u, v;
    }

	public struct BlendingValues
	{
		public byte R;
		public byte G;
		public byte B;
		public byte A;

		public BlendingValues(byte a)
		{
			R = G = B = 255;
			A = a;
		}

		public BlendingValues(byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;
			A = 255;
		}

		public BlendingValues(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public void Init(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}
	}
}
