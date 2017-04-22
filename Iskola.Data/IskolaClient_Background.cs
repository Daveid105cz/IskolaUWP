using Iskola.Data.DataTabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iskola.Data
{
    public partial class IskolaClient
    {
        public async Task<List<Mark>> GetNewUploadedMarks(List<Mark> MarksBackup,String Username,String Password,String School)
        {
            List<Mark> newMarks = new List<Mark>();
            await Login(Username, Password, School);
            RatingDataTab ratingDataTab = new RatingDataTab(this);
            await ratingDataTab.DownloadDataAsync();
            MarksTable marksTable = ratingDataTab.Marks;
            List<Mark> marks = new List<Mark>();
            List<Mark> oldMarks = new List<Mark>();
            foreach (RatedSubject subject in marksTable.RatedSubjects)
            {
                foreach(Mark mark in subject.Marks)
                {
                    marks.Add(mark);
                    oldMarks.Add(mark);
                }
            }
            oldMarks.RemoveRange(0, 5);
            foreach (Mark mark in marks)
            {
                if (!oldMarks.Contains(mark))
                    newMarks.Add(mark);
            }
            SendLogoutRequest();
            return newMarks;
        }
    }
}
