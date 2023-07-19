using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Util
{
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }

    public static Vector3 LinearBezierCurve(float t, Vector3 p0, Vector3 p1)
    {
        Vector3 v = p0;
        v += t * (p1 - p0);

        return v;
    }

    public static Vector3 QuadraticBezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // return = p1 + (1-t)2(p0-p1) + t2(p2-p1)
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;

        Vector3 v = p1;
        v += uu * (p0 - p1);
        v += tt * (p2 - p1);

        return v;
    }

    public static Vector3 CubicBezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // return = (1-t)3p0 + 3(1-t)2tp1 + 3(1-t)t2p2 + t3p3
        float u = 1 - t;
        float uu = u * u;
        float uuu = uu * u;
        float tt = t * t;
        float ttt = tt * t;

        Vector3 v = uuu * p0;
        v += 3 * uu * t * p1;
        v += 3 * u * tt * p2;
        v += ttt * p3;

        return v;
    }


    public static float GetAngle(Vector3 v1, Vector3 v2) => Mathf.Acos(Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;


    public static string ReplaceName(string name) => name.Replace("(Clone)", "");

    public static List<T> DictionaryToList<T1, T>(Dictionary<T1, T> pairs)
    {
        List<T> result = new List<T>();

        foreach (var pair in pairs)
        {
            result.Add(pair.Value);
        }

        return result;
    }

    public static byte[] StructToByte(object obj)
    {
        int size = Marshal.SizeOf(obj);

        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(obj, ptr, true);

        Marshal.Copy(ptr, arr, 0, size);

        Marshal.FreeHGlobal(ptr);

        return arr;
    }

    public static T ByteToStruct<T>(byte[] buffer) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));

        if (size > buffer.Length)
        {
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.Copy(buffer, 0, ptr, size);

        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));

        return obj;
    }

    public static T ByteToStruct<T>(byte[] buffer, int index) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T)) - index;

        if (size > buffer.Length)
        {
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.Copy(buffer, 0, ptr, size);

        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));

        return obj;
    }



    public static T GetNextIndex<T>(this T scr, bool ignoreLastIndex = false, int count = 1) where T : struct
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));
        }

        T[] arr = (T[])Enum.GetValues(scr.GetType());
        int j = (Array.IndexOf<T>(arr, scr) + count) % arr.Length;
        if (ignoreLastIndex)
        {
            if (j == arr.Length)
            {
                return arr[0];
            }
        }
        return arr[j];
    }

    //ORTHOGRAPHIC_CAMERA
    public static float GetScreenWorldHeight(Camera Cam)
    {
        if (Cam.orthographic == false) { return -1f; }
        return Cam.orthographicSize * 2f;
    }

    public static float GetScreenWorldWidth(Camera Cam)
    {
        var height = GetScreenWorldHeight(Cam);
        if (height < 0f) return height;
        return height * ((float)Screen.width / Screen.height);
    }

    //RANDOM_SUFFLE
    public static void RandomSuffle<T>(ref T[] deck, int count = 10000)
    {
        for (int i = 0; i < count; i++)
        {
            int dest = UnityEngine.Random.Range(0, deck.Length);
            int sour = UnityEngine.Random.Range(0, deck.Length);

            T temp = deck[dest];
            deck[dest] = deck[sour];
            deck[sour] = temp;
        }
    }
    public static void RandomSuffle<T>(ref List<T> deck, int count = 10000)
    {
        for (int i = 0; i < count; i++)
        {
            int dest = UnityEngine.Random.Range(0, deck.Count);
            int sour = UnityEngine.Random.Range(0, deck.Count);
            T temp = deck[dest];
            deck[dest] = deck[sour];
            deck[sour] = temp;
        }
    }

    public static void RandomSuffle(ref System.Array deck, int count = 10000)
    {
        for (int i = 0; i < count; i++)
        {
            int dest = UnityEngine.Random.Range(0, deck.Length);
            int sour = UnityEngine.Random.Range(0, deck.Length);

            object temp = deck.GetValue(dest);
            deck.SetValue(deck.GetValue(sour), dest);
            deck.SetValue(temp, sour);
        }
    }


    public static int PercentToInt(float percent, int maxCount, bool containMax = true)
    {
        if (containMax)
        {
            return (percent < 1f) ? Mathf.FloorToInt(percent * maxCount) + 1 : maxCount;
        }
        else
        {
            return (percent < 1f) ? Mathf.FloorToInt(percent * maxCount) : maxCount;
        }
    }

    //About Sercurity
    public static string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            throw new Exception("Expected nonce to have positive length");
        }

        const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
        var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
        var result = string.Empty;
        var remainingLength = length;

        var randomNumberHolder = new byte[1];
        while (remainingLength > 0)
        {
            var randomNumbers = new List<int>(16);
            for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
            {
                cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                randomNumbers.Add(randomNumberHolder[0]);
            }

            for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
            {
                if (remainingLength == 0)
                {
                    break;
                }

                var randomNumber = randomNumbers[randomNumberIndex];
                if (randomNumber < charset.Length)
                {
                    result += charset[randomNumber];
                    remainingLength--;
                }
            }
        }

        return result;
    }

    public static string GenerateSHA256NonceFromRawNonce(string rawNonce)
    {
        var sha = new SHA256Managed();
        var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
        var hash = sha.ComputeHash(utf8RawNonce);

        var result = string.Empty;
        for (var i = 0; i < hash.Length; i++)
        {
            result += hash[i].ToString("x2");
        }

        return result;
    }



    //ABOUT_COLOR
    public static Color StrToColor(string str)
    {
        str = str.ToLowerInvariant();
        if (str.Length == 6)
        {
            char[] arr = str.ToCharArray();
            char[] color_arr = new char[6];

            for (int i = 0; i < 6; i++)
            {
                if (arr[i] >= '0' && arr[i] <= '9')
                    color_arr[i] = (char)(arr[i] - '0');
                else if (arr[i] >= 'a' && arr[i] <= 'f')
                    color_arr[i] = (char)(10 + arr[i] - 'a');
                else
                    color_arr[i] = (char)0;
            }

            float red = (color_arr[0] * 16 + color_arr[1]) / 255.0f;
            float green = (color_arr[2] * 16 + color_arr[3]) / 255.0f;
            float blue = (color_arr[4] * 16 + color_arr[5]) / 255.0f;

            return new Color(red, green, blue);
        }
        else
        {
            return Color.clear;
        }
    }
    public static string ColorToStr(Color color)
    {
        string r = ((int)(color.r * 255)).ToString("X2");
        string g = ((int)(color.g * 255)).ToString("X2");
        string b = ((int)(color.b * 255)).ToString("X2");
        string a = ((int)(color.a * 255)).ToString("X2");


        string result = string.Format("{0}{1}{2}{3}", r, g, b, a);


        return result;
    }

    public static Color PingPongColor(Color color, Color toColor, float factor)
    {
        var t = Mathf.PingPong(factor, 1f);
        return Color.Lerp(color, toColor, t);
    }
}
