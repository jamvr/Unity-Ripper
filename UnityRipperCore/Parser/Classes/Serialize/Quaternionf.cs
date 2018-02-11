using System;

namespace UnityRipper.Classes
{
	public sealed class Quaternionf : Vector4f
	{
		public Quaternionf()
		{
		}

		public Quaternionf(float x, float y, float z, float w):
			base(x, y, z, w)
		{
		}

		public Vector3f ToEuler()
		{
			double eax = 0;
			double eay = 0;
			double eaz = 0;

			float qx = X;
			float qy = -Y;
			float qz = -Z;
			float qw = W;

			double[,] M = new double[4, 4];

			double Nq = qx * qx + qy * qy + qz * qz + qw * qw;
			double s = (Nq > 0.0) ? (2.0 / Nq) : 0.0;
			double xs = qx * s, ys = qy * s, zs = qz * s;
			double wx = qw * xs, wy = qw * ys, wz = qw * zs;
			double xx = qx * xs, xy = qx * ys, xz = qx * zs;
			double yy = qy * ys, yz = qy * zs, zz = qz * zs;

			M[0, 0] = 1.0 - (yy + zz); M[0, 1] = xy - wz; M[0, 2] = xz + wy;
			M[1, 0] = xy + wz; M[1, 1] = 1.0 - (xx + zz); M[1, 2] = yz - wx;
			M[2, 0] = xz - wy; M[2, 1] = yz + wx; M[2, 2] = 1.0 - (xx + yy);
			M[3, 0] = M[3, 1] = M[3, 2] = M[0, 3] = M[1, 3] = M[2, 3] = 0.0; M[3, 3] = 1.0;

			double test = Math.Sqrt(M[0, 0] * M[0, 0] + M[1, 0] * M[1, 0]);
			if (test > 16 * 1.19209290E-07F)//FLT_EPSILON
			{
				eax = Math.Atan2(M[2, 1], M[2, 2]);
				eay = Math.Atan2(-M[2, 0], test);
				eaz = Math.Atan2(M[1, 0], M[0, 0]);
			}
			else
			{
				eax = Math.Atan2(-M[1, 2], M[1, 1]);
				eay = Math.Atan2(-M[2, 0], test);
				eaz = 0;
			}

			float x = (float)(eax * 180.0 / Math.PI);
			float y = (float)(eay * 180.0 / Math.PI);
			float z = (float)(eaz * 180.0 / Math.PI);
			Vector3f euler = new Vector3f(x, y, z);
			return euler;
		}
	}
}
