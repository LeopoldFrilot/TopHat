
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public static class Help
{
    public static Tunables_General Tunables => GameWizard.Instance.generalTunables;public static float MinutesToSeconds(float minutes)
    {
        return minutes * 60f;
    }

    public static float SecondsToMinutes(float seconds)
    {
        return seconds / 60f;
    }

    public static float SecondsToHours(float seconds)
    {
        return SecondsToMinutes(seconds) / 60f;
    }

    public static double GetDifferenceBetweenTimesInSeconds(DateTime a, DateTime b)
    {
        TimeSpan result = a.Subtract(b);
        return result.TotalSeconds;
    }

    public static double GetDifferenceBetweenTimesInMinutes(DateTime a, DateTime b)
    {
        return SecondsToMinutes((float)GetDifferenceBetweenTimesInSeconds(a, b));
    }
    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static System.Random Local;

        public static System.Random ThisThreadsRandom
        {
            get { return Local ??= new System.Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)); }
        }
    }

    public static void ShuffleList<T>(this IList<T> list)  
    {  
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private static Camera _camera;

    public static Camera MainCamera
    {
        get
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            return _camera;
        }
    }

    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new();

    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait))
        {
            return wait;
        }

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    // Used to get world object to follow UI object position
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, MainCamera, out var result);
        return result;
    }

    public static Vector2 GetScreenpointOfWorldObject(Transform transform)
    {
        return RectTransformUtility.WorldToScreenPoint(MainCamera, transform.position);
    }

    public static Vector2 GetScreenCenter()
    {
        return new Vector2(Screen.width * .5f, Screen.height * .5f);
    }

    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static T RandomElementInArray<T>(List<T> array)
    {
        int index = -1;
        if (array == null || array.Count == 0)
        {
            return default;
        }
        
        index = Random.Range(0, array.Count);
        return array[index];
    }

    public static bool RollChance01(float chance)
    {
        return Random.value <= chance;
    }

    public static void EmptyDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            var paths = Directory.EnumerateFiles(directoryPath);
            foreach (var path in paths)
            {
                File.Delete(path);
            }
        }
    }

    public static void EmptyAndDeleteDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            EmptyDirectory(directoryPath);
            Directory.Delete(directoryPath);
        }
    }

    public static Color GetRandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public static List<T> ConsolidateArray<T>(List<T> array)
    {
        List<T> list = new();
        foreach (var element in array)
        {
            if (!list.Contains(element))
            {
                list.Add(element);
            }
        }
        
        return list;
    }
}