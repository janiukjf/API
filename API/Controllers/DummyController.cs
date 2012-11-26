using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace API.Controllers {
    public class DummyController : Controller {

        public void GenerateCategoryCSV() {
            string filepath = @"C:\tmp\hitches2go.csv";
            string delimiter = ",";

            List<Dictionary<int, List<string>>> output = new List<Dictionary<int, List<string>>>();

            CurtDevDataContext db = new CurtDevDataContext();
            List<int> part_ids = db.Parts.Select(x => x.partID).ToList<int>();
            foreach (int part_id in part_ids) {
                //string[] found_cats = new string[] { part_id.ToString() };
                Dictionary<int, List<string>> found_cats = new Dictionary<int, List<string>>();
                List<string> cat_list = new List<string>();

                // Get the cateogries for this part
                List<Categories> cats = (from c in db.Categories
                                         join cp in db.CatParts on c.catID equals cp.catID
                                         where cp.partID.Equals(part_id)
                                         select c).ToList<Categories>();
                foreach (Categories cat in cats) {
                    
                    // Get the parent category
                    int parentID = cat.parentID;
                    int counter = 0;
                    Categories parent_cat = new Categories();
                    while (parentID != 0 && counter < 20) {
                        parent_cat = db.Categories.Where(x => x.catID.Equals(parentID)).FirstOrDefault<Categories>();
                        parentID = parent_cat.parentID;
                        counter++;
                    }
                    cat_list.Add(parent_cat.catTitle.Trim());
                }
                found_cats.Add(part_id, cat_list);
                output.Add(found_cats);
            }
            
            StringBuilder sb = new StringBuilder();
            foreach (Dictionary<int, List<string>> record in output) {
                KeyValuePair<int, List<string>> rec = record.FirstOrDefault();
                string line = rec.Key + ",";
                foreach (string c in rec.Value) {
                    line += c + ",";
                    /*if (c != rec.Value.Last()) {
                        line += ",";
                    }*/
                }
                sb.AppendLine(line);
            }
            System.IO.File.WriteAllText(filepath, sb.ToString());
        }

    }
}
