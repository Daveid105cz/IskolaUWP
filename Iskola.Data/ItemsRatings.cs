using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iskola.Data
{
    public class MarksTable
    {
        List<RatedSubject> _ratedSubjects = new List<RatedSubject>();
        public List<RatedSubject> RatedSubjects { get { return _ratedSubjects; } }
    }
    public class RatedSubject
    {
        public String Average { get; internal set; }//Průměr
        public String Qualification { get; internal set; }//Vysvedceni
        public String SubjectName { get; internal set;}

        List<Mark> _marks = new List<Mark>();
        public List<Mark> Marks { get { return _marks; } }
    }
    public class Mark
    {
        public String Value { get; internal set; }
        public String Date { get; internal set; }
        public String Teacher { get; internal set; }
        public String ForWhat { get; internal set; }//Hodnocení za co ?
        public String TeachingOkruh { get; internal set; }//Okruh uciva
        public String StructComment { get; internal set; }//Strucny komentar
        public long ID { get; internal set; }
    } 
    public class MarkInfo:Mark
    {
        public String Subject { get; internal set; }
        public String DateOfEnter { get; internal set; }
        public String MarkValuability { get; internal set; }

    }
}
