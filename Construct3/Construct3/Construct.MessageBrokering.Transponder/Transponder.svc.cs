using System;
using System.Linq;

namespace Construct.MessageBrokering.Transponder
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Transponder" in code, svc and config file together.
    public class Transponder : ITransponder
    {
        public bool AddObject(string jsondata)
        {
            try
            {
                string x = jsondata.ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}