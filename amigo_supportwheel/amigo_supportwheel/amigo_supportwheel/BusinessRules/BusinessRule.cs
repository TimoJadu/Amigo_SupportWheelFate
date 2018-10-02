using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace amigo_supportwheel.BusinessRules
{
    public sealed class BuildDataSet
    {
        private static BuildDataSet instance = null;
        internal Dictionary<DateTime, List<string[]>> dutyList = new Dictionary<DateTime, List<string[]>>();
        private DateTime _date;
        List<string[]> empList = null;
        public string errorMessage = string.Empty;

        FirstRule fr = null;

        private static readonly object InstanceLock = new object();

        private BuildDataSet() { }

        public static BuildDataSet GetInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new BuildDataSet();
                        }
                    }
                }
                return instance;
            }
        }

        public void formDataSet(DateTime _dateTime, string _employeeName, string shiftType)
        {
            string[] employeeDetails = new string[2];
            employeeDetails[0] = _employeeName;
            employeeDetails[1] = shiftType;

            if (dutyList.Count == 0)
            {
                empList = new List<string[]>();
                empList.Add(employeeDetails);
                dutyList.Add(_dateTime, empList);
            }
            else
            {
                fr = new ThirdRule(dutyList, _dateTime, _employeeName, shiftType);
                if (fr.ruleValidated)
                {
                    if (dutyList.ContainsKey(_dateTime))
                    {
                        dutyList[_dateTime].Add(employeeDetails);
                    }
                    else
                    {
                        empList = new List<string[]>();
                        empList.Add(employeeDetails);
                        dutyList.Add(_dateTime, empList);
                    }
                    errorMessage = string.Empty;
                }
                else { errorMessage = fr.errorMessage; }
            }
        }
    }
    public class FirstRule
    {
        public bool ruleValidated = true;
        public string errorMessage = string.Empty;

        public FirstRule(Dictionary<DateTime, List<string[]>> _dutyList, DateTime _dateTime, string _employeeName, string shiftType)
        {
            if (shiftType == "Half")
            {
                string[] employeeDetails = new string[2];
                employeeDetails[0] = _employeeName;
                employeeDetails[1] = shiftType;

                //new System.Collections.Generic.Mscorlib_CollectionDebugView<string[]>(_dutyList[_dateTime]).Items[0]
                //if (_dutyList[_dateTime].Find(employeeDetails))
                if (_dutyList.ContainsKey(_dateTime))
                {
                    for (int i = 0; i < _dutyList[_dateTime].Count; i++)
                    {
                        if ((_dutyList[_dateTime][i][0] == _employeeName) && (_dutyList[_dateTime][i][1] == shiftType))
                        {
                            ruleValidated = false;
                            errorMessage = "An engineer can do at most one half day shift in a day.";
                            break;
                        }
                    }
                }
            }

            Console.WriteLine("1 called");
        }
        public virtual string employeeSelected()
        {
            Console.WriteLine("1 called");
            return string.Empty;
        }
    }

    public class SecondRule : FirstRule
    {
        public SecondRule(Dictionary<DateTime, List<string[]>> _dutyList, DateTime _dateTime, string _employeeName, string shiftType) : base(_dutyList, _dateTime, _employeeName, shiftType)
        {
            if (!(errorMessage == "An engineer can do at most one half day shift in a day."))
            {
                string[] employeeDetails = new string[2];
                employeeDetails[0] = _employeeName;
                employeeDetails[1] = shiftType;

                //new System.Collections.Generic.Mscorlib_CollectionDebugView<string[]>(_dutyList[_dateTime]).Items[0]
                //if (_dutyList[_dateTime].Find(employeeDetails))
                if (shiftType == "Half")
                {
                    for (int i = 0; i < _dutyList.ElementAt(_dutyList.Count - 1).Value.Count; i++)
                    {
                        if ((_dutyList.ElementAt(_dutyList.Count - 1).Value[i][0] == _employeeName) && (_dutyList.ElementAt(_dutyList.Count - 1).Value[i][1] == shiftType))
                        {
                            ruleValidated = false;
                            errorMessage = "An engineer cannot have half day shifts on consecutive days.";
                            break;
                        }
                    }
                }
            }

            Console.WriteLine("2 called");
        }
        public override string employeeSelected()
        {
            Console.WriteLine("2 called");
            // return base.employeeSelected();
            return string.Empty;
        }
    }

    public class ThirdRule : SecondRule
    {
        public ThirdRule(Dictionary<DateTime, List<string[]>> _dutyList, DateTime _dateTime, string _employeeName, string shiftType) : base(_dutyList, _dateTime, _employeeName, shiftType)
        {
            if (shiftType == "Full")
            {
                Dictionary<DateTime, List<string[]>> _dutyListFullTimer = new Dictionary<DateTime, List<string[]>>();
                List<string> tempList = new List<string>();
                for (int j = _dutyList.Count; j > 0; j--)
                {
                    for (int i = 0; i < _dutyList.ElementAt(j-1).Value.Count; i++)
                    {
                        if ((_dutyList.ElementAt(j-1)).Value[i][1] == "Full")
                        {
                            tempList.Add((_dutyList.ElementAt(j-1)).Value[i][0]);
                        }
                    }
                }
                ///employees provided full time support prepared
                ///
                //tempList.Reverse();
                if (tempList.Count > 0)
                {
                    int range = (tempList.Count < 10) ? tempList.Count : 10;
                    //string check = tempList.GetRange(0, range).Where(x => x == _employeeName).ToString();

                    if (tempList.GetRange(0, range).Where(x => x == _employeeName).Any())
                    {
                        ruleValidated = false;
                        errorMessage = "Each engineer should have completed one whole day of support in any 2 week period.";
                        return;
                    }
                }
            }

            Console.WriteLine("3 called");
        }
        public override string employeeSelected()
        {
            Console.WriteLine("3 called");
            //return base.employeeSelected();
            return string.Empty;
        }
    }
}
