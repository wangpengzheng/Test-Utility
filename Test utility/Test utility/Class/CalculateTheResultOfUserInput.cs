using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TestUtility.Class
{
    class CalculateTheResultOfUserInput
    {
        public char[] CalculateCharInputType(Assembly assemblyOfInstance,string sortMethod,string nameSpaceOfInstance,char[] inputArray)
        {
            Type typeOfNameSpace = assemblyOfInstance.GetType(nameSpaceOfInstance);

            MethodInfo me = typeOfNameSpace.GetMethod(sortMethod);

            Type[] argTypes = { typeof(char) };

            MethodInfo ShowMethod = me.MakeGenericMethod(argTypes);

            object obj = assemblyOfInstance.CreateInstance(nameSpaceOfInstance);

            char[] intArrayForInput = (char[])ShowMethod.Invoke(obj, new object[] { inputArray });

            return intArrayForInput;
        }

        public int[] CalculateIntInputType(Assembly assemblyOfInstance, string sortMethod, string nameSpaceOfInstance, int[] inputArray)
        {

            Type typeOfNameSpace = assemblyOfInstance.GetType(nameSpaceOfInstance);

            MethodInfo me = typeOfNameSpace.GetMethod(sortMethod);

            Type[] argTypes = { typeof(int) };

            MethodInfo ShowMethod = me.MakeGenericMethod(argTypes);

            object obj = assemblyOfInstance.CreateInstance(nameSpaceOfInstance);

            int[] intArrayForInput = (int[])ShowMethod.Invoke(obj, new object[] { inputArray });

            return intArrayForInput;
        }

        public float[] CalculateFloatInputType(Assembly assemblyOfInstance, string sortMethod, string nameSpaceOfInstance, float[] inputArray)
        {            

            Type typeOfNameSpace = assemblyOfInstance.GetType(nameSpaceOfInstance);

            MethodInfo me = typeOfNameSpace.GetMethod(sortMethod);

            Type[] argTypes = { typeof(float) };

            MethodInfo ShowMethod = me.MakeGenericMethod(argTypes);

            object obj = assemblyOfInstance.CreateInstance(nameSpaceOfInstance);

            float[] intArrayForInput = (float[])ShowMethod.Invoke(obj, new object[] { inputArray });

            return intArrayForInput;
        }

        public double[] CalculateDoubleInputType(Assembly assemblyOfInstance, string sortMethod, string nameSpaceOfInstance, double[] inputArray)
        {
            Type typeOfNameSpace = assemblyOfInstance.GetType(nameSpaceOfInstance);

            MethodInfo me = typeOfNameSpace.GetMethod(sortMethod);

            Type[] argTypes = { typeof(double) };

            MethodInfo ShowMethod = me.MakeGenericMethod(argTypes);

            object obj = assemblyOfInstance.CreateInstance(nameSpaceOfInstance);

            double[] intArrayForInput = (double[])ShowMethod.Invoke(obj, new object[] { inputArray });

            return intArrayForInput;
        }
    }
}
