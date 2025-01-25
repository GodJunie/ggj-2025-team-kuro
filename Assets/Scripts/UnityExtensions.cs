using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UnityExtensions {
    public static Vector3 GetRandomVector3() {
        return new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)).normalized;
    }

    public static T Random<T>(this IEnumerable<T> list) {
        return list.ToList().Random();
    }

    public static T Random<T>(this T[] array) {
        int index = UnityEngine.Random.Range(0, array.Length);
        return array[index];
    }

    public static T Random<T>(this List<T> list) {
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }

    public static void Shuffle<T>(this IList<T> list) {
        var rng = new System.Random();

        int n = list.Count;
        while(n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
