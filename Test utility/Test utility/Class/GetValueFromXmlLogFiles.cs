using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

namespace TestUtility.Class
{
    class GetValueFromXmlLogFiles
    {
        private ArrayList arrayForTypes = new ArrayList();
        private ArrayList arrayForMethods = new ArrayList();
        private ArrayList arrayForInputValues = new ArrayList();
        private ArrayList arrayForExpectationValues = new ArrayList();

        private ArrayList arrayForSuccessOrFail = new ArrayList();
        private ArrayList arrayForActually = new ArrayList();

        public void ReadXml(XmlDocument xmlDoc)
        {
            XmlNodeList members = xmlDoc.SelectNodes(@"TestSolution/Member");

            foreach (XmlNode m in members)
            {
                XmlNode sortMethod = m.SelectSingleNode("SortMethod");
                XmlNode valueType = m.SelectSingleNode("ValueType");
                XmlNode inputValue = m.SelectSingleNode("InputValue");
                XmlNode expectationOutput = m.SelectSingleNode("ExpectationOutput");

                XmlNode successOrFail = m.SelectSingleNode("SuccessOrFail");
                XmlNode ForActually = m.SelectSingleNode("ArrayForActually");
                
                arrayForTypes.Add(sortMethod.InnerText);
                arrayForMethods.Add(valueType.InnerText);
                arrayForInputValues.Add(inputValue.InnerText);
                arrayForExpectationValues.Add(expectationOutput.InnerText);

                arrayForSuccessOrFail.Add(successOrFail.InnerText);
                arrayForActually.Add(ForActually.InnerText);
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

        public ArrayList GetForSuccessOrFailValueFromXml()
        {
            return arrayForSuccessOrFail;
        }

        public ArrayList GetActuallyValueFromXml()
        {
            return arrayForActually;
        }
    }
}
