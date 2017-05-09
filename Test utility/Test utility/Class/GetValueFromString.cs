using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TestUtility.Class
{
    class GetValueFromString
    {
        public ArrayList GetArrayByString(string InputValue)
        {
            ArrayList arrayForReturn=new ArrayList(); 
            InputValue=InputValue.Remove(0, 1);
            InputValue = InputValue.Remove(InputValue.Length - 1, 1);

            char[] charSeparators = new char[] { ',' };

            string[] tempString;

            tempString = InputValue.Split(charSeparators);

            arrayForReturn = toArrayList(tempString);

            return arrayForReturn;
        }

        public string GetArrayListNumberToString(ArrayList arrForInput)
        {
            if (arrForInput.Count == 0)
                return "{}";

            string ValueForReturn="";

            Boolean isFistTimeToStringOne = false;
            foreach (object tempObj in arrForInput)
            {
                if (isFistTimeToStringOne)
                {
                    ValueForReturn = ValueForReturn + "," + tempObj.ToString();
                }
                else
                    ValueForReturn = "{" + tempObj.ToString();
                isFistTimeToStringOne = true;
            }
            ValueForReturn = ValueForReturn + "}";

            return ValueForReturn;
        }

        private ArrayList toArrayList(String[] productIDs)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < productIDs.Length; i++)
            {
                    list.Add(productIDs[i]);
            }
            return list;
        }
    }
}
