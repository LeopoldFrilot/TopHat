﻿using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	/// <summary>
	/// Representation of an angle.
	/// </summary>
	[Serializable]
	public struct FAngle : IEquatable<FAngle>
	{
		public FP Radians;

		private FAngle(FP radians)
		{
			Radians = radians;
		}

		public FP Degrees
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Radians * FP.Rad2Deg;
		}

		public FRotation2D Counterclockwise
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FRotation2D(this);
		}

		public FRotation2D Clockwise
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FRotation2D(new FAngle(-Radians));
		}

		public static FAngle Zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FAngle(FP.Zero);
		}

		public static FAngle TwoPI
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FAngle(FP.TwoPi);
		}

		public static FAngle HalfPI
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FAngle(FP.HalfPi);
		}

		public static FAngle PI
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FAngle(FP.Pi);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle FromDegrees(FP degrees)
		{
			return new FAngle(degrees * FP.Deg2Rad);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle FromRadians(FP radians)
		{
			return new FAngle(radians);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle Normalize360(FAngle angle)
		{
			var angleRad = angle.Radians % FP.TwoPi;

			if (angleRad < FP.Zero)
			{
				angleRad += FP.TwoPi;
			}
			else if (angleRad >= FP.TwoPi)
			{
				angleRad -= FP.TwoPi;
			}

			return new FAngle(angleRad);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle Normalize180(FAngle angle)
		{
			var angleRad = angle.Radians % FP.TwoPi;

			if (angleRad <= -FP.Pi)
			{
				angleRad += FP.TwoPi;
			}
			else if (angleRad > FP.Pi)
			{
				angleRad -= FP.TwoPi;
			}

			return new FAngle(angleRad);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle Lerp360(FAngle from, FAngle to, FP factor)
		{
			var difference = Normalize180(to - from);

			return Normalize360(from + difference * FMath.Clamp01(factor));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle Lerp180(FAngle from, FAngle to, FP factor)
		{
			var difference = Normalize180(to - from);

			return Normalize180(from + difference * FMath.Clamp01(factor));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle Abs(FAngle angle)
		{
			return new FAngle(FMath.Abs(angle.Radians));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle Delta(FAngle from, FAngle to)
		{
			return Normalize180(to - from);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle UnsignedDelta(FAngle from, FAngle to)
		{
			return Abs(Delta(from, to));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle MoveTowards(FAngle from, FAngle to, FAngle maxDelta)
		{
			var delta = Delta(from, to);

			if (-maxDelta < delta && delta < maxDelta)
				return to;

			return FromRadians(FMath.MoveTowards(from.Radians, (from + delta).Radians, maxDelta.Radians));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle Max(FAngle a, FAngle b)
		{
			return FromRadians(FMath.Max(a.Radians, b.Radians));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle Min(FAngle a, FAngle b)
		{
			return FromRadians(FMath.Min(a.Radians, b.Radians));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle operator *(FAngle angle, FP value)
		{
			return new FAngle(angle.Radians * value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle operator *(FP value, FAngle angle)
		{
			return angle * value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle operator *(FAngle angle, int value)
		{
			return new FAngle(angle.Radians * value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle operator *(int value, FAngle angle)
		{
			return angle * value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle operator /(FAngle angle, FP value)
		{
			return new FAngle(angle.Radians / value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle operator /(FAngle angle, int value)
		{
			return new FAngle(angle.Radians / value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle operator +(FAngle a, FAngle b)
		{
			return new FAngle(a.Radians + b.Radians);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle operator -(FAngle a)
		{
			return new FAngle(-a.Radians);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle operator -(FAngle a, FAngle b)
		{
			return -b + a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FAngle a, FAngle b)
		{
			return FMath.ApproximatelyEqual(a.Radians, b.Radians);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FAngle a, FAngle b)
		{
			return !(a == b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(FAngle a, FAngle b)
		{
			return a.Radians >= b.Radians;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(FAngle a, FAngle b)
		{
			return a.Radians <= b.Radians;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(FAngle a, FAngle b)
		{
			return a.Radians > b.Radians;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(FAngle a, FAngle b)
		{
			return a.Radians < b.Radians;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			return obj is FAngle other && Equals(other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FAngle other) => Radians.Equals(other.Radians);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => Radians.GetHashCode();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => $"{Degrees}°";
	}
}
