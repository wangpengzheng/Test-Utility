using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Collections;
using Microsoft.VisualBasic;
using System.Xml;
using TestUtility.Class;

namespace TestUtility
{
    public partial class TestUtility : Form
    {
        public TestUtility()
        {
            InitializeComponent();
        }

        #region  The location about dll files.
        string dllFileName;
        string dllFilePlace;
        string dllLogName;
        string dllLogPlace;
        string dllViewer;
        string dllViewerPlace;
        string dllNameSpace;
        #endregion



        #region  Define the values for input test files
        ArrayList arrayForInput = new ArrayList();
        ArrayList arrayForExpectation = new ArrayList();

        ArrayList arrayForGuid = new ArrayList();

        string currentInputType;
        string currentSortType;

        string currentstrInput = "";
        string currentstrExpectation = "";


        #endregion

        #region Define the globle values
        //For case Backup
        string currentCaseFileName;
        string currentCaseFileLocation;

        //For Log BackUp
        string currentLogFileName;
        string currentLogFileLocation;

        //For detail files show
        string currentDetailFilesName;
        string currentDetailFilesLocation;
        #endregion

        #region  Define temp globle items
        string tempSortMethod="";

        //format user input 
        ArrayList tempArrayForInput = new ArrayList();
        ArrayList tempArrayForExpectation = new ArrayList();
        #endregion


        Assembly AssemblyForShowDll;


        /// <summary>Main Form Load,To name a new case file name and initail UI display
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestUtilityLoad(object sender, EventArgs e)
        {
            try
            {
                DataGridViewShowResultWindow.Rows[0].Cells[0].Value = ImageList.Images[2];

                #region Add CombBox Values
                if (!Directory.Exists(Application.StartupPath + "\\LogFiles\\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\LogFiles\\");
                }

                foreach (string fileOfLog in Directory.GetFiles(Application.StartupPath + "\\LogFiles\\"))
                {
                    ComboBoxLogFile.Items.Add(fileOfLog);
                }

                if (!Directory.Exists(Application.StartupPath + "\\Detail analysis\\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\Detail analysis\\");
                }

                foreach (string fileOfDetail in Directory.GetFiles(Application.StartupPath + "\\Detail analysis\\"))
                {
                    ComboBoxDetailFiles.Items.Add(fileOfDetail);
                }
                #endregion

                #region Named Case File.
                string hostname = System.Net.Dns.GetHostName();

                int curCaseFileIndex = 0;

                currentCaseFileName = "XmlTestCaseBackUp.ComputerName " + hostname + " [" + DateAndTime.Today.ToString("yyyy-MM-dd") + "](" + curCaseFileIndex + ").xml";
                while (File.Exists(System.Environment.CurrentDirectory + "\\InputCase\\" + currentCaseFileName))
                {
                    curCaseFileIndex += 1;
                    currentCaseFileName = "XmlTestCaseBackUp.ComputerName " + hostname + " [" + DateAndTime.Today.ToString("yyyy-MM-dd") + "](" + curCaseFileIndex + ").xml";
                }
                //renew label to show the curCaseFileName
                ToolStripStatusLabelCurrentCaseInside.Text = currentCaseFileName;
                currentCaseFileLocation = System.Environment.CurrentDirectory + "\\" + currentCaseFileName;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>Dissolving effect
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestUtilityFormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int i = 100; i > 0; i--)
                {
                    this.Opacity = i / 100.0;
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>Show the structure in dll files while load the dll
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonChangeDll_Click(object sender, EventArgs e)
        {
            try
            {
                Type[] types;

                #region  //Show the dll file Location.
                OpenFileDialog dllFile = new OpenFileDialog();
                dllFile.Filter = "Dll Files(*.dll) |*.dll";
                dllFile.InitialDirectory = System.Environment.CurrentDirectory;
                dllFile.ShowDialog();
                if (dllFile.FileName == "")
                {
                    return;
                }
                dllFileName = Path.GetFileName(dllFile.FileName);
                LabelDllName.Text = dllFileName;
                dllFilePlace = dllFile.FileName;
                TextBoxDllInputPlace.Text = dllFile.FileName;
                #endregion

                #region Show the reflection in the treeview.

                AssemblyForShowDll = Assembly.LoadFile(dllFilePlace);
                types = AssemblyForShowDll.GetTypes();



                //Build Tree Node of NameSpace
                TreeNode nameSpace = new TreeNode();
                TreeNode rootNode = new TreeNode(dllFileName);
                TreeViewShowDllStructure.Nodes.Add(rootNode);

                foreach (Type type in types)
                {
                    //new a Method node
                    MethodInfo[] methods = type.GetMethods();
                    int numberOfMethodUnderNameSpace = 0;

                    if (type.ToString() != "MBS.Training.ISortAlgorithm")
                    {
                        nameSpace = new TreeNode(type.ToString());
                        rootNode.Nodes.Add(nameSpace);
                        dllNameSpace = type.ToString();

                        //Build Tree Node Array of Namespace
                        TreeNode[] Child = new TreeNode[methods.Length];
                        foreach (MethodInfo method in methods)
                        {
                            string methodName = method.ToString();
                            if (methodName.IndexOf("T") == 0)
                            {
                                Child[numberOfMethodUnderNameSpace] = new TreeNode(method.ToString());
                                nameSpace.Nodes.Add(Child[numberOfMethodUnderNameSpace]);
                                numberOfMethodUnderNameSpace++;
                            }
                        }
                    }
                }
                #endregion

                TreeViewShowDllStructure.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        /// <summary>Control the quick input
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxInputValuePreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                ButtonAddItems.Focus();
            }
        }

        /// <summary>Control the type for input 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxInputValueLeave(object sender, EventArgs e)
        {
            if (TextBoxInputValue.Text == "")
                return;

            string tmpForSelectedNumType = ComboBoxSelectInputNumType.Text.ToString();

            if (tmpForSelectedNumType == "")
            {
                MessageBox.Show("Please select one type first!");
                ComboBoxSelectInputNumType.Focus();
                return;
            }
            try
            {
                if (tmpForSelectedNumType == "Int")
                {
                    int.Parse(TextBoxInputValue.Text);
                }
                else if (tmpForSelectedNumType == "Char")
                {
                    char.Parse(TextBoxInputValue.Text);
                }
                else if (tmpForSelectedNumType == "Float")
                {
                    float.Parse(TextBoxInputValue.Text);
                }
                else if (tmpForSelectedNumType == "Double")
                {
                    double.Parse(TextBoxInputValue.Text);
                }
                else
                {
                    MessageBox.Show("Please select one effective type!", "Error");
                    ComboBoxSelectInputNumType.Focus();
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Please input the right type as selected!");
                TextBoxInputValue.Text = "";
                TextBoxInputValue.Focus();
            }
        }

        /// <summary>for Compare the Number of input and Expectation
        /// 
        /// </summary>
        int countInput = 0;
        int countExceptation = 0;

        /// <summary>Add new number for one case sort
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddItemsClick(object sender, EventArgs e)
        {
            try
            {
                if (TextBoxInputValue.Text == "")
                {
                    TextBoxInputValue.Focus();
                    return;
                }

                //once input one case the file type for input can't be changed!
                if (ComboBoxSelectInputNumType.Enabled == true)
                    ComboBoxSelectInputNumType.Enabled = false;

                if (ComboBoxSelectTypeOfInputOrOutput.Text == "For Test Input")
                {
                    if (ComboBoxSelectInputNumType.Text.ToString() == "Int")
                    {
                        tempArrayForInput.Add(int.Parse(TextBoxInputValue.Text.ToString()));
                    }
                    else if (ComboBoxSelectInputNumType.Text.ToString() == "Char")
                    {
                        tempArrayForInput.Add(char.Parse(TextBoxInputValue.Text.ToString()));
                    }
                    else if (ComboBoxSelectInputNumType.Text.ToString() == "Float")
                    {
                        tempArrayForInput.Add(float.Parse(TextBoxInputValue.Text.ToString()));
                    }
                    else
                    {
                        tempArrayForInput.Add(double.Parse(TextBoxInputValue.Text.ToString()));
                    }
                    arrayForInput.Add(tempArrayForInput[countInput]);

                    TextBoxShowInputValue.Text = reshowTheString();
                    LabelNoteNumChoose.Text = "The number of input Array:";
                    TextBoxNumberOfInsertValue.Text = arrayForInput.Count.ToString();

                    countInput += 1;
                }
                else if (ComboBoxSelectTypeOfInputOrOutput.Text == "For Expectation")
                {
                    if (ComboBoxSelectInputNumType.Text.ToString() == "Int")
                    {
                        tempArrayForExpectation.Add(int.Parse(TextBoxInputValue.Text.ToString()));
                    }
                    else if (ComboBoxSelectInputNumType.Text.ToString() == "Char")
                    {
                        tempArrayForExpectation.Add(char.Parse(TextBoxInputValue.Text.ToString()));
                    }
                    else if (ComboBoxSelectInputNumType.Text.ToString() == "Float")
                    {
                        tempArrayForExpectation.Add(float.Parse(TextBoxInputValue.Text.ToString()));
                    }
                    else
                    {
                        tempArrayForExpectation.Add(double.Parse(TextBoxInputValue.Text.ToString()));
                    }
                    arrayForExpectation.Add(tempArrayForExpectation[countExceptation]);

                    TextBoxShowInputValue.Text = reshowTheString();
                    LabelNoteNumChoose.Text = "The number of Expection Array:";
                    TextBoxNumberOfInsertValue.Text = arrayForExpectation.Count.ToString();

                    countExceptation += 1;
                }
                TextBoxInputValue.Text = "";
                TextBoxInputValue.Focus();

                #region AutoInput Fuction
                if (ComboBoxSelectTypeOfInputOrOutput.Text == "For Test Input")
                {
                    TextBoxInputValue.AutoCompleteCustomSource.Clear();
                    foreach (object value in arrayForExpectation)
                    {
                        TextBoxInputValue.AutoCompleteCustomSource.Add(value.ToString());
                    }
                }
                else
                {
                    TextBoxInputValue.AutoCompleteCustomSource.Clear();
                    foreach (object value in arrayForInput)
                    {
                        TextBoxInputValue.AutoCompleteCustomSource.Add(value.ToString());
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>Return for user's last input
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDelLastInputClick(object sender, EventArgs e)
        {
            try
            {
                if (ComboBoxSelectTypeOfInputOrOutput.Text == "For Test Input")
                {
                    if (0 == arrayForInput.Count)
                        return;
                    else
                    {
                        arrayForInput.RemoveAt(arrayForInput.Count - 1);
                        tempArrayForInput.RemoveAt(arrayForInput.Count - 1);
                        countInput -= 1;
                    }
                    TextBoxShowInputValue.Text = reshowTheString();
                    TextBoxNumberOfInsertValue.Text = arrayForInput.Count.ToString();
                }
                else if (ComboBoxSelectTypeOfInputOrOutput.Text == "For Expectation")
                {
                    if (0 == arrayForExpectation.Count)
                        return;
                    else
                    {
                        arrayForExpectation.RemoveAt(arrayForExpectation.Count - 1);
                        tempArrayForExpectation.RemoveAt(arrayForInput.Count - 1);
                        countExceptation -= 1;
                    }
                    TextBoxShowInputValue.Text = reshowTheString();
                    TextBoxNumberOfInsertValue.Text = arrayForExpectation.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>Show the valus users input & Exception
        /// 
        /// </summary>
        /// <returns></returns>
        string reshowTheString()
        {
            try
            {
                string TheFirstPartOfString = "Array for Input:\r\n ";
                string TheSecondPartOfString = "\r\nArray for Expectation:\r\n ";

                GetValueFromString ManageTheStringInput = new GetValueFromString();

                TheFirstPartOfString = TheFirstPartOfString + ManageTheStringInput.GetArrayListNumberToString(arrayForInput);

                TheSecondPartOfString = TheSecondPartOfString + ManageTheStringInput.GetArrayListNumberToString(arrayForExpectation);

                return TheFirstPartOfString + TheSecondPartOfString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return "";
            }
        }

        private void CombBoxSelectTypeOfInputOrOutputClick(object sender, EventArgs e)
        {
            try
            {
                if (ComboBoxSelectTypeOfInputOrOutput.Text == "For Test Input")
                {
                    ComboBoxSelectTypeOfInputOrOutput.Text = "For Expectation";
                    LabelNoteNumChoose.Text = "The number of Wishing Array:";
                    TextBoxNumberOfInsertValue.Text = arrayForExpectation.Count.ToString();
                }
                else if (ComboBoxSelectTypeOfInputOrOutput.Text == "For Expectation")
                {
                    ComboBoxSelectTypeOfInputOrOutput.Text = "For Test Input";
                    LabelNoteNumChoose.Text = "The number of Input Array:";
                    TextBoxNumberOfInsertValue.Text = arrayForInput.Count.ToString();
                }
                else
                {
                    ComboBoxSelectTypeOfInputOrOutput.Text = "For Test Input";
                    LabelNoteNumChoose.Text = "The number of Input Array:";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary> Add a new case 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddCaseClick(object sender, EventArgs e)
        {
            try
            {
                #region If input is invalid,Jump out!
                //If the Array is null ,Jump out!
                if (arrayForInput.Count == 0)
                {
                    MessageBox.Show("Please input the value for testing!", "No Elements Found!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TextBoxInputValue.Focus();
                    return;
                }

                //If TreeView is not selected,method have not choose.Jump out!
                if (TextBoxSortMethodSelect.Text == "")
                {
                    MessageBox.Show("Please choose your sort method in the treeview first.", "No Method Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //If length of array is different,jump out!
                if (countExceptation != countInput)
                {
                    MessageBox.Show("The Input and the Expectation Array length is not same!Please Check it again!",
                        "Different Array Length!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TextBoxInputValue.Focus();
                    return;
                }
                #endregion

                currentInputType = ComboBoxSelectInputNumType.Text;
                currentSortType = TextBoxSortMethodSelect.Text;
                inputValueToXml();

                //ResetAll input value
                ReSetInput();
                for (int i = 0; i < FucTheActualRowInput(); i++)
                {
                    DataGridViewShowInputWindow.Rows[i].Cells[0].Value = true;
                    hadSelectedAll = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        #region For Apply Fuctions

        //Build a new xml file.
        private void createXml(string StrFileName)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode RootNode = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            xmlDoc.AppendChild(RootNode);

            //XmlElement xmlElem = xmlDoc.CreateElement("", "ROOT", "");
            XmlElement xmlElem = xmlDoc.CreateElement("TestSolution");
            xmlDoc.AppendChild(xmlElem);

            currentCaseFileLocation = System.Environment.CurrentDirectory + "\\InputCase\\" + currentCaseFileName;

            if (!Directory.Exists(System.Environment.CurrentDirectory + "\\InputCase"))
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\InputCase");
            }
            xmlDoc.Save(currentCaseFileLocation);
        }

        //Write Xml with the items user input
        private void inputValueToXml()
        {
            if (!File.Exists(currentCaseFileLocation))
            {
                createXml(currentCaseFileName);
            }

            #region Change Array to string
            GetValueFromString ManageTheStringInput = new GetValueFromString();

            currentstrInput = ManageTheStringInput.GetArrayListNumberToString(arrayForInput);
            currentstrExpectation = ManageTheStringInput.GetArrayListNumberToString(arrayForExpectation);
            #endregion

            #region Write Xml
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(currentCaseFileLocation);

            XmlElement group = (XmlElement)xmlDoc.SelectSingleNode("TestSolution");

            XmlElement xmlElem = xmlDoc.CreateElement("Member");

            string guidTemp = Guid.NewGuid().ToString();

            xmlElem.SetAttribute("Guid", guidTemp);

            arrayForGuid.Add(guidTemp);

            XmlElement xmlElemSortMethod = xmlDoc.CreateElement("SortMethod");
            xmlElemSortMethod.InnerText = currentSortType;
            XmlElement xmlElemValueType = xmlDoc.CreateElement("ValueType");
            xmlElemValueType.InnerText = currentInputType;
            XmlElement xmlElemInputValue = xmlDoc.CreateElement("InputValue");
            xmlElemInputValue.InnerText = currentstrInput;
            XmlElement xmlElemExpectationOutput = xmlDoc.CreateElement("ExpectationOutput");
            xmlElemExpectationOutput.InnerText = currentstrExpectation; ;

            xmlElem.AppendChild(xmlElemSortMethod);
            xmlElem.AppendChild(xmlElemValueType);
            xmlElem.AppendChild(xmlElemInputValue);
            xmlElem.AppendChild(xmlElemExpectationOutput);
            group.AppendChild(xmlElem);

            XmlTextWriter xmlTr = new XmlTextWriter(currentCaseFileLocation, Encoding.UTF8);
            xmlTr.Formatting = Formatting.Indented;
            xmlDoc.WriteContentTo(xmlTr);
            xmlTr.Close();

            #endregion

            //DgvShowInputWdw.Rows.Add();
            ReadFromXmlToDataGridView(xmlDoc);
        }

        //Read xml from xml and show with DataGridView
        private void ReadFromXmlToDataGridView(XmlDocument xmlDoc)
        {
            DataGridViewShowInputWindow.Rows.Clear();
            DataGridViewShowInputWindow.Refresh();

            try
            {
                xmlDoc.Load(currentCaseFileLocation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("The xml you load is invalid.Please checkit again.", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GetValueFromXml getvalueFromXml = new GetValueFromXml();

            try
            {
                getvalueFromXml.ReadXml(xmlDoc);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read xml Error, please check you xml.", ex.Message);
            }

            ArrayList arrayForInputType = new ArrayList();
            arrayForInputType = getvalueFromXml.GetInputTypesFromXml();
            int numberOfCase = arrayForInputType.Count;

            for (int i = 0; i < numberOfCase; i++)
            {
                DataGridViewShowInputWindow.Rows.Add();
                DataGridViewShowInputWindow.Rows[i].Cells[1].Value = getvalueFromXml.GetSortMethodsFromXml()[i];
                DataGridViewShowInputWindow.Rows[i].Cells[2].Value = getvalueFromXml.GetInputTypesFromXml()[i];
                DataGridViewShowInputWindow.Rows[i].Cells[3].Value = getvalueFromXml.GetInputValueFromXml()[i];
                DataGridViewShowInputWindow.Rows[i].Cells[4].Value = getvalueFromXml.GetExpectationValueFromXml()[i];
            }
        }

        //Check the actual rows input;
        private int FucTheActualRowInput()
        {
            int theActualRowInput = DataGridViewShowInputWindow.Rows.Count;

            for (int i = 0; i < DataGridViewShowInputWindow.Rows.Count; i++)
            {
                try
                {
                    string s = DataGridViewShowInputWindow.Rows[i].Cells[2].Value.ToString();
                }
                catch
                {
                    theActualRowInput = i;
                    return theActualRowInput;
                }
            }
            return 0;
        }

        private void ReadFromDgvToXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(currentCaseFileLocation);
            XmlNode rootNode = xmlDoc.SelectSingleNode(@"TestSolution");

            rootNode.RemoveAll();

            int theActualRowInput = FucTheActualRowInput();

            #region Write Xml
            XmlElement group = (XmlElement)xmlDoc.SelectSingleNode("TestSolution");

            string tmpCurSortType;
            string tmpCurInputType;
            string tmpCurstrInput;
            string tmpCurstrExpectation;

            arrayForGuid.Clear();

            for (int i = 0; i < theActualRowInput; i++)
            {
                XmlElement xmlElem = xmlDoc.CreateElement("Member");

                string tempGuid = Guid.NewGuid().ToString();

                xmlElem.SetAttribute("Guid", tempGuid);

                arrayForGuid.Add(tempGuid);

                tmpCurSortType = DataGridViewShowInputWindow.Rows[i].Cells[1].Value.ToString();
                tmpCurInputType = DataGridViewShowInputWindow.Rows[i].Cells[2].Value.ToString();
                tmpCurstrInput = DataGridViewShowInputWindow.Rows[i].Cells[3].Value.ToString();
                tmpCurstrExpectation = DataGridViewShowInputWindow.Rows[i].Cells[4].Value.ToString();

                XmlElement xmlElemSortMethod = xmlDoc.CreateElement("SortMethod");
                xmlElemSortMethod.InnerText = tmpCurSortType;
                XmlElement xmlElemValueType = xmlDoc.CreateElement("ValueType");
                xmlElemValueType.InnerText = tmpCurInputType;
                XmlElement xmlElemInputValue = xmlDoc.CreateElement("InputValue");
                xmlElemInputValue.InnerText = tmpCurstrInput;
                XmlElement xmlElemExpectationOutput = xmlDoc.CreateElement("ExpectationOutput");
                xmlElemExpectationOutput.InnerText = tmpCurstrExpectation; ;

                xmlElem.AppendChild(xmlElemSortMethod);
                xmlElem.AppendChild(xmlElemValueType);
                xmlElem.AppendChild(xmlElemInputValue);
                xmlElem.AppendChild(xmlElemExpectationOutput);
                group.AppendChild(xmlElem);
            }

            XmlTextWriter xmlTr = new XmlTextWriter(currentCaseFileLocation, Encoding.UTF8);
            xmlTr.Formatting = Formatting.Indented;
            xmlDoc.WriteContentTo(xmlTr);
            xmlTr.Close();
            #endregion

            xmlDoc.Save(currentCaseFileLocation);
        }

        #endregion

        /// <summary>Create a new input case
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNewCase_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dR;
                dR = MessageBox.Show("Are you sure to add a new case?", "Confirm!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                if (DialogResult.Yes == dR)
                {
                    ReSetInput();
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        #region For Reset Fuction

        /// <summary>reset the console for input 
        /// 
        /// </summary>
        private void ReSetInput()
        {
            arrayForExpectation.Clear();
            arrayForInput.Clear();
            tempArrayForInput.Clear();
            tempArrayForExpectation.Clear();

            countExceptation = 0;
            countInput = 0;

            currentInputType = "";
            currentSortType = "";

            ComboBoxSelectInputNumType.Enabled = true;

            ComboBoxSelectInputNumType.Text = "Int";
            ComboBoxSelectTypeOfInputOrOutput.Text = "For Test Input";
            TextBoxSortMethodSelect.Text = tempSortMethod;
            TextBoxNumberOfInsertValue.Text = "0";
            TextBoxInputValue.Text = "";
            TextBoxShowInputValue.Text = "";
            LabelNoteNumChoose.Text = " The number you has inserted :";
            ToolStripStatusLabelRunningState.Text = "The progress of the program:";
            ToolStripProceessBar.Value = 0;

            ComboBoxSelectInputNumType.Focus();
        }

        /// <summary>Reset All UI
        /// 
        /// </summary>
        private void ReSetAll()
        {
            ReSetInput();
            arrayForGuid.Clear();

            #region Reset the dll file location and it's treeview
            DataGridViewShowInputWindow.Rows.Clear();
            DataGridViewShowInputWindow.Refresh();

            DataGridViewShowResultWindow.Rows.Clear();
            DataGridViewShowResultWindow.Refresh();
            DataGridViewShowResultWindow.Rows[0].Cells[0].Value = ImageList.Images[2];

            TreeViewShowDllStructure.Nodes.Clear();
            LabelDllName.Text = "No Dll Input.";
            TextBoxDllInputPlace.Text = "";
            ToolStripProceessBar.Value = 0;
            toolStripStatusLabelTheProbilityOftheCaseSuccess.Text = "No Running Case Now.";

            ToolStripStatusLabelRunningState.Text = "The progress of the program:";
            #endregion

            #region Reset the DataGridViews and logs

            DataGridViewShowInputWindow.Rows.Clear();
            ToolStripStatusLabelCurrentCaseInside.Text = "No Current Case Inside!";
            #endregion
        }

        #endregion

        private void ButtonRestAllClick(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure to reset all?", "Reset Confrim", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    ReSetAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ButtonViewResultClick(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 2;
            TextBoxShowLogF.Focus();
        }
        #region The Lable Fuction

        /// <summary>Open a xml for input 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripButtonClick(object sender, EventArgs e)
        {
            try
            {
                arrayForGuid.Clear();

                OpenFileDialog XmlFileCase = new OpenFileDialog();
                XmlFileCase.Filter = "Xml Files(*.xml)|*.xml";

                //if (!Directory.Exists(System.Environment.CurrentDirectory + @"\InputCase"))
                //{
                //    Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\InputCase");
                //}

                XmlFileCase.InitialDirectory = System.Environment.CurrentDirectory + @"\InputCase";

                XmlFileCase.ShowDialog();

                if (XmlFileCase.FileName == "")
                {
                    return;
                }

                currentCaseFileLocation = XmlFileCase.FileName;
                currentCaseFileName = Path.GetFileName(currentCaseFileLocation);
                ToolStripStatusLabelCurrentCaseInside.Text = currentCaseFileName;

                XmlDocument xmlDoc = new XmlDocument();
                ReadFromXmlToDataGridView(xmlDoc);

                foreach (XmlElement Element in xmlDoc.SelectNodes(@"TestSolution/Member"))
                {
                    arrayForGuid.Add(Element.GetAttribute("Guid"));
                }

                #region Reset the UI
                ToolStripStatusLabelRunningState.Text = "The progress of the program:";
                ToolStripProceessBar.Value = 0;
                toolStripStatusLabelTheProbilityOftheCaseSuccess.Text = "No Running Case Now.";

                for (int i = 0; i < FucTheActualRowInput(); i++)
                {
                    DataGridViewShowInputWindow.Rows[i].Cells[0].Value = true;
                    hadSelectedAll = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        Boolean hadSelectedAll = false;
        private void ToolSripButtonShowCase(object sender, EventArgs e)
        {
            try
            {
                int row = FucTheActualRowInput();
                if (!hadSelectedAll)
                {
                    for (int i = 0; i < row; i++)
                    {
                        DataGridViewShowInputWindow.Rows[i].Cells[0].Value = true;
                        hadSelectedAll = true;
                    }
                }
                else
                {
                    for (int i = 0; i < row; i++)
                    {
                        DataGridViewShowInputWindow.Rows[i].Cells[0].Value = false;
                        hadSelectedAll = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ToolSripButtonEditCaseInputClick(object sender, EventArgs e)
        {
            try
            {
                DataGridViewShowInputWindow.EndEdit();

                if (!File.Exists(currentCaseFileLocation))
                {
                    MessageBox.Show("There is no XML Files to read!", "File Not Found!");
                    return;
                }

                if (MessageBox.Show("Do you want to Accept Change in the File?", "Update Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                    return;

                try
                {
                    ReadFromDgvToXml();
                }
                catch
                {
                    MessageBox.Show("The case is invalid,please check it again.", "Wrong Input Case", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                //ShowProcessBar 
                ToolStripStatusLabelRunningState.Text = "The progress of Update:";
                ToolStripProceessBar.Value = 0;
                for (int i = 0; i < 100; i++)
                {
                    ToolStripProceessBar.Value = i;
                    System.Threading.Thread.Sleep(1);
                }
                ToolStripProceessBar.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ToolStripButtonDelSelectedCaseClick(object sender, EventArgs e)
        {
            try
            {
                DataGridViewShowInputWindow.EndEdit();
                XmlDocument xmlDoc = new XmlDocument();


                try
                {
                    xmlDoc.Load(currentCaseFileLocation);
                }
                catch
                {
                    MessageBox.Show("Create/Input a case first!", "No case found");
                    return;
                }

                //MessageBox.Show(DataGridViewShowInputWindow.Rows[0].Cells[0].FormattedValue.ToString());

                ArrayList selectedRemoveRows = new ArrayList();

                for (int i = 0; i < FucTheActualRowInput(); i++)
                {
                    if (DataGridViewShowInputWindow.Rows[i].Cells[0].FormattedValue.ToString() == "True")
                    {
                        selectedRemoveRows.Add(i);
                    }
                }

                foreach (int itemsForDelete in selectedRemoveRows)
                {
                    foreach (XmlElement Element in xmlDoc.SelectNodes(@"TestSolution/Member"))
                    {
                        if (Element.GetAttribute("Guid").ToString() == arrayForGuid[itemsForDelete].ToString())
                        {
                            Element.ParentNode.RemoveChild(Element);
                            xmlDoc.Save(currentCaseFileLocation);
                        }
                    }
                }
                xmlDoc.Save(currentCaseFileLocation);

                ReadFromXmlToDataGridView(xmlDoc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ToolStripButtonDllAllCaseClick(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you want to Delete all case in the file :" + currentCaseFileName + "?", "Delete Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                    return;

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(currentCaseFileLocation);
                XmlNode rootNode = xmlDoc.SelectSingleNode("TestSolution");

                rootNode.RemoveAll();

                xmlDoc.Save(currentCaseFileLocation);

                ReadFromXmlToDataGridView(xmlDoc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
        #endregion

        Boolean hadSelectedValue = false;

        private Boolean judgeSelected()
        {
            int rowOfShow = FucTheActualRowInput();
            for (int i = 0; i < rowOfShow; i++)
            {
                if (DataGridViewShowInputWindow.Rows[i].Cells[0].FormattedValue.ToString() == "True")
                {
                    return true;
                }
            }
            return false;
        }


        private void ButtonRunTestClick(object sender, EventArgs e)
        {
            try
            {
                if (currentCaseFileLocation == "" || DataGridViewShowInputWindow.Rows.Count == 0)
                    return;

                if (dllFileName == null)
                {
                    MessageBox.Show("Please input the dll first", "No Dll Found");
                    ButtonChangeDll.Focus();
                    return;
                }

                ArrayList arrayForActullyOutput = new ArrayList();

                ArrayList arrayForSort = new ArrayList();

                ArrayList arrayForSuccessOrFail = new ArrayList();

                ArrayList arrayForTypes = new ArrayList();
                ArrayList arrayForMethods = new ArrayList();
                ArrayList arrayForInput = new ArrayList();
                ArrayList arrayForExpectation = new ArrayList();

                if (judgeSelected())
                {

                    ArrayList selectedRemoveRows = new ArrayList();

                    for (int i = 0; i < FucTheActualRowInput(); i++)
                    {
                        if (DataGridViewShowInputWindow.Rows[i].Cells[0].FormattedValue.ToString() == "True")
                        {
                            selectedRemoveRows.Add(i);
                        }
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(currentCaseFileLocation);
                    foreach (int itemsForDelete in selectedRemoveRows)
                    {
                        foreach (XmlElement Element in xmlDoc.SelectNodes(@"TestSolution/Member"))
                        {
                            if (Element.GetAttribute("Guid").ToString() == arrayForGuid[itemsForDelete].ToString())
                            {
                                XmlNode SortMethod = Element.SelectSingleNode("SortMethod");
                                XmlNode ValueType = Element.SelectSingleNode("ValueType");
                                XmlNode InputValue = Element.SelectSingleNode("InputValue");
                                XmlNode ExpectationOutput = Element.SelectSingleNode("ExpectationOutput");

                                arrayForTypes.Add(ValueType.InnerText);
                                arrayForMethods.Add(SortMethod.InnerText);
                                arrayForInput.Add(InputValue.InnerText);
                                arrayForExpectation.Add(ExpectationOutput.InnerText);
                            }
                        }
                    }

                }
                else
                {
                    GetValueFromXml theValueFromXml = new GetValueFromXml();
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(currentCaseFileLocation);
                    try
                    {
                        theValueFromXml.ReadXml(xmldoc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Read xml Error, please check you xml.", ex.Message);
                    }
                    arrayForTypes = theValueFromXml.GetInputTypesFromXml();
                    arrayForMethods = theValueFromXml.GetSortMethodsFromXml();
                    arrayForInput = theValueFromXml.GetInputValueFromXml();
                    arrayForExpectation = theValueFromXml.GetExpectationValueFromXml();
                }

                #region Check and set the log file name
                if (Directory.Exists(currentLogFileLocation))
                {
                    Directory.CreateDirectory(currentLogFileLocation);
                }

                string hostname = System.Net.Dns.GetHostName();

                int curLogFileIndex = 0;

                currentLogFileName = "XmlTestLog.ComputerName " + hostname + " [" + DateAndTime.Today.ToString("yyyy-MM-dd") + "](" + curLogFileIndex + ").xml";
                while (File.Exists(System.Environment.CurrentDirectory + "\\LogFiles\\" + currentLogFileName))
                {
                    curLogFileIndex += 1;
                    currentLogFileName = "XmlTestLog.ComputerName " + hostname + " [" + DateAndTime.Today.ToString("yyyy-MM-dd") + "](" + curLogFileIndex + ").xml";
                }
                currentLogFileLocation = Application.StartupPath + "\\LogFiles\\" + currentLogFileName;

                XmlDocument xmldocForLog = new XmlDocument();
                XmlNode RootNode = xmldocForLog.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                xmldocForLog.AppendChild(RootNode);

                //XmlElement xmlElem = xmlDoc.CreateElement("", "ROOT", "");
                XmlElement xmlElem = xmldocForLog.CreateElement("TestSolution");
                xmldocForLog.AppendChild(xmlElem);

                currentLogFileLocation = Application.StartupPath + "\\LogFiles\\" + currentLogFileName;

                if (!Directory.Exists(Application.StartupPath + "\\LogFiles"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\LogFiles");
                }
                xmldocForLog.Save(currentLogFileLocation);
                #endregion

                #region Get the Value from xml case

                int[] IntValue;
                float[] floatValue;
                double[] doubleValue;
                char[] charValue;

                string stringAfterSort;

                GetValueFromString changeStringToValue = new GetValueFromString();

                int RowsOfCase = arrayForTypes.Count;

                Boolean[] caseSuccess = new Boolean[RowsOfCase];

                //Record success case
                int numberOfSuccess = 0;

                ToolStripProceessBar.Value = 0;

                for (int i = 0; i < RowsOfCase; i++)
                {
                    if (arrayForTypes[i].ToString() == "Int")
                    {
                        int numBeSortCount;
                        arrayForSort = changeStringToValue.GetArrayByString(arrayForInput[i].ToString());
                        numBeSortCount = arrayForSort.Count;
                        IntValue = new int[numBeSortCount];

                        int changeInt = 0;
                        foreach (string num in arrayForSort)
                        {
                            IntValue[changeInt++] = int.Parse(num);
                        }

                        CalculateTheResultOfUserInput calc = new CalculateTheResultOfUserInput();
                        int[] IntSortResult = new int[numBeSortCount];

                        IntSortResult = calc.CalculateIntInputType(AssemblyForShowDll, arrayForMethods[i].ToString(), dllNameSpace, IntValue);

                        ArrayList tempArray = new ArrayList();
                        for (int j = 0; j < numBeSortCount; j++)
                        {
                            tempArray.Add(IntSortResult[j]);
                        }

                        stringAfterSort = changeStringToValue.GetArrayListNumberToString(tempArray);
                        arrayForActullyOutput.Add(stringAfterSort);

                        if (stringAfterSort != arrayForExpectation[i].ToString())
                        {
                            caseSuccess[i] = false;
                            arrayForSuccessOrFail.Add("False");
                        }
                        else
                        {
                            caseSuccess[i] = true;
                            arrayForSuccessOrFail.Add("True");
                            numberOfSuccess += 1;
                            ToolStripProceessBar.Value = 100 * numberOfSuccess / RowsOfCase;
                        }
                    }
                    else if (arrayForTypes[i].ToString() == "Char")
                    {
                        int numCount;
                        arrayForSort = changeStringToValue.GetArrayByString(arrayForInput[i].ToString());
                        numCount = arrayForSort.Count;
                        charValue = new char[numCount];

                        //change into actually arrays
                        int changeInt = 0;
                        foreach (string num in arrayForSort)
                        {
                            charValue[changeInt++] = char.Parse(num);
                        }

                        CalculateTheResultOfUserInput calc = new CalculateTheResultOfUserInput();
                        char[] charSortResult = new char[numCount];

                        charSortResult = calc.CalculateCharInputType(AssemblyForShowDll, arrayForMethods[i].ToString(), dllNameSpace, charValue);

                        ArrayList tempArray = new ArrayList();
                        for (int j = 0; j < numCount; j++)
                        {
                            tempArray.Add(charSortResult[j]);
                        }

                        stringAfterSort = changeStringToValue.GetArrayListNumberToString(tempArray);
                        arrayForActullyOutput.Add(stringAfterSort);

                        if (stringAfterSort != arrayForExpectation[i].ToString())
                        {
                            caseSuccess[i] = false;
                            arrayForSuccessOrFail.Add("False");
                        }
                        else
                        {
                            caseSuccess[i] = true;
                            arrayForSuccessOrFail.Add("True");
                            numberOfSuccess += 1;
                            ToolStripProceessBar.Value = 100 * numberOfSuccess / RowsOfCase;
                        }
                    }
                    else if (arrayForTypes[i].ToString() == "Float")
                    {
                        int numCount;
                        arrayForSort = changeStringToValue.GetArrayByString(arrayForInput[i].ToString());
                        numCount = arrayForSort.Count;
                        floatValue = new float[numCount];

                        //change into actually arrays
                        int changeInt = 0;
                        foreach (string num in arrayForSort)
                        {
                            floatValue[changeInt++] = float.Parse(num);
                        }

                        CalculateTheResultOfUserInput calc = new CalculateTheResultOfUserInput();
                        float[] floatSortResult = new float[numCount];

                        floatSortResult = calc.CalculateFloatInputType(AssemblyForShowDll, arrayForMethods[i].ToString(), dllNameSpace, floatValue);

                        ArrayList tempArray = new ArrayList();
                        for (int j = 0; j < numCount; j++)
                        {
                            tempArray.Add(floatSortResult[j]);
                        }

                        stringAfterSort = changeStringToValue.GetArrayListNumberToString(tempArray);
                        arrayForActullyOutput.Add(stringAfterSort);

                        if (stringAfterSort != arrayForExpectation[i].ToString())
                        {
                            caseSuccess[i] = false;
                            arrayForSuccessOrFail.Add("False");
                        }
                        else
                        {
                            caseSuccess[i] = true;
                            arrayForSuccessOrFail.Add("True");
                            numberOfSuccess += 1;
                            ToolStripProceessBar.Value = 100 * numberOfSuccess / RowsOfCase;
                        }
                    }
                    else if (arrayForTypes[i].ToString() == "Double")
                    {
                        int numCount;
                        arrayForSort = changeStringToValue.GetArrayByString(arrayForInput[i].ToString());
                        numCount = arrayForSort.Count;
                        doubleValue = new double[numCount];

                        //change into actually arrays
                        int changeInt = 0;
                        foreach (string num in arrayForSort)
                        {
                            doubleValue[changeInt++] = double.Parse(num);
                        }

                        CalculateTheResultOfUserInput calc = new CalculateTheResultOfUserInput();
                        double[] doubleSortResult = new double[numCount];

                        doubleSortResult = calc.CalculateDoubleInputType(AssemblyForShowDll, arrayForMethods[i].ToString(), dllNameSpace, doubleValue);

                        ArrayList tempArray = new ArrayList();
                        for (int j = 0; j < numCount; j++)
                        {
                            tempArray.Add(doubleSortResult[j]);
                        }

                        stringAfterSort = changeStringToValue.GetArrayListNumberToString(tempArray);
                        arrayForActullyOutput.Add(stringAfterSort);

                        if (stringAfterSort != arrayForExpectation[i].ToString())
                        {
                            caseSuccess[i] = false;
                            arrayForSuccessOrFail.Add("False");
                        }
                        else
                        {
                            caseSuccess[i] = true;
                            arrayForSuccessOrFail.Add("True");
                            numberOfSuccess += 1;
                            ToolStripProceessBar.Value = 100 * numberOfSuccess / RowsOfCase;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The File Name of input value is invalid,please check Type of input value", "Wrong File Input Type");
                        DataGridViewShowInputWindow.Rows[i].Selected = true;
                        return;
                    }
                }
                #endregion

                int numberOfFailed = RowsOfCase - numberOfSuccess;

                double ratOfSuccess;
                ratOfSuccess = Math.Round(100.0 * numberOfSuccess / RowsOfCase, 2);

                toolStripStatusLabelTheProbilityOftheCaseSuccess.Text = numberOfSuccess + " Success //"
                    + numberOfFailed + " Failed.  Totally Case:" + RowsOfCase + "  Success rate: " + ratOfSuccess.ToString() + "%";

                #region Write Xml
                for (int temp = 0; temp < arrayForActullyOutput.Count; temp++)
                {
                    xmldocForLog.Load(currentLogFileLocation);

                    XmlElement group = (XmlElement)xmldocForLog.SelectSingleNode("TestSolution");

                    XmlElement xmlElemInsert = xmldocForLog.CreateElement("Member");

                    string guidTemp = Guid.NewGuid().ToString();

                    xmlElemInsert.SetAttribute("Guid", guidTemp);

                    //arrForGuid.Add(guidTemp);

                    XmlElement xmlElemSortMethod = xmldocForLog.CreateElement("SortMethod");
                    xmlElemSortMethod.InnerText = arrayForMethods[temp].ToString();
                    XmlElement xmlElemValueType = xmldocForLog.CreateElement("ValueType");
                    xmlElemValueType.InnerText = arrayForTypes[temp].ToString();
                    XmlElement xmlElemInputValue = xmldocForLog.CreateElement("InputValue");
                    xmlElemInputValue.InnerText = arrayForInput[temp].ToString();
                    XmlElement xmlElemExpectationOutput = xmldocForLog.CreateElement("ExpectationOutput");
                    xmlElemExpectationOutput.InnerText = arrayForExpectation[temp].ToString();

                    XmlElement xmlElemSuccessOrFail = xmldocForLog.CreateElement("SuccessOrFail");

                    xmlElemSuccessOrFail.InnerText = arrayForSuccessOrFail[temp].ToString();
                    XmlElement xmlElemActuallyOutput = xmldocForLog.CreateElement("ActuallyOutput");
                    xmlElemActuallyOutput.InnerText = arrayForActullyOutput[temp].ToString();

                    xmlElemInsert.AppendChild(xmlElemSuccessOrFail);
                    xmlElemInsert.AppendChild(xmlElemSortMethod);
                    xmlElemInsert.AppendChild(xmlElemValueType);
                    xmlElemInsert.AppendChild(xmlElemInputValue);
                    xmlElemInsert.AppendChild(xmlElemExpectationOutput);
                    xmlElemInsert.AppendChild(xmlElemActuallyOutput);

                    group.AppendChild(xmlElemInsert);

                    XmlTextWriter xmlTr = new XmlTextWriter(currentLogFileLocation, Encoding.UTF8);
                    xmlTr.Formatting = Formatting.Indented;
                    xmldocForLog.WriteContentTo(xmlTr);
                    xmlTr.Close();
                }
                #endregion

                #region Add the Elements in the DataGridView
                DataGridViewShowResultWindow.Rows.Clear();

                DataGridViewShowResultWindow.Rows[0].Cells[0].Value = ImageList.Images[2];

                for (int tempShow = 0; tempShow < arrayForActullyOutput.Count; tempShow++)
                {
                    DataGridViewShowResultWindow.Rows.Add();
                    //DataGridViewShowResultWdw.Rows[tempShow].Cells[1].Dispose  = ImageList[0];
                    //(DataGridViewImageCell)DataGridViewShowResultWdw.Rows[tempShow].Cells[1].Value = GetImage(ImageList[0]);
                    DataGridViewShowResultWindow.Rows[tempShow].Cells[1].Value = arrayForSuccessOrFail[tempShow].ToString();

                    if (arrayForSuccessOrFail[tempShow].ToString() == "False")
                    {
                        DataGridViewShowResultWindow.Rows[tempShow].DefaultCellStyle.ForeColor = Color.Red;
                        DataGridViewShowResultWindow.Rows[tempShow].Cells[0].Value = ImageList.Images[1];
                    }
                    else
                    {
                        DataGridViewShowResultWindow.Rows[tempShow].Cells[0].Value = ImageList.Images[0];
                    }

                    DataGridViewShowResultWindow.Rows[tempShow].Cells[2].Value = arrayForMethods[tempShow].ToString();
                    DataGridViewShowResultWindow.Rows[tempShow].Cells[3].Value = arrayForTypes[tempShow].ToString();
                    DataGridViewShowResultWindow.Rows[tempShow].Cells[4].Value = arrayForInput[tempShow].ToString();
                    DataGridViewShowResultWindow.Rows[tempShow].Cells[5].Value = arrayForExpectation[tempShow].ToString();
                    DataGridViewShowResultWindow.Rows[tempShow].Cells[6].Value = arrayForActullyOutput[tempShow].ToString();
                }
                #endregion

                #region Add the xml files to the LogViewer
                if (!File.Exists(currentLogFileLocation))
                {
                    return;
                }

                StreamReader readTheXmlLogFile = new StreamReader(currentLogFileLocation);
                TextBoxShowLogF.Text = readTheXmlLogFile.ReadToEnd();
                LabelLogFileName.Text = Path.GetFileName(currentLogFileLocation);
                TextBoxLogFilePlace.Text = currentLogFileLocation;
                readTheXmlLogFile.Close();
                #endregion

                #region Build the detail file about running case
                //Build string 
                string showRunningResultWithTextBox = "Running results :\r\n           " + "Successed Case Number: " + numberOfSuccess + "\r\n           "
                     + " Failed Case Number: " + +numberOfFailed + "\r\n           Total Case:  " + RowsOfCase + "\r\n           Success Rate: " + ratOfSuccess.ToString() + "%\r\n" + " \r\n \r\n ";
                currentDetailFilesName = Path.GetFileNameWithoutExtension(currentLogFileLocation);
                currentDetailFilesLocation = Application.StartupPath + "\\Detail analysis\\" + currentDetailFilesName + ".txt";

                string fileForFailedCase = "           /******************************The Failed Case******************************/\r\n";
                string fileForSuccessedCase = "           \\******************************The Successed Case******************************\\\r\n";
                int numberOfFailedCase = 0;
                int numberOfSuccessedCase = 0;

                for (int chooseFailOrSuccess = 0; chooseFailOrSuccess < RowsOfCase; chooseFailOrSuccess++)
                {
                    if (caseSuccess[chooseFailOrSuccess])
                    {
                        numberOfSuccessedCase += 1;
                        fileForSuccessedCase = fileForSuccessedCase + "Successed File " + numberOfSuccessedCase.ToString() + ":\r\n           ";
                        fileForSuccessedCase = fileForSuccessedCase + "Sort Method: " + arrayForMethods[chooseFailOrSuccess].ToString() + "\r\n           ";
                        fileForSuccessedCase = fileForSuccessedCase + "Value Type: " + arrayForTypes[chooseFailOrSuccess].ToString() + "\r\n           ";
                        fileForSuccessedCase = fileForSuccessedCase + "\r\n           Input Values:             " + arrayForInput[chooseFailOrSuccess].ToString() + "\r\n           ";
                        fileForSuccessedCase = fileForSuccessedCase + "Expectation Output: " + arrayForExpectation[chooseFailOrSuccess].ToString() + "\r\n           ";
                        fileForSuccessedCase = fileForSuccessedCase + "Actually Output:        " + arrayForActullyOutput[chooseFailOrSuccess].ToString() + "\r\n\r\n";
                    }
                    else
                    {
                        numberOfFailedCase += 1;
                        fileForFailedCase = fileForFailedCase + "Failed File " + numberOfFailedCase.ToString() + ":\r\n           ";
                        fileForFailedCase = fileForFailedCase + "Sort Method: " + arrayForMethods[chooseFailOrSuccess].ToString() + "\r\n           ";
                        fileForFailedCase = fileForFailedCase + "Value Type: " + arrayForTypes[chooseFailOrSuccess].ToString() + "\r\n           ";
                        fileForFailedCase = fileForFailedCase + "\r\n           Input Values:             " + arrayForInput[chooseFailOrSuccess].ToString() + "\r\n           ";
                        fileForFailedCase = fileForFailedCase + "Expectation Output: " + arrayForExpectation[chooseFailOrSuccess].ToString() + "\r\n           ";
                        fileForFailedCase = fileForFailedCase + "Actually Output:        " + arrayForActullyOutput[chooseFailOrSuccess].ToString() + "\r\n\r\n";
                    }
                }
                //if (numberOfFailed == 0)
                fileForFailedCase = fileForFailedCase + "           /******************************End of the Failed Case ******************************/\r\n\r\n";
                //else
                //    fileForFailedCase = fileForFailedCase + "/******************************End of the Failed Case ******************************/\r\n\r\n";

                //if(numberOfSuccess==0)
                fileForSuccessedCase = fileForSuccessedCase + "           \\******************************End of the Successed Case******************************\\\r\n";
                //else
                //    fileForSuccessedCase = fileForSuccessedCase + "\\******************************End of the Successed Case******************************\\\r\n";

                showRunningResultWithTextBox = showRunningResultWithTextBox + fileForFailedCase + fileForSuccessedCase;

                //check file and save detail
                if (!Directory.Exists(Application.StartupPath + "\\Detail analysis\\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\Detail analysis\\");
                }
                if (!File.Exists(currentDetailFilesLocation))
                {
                    FileStream StreamForDetails = File.Create(currentDetailFilesLocation);
                    StreamForDetails.Close();
                }

                StreamWriter writeDetail = new StreamWriter(currentDetailFilesLocation);
                writeDetail.Write(showRunningResultWithTextBox);
                writeDetail.Close();
                TextBoxRunningResultViewer.Text = showRunningResultWithTextBox;
                TextBoxDetailsPlace.Text = currentDetailFilesLocation;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
       }

        private void ButtonNewInputCaseFile_Click(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 1;
            ButtonOpenLogFile.Focus();
        }

        private void ButtonOpenLogFClick(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openLogFile = new OpenFileDialog();
                openLogFile.Filter = "Xml Files(*.xml)|*.xml|All Files|*.*";
                if (!Directory.Exists(Application.StartupPath + "\\LogFiles\\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\LogFiles\\");
                }
                openLogFile.InitialDirectory = Application.StartupPath + "\\LogFiles\\";

                XmlDocument xmlDoc = new XmlDocument();

                openLogFile.ShowDialog();

                if (!File.Exists(openLogFile.FileName))
                {
                    return;
                }
                StreamReader readTheXmlLogFile = new StreamReader(openLogFile.FileName);

                TextBoxShowLogF.Text = readTheXmlLogFile.ReadToEnd();
                LabelLogFileName.Text = Path.GetFileName(openLogFile.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ComboBoxLogFileSelectedSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(ComboBoxLogFile.Text))
                {
                    StreamReader readTheXmlLogFile = new StreamReader(ComboBoxLogFile.Text);
                    TextBoxShowLogF.Text = readTheXmlLogFile.ReadToEnd();
                    LabelLogFileName.Text = Path.GetFileName(ComboBoxLogFile.Text);
                    readTheXmlLogFile.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ComboBoxDetailFilesSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(ComboBoxDetailFiles.Text))
                {
                    StreamReader readTheXmlLogFile = new StreamReader(ComboBoxDetailFiles.Text);
                    TextBoxRunningResultViewer.Text = readTheXmlLogFile.ReadToEnd();
                    LabelDetailFileName.Text = Path.GetFileName(ComboBoxLogFile.Text);
                    readTheXmlLogFile.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void TreeViewShowDllStructureAfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                string nameOfSelectedCase = TreeViewShowDllStructure.SelectedNode.Text.ToString();
                int nameOfSelectedCaseLength = nameOfSelectedCase.Length - 12;

                if (nameOfSelectedCase.IndexOf("T") != 0)
                {
                    return;
                }
                TextBoxSortMethodSelect.Text = nameOfSelectedCase.Substring(4, nameOfSelectedCaseLength);
                tempSortMethod = nameOfSelectedCase.Substring(4, nameOfSelectedCaseLength);
                TextBoxInputValue.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ButtonOpenDetailsView_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDetailFile = new OpenFileDialog();
                openDetailFile.Filter = "Text Files|*.txt|All Files|*.*";
                if (!Directory.Exists(Application.StartupPath + "\\Detail analysis\\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\Detail analysis\\");
                }
                openDetailFile.InitialDirectory = Application.StartupPath + "\\Detail analysis\\";

                openDetailFile.ShowDialog();

                if (!File.Exists(openDetailFile.FileName))
                {
                    return;
                }
                TextBoxDetailsPlace.Text = openDetailFile.FileName;
                currentDetailFilesLocation = openDetailFile.FileName;
                currentDetailFilesName = Path.GetFileName(currentDetailFilesLocation);

                StreamReader readTheXmlLogFile = new StreamReader(openDetailFile.FileName);
                TextBoxRunningResultViewer.Text = readTheXmlLogFile.ReadToEnd();
                LabelDetailFileName.Text = Path.GetFileName(openDetailFile.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ButtonSaveAsTheDetailFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentDetailFilesLocation == null)
                    return;
                StreamWriter writeDetail = new StreamWriter(currentDetailFilesLocation);
                writeDetail.Write(TextBoxRunningResultViewer.Text);
                writeDetail.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ButtonSaveTheLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentLogFileLocation == null)
                    return;
                StreamWriter writeDetail = new StreamWriter(currentLogFileLocation);
                writeDetail.Write(TextBoxShowLogF.Text);
                writeDetail.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ButtonReturnFormLogToMainFormClick(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 0;
        }

        private void ToolStripStatusLabelCurrentCaseInside_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(currentCaseFileLocation);
            }
            catch
            { }
        }

        private void howDoIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Copyright:Vincent wang                                         Company:Wicresoft  ", "TestUtility v1.0");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void viewRunResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 2;
        }

























    }
}
