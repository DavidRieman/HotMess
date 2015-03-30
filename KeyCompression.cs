using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HotMess
{
    public static class KeyCompression
    {
        private static Key[] baseCompressionKeys = { Key.A, Key.H, Key.O, Key.V, Key.D1, Key.F1 };

        public static string Summary(IEnumerable<Key> keys)
        {
            var decompressedKeys = DecompressKeys(keys).Distinct();
            var sortedKeyTexts = from key in decompressedKeys
                                 orderby key
                                 select key.ToString();
            return string.Join(", ", sortedKeyTexts);
        }

        public static IEnumerable<Key> DecompressKeys(IEnumerable<Key> keys)
        {
            // The compression algorithm used is an optimization which takes up to 3 input keys from the device 
            // and 'compresses' them into a single keystroke, to work around the maximum keys held on keyboard.
            // For example, if we see the following, we can decompress the input into simulating these keys:
            // INPUT:  OUTPUT:  INDEXING CODE:
            //  A       A        compressionKey+0 or key
            //  B       B        compressionKey+1 or key
            //  C       C        compressionKey+2 or key
            //  D       A+B      compressionKey+3
            //  E       A+C      compressionKey+4
            //  F       B+C      compressionKey+5
            //  G       A+B+C    compressionKey+6
            // Keys which are not near a compression key will be ignored (IE F8-F12) and could be used safely 
            // by the program for other purposes (such as bringing up a configuration screen).
            // Note that there are two special cases: '8' stands in for 'z'+1 and '9' stands in for 'z'+2, so
            // for simplicity we'll turn these keys into Key.Z+1 and Key.Z+2 to leave the algorithm intact.
            // For testing the maximum number of keys simulated via actual keyboard instead of via arduino,
            // hold these keys: G, N, U, 9, 7, F7.
            foreach (var loopKey in keys)
            {
                var key = loopKey; // Assign to local so we can modify it for our edge cases.
                if (key == Key.D8)
                    key = Key.Z + 1;
                else if (key == Key.D9)
                    key = Key.Z + 2;

                foreach (var compressionKey in baseCompressionKeys)
                {
                    if (key == compressionKey + 0 || key == compressionKey + 1 || key == compressionKey + 2)
                    {
                        yield return key;
                    }
                    else if (key == compressionKey + 3)
                    {
                        yield return compressionKey + 0;
                        yield return compressionKey + 1;
                    }
                    else if (key == compressionKey + 4)
                    {
                        yield return compressionKey + 0;
                        yield return compressionKey + 2;
                    }
                    else if (key == compressionKey + 5)
                    {
                        yield return compressionKey + 1;
                        yield return compressionKey + 2;
                    }
                    else if (key == compressionKey + 6)
                    {
                        yield return compressionKey + 0;
                        yield return compressionKey + 1;
                        yield return compressionKey + 2;
                    }
                }
            }
        }
    }
}
