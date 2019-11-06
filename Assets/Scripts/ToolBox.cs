//using System;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization.Formatters.Soap;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

class FunctionList
{

}

class RemoteLoader
{

}

public class HistoryKeyFrameSimple
{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 velocity;
}

public class HistoryKeyFrameVehicle
{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 velocity;

	public float steering;
	public float throttle;
	public float brake;
}

public class GUIHelper
{
	static public Rect PixelsToScreen(Rect pixelsRect)
	{
		return new Rect(pixelsRect.x / Screen.width, pixelsRect.y / Screen.height, pixelsRect.width / Screen.width, pixelsRect.height / Screen.height);
	}
	static public Rect ScreenToPixels(Rect screenRect)
	{
		return new Rect(screenRect.x * Screen.width, screenRect.y * Screen.height, screenRect.width * Screen.width, screenRect.height * Screen.height);
	}
}

public class MathsHelper
{
	private readonly float _radians;
	//Power converters
	static public float hpToKwConstant = 0.746f;
	static public float kwToNmConstant = 9.5488f;

	//Measurement converters
	static public float mmToCmConstant = 10;
	static public float mmToMtsConstant = 1000;
	static public float cmToMtsConstant = 100;
	static public float mtsToMmConstant = 1000;
	static public float mtsToCmConstant = 100;

	//Heat constants
	static public float kelvinConstant = 273;
	static public float universalGasConstant = 1.9869f;

	//Math constants
	static public float naturalLog = 2.71828182845904523536f; // CHECK need to be double?, see if it's needed anywhere (seems most function are floats that use it)
	static public float epsilon = 1e-06f;

	public static float twoPi = Mathf.PI * 2;

	// Random Vector3
	public static Vector3 RandomVector3(float min, float max)
	{
		return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
	}

	// Random direction
	public static Vector3 RandomDirection()
	{
		return RandomVector3(-1,1).normalized;
	}

	// Expand value from -1,1, to 0,1
	public static float Expand(float value)
	{
		return value/2 + 0.5f;
	}

	// Either returns 1 or -1
	public static float Normalise(float value)
	{
		return value / Mathf.Abs(value);
	}
	public static int Normalise(int value)
	{
		return value / Mathf.Abs(value);
	}
	public static bool IsPositive(float value)
	{
		return value / Mathf.Abs(value) > 0;
	}

	public static int Wrap(int value, int upperValue)
	{
		if (value > 0 && value < upperValue)
			return value;
		else if (value > upperValue)
		{
			return value % upperValue;
		}
		else
			return upperValue - value;
	}
	
	public static Vector3 MultiplyVector3(Vector3 one, Vector3 two)
	{
		return new Vector3(one.x * two.x, one.y * two.y, one.z * two.z);	
	}
	
	public static Color NormalizeColor(Color col)
	{
		//Calculate the magnitude of the color rgb
		float inverseMagnitude = 1.0f / Mathf.Sqrt((col.r * col.r) + (col.g * col.g) + (col.b * col.b));
		
		return new Color(col.r * inverseMagnitude, col.g * inverseMagnitude, col.b * inverseMagnitude, col.a);
	}

	public static Quaternion ConjugateQuaternion(Quaternion q)
	{
		Quaternion cQ;

		Vector3 flippedAxis = -1.0f * new Vector3(q.x, q.y, q.z);
		cQ = new Quaternion(flippedAxis.x, flippedAxis.y, flippedAxis.z, q.w);

		return cQ;
	}
	
	public static Quaternion MatrixToQuaternion(Matrix4x4 m)
	{
		// Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
		Quaternion q = new Quaternion();
		
		float trace = m.m00 + m.m11 + m.m22;
		
		if( trace > 0 ) 
		{
			float s = 0.5f / Mathf.Sqrt(trace+ 1.0f);
			q.w = 0.25f / s;
			q.x = ( m.m21 - m.m12 ) * s;
			q.y = ( m.m02 - m.m20 ) * s;
			q.z = ( m.m10	 - m.m01 ) * s;
		} 
		else 
		{
			if ( m.m00 > m.m11 && m.m00 > m.m22 ) 
			{
				float s = 2.0f * Mathf.Sqrt( 1.0f + m.m00 - m.m11 - m.m22);
				q.w = (m.m21 - m.m12 ) / s;
				q.x = 0.25f * s;
				q.y = (m.m01 + m.m10 ) / s;
				q.z = (m.m02 + m.m20 ) / s;
			} 
			else if (m.m11 > m.m22) 
			{
				float s = 2.0f * Mathf.Sqrt( 1.0f + m.m11 - m.m00 - m.m22);
				q.w = (m.m02 - m.m20 ) / s;
				q.x = (m.m01 + m.m10 ) / s;
				q.y = 0.25f * s;
				q.z = (m.m12 + m.m21 ) / s;
			} 
			else 
			{
				float s = 2.0f * Mathf.Sqrt( 1.0f + m.m22 - m.m00 - m.m11 );
				q.w = (m.m10 - m.m01 ) / s;
				q.x = (m.m02 + m.m20 ) / s;
				q.y = (m.m12 + m.m21 ) / s;
				q.z = 0.25f * s;
			 }
		}
		
		return q;
	}
	
	public static Matrix4x4 QuaternionToMatrix(Quaternion q)
	{
		Matrix4x4 m = new Matrix4x4();
		
		//Square the components of the quaternion
		float w2 = q.w * q.w;
		float x2 = q.x * q.x;
		float y2 = q.y * q.y;
		float z2 = q.z * q.z;
		
		//Inverse (inverse square length) is only required if quaternion is not already normalised
		float invs = 1.0f / (x2 + y2 + z2 + w2);
		
		float m00 = (x2 - y2 - z2 + w2) * invs;
		float m11 = (-x2 + y2 - z2 + w2) * invs;
		float m22 = (-x2 - y2 + z2 + w2) * invs;
		
		float xy = q.x * q.y;
		float zw = q.z * q.w;
		float m10 = 2.0f * (xy + zw) * invs;
		float m01 = 2.0f * (xy - zw) * invs;
		
		float xz = q.x * q.z;
		float yw = q.y * q.w;
		float m20 = 2.0f * (xz - yw) * invs;
		float m02 = 2.0f * (xz + yw) * invs;
		
		float yz = q.y * q.z;
		float xw = q.x * q.w;
		float m21 = 2.0f * (yz + xw) * invs;
		float m12 = 2.0f * (yz - xw) * invs;
		
		m.SetRow(0, new Vector4(m00, m01, m02, 0.0f));
		m.SetRow(1, new Vector4(m10, m11, m12, 0.0f));
		m.SetRow(2, new Vector4(m20, m21, m22, 0.0f));
		m.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
		
		return m;
	}

	//Subtracts matrix 1 from matrix 2
	public static Matrix4x4 MatrixSubtract(Matrix4x4 m1, Matrix4x4 m2)
	{
		Matrix4x4 matrix = new Matrix4x4();

		matrix.SetRow(0, new Vector4(m2.m00 - m1.m00, m2.m01 - m1.m01, m2.m03 - m1.m03, 0));
		matrix.SetRow(0, new Vector4(m2.m10 - m1.m10, m2.m11 - m1.m11, m2.m13 - m1.m13, 0));
		matrix.SetRow(0, new Vector4(m2.m20 - m1.m20, m2.m21 - m1.m21, m2.m23 - m1.m23, 0));
		matrix.SetRow(0, new Vector4(0,0,0, 1));
		return matrix;
	}

	public static Matrix4x4 MatrixFromVector3(Vector3 Right, Vector3 Up, Vector3 Forward)
	{
		Matrix4x4 matrix = new Matrix4x4();

		matrix.SetRow(0, new Vector4(Right.x, Right.y, Right.z, 0.0f));
		matrix.SetRow(1, new Vector4(Up.x, Up.y, Up.z, 0.0f));
		matrix.SetRow(2, new Vector4(Forward.x, Forward.y, Forward.z, 0.0f));
		matrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

		return matrix;
	}

	public static Vector3 SmoothVector3(Vector3 from, Vector3 to, float smoothingFactor)
	{
		return ((from*smoothingFactor) + to)/(smoothingFactor + 1.0f);
	}

	public static float SmoothFloat(float from, float to, float smoothingFactor)
	{
		return ((from * smoothingFactor) + to) / (smoothingFactor + 1.0f);
	}
	
	//Generate a perpendicular vector from a vector3
	public static Vector3 PerpendicularFromVector3(Vector3 vector)
	{
		return Vector3.Cross(vector.normalized, Vector3.up);
	}

	public static float Distance(Vector3 value1, Vector3 value2)
	{
		return (value1 - value2).magnitude;
	}

	public static float RescaleValueToFloat(float sourceLower, float sourceUpper, float value)
	{
		return (value / (sourceUpper - sourceLower)) - (sourceLower / (sourceUpper - sourceLower));
	}

	public static float InterpolateFloat(float from, float to, float amount)
	{
		return from + amount * (to - from);
	}
	/*
	public static void ClampRef<T>(ref T value, T low, T high) where T : IComparable
	{
		if (value.CompareTo(low) < 0)
			value = low;
		else if (value.CompareTo(high) > 0)
			value = high;
	}

	public static T Clamp<T>(T value, T max, T min) where T : System.IComparable<T>
	{
		T result = value;
		if (value.CompareTo(max) > 0)
			result = max;
		if (value.CompareTo(min) < 0)
			result = min;
		return result;
	}
	*/
	public static float ConvertRadiansToRevolutions(float radians)
	{
		return radians / MathsHelper.twoPi;
	}

	public static float ConvertRevolutionsToRadians(float revolutions)
	{
		return revolutions * MathsHelper.twoPi;
	}
	/*
	public static float ConvertRadiansToDegrees(float radians)
	{
		return radians * (180.0f / (float)Math.PI);
	}

	public static float ConvertDegreesToRadians(float degrees)
	{
		return degrees * ((float)Math.PI / 180.0f);
	}
	*/
	public static float Sqr(float value)
	{
		return value * value;
	}

	// ***********************************************************
	// WIKI Maths functions
	// ***********************************************************

	public static float Hermite(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
	}

	public static float Sinerp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
	}

	public static float Coserp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
	}

	public static float Berp(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	public static float SmoothStep(float x, float min, float max)
	{
		x = Mathf.Clamp(x, min, max);
		float v1 = (x - min) / (max - min);
		float v2 = (x - min) / (max - min);
		return -2 * v1 * v1 * v1 + 3 * v2 * v2;
	}

	public static float Lerp(float start, float end, float value)
	{
		return ((1.0f - value) * start) + (value * end);
	}

	public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
		float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
		return lineStart + (closestPoint * lineDirection);
	}

	public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 fullDirection = lineEnd - lineStart;
		Vector3 lineDirection = Vector3.Normalize(fullDirection);
		float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
		return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
	}
	public static float Bounce(float x)
	{
		return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
	}

	// test for value that is near specified float (due to floating point inprecision)
	// all thanks to Opless for this!
	public static bool Approx(float val1, float val2, float precision)
	{
		return (val1 >= val2 - precision && val1 <= val2 + precision);
//        return ((Mathf.Abs(val1 - val2) <= range));
	}

	// test if a Vector3 is close to another Vector3 (due to floating point inprecision)
	// compares the square of the distance to the square of the range as this
	// avoids calculating a square root which is much slower than squaring the range
	public static bool Approx(Vector3 val, Vector3 about, float precision)
	{
		return ((val - about).sqrMagnitude < precision * precision);
	}
	public static bool Approx(Vector2 val, Vector2 about, float precision)
	{
		return ((val - about).sqrMagnitude < precision * precision);
	}

	/*
	  * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
	  * This is useful when interpolating eulerAngles and the object
	  * crosses the 0/360 boundary.  The standard Lerp function causes the object
	  * to rotate in the wrong direction and looks stupid. Clerp fixes that.
	  */
	public static float Clerp(float start, float end, float value)
	{
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
		float retval = 0.0f;
		float diff = 0.0f;

		if ((end - start) < -half)
		{
			diff = ((max - start) + end) * value;
			retval = start + diff;
		}
		else if ((end - start) > half)
		{
			diff = -((max - end) + start) * value;
			retval = start + diff;
		}
		else retval = start + (end - start) * value;

		// Debug.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
		return retval;
	}

	public static float SqrDistance(Vector3 position, Vector3 target)
	{
		return (position - target).sqrMagnitude;
	}

    [System.Serializable]
    public class Range
    {
        public float min;
        public float max;
    }
    [System.Serializable]
    public class RangeInt
    {
        public int min;
        public int max;
    }
}

// Euler class for Ogre
// Author: Kojack
// License: Do whatever you want with it.
// Converted to C#: Vectrex
/*
	public class Euler
	{
		public Euler(Mogre.Radian y, Mogre.Radian p, Mogre.Radian r)
		{
			y = 0.0f;
			p = 0.0f;
			r = 0.0f;

			m_yaw = y;
			m_pitch = p;
			m_roll = r;
			m_changed = true;
		}

		// Get the Yaw angle.
		public Mogre.Radian getYaw()
		{
			return m_yaw;
		}

		// Get the Pitch angle.
		public Mogre.Radian getPitch()
		{
			return m_pitch;
		}

		// Get the Roll angle.
		public Mogre.Radian getRoll()
		{
			return m_roll;
		}

		// Apply a relative yaw. (Adds angle to current yaw)
		// Angles wrap around within the range 0 to 2*PI radians (0 to 360 degrees)
		public Euler yaw(Mogre.Radian y)
		{
			m_yaw += y;
			if (m_yaw < 0.0f)
			{
				m_yaw = (float)System.Math.IEEERemainder(m_yaw, Mathf.PI * 2.0f) + Mathf.PI * 2.0f;
			}
			else if (m_yaw > Mathf.PI)
			{
				m_yaw = (float)System.Math.IEEERemainder(m_yaw, Mathf.PI * 2.0f);
			}
			m_changed = true;
			return this;
		}

		// Apply a relative pitch. (Adds angle to current pitch)
		public Euler pitch(Mogre.Radian p)
		{
			m_pitch += p;
			m_changed = true;
			return this;
		}

		// Apply a relative roll. (Adds angle to current roll)
		public Euler roll(Mogre.Radian r)
		{
			m_roll += r;
			m_changed = true;
			return this;
		}

		// Set the yaw.
		public Euler setYaw(Mogre.Radian y)
		{
			m_yaw = y;
			m_changed = true;
			return this;
		}

		// Set the pitch.
		public Euler setPitch(Mogre.Radian p)
		{
			m_pitch = p;
			m_changed = true;
			return this;
		}

		// Set the roll.
		public Euler setRoll(Mogre.Radian r)
		{
			m_roll = r;
			m_changed = true;
			return this;
		}

		// Get a vector pointing forwards.
		public Mogre.Vector3 getForward()
		{
			return toQuaternion() * Mogre.Vector3.NEGATIVE_UNIT_Z;
		}

		// Get a vector pointing to the right.
		public Mogre.Vector3 getRight()
		{
			return toQuaternion() * Mogre.Vector3.UNIT_X;
		}

		// Get a vector pointing up.
		public Mogre.Vector3 getUp()
		{
			return toQuaternion() * Mogre.Vector3.UNIT_Y;
		}

		// Calculate the quaternion of a euler object.
		// The result is cached, it is only recalculated when the component euler angles are changed.
		public Mogre.Quaternion toQuaternion()
		{
			if (m_changed)
			{
				m_cachedQuaternion = new Quaternion(m_yaw, Mogre.Vector3.NEGATIVE_UNIT_Y) * new Quaternion(m_pitch, Mogre.Vector3.UNIT_X) * new Quaternion(m_roll, Mogre.Vector3.NEGATIVE_UNIT_Z);
				m_changed = false;
			}
			return m_cachedQuaternion;
		}
*/
// Casting operator. This allows any ogre function that wants a Quaternion to accept a Euler instead.
/*
	public static implicit operator Mogre.Quaternion(Euler ImpliedObject)
	{
		return toQuaternion();
	}
*/


// Set the yaw and pitch to face in the given direction.
// The direction doesn't need to be normalised.
// Roll is unaffected.
/*		private Euler setDirection(Mogre.Vector3 v)
		{
			Mogre.Vector3 d = v.NormalisedCopy;
			m_pitch = Mathf.ASin(d.y);
			m_yaw = Mathf.Atan2(d.z, d.x) + (Degree)(Mathf.PI / 2.0);
			m_changed = true;

			return this;
		}

		// Get the angular difference between the current yaw and the specified yaw.
		// Only yaw is considered, pitch and roll are ignored.
		private Mogre.Radian getYawToDirection(Mogre.Radian a)
		{
			float angle = (a - m_yaw);
			if (angle > Mathf.PI)
				angle = -Mathf.PI * 2.0f + angle;
			else if (angle < -Mathf.PI)
				angle = Mathf.PI * 2.0f + angle;
			return angle;
		}

		// Get the angular difference between the current yaw and the specified direction vector.
		// Only yaw is considered, pitch and roll are ignored.
		private Mogre.Radian getYawToDirection(Mogre.Vector3 v)
		{
			return getYawToDirection(Mathf.Atan2(v.z, v.x) + (Degree)Mathf.PI / 2.0f);
		}

		// Get the angular difference between the current yaw and the specified euler object.
		// Only yaw is considered, pitch and roll are ignored.
		private Mogre.Radian getYawToDirection(Euler e)
		{
			return getYawToDirection(e.m_yaw);
		}

		// Change the yaw to face in the direction of the vector.
		// Only yaw is changed, pitch and roll are ignored.
		private Euler yawToDirection(Mogre.Vector3 v)
		{
			m_yaw = getYawToDirection(v);
			m_changed = true;
			return this;
		}

		// Change the yaw to face in the direction of the euler object.
		// Only yaw is changed, pitch and roll are ignored.
		private Euler yawToDirection(Euler e)
		{
			m_yaw = getYawToDirection(e);
			m_changed = true;
			return this;
		}
*/
// stream operator, for printing the euler component angles to a stream
/*
	private static friend std.ostream operator <<(ref std.ostream o, Euler e)
	{
		o << "<Y:" << e.m_yaw << ", P:" << e.m_pitch << ", R:" << e.m_roll << ">";
		return o;
	}
*/
/*
		protected Mogre.Radian m_yaw = new Mogre.Radian(); // Rotation around the Y axis.
		protected Mogre.Radian m_pitch = new Mogre.Radian(); // Rotation around the X axis.
		protected Mogre.Radian m_roll = new Mogre.Radian(); // Rotation around the Z axis.
		protected Mogre.Quaternion m_cachedQuaternion = new Mogre.Quaternion(); // Cached quaternion equivalent of this euler object.
		protected bool m_changed; // Is the cached quaternion out of date?
*/























/// <summary>
/// Provides a method for performing a deep copy of an object.
/// Binary Serialization is used to perform the copy.
/// </summary>
// 	public static class ObjectCopier
// 	{
// 		/// <summary>
// 		/// Perform a deep Copy of the object.
// 		/// </summary>
// 		/// <typeparam name="T">The type of object being copied.</typeparam>
// 		/// <param name="source">The object instance to copy.</param>
// 		/// <returns>The copied object.</returns>
// 		public static T Clone<T>(this T source)
// 		{
// 			if (!typeof(T).IsSerializable)
// 			{
// 				throw new ArgumentException("The type must be serializable.", "source");
// 			}
// 
// 			// Don't serialize a null object, simply return the default for that object
// 			if (Object.ReferenceEquals(source, null))
// 			{
// 				return default(T);
// 			}
// 
// 			IFormatter formatter = new BinaryFormatter();
// 			Stream stream = new MemoryStream();
// 			using (stream)
// 			{
// 				formatter.Serialize(stream, source);
// 				stream.Seek(0, SeekOrigin.Begin);
// 				return (T)formatter.Deserialize(stream);
// 			}
// 		}
// 	}


/*
public class Serialization
{
	/// <summary>
	///  Binary Serializes an object
	/// </summary>
	/// <param name="obj">The object to serialize</param>
	/// <returns>byte array</returns>
	public static byte[] BinarySerialize(Object obj)
	{
		byte[] serializedObject;
		MemoryStream ms = new MemoryStream();
		BinaryFormatter b = new BinaryFormatter();
		b.Serialize(ms, obj);
		ms.Seek(0, 0);
		serializedObject = ms.ToArray();
		ms.Close();

		return serializedObject;
	}

	/// <summary>
	///  Binary DeSerializes an object
	/// </summary>
	/// <param name="obj">The object to serialize</param>
	/// <returns>The deserialized object</returns>
	public static T BinaryDeSerialize<T>(byte[] serializedObject)
	{
		MemoryStream ms = new MemoryStream();
		ms.Write(serializedObject, 0, serializedObject.Length);
		ms.Seek(0, 0);
		BinaryFormatter b = new BinaryFormatter();
		System.Object obj = b.Deserialize(ms);
		ms.Close();

		return (T)obj;
	}

	/// <summary>
	/// Serialize the object to a file
	/// </summary>
	/// <param name="obj">Object to be serialized.Ensure that is Serializable !</param>
	/// <param name="filePath">File( with the entire file path) where the object will be serialized to</param>
	/// <returns>True on successful serialization.</returns>
	public static bool FileSerialize(Object obj, string filePath)
	{
		FileStream fileStream = null;

		try
		{
			fileStream = new FileStream(filePath, FileMode.Create);
			BinaryFormatter b = new BinaryFormatter();
			b.Serialize(fileStream, obj);
		}
		catch
		{
			throw;
		}
		finally
		{
			if (fileStream != null)
				fileStream.Close();
		}

		return true;
	}

	/// <summary>
	/// Deserializes a binary formatted object.
	/// </summary>
	/// <param name="filePath">Full path of the file</param>
	/// <returns>The deserialized object</returns>
	public static T FileDeSerialize<T>(string filePath)
	{
		FileStream fileStream = null;
		System.Object obj;
		try
		{
			if (File.Exists(filePath) == false)
				throw new FileNotFoundException("The file was not found.", filePath);

			fileStream = new FileStream(filePath, FileMode.Open);
			BinaryFormatter b = new BinaryFormatter();
			obj = b.Deserialize(fileStream);
		}
		catch
		{
			throw;
		}
		finally
		{
			if (fileStream != null)
				fileStream.Close();
		}

		return (T)obj;
	}

	/// <summary>
	/// Serializes the passed object using SOAP serialization
	/// </summary>
	/// <param name="obj">The object to serialize</param>
	/// <param name="encodingType">The encoding type to use</param>
	/// <returns>A string representing the serialized object.</returns>
	/// <remarks>encodingType is abstract: pass in a subtype of Encoding, for example instantiate: 
	/// System.Text.UTF8Encoding</remarks>
	
				public static string SoapMemoryStreamSerialization(object obj, Encoding encodingType)
				{
				   string xmlResult;
	  
				   using (Stream stream = new MemoryStream())
				   {
					  try
					  {
						 SoapFormatter sf = new SoapFormatter();
						 sf.Serialize(stream, obj);
					  }
					  catch
					  {
						 throw;
					  }
	  
					  stream.Position = 0;
					  byte[] b = new byte[stream.Length];
					  stream.Read(b, 0, (int)stream.Length);
	  
					  xmlResult = encodingType.GetString(b, 0, b.Length);
				   }
	  
				   return xmlResult;
				}
	  
				/// <summary>
				/// Deserailizes a SOAP serialized object
				/// </summary>
				/// <param name="input">The XML string to deserialize.</param>
				/// <param name="encodingType">The encoding type to use</param>
				/// <returns>The deserialized object.</returns>
				/// <remarks>encodingType is abstract: pass in a subtype of Encoding, for example instantiate: 
				/// System.Text.UTF8Encoding</remarks>
				public T SoapDeserailization<T>(string input, System.Text.Encoding encodingType)
				{
				   Object obj = null;
	  
				   using (StringReader sr = new StringReader(input))
				   {
					  byte[] b;
					  b = encodingType.GetBytes(input);
	  
					  Stream stream = new MemoryStream(b);
	  
					  try
					  {
						 SoapFormatter sf = new SoapFormatter();
						 obj = (object)sf.Deserialize(stream);
					  }
					  catch
					  {
						 throw;
					  }
				   }
	  
				   return (T)obj;
				}

}
*/

static class ToolBox
{
	static ulong newID = 0ul;
	static public ulong UniqueID()
	{
		return newID = newID + 1ul;
	}

	static public string UniqueName()
	{
		return "_ID" + UniqueID(); // CHECK this doesn't blow out over time
	}

	static public Vector3 SnapToGrid(Vector3 hitPoint, Vector3 gridSize)
	{
		return new Vector3(Mathf.Round(hitPoint.x / gridSize.x) * gridSize.x, Mathf.Round((hitPoint.y + 0.1f) / gridSize.y) * gridSize.y, Mathf.Round(hitPoint.z / gridSize.z) * gridSize.z); // CHECK: Why the +0.1f?
	}

	//static public Vector3 SnapToRotationGrid(Quaternion unsnappedRotation)
//	{
//		Quaternion q = unsnappedRotation.eulerAngles
	//	return new Vector3(Mathf.RoundToInt(hitPoint.x), Mathf.RoundToInt(hitPoint.y + 0.1f), Mathf.RoundToInt(hitPoint.z));
//	}

	static public GameObject GetChildRecursively(string name, Transform startTransform)
	{
		Component[] transforms = startTransform.GetComponentsInChildren(typeof(Transform), true);

		foreach (Transform transform in transforms)
		{
//			Debug.Log("Searching transform = " + transform.gameObject.name);
			if (transform.gameObject.name == name)
			{
				return transform.gameObject;
			}
		}

		return null;
	}


	static public T GetComponentInParents<T>(Transform startingTransform) where T : Component
	{
		if(startingTransform != null)
		{
			T component = startingTransform.GetComponent<T>();
			if (component)
				return component;
		}
		else
		{
			Debug.LogWarning("WARNING: GetComponentInParents 'startingTransform' is null");
			return null;
		}

		return GetComponentInParents<T>(startingTransform.parent);
	}


	/*
	static public string FormatTime(float fTime)
	{
		TimeSpan t = TimeSpan.FromSeconds(fTime);

		// You can add more digits by adding more digits eg: {1:D2}:{2:D2}:{3:D2}:{4:D2} to also display milliseconds.
		return string.Format("{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
	}
*/

	// 		static public ManualObject CreateGrid(int numcols, int numrows, float unitsize)
	// 		{
	//             if (null == MeshManager.Singleton) // Bail if no renderer active  CHECK: Should I be doing this here? Because I've got quite a few direct calls to it like 'GetCom<Mesh>.SetMesh()'
	//             {
	//                 Debug.Log("Grid : WARNING: No renderer active");
	//                 return null;
	//             }
	// 
	// 			ManualObject grid = Renderer.Instance.SceneManager.CreateManualObject("grid");
	// 
	// 			grid.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_LINE_LIST);
	// 
	// 			float width = (float)numcols * unitsize;
	// 			float depth = (float)numrows * unitsize;
	// 			Vector3 center = new Vector3(-width / 2.0f, 0, -depth / 2.0f);
	// 
	// 			for (int i = 0; i < numrows; ++i)
	// 			{
	// 				Vector3 s, e;
	// 				s.x = 0.0f;
	// 				s.z = i * unitsize;
	// 				s.y = 0.0f;
	// 
	// 				e.x = width;
	// 				e.z = i * unitsize;
	// 				e.y = 0.0f;
	// 
	// 				grid.Position(s + center);
	// 				grid.Position(e + center);
	// 			}
	// 			grid.Position(new Vector3(0.0f, 0.0f, numrows * unitsize) + center);
	// 			grid.Position(new Vector3(width, 0.0f, numrows * unitsize) + center);
	// 
	// 			for (int i = 0; i < numcols; ++i)
	// 			{
	// 				Vector3 s, e;
	// 				s.x = i * unitsize;
	// 				s.z = depth;
	// 				s.y = 0.0f;
	// 
	// 				e.x = i * unitsize;
	// 				e.z = 0.0f;
	// 				e.y = 0.0f;
	// 
	// 				grid.Position(s + center);
	// 				grid.Position(e + center);
	// 			}
	// 			grid.Position(new Vector3(numcols * unitsize, 0.0f, 0.0f) + center);
	// 			grid.Position(new Vector3(numcols * unitsize, 0.0f, depth) + center);
	// 			grid.End();
	// 
	// 			return grid;
	// 			//			Renderer.Instance.SceneManager.RootSceneNode.AttachObject(grid);
	// 		}

	// 	static void LogGameObject(GameObject gameObject, bool children)
	// 	{
	// 		Component[] components = gameObject.GetComponents(typeof(Component));
	// 		FieldInfo[] fields;
	// 		PropertyInfo[] properties;
	// 
	// 		Debug.Log(gameObject.name + ":");
	// 
	// 		foreach (Component component in components)
	// 		{
	// 			Debug.Log(" - " + component.GetType().Name + ":");
	// 			fields = component.GetType().GetFields();
	// 			foreach (FieldInfo NewTestField in fields)
	// 			{
	// 				Debug.Log(" ." + NewTestField.Name + " = " + NewTestField.GetValue(component));
	// 			}
	// 
	// 			properties = component.GetType().GetProperties();
	// 			foreach (PropertyInfo property in properties)
	// 			{
	// 				Debug.Log(" ." + property.Name + " = " + property.GetGetMethod().Invoke(component, null));
	// 			}
	// 		}
	// 
	// 		if (children)
	// 		{
	// 			foreach (Transform transform in gameObject.transform)
	// 			{
	// 				Debug.Log("->");
	// 				LogGameObject(transform.gameObject, children);
	// 			}
	// 		}
	// 	}
	// 
	// 	static void LogGameObject(GameObject gameObject)
	// 	{
	// 		LogGameObject(gameObject, false);
	// 	}

}
