using System;

namespace AssetRipper.TpkCreation.Tests
{
	internal static class RandomUtils
	{
		public static Random RandomInstance { get; } = new Random();

		/// <summary>
		/// Get a random integer inside a range
		/// </summary>
		/// <param name="min">Minimum inclusive</param>
		/// <param name="max">Maximum exclusive</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException">Max less than or equal to min</exception>
		public static int RandomInt(int min, int max)
		{
			return max - min > 0 ? RandomInstance.Next(max - min) : throw new ArgumentOutOfRangeException();
		}

		public static byte[] RandomBytes() => RandomBytes(16, 65);
		public static byte[] RandomBytes(int min, int max) => RandomBytes(RandomInt(min, max));
		public static byte[] RandomBytes(int count)
		{
			byte[] bytes = new byte[count];
			RandomInstance.NextBytes(bytes);
			return bytes;
		}

		public static string RandomString() => RandomString(4, 12);
		public static string RandomString(int minLength, int maxLength) => RandomString(RandomInt(minLength, maxLength));
		public static string RandomString(int length)
		{
			char[] characters = new char[length];
			for(int i = 0; i < length; i++)
			{
				characters[i] = RandomLowerCaseLetter();
			}
			return new string(characters);
		}

		public static char RandomLowerCaseLetter()
		{
			int index = RandomInstance.Next(26);
			return (char)('a' + index);
		}
	}
}
