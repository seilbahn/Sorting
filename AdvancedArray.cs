using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace Sorting
{
    /// <summary>
    /// The class AdvancedArrays&lt;T&gt; is designed  for:<br/>
    /// 1) creating arrays in the range from 0 to 2_146_435_071. It supports
    /// also assigment of max- and min- values in the array and how many
    /// same numbers it contains;<br/>
    /// 2) operations to double, reverse and shuffle array.<br/>
    /// It is a generic type class which implements IComparable interface.<br/>
    /// You should use sbyte, byte, short, ushort, int, uint,
    /// long, ulong, float, double, decimal, char types for correct working.
    /// </summary>
    /// <typeparam name="T">sbyte, byte, short, ushort, int, uint,
    /// long, ulong, float, double, decimal, char</typeparam>    
    [Serializable]
    public class AdvancedArray<T> : ICloneable, IChangable<T> where T : IComparable
    {
        /// <summary>
        /// 2_146_435_071 is the maximum size for arrays of any structure.<br/>
        /// Accorting to the site:<br/>
        /// https://learn.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/gcallowverylargeobjects-element.<br/>
        /// This value was chosen to avoid out of memory and any other exceptions although there is an opportunity to use
        /// bigger values.<br/>
        /// This class is created for the array capacity within System.Int32.<br/>
        /// If you need bigger arrays you should use BigArray&lt;T&gt;.
        /// </summary>
        public static readonly int MAXIMUM_INDEX = 2_146_435_071;

        /// <summary>
        /// The default array size.<br/>
        /// This value is used to create an instance of the class without parameters.<br/>
        /// It is equal: 10.
        /// </summary>
        private static readonly int DEFAULT_SIZE = 10;

        /// <summary>
        /// The type of the array.
        /// ArrayType.Random - the default random generated array (default).<br/>
        /// ArrayType.Sorted - the array will be sorted ascending.<br/>
        /// ArrayType.NearlySorted - the array will contain unsorted elements, but
        /// mostly it will be sorted ascending.<br/>
        /// ArrayType.Reversed - the array will be sorted descending.<br/>
        /// ArrayType.FewUnique - the array will contain a very limited set of values.
        /// </summary>
        public ArrayType ArrayType { get; set; }

        /// <summary>
        /// The reference to the array which is used inside the class.
        /// </summary>
        /// <value>The internal array.</value>
        public T[] TArray { get; set; }

        /// <summary>
        /// It initializes a new instance of the AdvancedArray class.<br/>
        /// The array size is default (10).<br/>
        /// The minimum and maximum values are equal to the min- and maximum values of generic type &lt;T&gt;.
        /// </summary>
        /// <returns>A new object of AdvancedArray class.</returns>
        public AdvancedArray() : this(DEFAULT_SIZE, MinValueOf(typeof(T)), MaxValueOf(typeof(T)), 0.0M, ArrayType.Random)
        {
        }

        /// <summary>
        /// It initializes a new instance of the AdvancedArray class.<br/>
        /// The minimum and maximum values are equal to the min- and maximum values of generic type &lt;T&gt;.
        /// </summary>
        /// <param name="SizeOfArray"> The size of generating array.
        /// It should be within the range [1 ; 2_146_435_071].<br/>
        /// DON'T use int.MaxValue or System.Int32.MaxValue.<br/>
        /// There is a readonly value MAXIMUM_INDEX = 2_146_435_071 for this case.</param>
        /// <returns>A new object of AdvancedArray class.</returns>
        public AdvancedArray(int SizeOfArray) : this(SizeOfArray, MinValueOf(typeof(T)), MaxValueOf(typeof(T)), 0.0M, ArrayType.Random)
        {
        }

        /// <summary>
        /// It initializes a new instance of the AdvancedArray class.<br/>
        /// </summary>
        /// <param name="SizeOfArray"> The size of generating array.
        /// It should be within the range [1 ; 2_146_435_071].<br/>
        /// DON'T use int.MaxValue or System.Int32.MaxValue.<br/>
        /// There is a readonly value MAXIMUM_INDEX = 2_146_435_071 for
        /// this case.</param>
        /// <param name="MinimumValue">Minimum value of the array elements.<br/>
        /// It should be within the range [type.MinValue ; type.MaxValue].</param>
        /// <param name="MaximumValue">Maximum value of the array elements.<br/>
        /// It should be within the range (Minimum Value ; type.MaxValue].</param>
        /// <param name="SimilarElementsExpectancy">This parameter is used for
        /// assigning the probability of occurrence for one value in the whole array.<br/>
        /// It should be within the range [0.1 ; 1.0].<br/>
        /// 0.1 means that 10 % of the array will be filled with the same value
        /// (How many elements will be the same).</param>
        public AdvancedArray(int SizeOfArray, T MinimumValue, T MaximumValue, decimal SimilarElementsExpectancy = default, ArrayType arrayType = ArrayType.Random)
        {
            TArray = new T[SizeOfArray];
            TArray = CreateArray(SizeOfArray, MinimumValue, MaximumValue, SimilarElementsExpectancy, arrayType);
            ArrayType = arrayType;
        }

        /// <summary>
        /// The indexer for the internal array.
        /// </summary>
        /// <value>It returns the value at the position of the internal array.</value>
        public T this[int index]
        {
            get
            {
                if ((index < 0) || (index >= TArray.Length))
                {
                    throw new IndexOutOfRangeException("Index out of range!");
                }
                return TArray[index];
            }
            set
            {
                if ((index < 0) || (index >= TArray.Length))
                {
                    throw new IndexOutOfRangeException("Index out of range!");
                }
                TArray[index] = value;
            }
        }

        /// <summary>
        /// The size of the array.
        /// </summary>
        /// <value>It returns the size value of the array.</value>
        public int Size
        {
            get
            {
                return TArray.Length;
            }
        }

        /// <summary>
        /// Is the array sorted ascending or not.
        /// </summary>
        public bool Sorted
        {
            get
            {
                return IsSorted(TArray);
            }
        }

        /// <summary>
        /// The method checks whether the input array is sorted.
        /// </summary>
        /// <param name="input">The array for checking.</param>
        /// <returns>True if the input array is sorted.<br/>
        /// False if the input array is not sorted.</returns>
        public static bool IsSorted(T[] input)
        {
            bool isSorted;
            int i = input.Length - 1;
            if (i <= 0)
            {
                isSorted = true;
                return isSorted;
            }
            if ((i & 1) > 0)
            {
                if (Compare(input[i], input[i - 1]) < 0)
                {
                    isSorted = false;
                    return isSorted;
                }
                i--;
            }
            for (T ai = input[i]; i > 0; i -= 2)
            {
                if (Compare(ai, ai = input[i - 1]) < 0 || Compare(ai, ai = input[i - 2]) < 0)
                {
                    isSorted = false;
                    return isSorted;
                }
            }
            isSorted = (Compare(input[0], input[1]) <= 0);
            return isSorted;
        }

        public object Clone()
        {
            using var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(AdvancedArray<T>));
            serializer.Serialize(stream, this);
            stream.Position = 0;
            object? obj = serializer.Deserialize(stream);
            if (obj != null)
            {
                return obj;
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        /// <summary>
        /// This method generates an array with assigned parameters.<br/>
        /// The type of the returned array is the same as generic type.
        /// </summary>
        /// <param name="SizeOfArray"> The size of generating array.<br/>
        /// It should be within the range [1 ; 2_146_435_071].<br/>
        /// DON'T use int.MaxValue or System.Int32.MaxValue.
        /// There is a readonly value MAXIMUM_INDEX = 2_146_435_071 for
        /// this case.</param>
        /// <param name="MinimumValue">Minimum value of the array elements.<br/>
        /// It should be within the range [type.MinValue ; type.MaxValue].</param>
        /// <param name="MaximumValue">Maximum value of the array elements.<br/>
        /// It should be within the range (Minimum Value ; type.MaxValue].</param>
        /// <param name="SimilarElementsExpectancy">This parameter is used for
        /// assigning the probability of occurrence for one value in the whole array.<br/>
        /// It should be within the range [0.1 ; 1.0].<br/>
        /// 0.1 means that 10 % of the array will be filled with the same value
        /// (How many elements will be the same).</param>
        /// <param name="arrayType">It defines which type the array should be.<br/>
        /// Without this parameter the array will be random type.<br/>
        /// ArrayType.Random - the default random generated array.<br/>
        /// ArrayType.Sorted - the array will be sorted ascending.<br/>
        /// ArrayType.NearlySorted - the array will contain unsorted elements, but
        /// mostly it will be sorted ascending.<br/>
        /// ArrayType.Reversed - the array will be sorted descending.<br/>
        /// ArrayType.FewUnique - the array will contain a very limited set of values.</param>
        /// <returns>The generated array with the type of generic.</returns>
        public static T[] CreateArray(int SizeOfArray, T MinimumValue, T MaximumValue, decimal SimilarElementsExpectancy = default, ArrayType arrayType = ArrayType.Random)
        {
            T[] output = new T[SizeOfArray];
            Random random = new Random();

            dynamic MinValue = MinimumValue;
            dynamic MaxValue = MaximumValue;

            if (!IsArgumentsValid(SizeOfArray, MinValue, MaxValue, SimilarElementsExpectancy))
            {
                throw new ArgumentException();
            }

            if (
                (typeof(T).Equals(typeof(System.SByte))) ||
                (typeof(T).Equals(typeof(System.Byte))) ||
                (typeof(T).Equals(typeof(System.Int16))) ||
                (typeof(T).Equals(typeof(System.UInt16))) ||
                (typeof(T).Equals(typeof(System.Int32))) ||
                (typeof(T).Equals(typeof(System.UInt32))) ||
                (typeof(T).Equals(typeof(System.Int64))) ||
                (typeof(T).Equals(typeof(System.UInt64)))
                )
            {
                for (int i = 0; i < SizeOfArray; i++)
                {
                    dynamic temp = random.NextInt64(Convert.ToInt64(MinValue), Convert.ToInt64(MaxValue + 1));
                    output[i] = (T)Convert.ChangeType(temp, typeof(T));
                }
            }
            else if (typeof(T).Equals(typeof(System.Single)))
            {
                for (int i = 0; i < SizeOfArray; i++)
                {
                    dynamic temp = 0;
                    float tempFloat = random.NextSingle() * (MaxValue - MinValue) + MinValue;
                    if (MinValue < 0)
                    {
                        tempFloat = (random.NextSingle() > 0.5) ? tempFloat * (-1) : tempFloat;
                    }
                    temp = tempFloat;
                    output[i] = (T)Convert.ChangeType(temp, typeof(T));
                }
            }
            else if (typeof(T).Equals(typeof(System.Double)))
            {
                for (int i = 0; i < SizeOfArray; i++)
                {
                    dynamic temp = 0;
                    double tempDouble = random.NextSingle() * (MaxValue - MinValue) + MinValue;
                    if (MinValue < 0)
                    {
                        tempDouble = (random.NextDouble() > 0.5) ? tempDouble * (-1) : tempDouble;
                    }
                    temp = tempDouble;
                    output[i] = (T)Convert.ChangeType(temp, typeof(T));
                }
            }
            else if (typeof(T).Equals(typeof(System.Decimal)))
            {
                for (int i = 0; i < SizeOfArray; i++)
                {
                    dynamic temp = random.NextDecimal((decimal)MinValue, (decimal)MaxValue);
                    output[i] = (T)Convert.ChangeType(temp, typeof(T));
                }
            }
            else if (typeof(T).Equals(typeof(System.Char)))
            {
                for (int i = 0; i < SizeOfArray; i++)
                {
                    dynamic temp = (char)random.Next(MinValue, MaxValue + 1);
                    output[i] = (T)Convert.ChangeType(temp, typeof(T));
                }
            }

            if (SimilarElementsExpectancy != default)
            {
                SimilarArrayElements(output, SimilarElementsExpectancy);
            }

            if (arrayType.Equals(ArrayType.Random))
            {
                return output;
            }

            if (arrayType.Equals(ArrayType.Sorted))
            {
                Array.Sort(output);
            }

            if (arrayType.Equals(ArrayType.NearlySorted))
            {
                Array.Sort(output);
                for (int i = 0; i < (int)(Math.Sqrt(SizeOfArray)); i++)
                {
                    int firstElement = random.Next(0, SizeOfArray / 2);
                    int secondElement = random.Next(SizeOfArray / 2 + 1, SizeOfArray);
                    if (!output[firstElement].Equals(output[secondElement]))
                    {
                        T temp = output[firstElement];
                        output[firstElement] = output[secondElement];
                        output[secondElement] = temp;
                    }
                }
            }

            if (arrayType.Equals(ArrayType.Reversed))
            {
                //output = output.OrderByDescending(c => c).ToArray();
                Array.Sort(output);
                Array.Reverse(output);
            }

            if (arrayType.Equals(ArrayType.FewUnique))
            {
                if (SizeOfArray <= 2)
                {
                    return output;
                }

                int counter = (int)Math.Ceiling(Math.Sqrt(SizeOfArray));
                List<T> values = new List<T>();

                for (int i = 0; i < SizeOfArray && values.Count < counter; i++)
                {
                    if (!values.Contains(output[i]))
                    {
                        values.Add(output[i]);
                    }
                }

                for (int i = 0; i < SizeOfArray; i++)
                {
                    output[i] = values.Aggregate((x, y) => Math.Abs(Convert.ToDecimal(Subtract(x, output[i]))) < Math.Abs(Convert.ToDecimal(Subtract(y, output[i]))) ? x : y);
                }
            }

            return output;
        }

        /// <summary>
        /// This method generates an array with assigned parameters.<br/>
        /// The type of the returned array is the same as generic type.
        /// </summary>
        /// <param name="MinimumValue">Minimum value of the array elements.<br/>
        /// It should be within the range [type.MinValue ; type.MaxValue].</param>
        /// <param name="MaximumValue">Maximum value of the array elements.<br/>
        /// It should be within the range (Minimum Value ; type.MaxValue].</param>
        /// <param name="SimilarElementsExpectancy">This parameter is used for
        /// assigning the probability of occurrence for one value in the whole array.<br/>
        /// It should be within the range [0.1 ; 1.0].<br/>
        /// 0.1 means that 10 % of the array will be filled with the same value
        /// (How many elements will be the same).</param>
        /// <param name="arrayType">It defines which type the array should be.<br/>
        /// Without this parameter the array will be random type.<br/>
        /// ArrayType.Random - the default random generated array.<br/>
        /// ArrayType.Sorted - the array will be sorted ascending.<br/>
        /// ArrayType.NearlySorted - the array will contain unsorted elements, but
        /// mostly it will be sorted ascending.<br/>
        /// ArrayType.Reversed - the array will be sorted descending.<br/>
        /// ArrayType.FewUnique - the array will contain a very limited set of values.</param>
        public void CreateArray(T MinimumValue, T MaximumValue, decimal SimilarElementsExpectancy = default, ArrayType arrayType = ArrayType.Random)
        {
            TArray = CreateArray(Size, MinimumValue, MaximumValue, SimilarElementsExpectancy, arrayType);
            ArrayType = arrayType;
        }

        /// <summary>
        /// This method returns a duplicated array.<br/>
        /// The input array will be twiced.<br/>
        /// If the duplicated length of the input array is more than 2_146_435_071,
        /// the method will return the same array.
        /// </summary>
        /// <param name="input">The array for duplicating.</param>
        /// <returns>The duplicated array.</returns>
        public static T[] DoubleArray(T[] input)
        {
            int newLength;
            T[] newArray;
            try
            {
                newLength = input.Length * 2;
                if (newLength > MAXIMUM_INDEX)
                {
                    throw new OverflowException();
                }
                newArray = new T[newLength];
            }
            catch
            {
                return input;
            }
            Array.Copy(input, 0, newArray, 0, input.Length);
            Array.Copy(input, 0, newArray, input.Length, input.Length);
            return newArray;
        }

        /// <summary>
        /// This method duplicates the internal array.<br/>
        /// The array will be twiced.<br/>
        /// If the duplicated length of the internal array is more than 2_146_435_071,
        /// the method will return the same array.
        /// </summary>
        public void DoubleArray()
        {
            TArray = DoubleArray(TArray);
        }

        /// <summary>
        /// The method reverses the input array.
        /// </summary>
        /// <param name="input">The array for reversing.</param>
        public static void ReverseArray(T[] input)
        {
            Array.Reverse(input);
        }

        /// <summary>
        /// The method reverses the internal array.
        /// </summary>
        public void ReverseArray()
        {
            ReverseArray(TArray);
        }

        /// <summary>
        /// This method shuffles the input array.
        /// </summary>
        /// <param name="input">The array for shuffling.</param>
        public static void ShuffleArray(T[] input)
        {
            Random random = new Random();
            random.Shuffle(input); // It works
            // input = input.OrderBy(x => random.Next()).ToArray(); // It doesn't work            
        }

        /// <summary>
        /// This method shuffles the internal array.
        /// </summary>
        public void ShuffleArray()
        {
            ShuffleArray(TArray);
        }

        /// <summary>
        /// This method similizes the input array.<br/>
        /// It makes the elements more similar by replacing them with the same element.<br/>
        /// The parameter SimilarElementsExpectancy is used for
        /// assigning the probability of occurrence for one value in the whole array.<br/>
        /// It should be within the range [0.1 ; 1.0].<br/>
        /// 0.1 means that 10 % of the array will be filled with the same value
        /// (How many elements will be the same).
        /// </summary>
        /// <param name="input">The array for similaring elements.</param>
        /// <param name="SimilarElementsExpectancy">This parameter is used for
        /// assigning the probability of occurrence for one value in the whole array.<br/>
        /// It should be within the range [0.1 ; 1.0].<br/>
        /// 0.1 means that 10 % of the array will be filled with the same value
        /// (How many elements will be the same).</param>
        public static void SimilarArrayElements(T[] input, decimal SimilarElementsExpectancy = 0.5M)
        {
            Random random = new Random();

            if (SimilarElementsExpectancy < 0.1M)
            {
                throw new ArgumentException($"The value of {nameof(SimilarElementsExpectancy)} is less than {0.1M}."
                    + $"It should be within the range [{0.1M}, {1.0M}]");
            }
            if (SimilarElementsExpectancy > 1.0M)
            {
                throw new ArgumentException($"The value of {nameof(SimilarElementsExpectancy)} is bigger than {1.0M}."
                    + $"It should be within the range [{0.1M}, {1.0M}]");
            }

            dynamic elementForCopy = input[random.Next(input.Length)];
            int howManyElementsToCopy = Convert.ToInt32(input.Length * SimilarElementsExpectancy);
            int stepToCopy = Convert.ToInt32(Math.Floor(1 / SimilarElementsExpectancy));
            int howManyElementIsThere = default;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].Equals(elementForCopy))
                {
                    howManyElementIsThere++;
                }
            }

            for (int i = 0, copyCounter = 0; i < input.Length; i += stepToCopy)
            {
                if (howManyElementIsThere == howManyElementsToCopy)
                {
                    break;
                }
                if (copyCounter == howManyElementsToCopy)
                {
                    break;
                }
                if (input[i].Equals(elementForCopy))
                {
                    copyCounter++;
                    continue;
                }
                else
                {
                    input[i] = (T)Convert.ChangeType(elementForCopy, typeof(T));
                    copyCounter++;
                }
            }
        }

        /// <summary>
        /// This method similizes the internal array.<br/>
        /// It makes the elements more similar by replacing them with the same element.<br/>
        /// The parameter SimilarElementsExpectancy is used for
        /// assigning the probability of occurrence for one value in the whole array.<br/>
        /// It should be within the range [0.1 ; 1.0].<br/>
        /// 0.1 means that 10 % of the array will be filled with the same value
        /// (How many elements will be the same).
        /// </summary>
        /// <param name="SimilarElementsExpectancy">This parameter is used for
        /// assigning the probability of occurrence for one value in the whole array.<br/>
        /// It should be within the range [0.1 ; 1.0].<br/>
        /// 0.1 means that 10 % of the array will be filled with the same value
        /// (How many elements will be the same).</param>
        public void SimilarArrayElements(decimal SimilarElementsExpectancy = 0.5M)
        {
            SimilarArrayElements(TArray, SimilarElementsExpectancy);
        }

        /// <summary>
        /// The method prints the input array to the console.
        /// </summary>
        /// <param name="input">The array for printing.</param>
        public static void PrintToConsole(T[] input)
        {
            Console.WriteLine(ToString(input));
        }

        /// <summary>
        /// The method prints the internal array to the console.
        /// </summary>
        public void PrintToConsole()
        {
            PrintToConsole(TArray);
        }

        /// <summary>
        /// The method returns a string that represents the input array.
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <returns>A string value of the input array.</returns>
        public static string ToString(T[] input)
        {
            StringBuilder array = new StringBuilder();
            foreach (T element in input)
            {
                array.Append($"{element.ToString()} ");
            }
            return array.ToString();
        }

        /// <summary>
        /// The method returns a string that represents the internal array.
        /// </summary>
        /// <returns>A string value of the internal array.</returns>
        public override string ToString()
        {
            return ToString(TArray);
        }

        /// <summary>
        /// The method creates a copy of the input array.
        /// </summary>
        /// <param name="input">The reference to the array for copying.</param>
        /// <returns>The reference to the new array.</returns>
        public static T[] Copy(T[] input)
        {
            T[] output = new T[input.Length];
            Array.Copy(input, output, input.Length);
            return output;
        }

        /// <summary>
        /// The method creates a copy of the internal array.
        /// </summary>
        /// <returns>The reference to the new array.</returns>
        public T[] Copy()
        {
            return Copy(TArray);
        }

        /// <summary>
        /// The method creates a copy of the input arrat
        /// and sorts the new array in ascending order.
        /// </summary>
        /// <param name="input">The reference to the array for soring.</param>
        /// <returns>The reference to the new created sorted array.</returns>
        public static T[] Sort(T[] input)
        {
            T[] output = Copy(input);
            Array.Sort(output);
            return output;
        }

        /// <summary>
        /// The method sorts the internal array in ascending order.
        /// </summary>
        public void Sort()
        {
            TArray = Sort(TArray);
        }

        /// <summary>
        /// The method throws an exception with description
        /// that the min- or maxvalues were too large or too
        /// small.
        /// </summary>
        /// <exception cref="OverflowException">An overflow exception.
        /// </exception>
        private static void ThrowOverflowException()
        {
            throw new OverflowException($"The boundary values were either too large or too small for {typeof(T)}."
                + $"The \'MinValue\' and \'MaxValue\' should be within the range: [{MinValueOf(typeof(T))} ; {MaxValueOf(typeof(T))}].");
        }

        /// <summary>
        /// The method checks array parameters:<br/>
        /// the size, the min- and maxvalues and the similarity ratio.<br/>
        /// If at least one of them is not valid,
        /// then ArgumentException will be thhrown.
        /// </summary>
        /// <param name="SizeOfArray"> The size of generating array.<br/>
        /// It should be within the range [1 ; 2_146_435_071].<br/>
        /// DON'T use int.MaxValue or System.Int32.MaxValue.
        /// There is a readonly value MAXIMUM_INDEX = 2_146_435_071 for
        /// this case.</param>
        /// <param name="MinValue">Minimum value of the array elements.<br/>
        /// It should be within the range [type.MinValue ; type.MaxValue].</param>
        /// <param name="MaxValue">Maximum value of the array elements.<br/>
        /// It should be within the range (Minimum Value ; type.MaxValue].</param>
        /// <param name="SimilarElementsExpectancy">This parameter is used for
        /// assigning the probability of occurrence for one value in the whole array.
        /// It should be within the range [0.1 ; 1.0].
        /// 0.1 means that 10 % of the array will be filled with the same value
        /// (How many elements will be the same).</param>
        /// <returns>True, if all parameters are valid.
        /// Otherwise throws an argument exception.</returns>
        /// <exception cref="ArgumentException"></exception>
        private static bool IsArgumentsValid(int SizeOfArray, T MinValue, T MaxValue, decimal SimilarElementsExpectancy)
        {
            bool isValid = false;
            if (SizeOfArray <= 0)
            {
                throw new ArgumentException($"The value of {nameof(SizeOfArray)} is less or equal 0. It should be more than 0."
                    + $"The value of {nameof(SizeOfArray)} can be within the range [0 ; {MAXIMUM_INDEX}].");
            }
            else if (SizeOfArray > MAXIMUM_INDEX)
            {
                throw new ArgumentException($"The value of {nameof(SizeOfArray)} is more than {MAXIMUM_INDEX}."
                    + $"The value of {nameof(SizeOfArray)} can be within the range [0 ; {MAXIMUM_INDEX}].");
            }
            else if (AdvancedArray<T>.Compare(MinValue, MaxValue) == 0)
            {
                throw new ArgumentException($"The values of {nameof(MinValue)} and {nameof(MaxValue)} are the same.");
            }
            else if (AdvancedArray<T>.Compare(MinValue, MaxValue) > 0)
            {
                throw new ArgumentException($"The value of {nameof(MinValue)} is bigger than the value of {nameof(MaxValue)}.");
            }
            else if (AdvancedArray<T>.Compare(MaxValue, MinValue) < 0)
            {
                throw new ArgumentException($"The value of {nameof(MaxValue)} is less than the value of {nameof(MinValue)}.");
            }
            else if ((AdvancedArray<T>.Compare(MinValue, MinValueOf(typeof(T))) < 0) || (AdvancedArray<T>.Compare(MaxValue, MaxValueOf(typeof(T))) > 0))
            {
                AdvancedArray<T>.ThrowOverflowException();
            }
            else
            {
                isValid = true;
            }
            return isValid;
        }

        /// <summary>
        /// The method is created to compare two generic variables.<br/>
        /// <c>
        /// Compare(x, y);<br/>
        /// if (x &gt; y) then it returns &gt; 0<br/>
        /// if (x == y) then it returns 0<br/>
        /// if (x &lt; y) then it returns &lt; 0
        /// </c>
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>
        /// <c>
        /// Compare(x, y);<br/>
        /// if (x &gt; y) then it returns &gt; 0<br/>
        /// if (x == y) then it returns 0<br/>
        /// if (x &lt; y) then it returns &lt; 0
        /// </c>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If any of parameters is null,
        /// the method throws an argument
        /// exception.</exception>
        private static int Compare(T? x, T? y)
        {
            if ((x == null) || (y == null))
            {
                throw new ArgumentException();
            }
            return x.CompareTo(y);
        }

        /// <summary>
        /// The method returns the minimum value
        /// of the type.
        /// </summary>
        /// <param name="type">
        /// The type can be one of these:
        /// System.SByte, System.Byte,<br/>
        /// System.Int16, System.UInt16,<br/>
        /// System.Int32, System.UInt32,<br/>
        /// System.Int64, System.UInt64,<br/>
        /// System.Single, System.Double,<br/>
        /// System.Decimal, System.Char.</param>
        /// <returns>The minimum value of the type.</returns>
        private static T MinValueOf(Type type)
        {
            if (type.Equals(typeof(System.SByte)))
            {
                return (T)Convert.ChangeType(sbyte.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Byte)))
            {
                return (T)Convert.ChangeType(byte.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Int16)))
            {
                return (T)Convert.ChangeType(short.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.UInt16)))
            {
                return (T)Convert.ChangeType(ushort.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Int32)))
            {
                return (T)Convert.ChangeType(int.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.UInt32)))
            {
                return (T)Convert.ChangeType(uint.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Int64)))
            {
                return (T)Convert.ChangeType(long.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.UInt64)))
            {
                return (T)Convert.ChangeType(ulong.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Single)))
            {
                return (T)Convert.ChangeType(float.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Double)))
            {
                return (T)Convert.ChangeType(double.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Decimal)))
            {
                return (T)Convert.ChangeType(decimal.MinValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Char)))
            {
                return (T)Convert.ChangeType(char.MinValue, typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType(0, typeof(T));
            }
        }

        /// <summary>
        /// The method returns the maximum value
        /// of the type.
        /// </summary>
        /// <param name="type">
        /// The type can be one of these:
        /// System.SByte, System.Byte,<br/>
        /// System.Int16, System.UInt16,<br/>
        /// System.Int32, System.UInt32,<br/>
        /// System.Int64, System.UInt64,<br/>
        /// System.Single, System.Double,<br/>
        /// System.Decimal, System.Char.</param>
        /// <returns>The maximum value of the type.</returns>
        private static T MaxValueOf(Type type)
        {
            if (type.Equals(typeof(System.SByte)))
            {
                return (T)Convert.ChangeType(sbyte.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Byte)))
            {
                return (T)Convert.ChangeType(byte.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Int16)))
            {
                return (T)Convert.ChangeType(short.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.UInt16)))
            {
                return (T)Convert.ChangeType(ushort.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Int32)))
            {
                return (T)Convert.ChangeType(int.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.UInt32)))
            {
                return (T)Convert.ChangeType(uint.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Int64)))
            {
                return (T)Convert.ChangeType(long.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.UInt64)))
            {
                return (T)Convert.ChangeType(ulong.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Single)))
            {
                return (T)Convert.ChangeType(float.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Double)))
            {
                return (T)Convert.ChangeType(double.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Decimal)))
            {
                return (T)Convert.ChangeType(decimal.MaxValue, typeof(T));
            }
            else if (type.Equals(typeof(System.Char)))
            {
                return (T)Convert.ChangeType(char.MaxValue, typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType(0, typeof(T));
            }
        }

        private static T Subtract(T a, T b)
        {
            dynamic first = a;
            dynamic second = b;
            dynamic third = first - second;
            return (T)Convert.ChangeType(third, typeof(T));
        }
    }
}