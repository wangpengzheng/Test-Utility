using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace TestUtility.Class
{
    class GetValueFromXml
    {
        private ArrayList arrayForTypes=new ArrayList();
        private ArrayList arrayForMethods = new ArrayList();
        private ArrayList arrayForInputValues = new ArrayList();
        private ArrayList arrayForExpectationValues = new ArrayList();


        public void ReadXml(XmlDocument xmlDoc)
        {
            XmlNodeList members = xmlDoc.SelectNodes(@"TestSolution/Member");

            foreach (XmlNode member in members)
            {
                XmlNode SortMethod = member.SelectSingleNode("SortMethod");
                XmlNode ValueType = member.SelectSingleNode("ValueType");
                XmlNode InputValue = member.SelectSingleNode("InputValue");
                XmlNode ExpectationOutput = member.SelectSingleNode("ExpectationOutput");

                arrayForTypes.Add(ValueType.InnerText);
                arrayForMethods.Add(SortMethod.InnerText);
                arrayForInputValues.Add(InputValue.InnerText);
                arrayForExpectationValues.Add(ExpectationOutput.InnerText);
            }
        }

        public ArrayList GetInputTypesFromXml()
        {
            return arrayForTypes;
        }

        public ArrayList GetSortMethodsFromXml()
        {
            return arrayForMethods;
        }

        public ArrayList GetInputValueFromXml()
        {
            return arrayForInputValues;
        }

        public ArrayList GetExpectationValueFromXml()
        {
            return arrayForExpectationValues;
        }
    }
}
