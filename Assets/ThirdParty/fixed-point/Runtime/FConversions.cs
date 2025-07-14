using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static class FConversions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP ToFP(this long value)
		{
			return FP.FromRaw(value << FP.FractionalBits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP ToFP(this int value)
		{
			return FP.FromRaw((long)value << FP.FractionalBits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP ToFP(this float value)
		{
			return FP.FromRaw((long)(value * FP.OneRaw));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP ToFP(this double value)
		{
			return FP.FromRaw((long)(value * FP.OneRaw));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt(this FP value)
		{
			return (int)(value.RawValue >> FP.FractionalBits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToLong(this FP value)
		{
			return value.RawValue >> FP.FractionalBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToFloat(this FP value)
		{
			return (float)value.RawValue / FP.OneRaw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this FP value)
		{
			return (double)value.RawValue / FP.OneRaw;
		}
	}
}
