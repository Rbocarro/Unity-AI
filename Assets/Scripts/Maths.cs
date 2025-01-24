using System.Collections;
using System.Data.Common;
using UnityEngine;
//http://mrl.nyu.edu/~perlin/paper445.pdf 

public static class Maths
{
    static int[] permutations = 
     {
        151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,88,237,149,56,87,174,
        20,125,136,171,168,68,175,74,165,71,134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,
        230,220,105,92,41,55,46,245,40,244,102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,
        169,200,196,135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,5,202,38,
        147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,223,183,170,213,119,248,152,2,
        44,154,163,70,221,153,101,155,167,43,172,9,129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,
        104,218,246,97,228,251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,49,
        192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,138,236,205,93,222,114,67,29,
        24,72,243,141,128,195,78,66,215,61,156,180,151
    };// Hash lookup table used to generate random gradient vectors. can use own values but need to ensure they are truly randomly distributed and between 0-255


    public static float ImprovedPerlinNoise(float x, float y)    // Noise function that generates Perlin noise given two floating point values for the x and y coordinates.
    {   
        // Find the unit cube that the point is within by taking the floor of each coordinate,then bitwise ANDing(&) it with 255 (0xff).
        var X = Mathf.FloorToInt(x) & 0xff;
        var Y = Mathf.FloorToInt(y) & 0xff;
        
        // Find the relative coordinates of the point within the unit cube by subtracting the floor of the coordinates from the original values.
        x -= Mathf.Floor(x);
        y -= Mathf.Floor(y);

        // Compute the fade curves for the x and y coordinates.The fade function smooths out the values near the edges of the unit cube to avoid sharp transitions in the output.
        var u = Fade(x);
        var v = Fade(y);
        // Calculate indices for the four corners of the unit cube.The indices are based on the lookup table of random integers.
        var A = (permutations[X] + Y) & 0xff;
        var B = (permutations[X + 1] + Y) & 0xff;
        return Mathf.InverseLerp(-1, 1, Lerp(v, Lerp(u, Grad(permutations[A], x, y), Grad(permutations[B], x - 1, y)),
                                        Lerp(u, Grad(permutations[A + 1], x, y - 1), Grad(permutations[B + 1], x - 1, y - 1))));//remapped to 0 to 1
    }


    // Compute the fade curve for a givn value t.The fade function smooths out the values near the edges of the unit cube to avoid sharp transitions in the output??
    static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    // Perform a linear interpolation between values a and b with parameter t. based on the linear interpolation formula
    static float Lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }

    // Calculate a gradient vector based on a hash value and thedistance from a lattice point to a query point in the x and y dimensions.
    static float Grad(int hash, float x, float y)
    {
        return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
    }

}