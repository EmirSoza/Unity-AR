namespace LivelyChatBubbles
{
	public static class Extensions
	{
		public static float Remap(this float v, float l1, float h1, float l2, float h2)
		{
			return l2 + (v - l1) * (h2 - l2) / (h1 - l1);
		}
	}
}
