using System.ComponentModel;
using HBKPlatform.Database;

namespace HBKPlatform.Models.View;

public class PractitionerViewModel { 

        public int Idx { get; set; }
        public string Name { get; set; }
        public Title Title { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }
        public DateTime DOB { get; set; }
        public string Img { get; set; }

}
