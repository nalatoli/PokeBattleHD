using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtension
{
    /// <summary> Fills array with 'value'. </summary>
    /// <typeparam name="T"> Array type </typeparam>
    /// <param name="value"> Value to fill array with. </param>
    public static void Populate<T>(this T[] arr, T value)
    {
        for (int i = 0; i < arr.Length; i++)
            arr[i] = value;

    }

    /// <summary> Fills matrix with 'value'. </summary>
    /// <typeparam name="T"> Maxtrix type </typeparam>
    /// <param name="value"> Value to fill matrix with. </param>
    public static void Populate<T>(this T[,] mat, T value)
    {
        for (int i = 0; i < mat.GetLength(0); i++)
            for(int j = 0; j < mat.GetLength(1); j ++)
                mat[i, j] = value;

    }

}