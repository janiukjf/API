using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using API.Models;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data.Linq.SqlClient;
using System.Xml;
using LinqKit;

namespace API.Models {
    public class V2Model {

        /* Version 2.0 of CURT Manufacturing eCommerce Data API */

        public static string GetYearsJSON(string mount = "") {
            string result = "";
            CurtDevDataContext db = new CurtDevDataContext();
            List<double> yearList = new List<double>();
            mount = mount.Trim().ToLower();
            if (mount == "") {
                yearList = (from y in db.Years
                            join v in db.Vehicles on y.yearID equals v.yearID
                            join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                            select y.year1).Distinct().OrderByDescending(x => x).ToList<double>();
            } else if (mount.Contains("front")) {
                // 5 - id for front mount hitch
                yearList = (from y in db.Years
                            join v in db.Vehicles on y.yearID equals v.yearID
                            join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                            join p in db.Parts on vp.partID equals p.partID
                            where p.classID.Equals(5)
                            select y.year1).Distinct().OrderByDescending(x => x).ToList<double>();
            } else {
                // 5 - id for front mount hitch
                List<int> classids = new List<int> { 5, 11 };
                yearList = (from y in db.Years
                            join v in db.Vehicles on y.yearID equals v.yearID
                            join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                            join p in db.Parts on vp.partID equals p.partID
                            where !classids.Contains(p.classID)
                            select y.year1).Distinct().OrderByDescending(x => x).ToList<double>();
            }
            result = JsonConvert.SerializeObject(yearList);

            return result;
        }

        public static XDocument GetYearsXML(string mount = "") {
            XDocument xml = new XDocument();
            XElement years_container = new XElement("Years");
            List<double> years = new List<double>();
            CurtDevDataContext db = new CurtDevDataContext();
            mount = mount.Trim().ToLower();
            if (mount == "") {
                years = (from y in db.Years
                         join v in db.Vehicles on y.yearID equals v.yearID
                         join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                         select y.year1).Distinct().OrderByDescending(x => x).ToList<double>();
            } else if (mount.Contains("front")) {
                // 5 - id for front mount hitch
                years = (from y in db.Years
                         join v in db.Vehicles on y.yearID equals v.yearID
                         join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                         join p in db.Parts on vp.partID equals p.partID
                         where p.classID.Equals(5)
                         select y.year1).Distinct().OrderByDescending(x => x).ToList<double>();
            } else {
                // 5 - id for front mount hitch
                List<int> classids = new List<int> { 5, 11 };
                years = (from y in db.Years
                         join v in db.Vehicles on y.yearID equals v.yearID
                         join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                         join p in db.Parts on vp.partID equals p.partID
                         where !classids.Contains(p.classID)
                         select y.year1).Distinct().OrderByDescending(x => x).ToList<double>();
            }
            foreach (double year in years) {
                XElement y = new XElement("Year");
                y.Value = year.ToString();
                years_container.Add(y);
            }
            xml.Add(years_container);
            return xml;
        }

        public static string GetMakesJSON(string mount = "", double year = 0) {
            List<string> makes = new List<string>();
            CurtDevDataContext db = new CurtDevDataContext();

            mount = mount.Trim().ToLower();
            if (year == 0) {
                makes = (from m in db.Makes
                            join v in db.Vehicles on m.makeID equals v.makeID
                            join mm in db.MakeModels on m.makeID equals mm.makeID
                            join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                            select m.make1).Distinct().OrderBy(x => x).ToList<string>();
            } else {
                if (mount == "") {
                    makes = (from m in db.Makes
                             join v in db.Vehicles on m.makeID equals v.makeID
                             join y in db.Years on v.yearID equals y.yearID
                             join mm in db.MakeModels on m.makeID equals mm.makeID
                             join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                             where y.year1.Equals(year)
                             select m.make1).Distinct().OrderBy(x => x).ToList<string>();
                } else if (mount.Contains("front")) {
                    // 5 - id for front mount hitch
                    makes = (from m in db.Makes
                             join v in db.Vehicles on m.makeID equals v.makeID
                             join y in db.Years on v.yearID equals y.yearID
                             join mm in db.MakeModels on m.makeID equals mm.makeID
                             join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                             join p in db.Parts on vp.partID equals p.partID
                             where y.year1.Equals(year) && p.classID.Equals(5)
                             select m.make1).Distinct().OrderBy(x => x).ToList<string>();
                } else {
                    // 5 - id for front mount hitch
                    List<int> classids = new List<int> { 5, 11 };
                    makes = (from m in db.Makes
                             join v in db.Vehicles on m.makeID equals v.makeID
                             join y in db.Years on v.yearID equals y.yearID
                             join mm in db.MakeModels on m.makeID equals mm.makeID
                             join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                             join p in db.Parts on vp.partID equals p.partID
                             where y.year1.Equals(year) && !classids.Contains(p.classID)
                             select m.make1).Distinct().OrderBy(x => x).ToList<string>();
                }
            }

            return JsonConvert.SerializeObject(makes);
        }

        public static XDocument GetMakesXML(string mount = "", double year = 0) {
            XDocument xml = new XDocument();
            XElement makes_container = new XElement("Makes");
            List<string> makes = new List<string>();
            CurtDevDataContext db = new CurtDevDataContext();

            mount = mount.Trim().ToLower();
            if (year == 0) {
                makes = (from m in db.Makes
                            join v in db.Vehicles on m.makeID equals v.makeID
                            join mm in db.MakeModels on m.makeID equals mm.makeID
                            join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                            select m.make1).Distinct().OrderBy(x => x).ToList<string>();
            } else {
                if (mount == "") {
                    makes = (from m in db.Makes
                             join v in db.Vehicles on m.makeID equals v.makeID
                             join y in db.Years on v.yearID equals y.yearID
                             join mm in db.MakeModels on m.makeID equals mm.makeID
                             join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                             where y.year1.Equals(year)
                             select m.make1).Distinct().OrderBy(x => x).ToList<string>();
                } else if (mount.Contains("front")) {
                    makes = (from m in db.Makes
                             join v in db.Vehicles on m.makeID equals v.makeID
                             join y in db.Years on v.yearID equals y.yearID
                             join mm in db.MakeModels on m.makeID equals mm.makeID
                             join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                             join p in db.Parts on vp.partID equals p.partID
                             where y.year1.Equals(year) && p.classID.Equals(5)
                             select m.make1).Distinct().OrderBy(x => x).ToList<string>();
                } else {
                    List<int> classids = new List<int> { 5, 11 };
                    makes = (from m in db.Makes
                             join v in db.Vehicles on m.makeID equals v.makeID
                             join y in db.Years on v.yearID equals y.yearID
                             join mm in db.MakeModels on m.makeID equals mm.makeID
                             join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                             join p in db.Parts on vp.partID equals p.partID
                             where y.year1.Equals(year) && !classids.Contains(p.classID)
                             select m.make1).Distinct().OrderBy(x => x).ToList<string>();
                }
            }

            foreach (string make in makes) {
                XElement m = new XElement("Make");
                m.Value = make;
                makes_container.Add(m);
            }
            xml.Add(makes_container);
            return xml;
        }

        public static string GetModelsJSON(string mount = "", double year = 0, string make = "") {
            string models = "";
            List<string> modelList = new List<string>();
            CurtDevDataContext db = new CurtDevDataContext();
            mount = mount.Trim().ToLower();
            if (year != 0 && make.Length > 0) {
                if (mount == "") {
                    modelList = (from m in db.Models
                                 join v in db.Vehicles on m.modelID equals v.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join ms in db.ModelStyles on m.modelID equals ms.modelID
                                 where ma.make1.Equals(make) && y.year1.Equals(year)
                                 select m.model1).Distinct().OrderBy(x => x).ToList<string>();
                } else if (mount.Contains("front")) {
                    modelList = (from m in db.Models
                                 join v in db.Vehicles on m.modelID equals v.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join ms in db.ModelStyles on m.modelID equals ms.modelID
                                 join p in db.Parts on vp.partID equals p.partID
                                 where ma.make1.Equals(make) && y.year1.Equals(year) && p.classID.Equals(5)
                                 select m.model1).Distinct().OrderBy(x => x).ToList<string>();
                } else {
                    List<int> classids = new List<int> { 5, 11 };
                    modelList = (from m in db.Models
                                 join v in db.Vehicles on m.modelID equals v.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join ms in db.ModelStyles on m.modelID equals ms.modelID
                                 join p in db.Parts on vp.partID equals p.partID
                                 join c in db.Classes on p.classID equals c.classID
                                 where ma.make1.Equals(make) && y.year1.Equals(year) && !classids.Contains(p.classID)
                                 select m.model1).Distinct().OrderBy(x => x).ToList<string>();
                }
            } else {
                modelList = (from m in db.Models
                                select m.model1).Distinct().OrderBy(x => x).ToList<string>();
            }
            return JsonConvert.SerializeObject(modelList);
        }

        public static XDocument GetModelsXML(string mount = "", double year = 0, string make = "") {
            string models = "";
            XDocument xml = new XDocument();
            XElement models_container = new XElement("Models");
            List<string> modelList = new List<string>();
            CurtDevDataContext db = new CurtDevDataContext();
            mount = mount.Trim().ToLower();

            if (year != 0 && make.Length > 0) {
                if (mount == "") {
                    modelList = (from m in db.Models
                                 join v in db.Vehicles on m.modelID equals v.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join ms in db.ModelStyles on m.modelID equals ms.modelID
                                 where ma.make1.Equals(make) && y.year1.Equals(year)
                                 select m.model1).Distinct().OrderBy(x => x).ToList<string>();
                } else if (mount.Contains("front")) {
                    modelList = (from m in db.Models
                                 join v in db.Vehicles on m.modelID equals v.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join ms in db.ModelStyles on m.modelID equals ms.modelID
                                 join p in db.Parts on vp.partID equals p.partID
                                 where ma.make1.Equals(make) && y.year1.Equals(year) && p.classID.Equals(5)
                                 select m.model1).Distinct().OrderBy(x => x).ToList<string>();
                } else {
                    List<int> classids = new List<int> { 5, 11 };
                    modelList = (from m in db.Models
                                 join v in db.Vehicles on m.modelID equals v.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join ms in db.ModelStyles on m.modelID equals ms.modelID
                                 join p in db.Parts on vp.partID equals p.partID
                                 where ma.make1.Equals(make) && y.year1.Equals(year) && !classids.Contains(p.classID)
                                 select m.model1).Distinct().OrderBy(x => x).ToList<string>();
                }
            } else {
                modelList = (from m in db.Models
                                select m.model1).Distinct().OrderBy(x => x).ToList<string>();
            }

            foreach (string model in modelList) {
                XElement m = new XElement("Model");
                m.Value = model;
                models_container.Add(m);
            }
            xml.Add(models_container);
            return xml;
        }

        public static string GetStylesJSON(string mount = "", double year = 0, string make = "", string model = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            List<string> styleList = new List<string>();
            mount = mount.Trim().ToLower();

            if (year > 0 && make.Length > 0 && model.Length > 0) {
                if (mount == "") {
                    styleList = (from s in db.Styles
                                 join v in db.Vehicles on s.styleID equals v.styleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join mo in db.Models on v.modelID equals mo.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 where mo.model1.Equals(model) && ma.make1.Equals(make) && y.year1.Equals(year)
                                 select s.style1).Distinct().OrderBy(x => x).ToList<string>();
                } else if (mount.Contains("front")) {
                    styleList = (from s in db.Styles
                                 join v in db.Vehicles on s.styleID equals v.styleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join mo in db.Models on v.modelID equals mo.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 join p in db.Parts on vp.partID equals p.partID
                                 where mo.model1.Equals(model) && ma.make1.Equals(make) && y.year1.Equals(year) && p.classID.Equals(5)
                                 select s.style1).Distinct().OrderBy(x => x).ToList<string>();
                } else {
                    List<int> classids = new List<int> { 5, 11 };
                    styleList = (from s in db.Styles
                                 join v in db.Vehicles on s.styleID equals v.styleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join mo in db.Models on v.modelID equals mo.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 join p in db.Parts on vp.partID equals p.partID
                                 where mo.model1.Equals(model) && ma.make1.Equals(make) && y.year1.Equals(year) && !classids.Contains(p.classID)
                                 select s.style1).Distinct().OrderBy(x => x).ToList<string>();
                }
            } else {
                styleList = (from s in db.Styles
                                select s.style1).Distinct().OrderBy(x => x).ToList<string>();
            }
            return JsonConvert.SerializeObject(styleList);
        }

        public static XDocument GetStylesXML(string mount = "", double year = 0, string make = "", string model = "") {
            XDocument xml = new XDocument();
            XElement styles_container = new XElement("Styles");
            List<string> styleList = new List<string>();
            CurtDevDataContext db = new CurtDevDataContext();
            mount = mount.Trim().ToLower();

            if (year != 0 && make.Length > 0 && model.Length > 0) {
                if (mount == "") {
                    styleList = (from s in db.Styles
                                    join v in db.Vehicles on s.styleID equals v.styleID
                                    join y in db.Years on v.yearID equals y.yearID
                                    join ma in db.Makes on v.makeID equals ma.makeID
                                    join mo in db.Models on v.modelID equals mo.modelID
                                    join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                    where mo.model1.Equals(model) && ma.make1.Equals(make) && y.year1.Equals(year)
                                    select s.style1).Distinct().OrderBy(x => x).ToList<string>();
                } else if (mount.Contains("front")) {
                    styleList = (from s in db.Styles
                                 join v in db.Vehicles on s.styleID equals v.styleID
                                 join y in db.Years on v.yearID equals y.yearID
                                 join ma in db.Makes on v.makeID equals ma.makeID
                                 join mo in db.Models on v.modelID equals mo.modelID
                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                 join p in db.Parts on vp.partID equals p.partID
                                 where mo.model1.Equals(model) && ma.make1.Equals(make) && y.year1.Equals(year) && p.classID.Equals(5)
                                 select s.style1).Distinct().OrderBy(x => x).ToList<string>();
                } else {
                    List<int> classids = new List<int> { 5,11 };
                    styleList = (from s in db.Styles
                                    join v in db.Vehicles on s.styleID equals v.styleID
                                    join y in db.Years on v.yearID equals y.yearID
                                    join ma in db.Makes on v.makeID equals ma.makeID
                                    join mo in db.Models on v.modelID equals mo.modelID
                                    join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                    join p in db.Parts on vp.partID equals p.partID
                                    where mo.model1.Equals(model) && ma.make1.Equals(make) && y.year1.Equals(year) && !classids.Contains(p.classID)
                                    select s.style1).Distinct().OrderBy(x => x).ToList<string>();
                }
            } else {
                styleList = (from s in db.Styles
                                select s.style1).Distinct().OrderBy(x => x).ToList<string>();
            }


            foreach (string style in styleList) {
                XElement s = new XElement("Style");
                s.Value = style;
                styles_container.Add(s);
            }
            xml.Add(styles_container);
            return xml;
        }

        public static string GetVehiclesJSON(double year = 0, string make = "", string model = "", string style = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            List<FullVehicle> vehicleList = new List<FullVehicle>();

            if (year > 0 && make.Length > 0 && model.Length > 0 && style.Length > 0) {
                vehicleList = (from v in db.Vehicles
                               join y in db.Years on v.yearID equals y.yearID
                               join ma in db.Makes on v.makeID equals ma.makeID
                               join mo in db.Models on v.modelID equals mo.modelID
                               join s in db.Styles on v.styleID equals s.styleID
                               where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style)
                               select new FullVehicle {
                                   vehicleID = v.vehicleID,
                                   yearID = v.yearID,
                                   makeID = v.makeID,
                                   modelID = v.modelID,
                                   styleID = v.styleID,
                                   aaiaID = s.aaiaID,
                                   year = y.year1,
                                   make = ma.make1,
                                   model = mo.model1,
                                   style = s.style1
                               }).Distinct().OrderBy(x => x.vehicleID).ToList<FullVehicle>();
            } else { // Get all vehicles
                vehicleList = (from v in db.Vehicles
                               join y in db.Years on v.yearID equals y.yearID
                               join ma in db.Makes on v.makeID equals ma.makeID
                               join mo in db.Models on v.modelID equals mo.modelID
                               join s in db.Styles on v.styleID equals s.styleID
                               select new FullVehicle {
                                   vehicleID = v.vehicleID,
                                   yearID = v.yearID,
                                   makeID = v.makeID,
                                   modelID = v.modelID,
                                   styleID = v.styleID,
                                   aaiaID = s.aaiaID,
                                   year = y.year1,
                                   make = ma.make1,
                                   model = mo.model1,
                                   style = s.style1
                               }).Distinct().OrderBy(x => x.vehicleID).ToList<FullVehicle>();
            }
            return JsonConvert.SerializeObject(vehicleList);
        }

        public static XDocument GetVehiclesXML(double year = 0, string make = "", string model = "", string style = "") {
            XDocument xml = new XDocument();
            XElement vehicle_container = new XElement("Vehicles");
            CurtDevDataContext db = new CurtDevDataContext();
            List<FullVehicle> vehicleList = new List<FullVehicle>();

            if (year > 0 && make.Length > 0 && model.Length > 0 && style.Length > 0) {
                vehicleList = (from v in db.Vehicles
                               join y in db.Years on v.yearID equals y.yearID
                               join ma in db.Makes on v.makeID equals ma.makeID
                               join mo in db.Models on v.modelID equals mo.modelID
                               join s in db.Styles on v.styleID equals s.styleID
                               where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style)
                               select new FullVehicle {
                                   vehicleID = v.vehicleID,
                                   yearID = v.yearID,
                                   makeID = v.makeID,
                                   modelID = v.modelID,
                                   styleID = v.styleID,
                                   aaiaID = s.aaiaID,
                                   year = y.year1,
                                   make = ma.make1,
                                   model = mo.model1,
                                   style = s.style1
                               }).Distinct().OrderBy(x => x.vehicleID).ToList<FullVehicle>();
            } else { // Get all vehicles
                vehicleList = (from v in db.Vehicles
                               join y in db.Years on v.yearID equals y.yearID
                               join ma in db.Makes on v.makeID equals ma.makeID
                               join mo in db.Models on v.modelID equals mo.modelID
                               join s in db.Styles on v.styleID equals s.styleID
                               select new FullVehicle {
                                   vehicleID = v.vehicleID,
                                   yearID = v.yearID,
                                   makeID = v.makeID,
                                   modelID = v.modelID,
                                   styleID = v.styleID,
                                   aaiaID = s.aaiaID,
                                   year = y.year1,
                                   make = ma.make1,
                                   model = mo.model1,
                                   style = s.style1
                               }).Distinct().OrderBy(x => x.vehicleID).ToList<FullVehicle>();
            }
            foreach (FullVehicle v in vehicleList) {
                // Create vehicle Element
                XElement vehicle = new XElement("Vehicle",
                    new XAttribute("vehicleID", v.vehicleID)
                );
                // Nest year element
                XElement xml_year = new XElement("Year", new XAttribute("yearID", v.yearID));
                xml_year.Value = v.year.ToString();
                vehicle.Add(xml_year);

                // Nest make element
                XElement xml_make = new XElement("Make", new XAttribute("makeID", v.makeID));
                xml_make.Value = v.make;
                vehicle.Add(xml_make);

                // Nest model element
                XElement xml_model = new XElement("Model", new XAttribute("modelID", v.modelID));
                xml_model.Value = v.model;
                vehicle.Add(xml_model);

                // Nest style element
                XElement xml_style = new XElement("Style",
                    new XAttribute("styleID", v.styleID),
                    new XAttribute("aaiaID", v.aaiaID));
                xml_style.Value = v.style;
                vehicle.Add(xml_style);
                vehicle_container.Add(vehicle);
            }
            xml.Add(vehicle_container);
            return xml;
        }

        public static string GetVehiclesByPartJSON(int partID = 0) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<FullVehicle> vehicles = new List<FullVehicle>();

                vehicles = (from v in db.Vehicles
                            join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                            join s in db.Styles on v.styleID equals s.styleID
                            join mo in db.Models on v.modelID equals mo.modelID
                            join ma in db.Makes on v.makeID equals ma.makeID
                            join y in db.Years on v.yearID equals y.yearID
                            where vp.partID.Equals(partID)
                            select new FullVehicle {
                                vehicleID = v.vehicleID,
                                yearID = v.yearID,
                                makeID = v.makeID,
                                modelID = v.modelID,
                                styleID = v.styleID,
                                aaiaID = s.aaiaID,
                                year = y.year1,
                                make = ma.make1,
                                model = mo.model1,
                                style = s.style1,
                                installTime = (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0,
                                drilling = (vp.drilling != null) ? vp.drilling : "",
                                exposed = (vp.exposed != null) ? vp.exposed : "",
                                attributes = (from vpa in db.VehiclePartAttributes
                                              where vpa.vPartID == vp.vPartID
                                              select new APIAttribute {
                                                  key = vpa.field,
                                                  value = vpa.value
                                              }).ToList<APIAttribute>()
                            }).ToList<FullVehicle>();

                return JsonConvert.SerializeObject(vehicles);
            } catch (Exception e) {
                return "";
            }

        }

        public static XDocument GetVehiclesByPartXML(int partID = 0) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                XDocument xml = new XDocument();

                XElement vehicles = new XElement("Vehicles", from v in db.Vehicles
                                                             join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                                             join s in db.Styles on v.styleID equals s.styleID
                                                             join mo in db.Models on v.modelID equals mo.modelID
                                                             join ma in db.Makes on v.makeID equals ma.makeID
                                                             join y in db.Years on v.yearID equals y.yearID
                                                             where vp.partID.Equals(partID)
                                                             select new XElement("Vehicle",
                                                                 new XAttribute("vehicleID", v.vehicleID),
                                                                 new XAttribute("yearID", v.yearID),
                                                                 new XAttribute("makeID", v.makeID),
                                                                 new XAttribute("modelID", v.modelID),
                                                                 new XAttribute("styleID", v.styleID),
                                                                 new XAttribute("aaiaID", s.aaiaID),
                                                                 new XAttribute("year", y.year1),
                                                                 new XAttribute("make", ma.make1),
                                                                 new XAttribute("model", mo.model1),
                                                                 new XAttribute("style", s.style1),
                                                                 new XAttribute("installTime", (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0),
                                                                 new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                                                                 new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                                                                 new XElement("Attributes",(from vpa in db.VehiclePartAttributes
                                                                               where vpa.vPartID.Equals(vp.vPartID)
                                                                               orderby vpa.sort
                                                                               select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>())
                                                             ));
                xml.Add(vehicles);
                return xml;
            } catch (Exception e) {
                return new XDocument();
            }
        }

        public static string GetLatestPartsJSON(List<int> statuses = null, bool integrated = false, int customerID = 0, int count = 5) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where p.featured == true && statuslist.Contains(p.status)
                         orderby p.dateModified descending
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).Distinct().Take(count).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where p.featured == true && statuslist.Contains(p.status) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                         orderby p.dateModified descending
                         select new APIPart {
                             partID = p.partID,
                             custPartID = ci.custPartID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).Distinct().Take(count).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetLatestPartsXML(List<int> statuses = null, bool integrated = false, int customerID = 0, int count = 5) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            List<int> statuslist = statuses ?? new List<int>();

            //List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where p.featured == true && statuslist.Contains(p.status)
                                               orderby p.dateModified descending
                                               select new XElement("Part",
                                                   new XAttribute("partID", p.partID),
                                                   new XAttribute("status", p.status),
                                                   new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                   new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                   new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                   new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                   new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                   new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                   new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                   new XAttribute("relatedCount", p.RelatedParts.Count),
                                                   new XElement("Attributes", (from pa in p.PartAttributes
                                                                               orderby pa.sort
                                                                               select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                            orderby vp.vTypeID
                                                                            select new XElement("Video", vp.video,
                                                                                new XAttribute("videoID",vp.pVideoID),
                                                                                new XAttribute("isPrimary",vp.isPrimary),
                                                                                new XAttribute("type",vp.videoType.name),
                                                                                new XAttribute("icon",vp.videoType.icon),
                                                                                new XAttribute("typeID",vp.vTypeID)
                                                                            )).ToList<XElement>()),
                                                   new XElement("Packages", (from pp in p.PartPackages
                                                                             select new XElement("Package",
                                                                                 new XAttribute("height",pp.height),
                                                                                 new XAttribute("length",pp.length),
                                                                                 new XAttribute("width",pp.width),
                                                                                 new XAttribute("weight",pp.weight),
                                                                                 new XAttribute("quantity",pp.quantity),
                                                                                 new XAttribute("dimensionUnit",pp.dimensionUnit.code),
                                                                                 new XAttribute("dimensionUnitLabel",pp.dimensionUnit.name),
                                                                                 new XAttribute("weightUnit",pp.weightUnit.code),
                                                                                 new XAttribute("weightUnitLabel",pp.weightUnit.name),
                                                                                 new XAttribute("packageUnit",pp.packageUnit.code),
                                                                                 new XAttribute("packageUnitLabel",pp.packageUnit.name)
                                                                             )).ToList<XElement>()),
                                                   new XElement("Pricing", (from pr in p.Prices
                                                                            orderby pr.priceType
                                                                            select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                    new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in p.Reviews
                                                                                         where r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in p.Reviews
                                                                             where r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in p.PartImages
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in p.PartImages
                                                                                              where pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pis.PartImageSize.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).Distinct().Take(count).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join ci in db.CartIntegrations on p.partID equals ci.partID
                                               join cu in db.Customers on ci.custID equals cu.customerID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where p.featured == true && statuslist.Contains(p.status) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                                               orderby p.dateModified descending
                                               select new XElement("Part",
                                                   new XAttribute("partID", p.partID),
                                                   new XAttribute("custPartID", ci.custPartID),
                                                   new XAttribute("status", p.status),
                                                   new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                   new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                   new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                   new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                   new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                   new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                   new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                   new XAttribute("relatedCount", p.RelatedParts.Count),
                                                   new XElement("Attributes", (from pa in p.PartAttributes
                                                                               orderby pa.sort
                                                                               select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                   new XElement("Packages", (from pp in p.PartPackages
                                                                             select new XElement("Package",
                                                                                 new XAttribute("height", pp.height),
                                                                                 new XAttribute("length", pp.length),
                                                                                 new XAttribute("width", pp.width),
                                                                                 new XAttribute("weight", pp.weight),
                                                                                 new XAttribute("quantity", pp.quantity),
                                                                                 new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                 new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                 new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                 new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                 new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                 new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                             )).ToList<XElement>()),
                                                   new XElement("Pricing", (from pr in p.Prices
                                                                            orderby pr.priceType
                                                                            select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in p.Reviews
                                                                                         where r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in p.Reviews
                                                                             where r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in p.PartImages
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in p.PartImages
                                                                                              where pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pis.PartImageSize.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                        )).ToList<XElement>())
                                               )).Distinct().Take(count).ToList<XElement>());
                #endregion
            }

            xml.Add(parts);
            return xml;
        }

        public static string GetPartsJSON(double year = 0, string make = "", string model = "", string style = "", List<int> statuses = null, bool integrated = false, int customerID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join y in db.Years on v.yearID equals y.yearID
                         join ma in db.Makes on v.makeID equals ma.makeID
                         join mo in db.Models on v.modelID equals mo.modelID
                         join s in db.Styles on v.styleID equals s.styleID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && statuslist.Contains(p.status)
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             vehicleID = v.vehicleID,
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                               where vpa.vPartID.Equals(vp.vPartID)
                                               orderby vpa.sort
                                               select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             installTime = Convert.ToInt32(vp.installTime),
                             drilling = vp.drilling,
                             exposed = vp.exposed,
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join y in db.Years on v.yearID equals y.yearID
                         join ma in db.Makes on v.makeID equals ma.makeID
                         join mo in db.Models on v.modelID equals mo.modelID
                         join s in db.Styles on v.styleID equals s.styleID
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                         select new APIPart {
                             partID = p.partID,
                             custPartID = ci.custPartID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             vehicleID = v.vehicleID,
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  where vpa.vPartID.Equals(vp.vPartID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             installTime = Convert.ToInt32(vp.installTime),
                             drilling = vp.drilling,
                             exposed = vp.exposed,
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetPartsXML(double year = 0, string make = "", string model = "", string style = "", List<int> statuses = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            List<int> statuslist = statuses ?? new List<int>();

            //List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join vp in db.VehicleParts on p.partID equals vp.partID
                                               join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                               join y in db.Years on v.yearID equals y.yearID
                                               join ma in db.Makes on v.makeID equals ma.makeID
                                               join mo in db.Models on v.modelID equals mo.modelID
                                               join s in db.Styles on v.styleID equals s.styleID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && statuslist.Contains(p.status)
                                               select new XElement("Part",
                                                   new XAttribute("partID", p.partID),
                                                   new XAttribute("status", p.status),
                                                   new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                   new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                   new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                   new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                   new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                   new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                   new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                   new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                   new XAttribute("vehicleID", v.vehicleID),
                                                   new XElement("Attributes", (from pa in db.PartAttributes
                                                                               where pa.partID.Equals(p.partID)
                                                                               orderby pa.sort
                                                                               select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                   new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                               where vpa.vPartID.Equals(vp.vPartID)
                                                                               orderby vpa.sort
                                                                               select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from pv in p.PartVideos
                                                                           orderby pv.vTypeID
                                                                           select new XElement("Video", pv.video,
                                                                               new XAttribute("videoID", pv.pVideoID),
                                                                               new XAttribute("isPrimary", pv.isPrimary),
                                                                               new XAttribute("type", pv.videoType.name),
                                                                               new XAttribute("icon", pv.videoType.icon),
                                                                               new XAttribute("typeID", pv.vTypeID)
                                                                           )).ToList<XElement>()),
                                                   new XElement("Packages", (from pp in p.PartPackages
                                                                             select new XElement("Package",
                                                                                 new XAttribute("height", pp.height),
                                                                                 new XAttribute("length", pp.length),
                                                                                 new XAttribute("width", pp.width),
                                                                                 new XAttribute("weight", pp.weight),
                                                                                 new XAttribute("quantity", pp.quantity),
                                                                                 new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                 new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                 new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                 new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                 new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                 new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                             )).ToList<XElement>()),
                                                   new XElement("Pricing", (from pr in db.Prices
                                                                            where pr.partID.Equals(p.partID)
                                                                            orderby pr.priceType
                                                                            select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                   new XAttribute("installTime", (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0),
                                                   new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                                                   new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                             )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join vp in db.VehicleParts on p.partID equals vp.partID
                                               join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                               join y in db.Years on v.yearID equals y.yearID
                                               join ma in db.Makes on v.makeID equals ma.makeID
                                               join mo in db.Models on v.modelID equals mo.modelID
                                               join s in db.Styles on v.styleID equals s.styleID
                                               join ci in db.CartIntegrations on p.partID equals ci.partID
                                               join cu in db.Customers on ci.custID equals cu.customerID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                                               select new XElement("Part",
                                                   new XAttribute("partID", p.partID),
                                                   new XAttribute("custPartID", ci.custPartID),
                                                   new XAttribute("status", p.status),
                                                   new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                   new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                   new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                   new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                   new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                   new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                   new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                   new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                   new XAttribute("vehicleID", v.vehicleID),
                                                   new XElement("Attributes", (from pa in db.PartAttributes
                                                                               where pa.partID.Equals(p.partID)
                                                                               orderby pa.sort
                                                                               select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                   new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                      where vpa.vPartID.Equals(vp.vPartID)
                                                                                      orderby vpa.sort
                                                                                      select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from pv in p.PartVideos
                                                                           orderby pv.vTypeID
                                                                           select new XElement("Video", pv.video,
                                                                               new XAttribute("videoID", pv.pVideoID),
                                                                               new XAttribute("isPrimary", pv.isPrimary),
                                                                               new XAttribute("type", pv.videoType.name),
                                                                               new XAttribute("icon", pv.videoType.icon),
                                                                               new XAttribute("typeID", pv.vTypeID)
                                                                           )).ToList<XElement>()),
                                                   new XElement("Packages", (from pp in p.PartPackages
                                                                             select new XElement("Package",
                                                                                 new XAttribute("height", pp.height),
                                                                                 new XAttribute("length", pp.length),
                                                                                 new XAttribute("width", pp.width),
                                                                                 new XAttribute("weight", pp.weight),
                                                                                 new XAttribute("quantity", pp.quantity),
                                                                                 new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                 new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                 new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                 new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                 new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                 new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                             )).ToList<XElement>()),
                                                   new XElement("Pricing", (from pr in db.Prices
                                                                            where pr.partID.Equals(p.partID)
                                                                            orderby pr.priceType
                                                                            select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                   new XAttribute("installTime", (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0),
                                                   new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                                                   new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            }

            xml.Add(parts);
            return xml;
        }

        public static string GetPartsWithFilterJSON(double year = 0, string make = "", string model = "", string style = "", string catName = "", List<int> statuses = null, bool integrated = false, int customerID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join y in db.Years on v.yearID equals y.yearID
                         join ma in db.Makes on v.makeID equals ma.makeID
                         join mo in db.Models on v.modelID equals mo.modelID
                         join s in db.Styles on v.styleID equals s.styleID
                         join cp in db.CatParts on p.partID equals cp.partID
                         join cat in db.Categories on cp.catID equals cat.catID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && cat.catTitle.Equals(catName) && statuslist.Contains(p.status)
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             vehicleID = v.vehicleID,
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  where vpa.vPartID.Equals(vp.vPartID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             installTime = Convert.ToInt32(vp.installTime),
                             drilling = vp.drilling,
                             exposed = vp.exposed,
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join y in db.Years on v.yearID equals y.yearID
                         join ma in db.Makes on v.makeID equals ma.makeID
                         join mo in db.Models on v.modelID equals mo.modelID
                         join s in db.Styles on v.styleID equals s.styleID
                         join cp in db.CatParts on p.partID equals cp.partID
                         join cat in db.Categories on cp.catID equals cat.catID
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && cat.catTitle.Equals(catName) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                         select new APIPart {
                             partID = p.partID,
                             custPartID = ci.custPartID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             vehicleID = v.vehicleID,
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  where vpa.vPartID.Equals(vp.vPartID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             installTime = Convert.ToInt32(vp.installTime),
                             drilling = vp.drilling,
                             exposed = vp.exposed,
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetPartsWithFilterXML(double year = 0, string make = "", string model = "", string style = "", string catName = "", List<int> statuses = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join vp in db.VehicleParts on p.partID equals vp.partID
                                               join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                               join y in db.Years on v.yearID equals y.yearID
                                               join ma in db.Makes on v.makeID equals ma.makeID
                                               join mo in db.Models on v.modelID equals mo.modelID
                                               join s in db.Styles on v.styleID equals s.styleID
                                               join cp in db.CatParts on p.partID equals cp.partID
                                               join cat in db.Categories on cp.catID equals cat.catID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && cat.catTitle.Equals(catName) && statuslist.Contains(p.status)
                                               select new XElement("Part",
                                                  new XAttribute("partID", p.partID),
                                                  new XAttribute("status", p.status),
                                                  new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                  new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                  new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                  new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                  new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                  new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                  new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                  new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                  new XAttribute("vehicleID", v.vehicleID),
                                                  new XElement("Attributes", (from pa in db.PartAttributes
                                                                              where pa.partID.Equals(p.partID)
                                                                              orderby pa.sort
                                                                              select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                  new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                     where vpa.vPartID.Equals(vp.vPartID)
                                                                                     orderby vpa.sort
                                                                                     select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from pv in p.PartVideos
                                                                           orderby pv.vTypeID
                                                                           select new XElement("Video", pv.video,
                                                                               new XAttribute("videoID", pv.pVideoID),
                                                                               new XAttribute("isPrimary", pv.isPrimary),
                                                                               new XAttribute("type", pv.videoType.name),
                                                                               new XAttribute("icon", pv.videoType.icon),
                                                                               new XAttribute("typeID", pv.vTypeID)
                                                                           )).ToList<XElement>()),
                                                   new XElement("Packages", (from pp in p.PartPackages
                                                                             select new XElement("Package",
                                                                                 new XAttribute("height", pp.height),
                                                                                 new XAttribute("length", pp.length),
                                                                                 new XAttribute("width", pp.width),
                                                                                 new XAttribute("weight", pp.weight),
                                                                                 new XAttribute("quantity", pp.quantity),
                                                                                 new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                 new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                 new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                 new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                 new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                 new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                             )).ToList<XElement>()),
                                                  new XElement("Pricing", (from pr in db.Prices
                                                                           where pr.partID.Equals(p.partID)
                                                                           orderby pr.priceType
                                                                           select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                  new XAttribute("installTime", (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0),
                                                   new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                                                   new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join vp in db.VehicleParts on p.partID equals vp.partID
                                               join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                               join y in db.Years on v.yearID equals y.yearID
                                               join ma in db.Makes on v.makeID equals ma.makeID
                                               join mo in db.Models on v.modelID equals mo.modelID
                                               join s in db.Styles on v.styleID equals s.styleID
                                               join cp in db.CatParts on p.partID equals cp.partID
                                               join cat in db.Categories on cp.catID equals cat.catID
                                               join ci in db.CartIntegrations on p.partID equals ci.partID
                                               join cu in db.Customers on ci.custID equals cu.customerID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && cat.catTitle.Equals(catName) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                                               select new XElement("Part",
                                                  new XAttribute("partID", p.partID),
                                                  new XAttribute("custPartID", ci.custPartID),
                                                  new XAttribute("status", p.status),
                                                  new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                  new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                  new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                  new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                  new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                  new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                  new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                  new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                  new XAttribute("vehicleID", v.vehicleID),
                                                  new XElement("Attributes", (from pa in db.PartAttributes
                                                                              where pa.partID.Equals(p.partID)
                                                                              orderby pa.sort
                                                                              select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                  new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                     where vpa.vPartID.Equals(vp.vPartID)
                                                                                     orderby vpa.sort
                                                                                     select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from pv in p.PartVideos
                                                                           orderby pv.vTypeID
                                                                           select new XElement("Video", pv.video,
                                                                               new XAttribute("videoID", pv.pVideoID),
                                                                               new XAttribute("isPrimary", pv.isPrimary),
                                                                               new XAttribute("type", pv.videoType.name),
                                                                               new XAttribute("icon", pv.videoType.icon),
                                                                               new XAttribute("typeID", pv.vTypeID)
                                                                           )).ToList<XElement>()),
                                                   new XElement("Packages", (from pp in p.PartPackages
                                                                             select new XElement("Package",
                                                                                 new XAttribute("height", pp.height),
                                                                                 new XAttribute("length", pp.length),
                                                                                 new XAttribute("width", pp.width),
                                                                                 new XAttribute("weight", pp.weight),
                                                                                 new XAttribute("quantity", pp.quantity),
                                                                                 new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                 new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                 new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                 new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                 new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                 new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                             )).ToList<XElement>()),
                                                  new XElement("Pricing", (from pr in db.Prices
                                                                           where pr.partID.Equals(p.partID)
                                                                           orderby pr.priceType
                                                                           select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                  new XAttribute("installTime", (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0),
                                                   new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                                                   new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            }

            xml.Add(parts);
            return xml;
        }

        public static string GetPartsByVehicleIDJSON(int vehicleID = 0, List<int> statuses = null, bool integrated = false, int customerID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where v.vehicleID.Equals(vehicleID) && statuslist.Contains(p.status)
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             vehicleID = v.vehicleID,
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  where vpa.vPartID.Equals(vp.vPartID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             installTime = Convert.ToInt32(vp.installTime),
                             drilling = vp.drilling,
                             exposed = vp.exposed,
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where v.vehicleID.Equals(vehicleID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                         select new APIPart {
                             partID = p.partID,
                             custPartID = ci.custPartID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             vehicleID = v.vehicleID,
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  where vpa.vPartID.Equals(vp.vPartID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             installTime = Convert.ToInt32(vp.installTime),
                             drilling = vp.drilling,
                             exposed = vp.exposed,
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetPartsByVehicleIDXML(int vehicleID = 0, List<int> statuses = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join vp in db.VehicleParts on p.partID equals vp.partID
                                               join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where v.vehicleID.Equals(vehicleID) && statuslist.Contains(p.status)
                                               select new XElement("Part",
                                                      new XAttribute("partID", p.partID),
                                                      new XAttribute("status", p.status),
                                                      new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                      new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                      new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                      new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                      new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                      new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                      new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                      new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                      new XAttribute("vehicleID", v.vehicleID),
                                                      new XElement("Attributes", (from pa in db.PartAttributes
                                                                                  where pa.partID.Equals(p.partID)
                                                                                  orderby pa.sort
                                                                                  select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                      new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                          where vpa.vPartID.Equals(vp.vPartID)
                                                                                          orderby vpa.sort
                                                                                          select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                       new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                       new XElement("Videos", (from pv in p.PartVideos
                                                                               orderby pv.vTypeID
                                                                               select new XElement("Video", pv.video,
                                                                                   new XAttribute("videoID", pv.pVideoID),
                                                                                   new XAttribute("isPrimary", pv.isPrimary),
                                                                                   new XAttribute("type", pv.videoType.name),
                                                                                   new XAttribute("icon", pv.videoType.icon),
                                                                                   new XAttribute("typeID", pv.vTypeID)
                                                                               )).ToList<XElement>()),
                                                       new XElement("Packages", (from pp in p.PartPackages
                                                                                 select new XElement("Package",
                                                                                     new XAttribute("height", pp.height),
                                                                                     new XAttribute("length", pp.length),
                                                                                     new XAttribute("width", pp.width),
                                                                                     new XAttribute("weight", pp.weight),
                                                                                     new XAttribute("quantity", pp.quantity),
                                                                                     new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                     new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                     new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                     new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                     new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                     new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                 )).ToList<XElement>()),
                                                      new XElement("Pricing", (from pr in db.Prices
                                                                               where pr.partID.Equals(p.partID)
                                                                               orderby pr.priceType
                                                                               select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                      new XAttribute("installTime", (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0),
                                                   new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                                                   new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join vp in db.VehicleParts on p.partID equals vp.partID
                                               join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                               join ci in db.CartIntegrations on p.partID equals ci.partID
                                               join cu in db.Customers on ci.custID equals cu.customerID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where v.vehicleID.Equals(vehicleID) && statuslist.Contains(p.status) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                                               select new XElement("Part",
                                                      new XAttribute("partID", p.partID),
                                                      new XAttribute("custPartID", ci.custPartID),
                                                      new XAttribute("status", p.status),
                                                      new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                      new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                      new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                      new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                      new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                      new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                      new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                      new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                      new XAttribute("vehicleID", v.vehicleID),
                                                      new XElement("Attributes", (from pa in db.PartAttributes
                                                                                  where pa.partID.Equals(p.partID)
                                                                                  orderby pa.sort
                                                                                  select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                      new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                         where vpa.vPartID.Equals(vp.vPartID)
                                                                                         orderby vpa.sort
                                                                                         select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                       new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                       new XElement("Videos", (from pv in p.PartVideos
                                                                               orderby pv.vTypeID
                                                                               select new XElement("Video", pv.video,
                                                                                   new XAttribute("videoID", pv.pVideoID),
                                                                                   new XAttribute("isPrimary", pv.isPrimary),
                                                                                   new XAttribute("type", pv.videoType.name),
                                                                                   new XAttribute("icon", pv.videoType.icon),
                                                                                   new XAttribute("typeID", pv.vTypeID)
                                                                               )).ToList<XElement>()),
                                                       new XElement("Packages", (from pp in p.PartPackages
                                                                                 select new XElement("Package",
                                                                                     new XAttribute("height", pp.height),
                                                                                     new XAttribute("length", pp.length),
                                                                                     new XAttribute("width", pp.width),
                                                                                     new XAttribute("weight", pp.weight),
                                                                                     new XAttribute("quantity", pp.quantity),
                                                                                     new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                     new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                     new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                     new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                     new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                     new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                 )).ToList<XElement>()),
                                                      new XElement("Pricing", (from pr in db.Prices
                                                                               where pr.partID.Equals(p.partID)
                                                                               orderby pr.priceType
                                                                               select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                      new XAttribute("installTime", (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0),
                                                       new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                                                       new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                                                      new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                        new XElement("Images",
                                                                            (from pin in db.PartImages
                                                                             where pin.partID.Equals(p.partID)
                                                                             group pin by pin.sort into pi
                                                                             orderby pi.Key
                                                                             select new XElement("Index",
                                                                                                 new XAttribute("name", pi.Key.ToString()),
                                                                                                 (from pis in db.PartImages
                                                                                                  join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                                  where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                                  orderby pis.sort, pis.sizeID
                                                                                                  select new XElement(pig.size,
                                                                                                                      new XAttribute("imageID", pis.imageID),
                                                                                                                      new XAttribute("path", pis.path),
                                                                                                                      new XAttribute("height", pis.height),
                                                                                                                      new XAttribute("width", pis.width)
                                                                                                  )).ToList<XElement>()
                                                                         )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            }
            xml.Add(parts);
            return xml;
        }

        public static string GetPartsByVehicleIDWithFilterJSON(int vehicleID = 0, string catName = "", List<int> statuses = null, bool integrated = false, int customerID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join cp in db.CatParts on p.partID equals cp.partID
                         join cat in db.Categories on cp.catID equals cat.catID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where v.vehicleID.Equals(vehicleID) && cat.catTitle.Equals(catName) && statuslist.Contains(p.status)
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             vehicleID = v.vehicleID,
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  where vpa.vPartID.Equals(vp.vPartID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             installTime = Convert.ToInt32(vp.installTime),
                             drilling = vp.drilling,
                             exposed = vp.exposed,
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join cp in db.CatParts on p.partID equals cp.partID
                         join cat in db.Categories on cp.catID equals cat.catID
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where v.vehicleID.Equals(vehicleID) && cat.catTitle.Equals(catName) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                         select new APIPart {
                             partID = p.partID,
                             custPartID = ci.custPartID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             vehicleID = v.vehicleID,
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  where vpa.vPartID.Equals(vp.vPartID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             installTime = Convert.ToInt32(vp.installTime),
                             drilling = vp.drilling,
                             exposed = vp.exposed,
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetPartsByVehicleIDWithFilterXML(int vehicleID = 0, string catName = "", List<int> statuses = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join vp in db.VehicleParts on p.partID equals vp.partID
                                               join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                               join cp in db.CatParts on p.partID equals cp.partID
                                               join cat in db.Categories on cp.catID equals cat.catID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where v.vehicleID.Equals(vehicleID) && cat.catTitle.Equals(catName) && statuslist.Contains(p.status)
                                               select new XElement("Part",
                                                  new XAttribute("partID", p.partID),
                                                  new XAttribute("status", p.status),
                                                  new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                  new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                  new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                  new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                  new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                  new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                  new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                  new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                  new XAttribute("vehicleID", v.vehicleID),
                                                  new XElement("Attributes", (from pa in db.PartAttributes
                                                                              where pa.partID.Equals(p.partID)
                                                                              orderby pa.sort
                                                                              select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                  new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                     where vpa.vPartID.Equals(vp.vPartID)
                                                                                     orderby vpa.sort
                                                                                     select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from pv in p.PartVideos
                                                                           orderby pv.vTypeID
                                                                           select new XElement("Video", pv.video,
                                                                               new XAttribute("videoID", pv.pVideoID),
                                                                               new XAttribute("isPrimary", pv.isPrimary),
                                                                               new XAttribute("type", pv.videoType.name),
                                                                               new XAttribute("icon", pv.videoType.icon),
                                                                               new XAttribute("typeID", pv.vTypeID)
                                                                           )).ToList<XElement>()),
                                                  new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                  new XElement("Pricing", (from pr in db.Prices
                                                                           where pr.partID.Equals(p.partID)
                                                                           orderby pr.priceType
                                                                           select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                  new XAttribute("installTime", (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0),
                                                   new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                                                   new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                                                  new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join vp in db.VehicleParts on p.partID equals vp.partID
                                               join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                               join cp in db.CatParts on p.partID equals cp.partID
                                               join cat in db.Categories on cp.catID equals cat.catID
                                               join ci in db.CartIntegrations on p.partID equals ci.partID
                                               join cu in db.Customers on ci.custID equals cu.customerID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where v.vehicleID.Equals(vehicleID) && cat.catTitle.Equals(catName) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                                               select new XElement("Part",
                                                  new XAttribute("partID", p.partID),
                                                  new XAttribute("custPartID", ci.custPartID),
                                                  new XAttribute("status", p.status),
                                                  new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                  new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                  new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                  new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                  new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                  new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                  new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                  new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                  new XAttribute("vehicleID", v.vehicleID),
                                                  new XElement("Attributes", (from pa in db.PartAttributes
                                                                              where pa.partID.Equals(p.partID)
                                                                              orderby pa.sort
                                                                              select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                  new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                     where vpa.vPartID.Equals(vp.vPartID)
                                                                                     orderby vpa.sort
                                                                                     select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from pv in p.PartVideos
                                                                           orderby pv.vTypeID
                                                                           select new XElement("Video", pv.video,
                                                                               new XAttribute("videoID", pv.pVideoID),
                                                                               new XAttribute("isPrimary", pv.isPrimary),
                                                                               new XAttribute("type", pv.videoType.name),
                                                                               new XAttribute("icon", pv.videoType.icon),
                                                                               new XAttribute("typeID", pv.vTypeID)
                                                                           )).ToList<XElement>()),
                                                  new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                  new XElement("Pricing", (from pr in db.Prices
                                                                           where pr.partID.Equals(p.partID)
                                                                           orderby pr.priceType
                                                                           select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                  new XAttribute("installTime", (vp.installTime != null) ? Convert.ToInt32(vp.installTime) : 0),
                                                   new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                                                   new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                                                  new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            }
            xml.Add(parts);
            return xml;
        }

        public static string GetPartsByListWithVehicleIDJSON(int vehicleID = 0, List<int> partids = null, bool integrated = false, int customerID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where partids.Contains(p.partID)
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = p.RelatedParts.Count,
                             attributes = (from pa in p.PartAttributes
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                  where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in p.Prices
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in p.Reviews
                                        where r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in p.Reviews
                                              where r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in p.PartImages
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = pi.PartImageSize.size,
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where partids.Contains(p.partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                         select new APIPart {
                             partID = p.partID,
                             custPartID = ci.custPartID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = p.RelatedParts.Count,
                             attributes = (from pa in p.PartAttributes
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                  where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in p.Prices
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in p.Reviews
                                        where r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in p.Reviews
                                              where r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in p.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = pi.PartImageSize.size,
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetPartsByListWithVehicleIDXML(int vehicleID = 0, List<int> partids = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where partids.Contains(p.partID)
                                               select new XElement("Part",
                                                  new XAttribute("partID", p.partID),
                                                  new XAttribute("status", p.status),
                                                  new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                  new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                  new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                  new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                  new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                  new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                  new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                  new XAttribute("relatedCount", p.RelatedParts.Count),
                                                  new XElement("Attributes", (from pa in p.PartAttributes
                                                                              orderby pa.sort
                                                                              select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                  new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                     join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                                                     where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                                                     orderby vpa.sort
                                                                                     select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                  new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                  new XElement("Pricing", (from pr in p.Prices
                                                                           orderby pr.priceType
                                                                           select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                  new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in p.Reviews
                                                                                         where r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in p.Reviews
                                                                             where r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join ci in db.CartIntegrations on p.partID equals ci.partID
                                               join cu in db.Customers on ci.custID equals cu.customerID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where partids.Contains(p.partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                                               select new XElement("Part",
                                                  new XAttribute("partID", p.partID),
                                                  new XAttribute("custPartID", ci.custPartID),
                                                  new XAttribute("status", p.status),
                                                  new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                  new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                  new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                  new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                  new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                  new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                  new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                  new XAttribute("relatedCount", p.RelatedParts.Count),
                                                  new XElement("Attributes", (from pa in p.PartAttributes
                                                                              orderby pa.sort
                                                                              select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                  new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                     join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                                                     where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                                                     orderby vpa.sort
                                                                                     select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                  new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                  new XElement("Pricing", (from pr in p.Prices
                                                                           orderby pr.priceType
                                                                           select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                  new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in p.Reviews
                                                                                         where r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in p.Reviews
                                                                             where r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            }
            xml.Add(parts);
            return xml;
        }

        public static string GetPartsByListWithFilterJSON(double year = 0, string make = "", string model = "", string style = "", List<int> partids = null, bool integrated = false, int customerID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where partids.Contains(p.partID)
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = p.RelatedParts.Count,
                             attributes = (from pa in p.PartAttributes
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                  join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                  join y in db.Years on v.yearID equals y.yearID
                                                  join ma in db.Makes on v.makeID equals ma.makeID
                                                  join mo in db.Models on v.modelID equals mo.modelID
                                                  join s in db.Styles on v.styleID equals s.styleID
                                                  where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vp.partID.Equals(p.partID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in p.Prices
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in p.Reviews
                                        where r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in p.Reviews
                                              where r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in p.PartImages
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = pi.PartImageSize.size,
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where partids.Contains(p.partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                         select new APIPart {
                             partID = p.partID,
                             custPartID = ci.custPartID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = p.RelatedParts.Count,
                             attributes = (from pa in p.PartAttributes
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                  join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                  join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                  join y in db.Years on v.yearID equals y.yearID
                                                  join ma in db.Makes on v.makeID equals ma.makeID
                                                  join mo in db.Models on v.modelID equals mo.modelID
                                                  join s in db.Styles on v.styleID equals s.styleID
                                                  where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vp.partID.Equals(p.partID)
                                                  orderby vpa.sort
                                                  select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in p.Prices
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in p.Reviews
                                        where r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in p.Reviews
                                              where r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in p.PartImages
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = pi.PartImageSize.size,
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetPartsByListWithFilterXML(double year = 0, string make = "", string model = "", string style = "", List<int> partids = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where partids.Contains(p.partID)
                                               select new XElement("Part",
                                                  new XAttribute("partID", p.partID),
                                                  new XAttribute("status", p.status),
                                                  new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                  new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                  new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                  new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                  new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                  new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                  new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                  new XAttribute("relatedCount", p.RelatedParts.Count),
                                                  new XElement("Attributes", (from pa in p.PartAttributes
                                                                              orderby pa.sort
                                                                              select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                  new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                     join vp in db.VehicleParts on p.partID equals vp.partID
                                                                                     join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                                                     join y in db.Years on v.yearID equals y.yearID
                                                                                     join ma in db.Makes on v.makeID equals ma.makeID
                                                                                     join mo in db.Models on v.modelID equals mo.modelID
                                                                                     join s in db.Styles on v.styleID equals s.styleID
                                                                                     where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vpa.vPartID.Equals(vp.vPartID)
                                                                                     orderby vpa.sort
                                                                                     select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                  new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                  new XElement("Pricing", (from pr in p.Prices
                                                                           orderby pr.priceType
                                                                           select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in p.Reviews
                                                                                         where r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in p.Reviews
                                                                             where r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join ci in db.CartIntegrations on p.partID equals ci.partID
                                               join cu in db.Customers on ci.custID equals cu.customerID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where partids.Contains(p.partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                                               select new XElement("Part",
                                                  new XAttribute("partID", p.partID),
                                                  new XAttribute("custPartID", ci.custPartID),
                                                  new XAttribute("status", p.status),
                                                  new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                  new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                  new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                  new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                  new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                  new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                  new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                  new XAttribute("relatedCount", p.RelatedParts.Count),
                                                  new XElement("Attributes", (from pa in p.PartAttributes
                                                                              orderby pa.sort
                                                                              select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                  new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                     join vp in db.VehicleParts on p.partID equals vp.partID
                                                                                     join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                                                     join y in db.Years on v.yearID equals y.yearID
                                                                                     join ma in db.Makes on v.makeID equals ma.makeID
                                                                                     join mo in db.Models on v.modelID equals mo.modelID
                                                                                     join s in db.Styles on v.styleID equals s.styleID
                                                                                     where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vpa.vPartID.Equals(vp.vPartID)
                                                                                     orderby vpa.sort
                                                                                     select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                  new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                  new XElement("Pricing", (from pr in p.Prices
                                                                           orderby pr.priceType
                                                                           select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in p.Reviews
                                                                                         where r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in p.Reviews
                                                                             where r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            }

            xml.Add(parts);
            return xml;
        }

        public static string GetPartsByListJSON(List<int> partids = null, bool integrated = false, int customerID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where partids.Contains(p.partID)
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = p.RelatedParts.Count,
                             attributes = (from pa in p.PartAttributes
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in p.Reviews
                                        where r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in p.Reviews
                                              where r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in p.PartImages
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = pi.PartImageSize.size,
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where partids.Contains(p.partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                         select new APIPart {
                             partID = p.partID,
                             custPartID = ci.custPartID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = p.RelatedParts.Count,
                             attributes = (from pa in p.PartAttributes
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in p.Prices
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in p.Reviews
                                        where r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in p.Reviews
                                              where r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in p.PartImages
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = pi.PartImageSize.size,
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetPartsByListXML(List<int> partids = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");

            //List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where partids.Contains(p.partID)
                                               select new XElement("Part",
                                                   new XAttribute("partID", p.partID),
                                                   new XAttribute("status", p.status),
                                                   new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                   new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                   new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                   new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                   new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                   new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                   new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                   new XAttribute("relatedCount", p.RelatedParts.Count),
                                                   new XElement("Attributes", (from pa in p.PartAttributes
                                                                               orderby pa.sort
                                                                               select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                  new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                   new XElement("Pricing", (from pr in p.Prices
                                                                            orderby pr.priceType
                                                                            select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in p.Reviews
                                                                                         where r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in p.Reviews
                                                                             where r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                             )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Parts", (from p in db.Parts
                                               join ci in db.CartIntegrations on p.partID equals ci.partID
                                               join cu in db.Customers on ci.custID equals cu.customerID
                                               join c in db.Classes on p.classID equals c.classID into ClassTemp
                                               from c in ClassTemp.DefaultIfEmpty()
                                               where partids.Contains(p.partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                                               select new XElement("Part",
                                                   new XAttribute("partID", p.partID),
                                                   new XAttribute("custPartID", ci.custPartID),
                                                   new XAttribute("status", p.status),
                                                   new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                   new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                   new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                   new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                   new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                   new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                   new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                   new XAttribute("relatedCount", p.RelatedParts.Count),
                                                   new XElement("Attributes", (from pa in p.PartAttributes
                                                                               orderby pa.sort
                                                                               select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                  new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                   new XElement("Pricing", (from pr in p.Prices
                                                                            orderby pr.priceType
                                                                            select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in p.Reviews
                                                                                         where r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in p.Reviews
                                                                             where r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
                #endregion
            }

            xml.Add(parts);
            return xml;
        }

        public static string GetPartJSON(int partID = 0, bool integrated = false, int customerID = 0) {
            APIPart part = new APIPart();
            CurtDevDataContext db = new CurtDevDataContext();

            if (!integrated) {
                #region Not Integrated
                part = (from p in db.Parts
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID)
                        select new APIPart {
                            partID = p.partID,
                            status = p.status,
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = p.RelatedParts.Count,
                            attributes = (from pa in p.PartAttributes
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in p.Prices
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in p.Reviews
                                       where r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in p.Reviews
                                             where r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in p.PartImages
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = pi.PartImageSize.size,
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).FirstOrDefault<APIPart>();
                #endregion
            } else {
                #region Integrated
                part = (from p in db.Parts
                        join ci in db.CartIntegrations on p.partID equals ci.partID
                        join cu in db.Customers on ci.custID equals cu.customerID
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                        select new APIPart {
                            partID = p.partID,
                            custPartID = ci.custPartID,
                            status = p.status,
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                            attributes = (from pa in db.PartAttributes
                                          where pa.partID.Equals(p.partID)
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in db.Prices
                                       where pr.partID.Equals(p.partID)
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in db.Reviews
                                       where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in db.Reviews
                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in db.PartImages
                                      where pi.partID.Equals(partID)
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).FirstOrDefault<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(part);
        }

        public static string GetPartByVehicleIDJSON(int partID = 0, int vehicleID = 0, bool integrated = false, int customerID = 0) {
            APIPart part = new APIPart();
            CurtDevDataContext db = new CurtDevDataContext();

            if (!integrated) {
                #region Not Integrated
                part = (from p in db.Parts
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID)
                        select new APIPart {
                            partID = p.partID,
                            status = p.status,
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                            installTime = db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.installTime).FirstOrDefault() ?? 0,
                            vehicleID = db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.vehicleID).FirstOrDefault(),
                            exposed = db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.exposed).FirstOrDefault(),
                            drilling = db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.drilling).FirstOrDefault(),
                            attributes = (from pa in db.PartAttributes
                                          where pa.partID.Equals(p.partID)
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                 join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                 where vpa.vPartID.Equals(vp.vPartID) && vp.partID.Equals(p.partID) && vp.vehicleID.Equals(vehicleID)
                                                 orderby vpa.sort
                                                 select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in db.Prices
                                       where pr.partID.Equals(p.partID)
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in db.Reviews
                                       where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in db.Reviews
                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in db.PartImages
                                      where pi.partID.Equals(partID)
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).FirstOrDefault<APIPart>();
                #endregion
            } else {
                #region Not Integrated
                part = (from p in db.Parts
                        join vp in db.VehicleParts on p.partID equals vp.partID
                        join ci in db.CartIntegrations on p.partID equals ci.partID
                        join cu in db.Customers on ci.custID equals cu.customerID
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                        select new APIPart {
                            partID = p.partID,
                            custPartID = ci.custPartID,
                            status = p.status,
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                            installTime = db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.installTime).FirstOrDefault() ?? 0,
                            vehicleID = db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.vehicleID).FirstOrDefault(),
                            exposed = db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.exposed).FirstOrDefault(),
                            drilling = db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.drilling).FirstOrDefault(),
                            attributes = (from pa in db.PartAttributes
                                          where pa.partID.Equals(p.partID)
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                 where vpa.vPartID.Equals(vp.vPartID)
                                                 orderby vpa.sort
                                                 select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in db.Prices
                                       where pr.partID.Equals(p.partID)
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in db.Reviews
                                       where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in db.Reviews
                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in db.PartImages
                                      where pi.partID.Equals(partID)
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).FirstOrDefault<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(part);
        }

        public static string GetPartWithFilterJSON(int partID = 0, double year = 0, string make = "", string model = "", string style = "", bool integrated = false, int customerID = 0) {
            APIPart part = new APIPart();
            CurtDevDataContext db = new CurtDevDataContext();

            if (!integrated) {
                #region Not Integrated
                part = (from p in db.Parts
                        join vp in db.VehicleParts on p.partID equals vp.partID
                        join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                        join y in db.Years on v.yearID equals y.yearID
                        join ma in db.Makes on v.makeID equals ma.makeID
                        join mo in db.Models on v.modelID equals mo.modelID
                        join s in db.Styles on v.styleID equals s.styleID
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID) && y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style)
                        select new APIPart {
                            partID = p.partID,
                            status = p.status,
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                            installTime = (vp.installTime != null) ? (int)vp.installTime : 0,
                            vehicleID = (vp.vehicleID != null) ? (int)vp.vehicleID : 0,
                            exposed = vp.exposed ?? "",
                            drilling = vp.drilling ?? "",
                            attributes = (from pa in db.PartAttributes
                                          where pa.partID.Equals(p.partID)
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                 where vpa.vPartID.Equals(vp.vPartID)
                                                 orderby vpa.sort
                                                 select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in db.Prices
                                       where pr.partID.Equals(p.partID)
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in db.Reviews
                                       where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in db.Reviews
                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in db.PartImages
                                      where pi.partID.Equals(partID)
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).FirstOrDefault<APIPart>();
                #endregion
            } else {
                #region Not Integrated
                part = (from p in db.Parts
                        join vp in db.VehicleParts on p.partID equals vp.partID
                        join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                        join y in db.Years on v.yearID equals y.yearID
                        join ma in db.Makes on v.makeID equals ma.makeID
                        join mo in db.Models on v.modelID equals mo.modelID
                        join s in db.Styles on v.styleID equals s.styleID
                        join ci in db.CartIntegrations on p.partID equals ci.partID
                        join cu in db.Customers on ci.custID equals cu.customerID
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID) && cu.customerID.Equals(customerID) && y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && ci.custPartID > 0
                        select new APIPart {
                            partID = p.partID,
                            custPartID = ci.custPartID,
                            status = p.status,
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                            installTime = (vp.installTime != null) ? (int)vp.installTime : 0,
                            vehicleID = (vp.vehicleID != null) ? (int)vp.vehicleID : 0,
                            exposed = vp.exposed ?? "",
                            drilling = vp.drilling ?? "",
                            attributes = (from pa in db.PartAttributes
                                          where pa.partID.Equals(p.partID)
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                 where vpa.vPartID.Equals(vp.vPartID)
                                                 orderby vpa.sort
                                                 select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in db.Prices
                                       where pr.partID.Equals(p.partID)
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in db.Reviews
                                       where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in db.Reviews
                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in db.PartImages
                                      where pi.partID.Equals(partID)
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).FirstOrDefault<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(part);
        }

        public static XDocument GetPartXML(int partID = 0, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();

            XElement part = new XElement("Part");
            CurtDevDataContext db = new CurtDevDataContext();
            if (!integrated) {
                #region Not Integrated
                part = (from p in db.Parts
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID)
                        select new XElement("Part",
                            new XAttribute("partID", p.partID),
                            new XAttribute("status", p.status),
                            new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                            new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                            new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                            new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                            new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                            new XAttribute("pClass", (c != null) ? c.class1 : ""),
                            new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                            new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                            new XElement("Attributes", (from pa in db.PartAttributes
                                                        where pa.partID.Equals(p.partID)
                                                        orderby pa.sort
                                                        select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                            new XElement("Content", (from co in p.ContentBridges
                                                    orderby co.Content.ContentType.type, co.contentID
                                                    select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                            new XElement("Videos", (from vp in p.PartVideos
                                                    orderby vp.vTypeID
                                                    select new XElement("Video", vp.video,
                                                        new XAttribute("videoID", vp.pVideoID),
                                                        new XAttribute("isPrimary", vp.isPrimary),
                                                        new XAttribute("type", vp.videoType.name),
                                                        new XAttribute("icon", vp.videoType.icon ?? ""),
                                                        new XAttribute("typeID", vp.vTypeID)
                                                    )).ToList<XElement>()),
                            new XElement("Packages", (from pp in p.PartPackages
                                                    select new XElement("Package",
                                                        new XAttribute("height", pp.height),
                                                        new XAttribute("length", pp.length),
                                                        new XAttribute("width", pp.width),
                                                        new XAttribute("weight", pp.weight),
                                                        new XAttribute("quantity", pp.quantity),
                                                        new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                        new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                        new XAttribute("weightUnit", pp.weightUnit.code),
                                                        new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                        new XAttribute("packageUnit", pp.packageUnit.code),
                                                        new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                    )).ToList<XElement>()),
                            new XElement("Pricing", (from pr in db.Prices
                                                     where pr.partID.Equals(p.partID)
                                                     orderby pr.priceType
                                                     select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                            new XElement("Reviews",
                                new XAttribute("averageReview", (from r in db.Reviews
                                                                 where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                 select (double?)r.rating).Average() ?? 0.0),
                                                    (from r in db.Reviews
                                                     where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                     orderby r.createdDate descending
                                                     select new XElement("Review",
                                                         new XAttribute("reviewID", r.reviewID),
                                                         new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                         new XElement("rating", r.rating),
                                                         new XElement("name", r.name),
                                                         new XElement("email", r.email),
                                                         new XElement("subject", r.subject),
                                                         new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                            new XElement("Images",
                                    (from pin in db.PartImages
				                        where pin.partID.Equals(p.partID)
				                        group pin by pin.sort into pi   
				                        orderby pi.Key
				                        select new XElement("Index",
                                                            new XAttribute("name", pi.Key.ToString()),
										                    (from pis in db.PartImages                                    
										                    join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID                                    
										                    where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
										                    orderby pis.sort, pis.sizeID
										                    select new XElement(pig.size,
																                new XAttribute("imageID", pis.imageID),
																                new XAttribute("path", pis.path),
																                new XAttribute("height", pis.height),
																                new XAttribute("width", pis.width)
            												)).ToList<XElement>()
									)).ToList<XElement>())
                        )).FirstOrDefault<XElement>();
                #endregion
            } else {
                #region Integrated
                part = (from p in db.Parts
                        join ci in db.CartIntegrations on p.partID equals ci.partID
                        join cu in db.Customers on ci.custID equals cu.customerID
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                        select new XElement("Part",
                            new XAttribute("partID", p.partID),
                            new XAttribute("custPartID", ci.custPartID),
                            new XAttribute("status", p.status),
                            new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                            new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                            new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                            new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                            new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                            new XAttribute("pClass", (c != null) ? c.class1 : ""),
                            new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                            new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                            new XElement("Attributes", (from pa in db.PartAttributes
                                                        where pa.partID.Equals(p.partID)
                                                        orderby pa.sort
                                                        select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                            new XElement("Content", (from co in p.ContentBridges
                                                    orderby co.Content.ContentType.type, co.contentID
                                                    select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                            new XElement("Videos", (from vp in p.PartVideos
                                                    orderby vp.vTypeID
                                                    select new XElement("Video", vp.video,
                                                        new XAttribute("videoID", vp.pVideoID),
                                                        new XAttribute("isPrimary", vp.isPrimary),
                                                        new XAttribute("type", vp.videoType.name),
                                                        new XAttribute("icon", vp.videoType.icon ?? ""),
                                                        new XAttribute("typeID", vp.vTypeID)
                                                    )).ToList<XElement>()),
                            new XElement("Packages", (from pp in p.PartPackages
                                                      select new XElement("Package",
                                                          new XAttribute("height", pp.height),
                                                          new XAttribute("length", pp.length),
                                                          new XAttribute("width", pp.width),
                                                          new XAttribute("weight", pp.weight),
                                                          new XAttribute("quantity", pp.quantity),
                                                          new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                          new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                          new XAttribute("weightUnit", pp.weightUnit.code),
                                                          new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                          new XAttribute("packageUnit", pp.packageUnit.code),
                                                          new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                      )).ToList<XElement>()),
                            new XElement("Pricing", (from pr in db.Prices
                                                     where pr.partID.Equals(p.partID)
                                                     orderby pr.priceType
                                                     select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                            new XElement("Reviews",
                                new XAttribute("averageReview", (from r in db.Reviews
                                                                 where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                 select (double?)r.rating).Average() ?? 0.0),
                                                    (from r in db.Reviews
                                                     where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                     orderby r.createdDate descending
                                                     select new XElement("Review",
                                                         new XAttribute("reviewID", r.reviewID),
                                                         new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                         new XElement("rating", r.rating),
                                                         new XElement("name", r.name),
                                                         new XElement("email", r.email),
                                                         new XElement("subject", r.subject),
                                                         new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                            new XElement("Images",
                                                (from pin in db.PartImages
                                                 where pin.partID.Equals(p.partID)
                                                 group pin by pin.sort into pi
                                                 orderby pi.Key
                                                 select new XElement("Index",
                                                                     new XAttribute("name", pi.Key.ToString()),
                                                                     (from pis in db.PartImages
                                                                      join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                      where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                      orderby pis.sort, pis.sizeID
                                                                      select new XElement(pig.size,
                                                                                          new XAttribute("imageID", pis.imageID),
                                                                                          new XAttribute("path", pis.path),
                                                                                          new XAttribute("height", pis.height),
                                                                                          new XAttribute("width", pis.width)
                                                                      )).ToList<XElement>()
                                             )).ToList<XElement>())
                        )).FirstOrDefault<XElement>();
                #endregion
            }
            if (part == null) {
                XElement part_xml = new XElement("Part", "No part found");
                xml.Add(part_xml);
            } else {
                xml.Add(part);
            }
            return xml;
        }

        public static XDocument GetPartByVehicleIDXML(int partID = 0, int vehicleID = 0, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            CurtDevDataContext db = new CurtDevDataContext();

            XElement part = new XElement("Parts");
            if (!integrated) {
                #region Not Integrated
                part = (from p in db.Parts
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID)
                        select new XElement("Part",
                                new XAttribute("partID", p.partID),
                                new XAttribute("status", p.status),
                                new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                new XAttribute("installTime", db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.installTime).FirstOrDefault() ?? 0),
                                new XAttribute("vehicleID", db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.vehicleID).FirstOrDefault().ToString() ?? "0"),
                                new XAttribute("drilling", db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.drilling).FirstOrDefault().ToString() ?? ""),
                                new XAttribute("exposed", db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.exposed).FirstOrDefault().ToString() ?? ""),
                                new XElement("Attributes", (from pa in db.PartAttributes
                                                            where pa.partID.Equals(p.partID)
                                                            orderby pa.sort
                                                            select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                   join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                                   where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                                   orderby vpa.sort
                                                                   select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                new XElement("Content", (from co in p.ContentBridges
                                                        orderby co.Content.ContentType.type, co.contentID
                                                        select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                new XElement("Videos", (from vp in p.PartVideos
                                                        orderby vp.vTypeID
                                                        select new XElement("Video", vp.video,
                                                            new XAttribute("videoID", vp.pVideoID),
                                                            new XAttribute("isPrimary", vp.isPrimary),
                                                            new XAttribute("type", vp.videoType.name),
                                                            new XAttribute("icon", vp.videoType.icon ?? ""),
                                                            new XAttribute("typeID", vp.vTypeID)
                                                        )).ToList<XElement>()),
                                new XElement("Packages", (from pp in p.PartPackages
                                                          select new XElement("Package",
                                                              new XAttribute("height", pp.height),
                                                              new XAttribute("length", pp.length),
                                                              new XAttribute("width", pp.width),
                                                              new XAttribute("weight", pp.weight),
                                                              new XAttribute("quantity", pp.quantity),
                                                              new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                              new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                              new XAttribute("weightUnit", pp.weightUnit.code),
                                                              new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                              new XAttribute("packageUnit", pp.packageUnit.code),
                                                              new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                          )).ToList<XElement>()),
                                new XElement("Pricing", (from pr in db.Prices
                                                         where pr.partID.Equals(p.partID)
                                                         orderby pr.priceType
                                                         select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                new XElement("Reviews",
                                    new XAttribute("averageReview", (from r in db.Reviews
                                                                     where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                     select (double?)r.rating).Average() ?? 0.0),
                                                        (from r in db.Reviews
                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                         orderby r.createdDate descending
                                                         select new XElement("Review",
                                                             new XAttribute("reviewID", r.reviewID),
                                                             new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                             new XElement("rating", r.rating),
                                                             new XElement("name", r.name),
                                                             new XElement("email", r.email),
                                                             new XElement("subject", r.subject),
                                                             new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                new XElement("Images",
                                                    (from pin in db.PartImages
                                                        where pin.partID.Equals(p.partID)
                                                        group pin by pin.sort into pi
                                                        orderby pi.Key
                                                        select new XElement("Index",
                                                                            new XAttribute("name", pi.Key.ToString()),
                                                                            (from pis in db.PartImages
                                                                            join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                            where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                            orderby pis.sort, pis.sizeID
                                                                            select new XElement(pig.size,
                                                                                                new XAttribute("imageID", pis.imageID),
                                                                                                new XAttribute("path", pis.path),
                                                                                                new XAttribute("height", pis.height),
                                                                                                new XAttribute("width", pis.width)
                                                                            )).ToList<XElement>()
                                                    )).ToList<XElement>())
                        )).FirstOrDefault<XElement>();

                #endregion
            } else {
                #region Integrated
                part = (from p in db.Parts
                        join ci in db.CartIntegrations on p.partID equals ci.partID
                        join cu in db.Customers on ci.custID equals cu.customerID
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID) && cu.customerID.Equals(customerID) && ci.custPartID > 0
                        select new XElement("Part",
                            new XAttribute("partID", p.partID),
                            new XAttribute("custPartID", ci.custPartID),
                            new XAttribute("status", p.status),
                            new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                            new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                            new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                            new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                            new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                            new XAttribute("pClass", (c != null) ? c.class1 : ""),
                            new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                            new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                            new XAttribute("installTime", db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.installTime).FirstOrDefault() ?? 0),
                            new XAttribute("vehicleID", db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.vehicleID).FirstOrDefault().ToString() ?? "0"),
                            new XAttribute("drilling", db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.drilling).FirstOrDefault().ToString() ?? ""),
                            new XAttribute("exposed", db.VehicleParts.Where(x => x.partID.Equals(p.partID)).Where(x => x.vehicleID.Equals(vehicleID)).Select(x => x.exposed).FirstOrDefault().ToString() ?? ""),
                            new XElement("Attributes", (from pa in db.PartAttributes
                                                        where pa.partID.Equals(p.partID)
                                                        orderby pa.sort
                                                        select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                            new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                               join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                               where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                               orderby vpa.sort
                                                               select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                            new XElement("Content", (from co in p.ContentBridges
                                                    orderby co.Content.ContentType.type, co.contentID
                                                    select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                            new XElement("Videos", (from vp in p.PartVideos
                                                    orderby vp.vTypeID
                                                    select new XElement("Video", vp.video,
                                                        new XAttribute("videoID", vp.pVideoID),
                                                        new XAttribute("isPrimary", vp.isPrimary),
                                                        new XAttribute("type", vp.videoType.name),
                                                        new XAttribute("icon", vp.videoType.icon ?? ""),
                                                        new XAttribute("typeID", vp.vTypeID)
                                                    )).ToList<XElement>()),
                            new XElement("Packages", (from pp in p.PartPackages
                                                        select new XElement("Package",
                                                            new XAttribute("height", pp.height),
                                                            new XAttribute("length", pp.length),
                                                            new XAttribute("width", pp.width),
                                                            new XAttribute("weight", pp.weight),
                                                            new XAttribute("quantity", pp.quantity),
                                                            new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                            new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                            new XAttribute("weightUnit", pp.weightUnit.code),
                                                            new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                            new XAttribute("packageUnit", pp.packageUnit.code),
                                                            new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                        )).ToList<XElement>()),
                            new XElement("Pricing", (from pr in db.Prices
                                                     where pr.partID.Equals(p.partID)
                                                     orderby pr.priceType
                                                     select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                            new XElement("Reviews",
                                new XAttribute("averageReview", (from r in db.Reviews
                                                                 where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                 select (double?)r.rating).Average() ?? 0.0),
                                                    (from r in db.Reviews
                                                     where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                     orderby r.createdDate descending
                                                     select new XElement("Review",
                                                         new XAttribute("reviewID", r.reviewID),
                                                         new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                         new XElement("rating", r.rating),
                                                         new XElement("name", r.name),
                                                         new XElement("email", r.email),
                                                         new XElement("subject", r.subject),
                                                         new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                            new XElement("Images",
                                                (from pin in db.PartImages
                                                    where pin.partID.Equals(p.partID)
                                                    group pin by pin.sort into pi
                                                    orderby pi.Key
                                                    select new XElement("Index",
                                                                        new XAttribute("name", pi.Key.ToString()),
                                                                        (from pis in db.PartImages
                                                                        join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                        where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                        orderby pis.sort, pis.sizeID
                                                                        select new XElement(pig.size,
                                                                                            new XAttribute("imageID", pis.imageID),
                                                                                            new XAttribute("path", pis.path),
                                                                                            new XAttribute("height", pis.height),
                                                                                            new XAttribute("width", pis.width)
                                                                        )).ToList<XElement>()
                                                )).ToList<XElement>())
                        )).FirstOrDefault<XElement>();
                #endregion
            }
            if (part == null) {
                XElement part_xml = new XElement("Part", "No part found");
                xml.Add(part_xml);
            } else {
                xml.Add(part);
            }
            return xml;
        }

        public static XDocument GetPartWithFilterXML(int partID = 0, double year = 0, string make = "", string model = "", string style = "", bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();

            XElement part = new XElement("Part");
            CurtDevDataContext db = new CurtDevDataContext();
            if (!integrated) {
                #region Not Integrated
                part = (from p in db.Parts
                        join vp in db.VehicleParts on p.partID equals vp.partID
                        join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                        join y in db.Years on v.yearID equals y.yearID
                        join ma in db.Makes on v.makeID equals ma.makeID
                        join mo in db.Models on v.modelID equals mo.modelID
                        join s in db.Styles on v.styleID equals s.styleID
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID) && y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style)
                        select new XElement("Part",
                            new XAttribute("partID", p.partID),
                            new XAttribute("status", p.status),
                            new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                            new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                            new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                            new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                            new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                            new XAttribute("pClass", (c != null) ? c.class1 : ""),
                            new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                            new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                            new XAttribute("installTime", (vp.installTime != null) ? vp.installTime.ToString() : ""),
                            new XAttribute("vehicleID", (vp.vehicleID != null) ? vp.vehicleID.ToString() : ""),
                            new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                            new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                            new XElement("Attributes", (from pa in db.PartAttributes
                                                        where pa.partID.Equals(p.partID)
                                                        orderby pa.sort
                                                        select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                            new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                               where vpa.vPartID.Equals(vp.vPartID) && vp.partID.Equals(p.partID)
                                                               orderby vpa.sort
                                                               select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                            new XElement("Content", (from co in p.ContentBridges
                                                    orderby co.Content.ContentType.type, co.contentID
                                                    select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                            new XElement("Videos", (from pv in p.PartVideos
                                                    orderby pv.vTypeID
                                                    select new XElement("Video", pv.video,
                                                        new XAttribute("videoID", pv.pVideoID),
                                                        new XAttribute("isPrimary", pv.isPrimary),
                                                        new XAttribute("type", pv.videoType.name),
                                                        new XAttribute("icon", pv.videoType.icon ?? ""),
                                                        new XAttribute("typeID", pv.vTypeID)
                                                    )).ToList<XElement>()),
                            new XElement("Packages", (from pp in p.PartPackages
                                                        select new XElement("Package",
                                                            new XAttribute("height", pp.height),
                                                            new XAttribute("length", pp.length),
                                                            new XAttribute("width", pp.width),
                                                            new XAttribute("weight", pp.weight),
                                                            new XAttribute("quantity", pp.quantity),
                                                            new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                            new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                            new XAttribute("weightUnit", pp.weightUnit.code),
                                                            new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                            new XAttribute("packageUnit", pp.packageUnit.code),
                                                            new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                        )).ToList<XElement>()),
                            new XElement("Pricing", (from pr in db.Prices
                                                     where pr.partID.Equals(p.partID)
                                                     orderby pr.priceType
                                                     select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                            new XElement("Reviews",
                                new XAttribute("averageReview", (from r in db.Reviews
                                                                 where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                 select (double?)r.rating).Average() ?? 0.0),
                                                    (from r in db.Reviews
                                                     where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                     orderby r.createdDate descending
                                                     select new XElement("Review",
                                                         new XAttribute("reviewID", r.reviewID),
                                                         new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                         new XElement("rating", r.rating),
                                                         new XElement("name", r.name),
                                                         new XElement("email", r.email),
                                                         new XElement("subject", r.subject),
                                                         new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                            new XElement("Images",
                                                (from pin in db.PartImages
                                                    where pin.partID.Equals(p.partID)
                                                    group pin by pin.sort into pi
                                                    orderby pi.Key
                                                    select new XElement("Index",
                                                                        new XAttribute("name", pi.Key.ToString()),
                                                                        (from pis in db.PartImages
                                                                        join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                        where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                        orderby pis.sort, pis.sizeID
                                                                        select new XElement(pig.size,
                                                                                            new XAttribute("imageID", pis.imageID),
                                                                                            new XAttribute("path", pis.path),
                                                                                            new XAttribute("height", pis.height),
                                                                                            new XAttribute("width", pis.width)
                                                                        )).ToList<XElement>()
                                                )).ToList<XElement>())
                        )).FirstOrDefault<XElement>();
                #endregion
            } else {
                #region Integrated
                part = (from p in db.Parts
                        join vp in db.VehicleParts on p.partID equals vp.partID
                        join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                        join y in db.Years on v.yearID equals y.yearID
                        join ma in db.Makes on v.makeID equals ma.makeID
                        join mo in db.Models on v.modelID equals mo.modelID
                        join s in db.Styles on v.styleID equals s.styleID
                        join ci in db.CartIntegrations on p.partID equals ci.partID
                        join cu in db.Customers on ci.custID equals cu.customerID
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        where p.partID.Equals(partID) && cu.customerID.Equals(customerID) && y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && ci.custPartID > 0
                        select new XElement("Part",
                            new XAttribute("partID", p.partID),
                            new XAttribute("custPartID", ci.custPartID),
                            new XAttribute("status", p.status),
                            new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                            new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                            new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                            new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                            new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                            new XAttribute("pClass", (c != null) ? c.class1 : ""),
                            new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                            new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                            new XAttribute("installTime", (vp.installTime != null) ? vp.installTime.ToString() : ""),
                            new XAttribute("vehicleID", (vp.vehicleID != null) ? vp.vehicleID.ToString() : ""),
                            new XAttribute("drilling", (vp.drilling != null) ? vp.drilling : ""),
                            new XAttribute("exposed", (vp.exposed != null) ? vp.exposed : ""),
                            new XElement("Attributes", (from pa in db.PartAttributes
                                                        where pa.partID.Equals(p.partID)
                                                        orderby pa.sort
                                                        select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                            new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                               where vpa.vPartID.Equals(vp.vPartID) && vp.partID.Equals(p.partID)
                                                               orderby vpa.sort
                                                               select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                            new XElement("Content", (from co in p.ContentBridges
                                                    orderby co.Content.ContentType.type, co.contentID
                                                    select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                            new XElement("Videos", (from pv in p.PartVideos
                                                    orderby pv.vTypeID
                                                    select new XElement("Video", pv.video,
                                                        new XAttribute("videoID", pv.pVideoID),
                                                        new XAttribute("isPrimary", pv.isPrimary),
                                                        new XAttribute("type", pv.videoType.name),
                                                        new XAttribute("icon", pv.videoType.icon),
                                                        new XAttribute("typeID", pv.vTypeID)
                                                    )).ToList<XElement>()),
                            new XElement("Packages", (from pp in p.PartPackages
                                                        select new XElement("Package",
                                                            new XAttribute("height", pp.height),
                                                            new XAttribute("length", pp.length),
                                                            new XAttribute("width", pp.width),
                                                            new XAttribute("weight", pp.weight),
                                                            new XAttribute("quantity", pp.quantity),
                                                            new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                            new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                            new XAttribute("weightUnit", pp.weightUnit.code),
                                                            new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                            new XAttribute("packageUnit", pp.packageUnit.code),
                                                            new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                        )).ToList<XElement>()),
                            new XElement("Pricing", (from pr in db.Prices
                                                     where pr.partID.Equals(p.partID)
                                                     orderby pr.priceType
                                                     select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                            new XElement("Reviews",
                                new XAttribute("averageReview", (from r in db.Reviews
                                                                 where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                 select (double?)r.rating).Average() ?? 0.0),
                                                    (from r in db.Reviews
                                                     where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                     orderby r.createdDate descending
                                                     select new XElement("Review",
                                                         new XAttribute("reviewID", r.reviewID),
                                                         new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                         new XElement("rating", r.rating),
                                                         new XElement("name", r.name),
                                                         new XElement("email", r.email),
                                                         new XElement("subject", r.subject),
                                                         new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                            new XElement("Images",
                                                (from pin in db.PartImages
                                                    where pin.partID.Equals(p.partID)
                                                    group pin by pin.sort into pi
                                                    orderby pi.Key
                                                    select new XElement("Index",
                                                                        new XAttribute("name", pi.Key.ToString()),
                                                                        (from pis in db.PartImages
                                                                        join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                        where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                        orderby pis.sort, pis.sizeID
                                                                        select new XElement(pig.size,
                                                                                            new XAttribute("imageID", pis.imageID),
                                                                                            new XAttribute("path", pis.path),
                                                                                            new XAttribute("height", pis.height),
                                                                                            new XAttribute("width", pis.width)
                                                                        )).ToList<XElement>()
                                                )).ToList<XElement>())
                        )).FirstOrDefault<XElement>();
                #endregion
            }
            if (part == null) {
                XElement part_xml = new XElement("Part", "No part found");
                xml.Add(part_xml);
            } else {
                xml.Add(part);
            }
            return xml;
        }

        public static string GetPartPricesJSON(int partID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            if (partID > 0) {
                APIPrice prices = new APIPrice();
                prices = (from p in db.Parts
                          where p.partID.Equals(partID)
                          select new APIPrice {
                              partID = p.partID,
                              prices = (from pr in p.Prices
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>()
                          }).FirstOrDefault<APIPrice>();

                return JsonConvert.SerializeObject(prices);
            } else {
                List<APIPrice> prices = new List<APIPrice>();
                prices = (from p in db.Parts
                          select new APIPrice {
                              partID = p.partID,
                              prices = (from pr in p.Prices
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>()
                          }).OrderBy(x => x.partID).ToList<APIPrice>();

                return JsonConvert.SerializeObject(prices);
            }
        }

        public static XDocument GetPartPricesXML(int partID = 0) {
            XDocument xml = new XDocument();
            CurtDevDataContext db = new CurtDevDataContext();
            if (partID > 0) {
                XElement part = new XElement("Part");
                part = (from p in db.Parts
                        where p.partID.Equals(partID)
                        select new XElement("Part",
                            new XElement("Pricing", (from pr in db.Prices
                                                     where pr.partID.Equals(p.partID)
                                                     orderby pr.priceType
                                                     select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>())
                        )).FirstOrDefault<XElement>();

                if (part == null) {
                    XElement part_xml = new XElement("Part", "No part found");
                    xml.Add(part_xml);
                } else {
                    xml.Add(part);
                }
            } else {
                XElement parts = new XElement("Parts");
                parts = new XElement("Parts", new List<XElement>
                            (from p in db.Parts
                             select new XElement("Part",
                                 new XAttribute("partID", p.partID),
                                 new XElement("Pricing", (from pr in p.Prices
                                                          orderby pr.priceType
                                                          select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>())
                             )).ToList<XElement>());

                if (parts == null) {
                    XElement part_xml = new XElement("Parts", "No parts found");
                    xml.Add(part_xml);
                } else {
                    xml.Add(parts);
                }
            }
            return xml;
        }

        /// <summary>
        /// This still needs some retooling (it's taking WAY too long!)
        /// </summary>
        /// <returns></returns>
        public static string GetAllPartsJSON(List<int> statuses = null) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            parts = (from p in db.Parts
                     join c in db.Classes on p.classID equals c.classID into ClassTemp
                     from c in ClassTemp.DefaultIfEmpty()
                     where statuslist.Contains(p.status)
                     select new APIPart {
                         partID = p.partID,
                         status = p.status,
                         dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                         dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                         shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                         oldPartNumber = p.oldPartNumber,
                         listPrice = String.Format("{0:C}", (from prices in db.Prices
                                                             where prices.partID.Equals(p.partID) && prices.priceType.Equals("List")
                                                             select prices.price1 != null ? prices.price1 : (decimal?)0).FirstOrDefault<decimal?>()),
                         pClass = (c != null) ? c.class1 : "",
                         priceCode = p.priceCode,
                         relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                         attributes = (from pa in db.PartAttributes
                                       where pa.partID.Equals(p.partID)
                                       orderby pa.sort
                                       select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                         content = (from co in p.ContentBridges
                                    orderby co.contentID
                                    select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                         videos = (from pv in p.PartVideos
                                   orderby pv.vTypeID
                                   select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                         packages = (from pp in p.PartPackages
                                     select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                         pricing = (from pr in db.Prices
                                    where pr.partID.Equals(p.partID)
                                    select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                         reviews = (from r in db.Reviews
                                    where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                    orderby r.createdDate descending
                                    select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                         averageReview = (from r in db.Reviews
                                          where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                          select (double?)r.rating).Average() ?? 0.0,
                         images = (from pi in db.PartImages
                                   where pi.partID.Equals(p.partID)
                                   select new APIImage {
                                       imageID = pi.imageID,
                                       sort = pi.sort,
                                       path = pi.path,
                                       height = pi.height,
                                       width = pi.width,
                                       size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                       partID = pi.partID
                                   }).OrderBy(x => x.sort).ToList<APIImage>()
                     }).ToList<APIPart>();
            return JsonConvert.SerializeObject(parts);
            /*StreamWriter outfile = new StreamWriter("C:\\inetpub\\ftproot\\CURT-SILVER01-M\\API\\hitches2.0.json");
            outfile.Write(json);
            return "JSON File saved as hitches2.0.json";*/
        }

        /// <summary>
        /// This still needs some retooling (it's taking WAY too long!)
        /// </summary>
        /// <returns></returns>
        public static XDocument GetAllPartsXML(List<int> statuses = null) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            parts = new XElement("Parts", new List<XElement>
                                    (from p in db.Parts
                                     join c in db.Classes on p.classID equals c.classID into ClassTemp
                                     from c in ClassTemp.DefaultIfEmpty()
                                     where statuslist.Contains(p.status)
                                     select new XElement("Part",
                                         new XAttribute("PartID", p.partID),
                                         new XAttribute("Status", p.status),
                                         new XAttribute("DateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                         new XAttribute("DateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                         new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                         new XAttribute("OldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                         new XAttribute("ListPrice", String.Format("{0:C}", (from prices in db.Prices
                                                                                             where prices.partID.Equals(p.partID) && prices.priceType.Equals("List")
                                                                                             select prices.price1 != null ? prices.price1 : (decimal?)0).FirstOrDefault<decimal?>())),
                                         new XAttribute("pClass", (c.class1 != null) ? c.class1 : ""),
                                         new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                         new XElement("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                         new XElement("Attributes", (from pa in db.PartAttributes
                                                                     where pa.partID.Equals(p.partID)
                                                                     orderby pa.sort
                                                                     select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                        new XElement("Content", (from co in p.ContentBridges
                                                                orderby co.Content.ContentType.type, co.contentID
                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                        new XElement("Videos", (from vp in p.PartVideos
                                                                orderby vp.vTypeID
                                                                select new XElement("Video", vp.video,
                                                                    new XAttribute("videoID", vp.pVideoID),
                                                                    new XAttribute("isPrimary", vp.isPrimary),
                                                                    new XAttribute("type", vp.videoType.name),
                                                                    new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                    new XAttribute("typeID", vp.vTypeID)
                                                                )).ToList<XElement>()),
                                        new XElement("Packages", (from pp in p.PartPackages
                                                                  select new XElement("Package",
                                                                      new XAttribute("height", pp.height),
                                                                      new XAttribute("length", pp.length),
                                                                      new XAttribute("width", pp.width),
                                                                      new XAttribute("weight", pp.weight),
                                                                      new XAttribute("quantity", pp.quantity),
                                                                      new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                      new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                      new XAttribute("weightUnit", pp.weightUnit.code),
                                                                      new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                      new XAttribute("packageUnit", pp.packageUnit.code),
                                                                      new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                  )).ToList<XElement>()),
                                         new XElement("Pricing", (from pr in db.Prices
                                                                  where pr.partID.Equals(p.partID)
                                                                  select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1)).ToList<XElement>())

                                     )));

            xml.Add(parts);
            //xml.Save("C:\\inetpub\\ftproot\\CURT-SILVER01-M\\API\\hitches2.0.xml");

            return xml;
        }

        public static string GetAllPartIDJSON(List<int> statuses = null) {
            List<int> statuslist = statuses ?? new List<int>();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<int> partIDs = new List<int>();

                partIDs = db.Parts.Where(x => statuslist.Contains(x.status)).Select(x => x.partID).Distinct().OrderBy(x => x).ToList<int>();

                return JsonConvert.SerializeObject(partIDs);
            } catch (Exception) {
                return "";
            }
        }

        public static string GetInstallSheetJSON(int partID = 0) {
            APIContent installsheet = new APIContent();
            CurtDevDataContext db = new CurtDevDataContext();
            try {
                installsheet = (from c in db.ContentBridges
                                where c.Content.ContentType.type.ToLower() == "installationsheet" && c.partID == partID
                                select new APIContent {
                                    type = c.Content.ContentType.type,
                                    content = c.Content.text,
                                    isHTML = false
                                }).First();
            } catch { };
            return JsonConvert.SerializeObject(installsheet);
        }

        public static XDocument GetInstallSheetXML(int partID = 0) {
            XDocument xml = new XDocument();
            XElement installsheet = new XElement("InstallSheet");
            CurtDevDataContext db = new CurtDevDataContext();

            try {
                installsheet = (from c in db.ContentBridges
                                where c.Content.ContentType.type.ToLower() == "installationsheet" && c.partID == partID
                                select new XElement("InstallSheet",
                                    new XAttribute("type", c.Content.ContentType.type),
                                    new XAttribute("path", c.Content.text)
                                )).First<XElement>();
            } catch { };
            xml.Add(installsheet);
            return xml;
        }

        public static XDocument GetAllPartIDXML(List<int> statuses = null) {
            List<int> statuslist = statuses ?? new List<int>();
            try {
                XDocument xml = new XDocument();
                CurtDevDataContext db = new CurtDevDataContext();
                XElement ids = new XElement("Parts", (from p in db.Parts
                                                      where statuslist.Contains(p.status)
                                                      select new XElement(
                                                          "partID", p.partID)).ToList<XElement>());
                xml.Add(ids);
                return xml;
            } catch (Exception) {
                return new XDocument();
            }
        }

        public static string GetRelatedJSON(int partID = 0, List<int> statuses = null, bool integrated = false, int customerID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         join rp in db.RelatedParts on p.partID equals rp.relatedID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where rp.partID.Equals(partID) && statuslist.Contains(p.status)
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join rp in db.RelatedParts on p.partID equals rp.relatedID
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where rp.partID.Equals(partID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                         select new APIPart {
                             partID = p.partID,
                             custPartID = ci.custPartID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(customerID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             packages = (from pp in p.PartPackages
                                         select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetRelatedXML(int partID = 0, List<int> statuses = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Parts",
                    from p in db.Parts
                    join rp in db.RelatedParts on p.partID equals rp.relatedID
                    join c in db.Classes on p.classID equals c.classID into ClassTemp
                    from c in ClassTemp.DefaultIfEmpty()
                    where rp.partID.Equals(partID) && statuslist.Contains(p.status)
                    select new XElement("Part",
                       new XAttribute("partID", p.partID),
                       new XAttribute("status", p.status),
                       new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                       new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                       new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                       new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                       new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                       new XAttribute("pClass", (c != null) ? c.class1 : ""),
                       new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                       new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                       new XElement("Attributes", (from pa in db.PartAttributes
                                                   where pa.partID.Equals(p.partID)
                                                   orderby pa.sort
                                                   select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                        new XElement("Content", (from co in p.ContentBridges
                                                orderby co.Content.ContentType.type, co.contentID
                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                        new XElement("Videos", (from vp in p.PartVideos
                                                orderby vp.vTypeID
                                                select new XElement("Video", vp.video,
                                                    new XAttribute("videoID", vp.pVideoID),
                                                    new XAttribute("isPrimary", vp.isPrimary),
                                                    new XAttribute("type", vp.videoType.name),
                                                    new XAttribute("icon", vp.videoType.icon ?? ""),
                                                    new XAttribute("typeID", vp.vTypeID)
                                                )).ToList<XElement>()),
                        new XElement("Packages", (from pp in p.PartPackages
                                                    select new XElement("Package",
                                                        new XAttribute("height", pp.height),
                                                        new XAttribute("length", pp.length),
                                                        new XAttribute("width", pp.width),
                                                        new XAttribute("weight", pp.weight),
                                                        new XAttribute("quantity", pp.quantity),
                                                        new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                        new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                        new XAttribute("weightUnit", pp.weightUnit.code),
                                                        new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                        new XAttribute("packageUnit", pp.packageUnit.code),
                                                        new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                    )).ToList<XElement>()),
                       new XElement("Pricing", (from pr in db.Prices
                                                where pr.partID.Equals(p.partID)
                                                orderby pr.priceType
                                                select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                       new XElement("Reviews",
                            new XAttribute("averageReview", (from r in db.Reviews
                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                             select (double?)r.rating).Average() ?? 0.0),
                                                (from r in db.Reviews
                                                 where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                 orderby r.createdDate descending
                                                 select new XElement("Review",
                                                     new XAttribute("reviewID", r.reviewID),
                                                     new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                     new XElement("rating", r.rating),
                                                     new XElement("name", r.name),
                                                     new XElement("email", r.email),
                                                     new XElement("subject", r.subject),
                                                     new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                        new XElement("Images",
                                            (from pin in db.PartImages
                                                where pin.partID.Equals(p.partID)
                                                group pin by pin.sort into pi
                                                orderby pi.Key
                                                select new XElement("Index",
                                                                    new XAttribute("name", pi.Key.ToString()),
                                                                    (from pis in db.PartImages
                                                                    join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                    where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                    orderby pis.sort, pis.sizeID
                                                                    select new XElement(pig.size,
                                                                                        new XAttribute("imageID", pis.imageID),
                                                                                        new XAttribute("path", pis.path),
                                                                                        new XAttribute("height", pis.height),
                                                                                        new XAttribute("width", pis.width)
                                                                    )).ToList<XElement>()
                                            )).ToList<XElement>())
                    ));
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Parts",
                    from p in db.Parts
                    join ci in db.CartIntegrations on p.partID equals ci.partID
                    join cu in db.Customers on ci.custID equals cu.customerID
                    join rp in db.RelatedParts on p.partID equals rp.relatedID
                    join c in db.Classes on p.classID equals c.classID into ClassTemp
                    from c in ClassTemp.DefaultIfEmpty()
                    where rp.partID.Equals(partID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                    select new XElement("Part",
                       new XAttribute("partID", p.partID),
                       new XAttribute("custPartID", ci.custPartID),
                       new XAttribute("status", p.status),
                       new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                       new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                       new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                       new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                       new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                       new XAttribute("pClass", (c != null) ? c.class1 : ""),
                       new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                       new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                       new XElement("Attributes", (from pa in db.PartAttributes
                                                   where pa.partID.Equals(p.partID)
                                                   orderby pa.sort
                                                   select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                        new XElement("Content", (from co in p.ContentBridges
                                                orderby co.Content.ContentType.type, co.contentID
                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                        new XElement("Videos", (from vp in p.PartVideos
                                                orderby vp.vTypeID
                                                select new XElement("Video", vp.video,
                                                    new XAttribute("videoID", vp.pVideoID),
                                                    new XAttribute("isPrimary", vp.isPrimary),
                                                    new XAttribute("type", vp.videoType.name),
                                                    new XAttribute("icon", vp.videoType.icon ?? ""),
                                                    new XAttribute("typeID", vp.vTypeID)
                                                )).ToList<XElement>()),
                        new XElement("Packages", (from pp in p.PartPackages
                                                  select new XElement("Package",
                                                      new XAttribute("height", pp.height),
                                                      new XAttribute("length", pp.length),
                                                      new XAttribute("width", pp.width),
                                                      new XAttribute("weight", pp.weight),
                                                      new XAttribute("quantity", pp.quantity),
                                                      new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                      new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                      new XAttribute("weightUnit", pp.weightUnit.code),
                                                      new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                      new XAttribute("packageUnit", pp.packageUnit.code),
                                                      new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                  )).ToList<XElement>()),
                       new XElement("Pricing", (from pr in db.Prices
                                                where pr.partID.Equals(p.partID)
                                                orderby pr.priceType
                                                select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                       new XElement("Reviews",
                            new XAttribute("averageReview", (from r in db.Reviews
                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                             select (double?)r.rating).Average() ?? 0.0),
                                                (from r in db.Reviews
                                                 where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                 orderby r.createdDate descending
                                                 select new XElement("Review",
                                                     new XAttribute("reviewID", r.reviewID),
                                                     new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                     new XElement("rating", r.rating),
                                                     new XElement("name", r.name),
                                                     new XElement("email", r.email),
                                                     new XElement("subject", r.subject),
                                                     new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                        new XElement("Images",
                                            (from pin in db.PartImages
                                             where pin.partID.Equals(p.partID)
                                             group pin by pin.sort into pi
                                             orderby pi.Key
                                             select new XElement("Index",
                                                                 new XAttribute("name", pi.Key.ToString()),
                                                                 (from pis in db.PartImages
                                                                  join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                  where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                  orderby pis.sort, pis.sizeID
                                                                  select new XElement(pig.size,
                                                                                      new XAttribute("imageID", pis.imageID),
                                                                                      new XAttribute("path", pis.path),
                                                                                      new XAttribute("height", pis.height),
                                                                                      new XAttribute("width", pis.width)
                                                                  )).ToList<XElement>()
                                         )).ToList<XElement>())));
                #endregion
            }
            if (parts == null) {
                parts = new XElement("Parts");
            }
            xml.Add(parts);
            return xml;
        }

        public static string GetSPGridJSON(int partID = 0) {
            string response = "";
            CurtDevDataContext db = new CurtDevDataContext();

            // Get the UPC for this part
            string upc = (from pa in db.PartAttributes
                          where pa.partID.Equals(partID) && pa.field.Equals("UPC")
                          select pa.value).FirstOrDefault<string>();

            // Get the shipping weight for this part
            string weight = (from pa in db.PartAttributes
                             where pa.partID.Equals(partID) && pa.field.Equals("weight")
                             select pa.value).FirstOrDefault<string>();

            // Get the jobber price
            decimal jobber = (from pr in db.Prices
                              where pr.partID.Equals(partID) && pr.priceType.Equals("Jobber")
                              select pr.price1).FirstOrDefault<decimal>();

            // Get the map price
            decimal map = (from pr in db.Prices
                           where pr.partID.Equals(partID) && pr.priceType.Equals("Map")
                           select pr.price1).FirstOrDefault<decimal>();
            SPStatic sp_data = new SPStatic {
                upc = upc,
                weight = weight,
                jobber = jobber,
                map = map
            };
            return JsonConvert.SerializeObject(sp_data);
        }

        public static XDocument GetSPGridXML(int partID = 0) {
            string response = "";
            CurtDevDataContext db = new CurtDevDataContext();

            // Get the UPC for this part
            string upc = (from pa in db.PartAttributes
                          where pa.partID.Equals(partID) && pa.field.Equals("UPC")
                          select pa.value).FirstOrDefault<string>();

            // Get the shipping weight for this part
            string weight = (from pa in db.PartAttributes
                             where pa.partID.Equals(partID) && pa.field.Equals("weight")
                             select pa.value).FirstOrDefault<string>();

            // Get the jobber price
            decimal jobber = (from pr in db.Prices
                              where pr.partID.Equals(partID) && pr.priceType.Equals("Jobber")
                              select pr.price1).FirstOrDefault<decimal>();

            // Get the map price
            decimal map = (from pr in db.Prices
                           where pr.partID.Equals(partID) && pr.priceType.Equals("Map")
                           select pr.price1).FirstOrDefault<decimal>();
            XDocument xml = new XDocument();
            XElement grid_container = new XElement("GridData",
                new XAttribute("UPC", upc),
                new XAttribute("weight", weight),
                new XAttribute("jobber", jobber),
                new XAttribute("map", map));
            xml.Add(grid_container);
            return xml;
        }

        public static string GetPartsByDateModifiedJSON(string date, List<int> statuses = null) {
            CurtDevDataContext db = new CurtDevDataContext();
            DateTime dateModified = Convert.ToDateTime(date);
            List<APIPart> parts = new List<APIPart>();
            List<int> statuslist = statuses ?? new List<int>();
            parts = (from p in db.Parts
                     join c in db.Classes on p.classID equals c.classID into ClassTemp
                     from c in ClassTemp.DefaultIfEmpty()
                     where p.dateModified > dateModified && statuslist.Contains(p.status)
                     select new APIPart {
                         partID = p.partID,
                         status = p.status,
                         dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                         dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                         shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                         oldPartNumber = p.oldPartNumber,
                         listPrice = String.Format("{0:C}", (from prices in db.Prices
                                                             where prices.partID.Equals(p.partID) && prices.priceType.Equals("List")
                                                             select prices.price1 != null ? prices.price1 : (decimal?)0).FirstOrDefault<decimal?>()),
                         pClass = (c != null) ? c.class1 : "",
                         priceCode = p.priceCode,
                         relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                         attributes = (from pa in db.PartAttributes
                                       where pa.partID.Equals(p.partID)
                                       orderby pa.sort
                                       select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                         content = (from co in p.ContentBridges
                                    orderby co.contentID
                                    select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                         videos = (from pv in p.PartVideos
                                   orderby pv.vTypeID
                                   select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                         packages = (from pp in p.PartPackages
                                     select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                         pricing = (from pr in db.Prices
                                    where pr.partID.Equals(p.partID)
                                    select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                         reviews = (from r in db.Reviews
                                    where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                    orderby r.createdDate descending
                                    select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                         averageReview = (from r in db.Reviews
                                          where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                          select (double?)r.rating).Average() ?? 0.0,
                         images = (from pi in db.PartImages
                                   where pi.partID.Equals(p.partID)
                                   select new APIImage {
                                       imageID = pi.imageID,
                                       sort = pi.sort,
                                       path = pi.path,
                                       height = pi.height,
                                       width = pi.width,
                                       size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                       partID = pi.partID
                                   }).OrderBy(x => x.sort).ToList<APIImage>()
                     }).Distinct().OrderBy(x => x.partID).ToList<APIPart>();

            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetPartsByDateModifiedXML(string date, List<int> statuses = null) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            DateTime dateModified = Convert.ToDateTime(date);
            List<int> statuslist = statuses ?? new List<int>();

            //List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            parts = new XElement("Parts", (from p in db.Parts
                                           join c in db.Classes on p.classID equals c.classID into ClassTemp
                                           from c in ClassTemp.DefaultIfEmpty()
                                           where p.dateModified > dateModified && statuslist.Contains(p.status)
                                           select new XElement("Part",
                                                   new XAttribute("partID", p.partID),
                                                   new XAttribute("status", p.status),
                                                   new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                   new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                   new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                   new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                   new XAttribute("listPrice", String.Format("{0:C}", (from prices in db.Prices
                                                                                                       where prices.partID.Equals(p.partID) && prices.priceType.Equals("List")
                                                                                                       select prices.price1 != null ? prices.price1 : (decimal?)0).FirstOrDefault<decimal?>())),
                                                   new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                   new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                   new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                   new XElement("Attributes", (from pa in db.PartAttributes
                                                                               where pa.partID.Equals(p.partID)
                                                                               orderby pa.sort
                                                                               select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                   new XElement("Packages", (from pp in p.PartPackages
                                                                              select new XElement("Package",
                                                                                  new XAttribute("height", pp.height),
                                                                                  new XAttribute("length", pp.length),
                                                                                  new XAttribute("width", pp.width),
                                                                                  new XAttribute("weight", pp.weight),
                                                                                  new XAttribute("quantity", pp.quantity),
                                                                                  new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                  new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                  new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                  new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                  new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                  new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                              )).ToList<XElement>()),
                                                   new XElement("Pricing", (from pr in db.Prices
                                                                            where pr.partID.Equals(p.partID)
                                                                            orderby pr.priceType
                                                                            select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                   new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name",pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                               )).ToList<XElement>());
            xml.Add(parts);
            return xml;
        }

        public static string GetAttributesJSON(int partID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<APIAttribute> attrs = new List<APIAttribute>();

            attrs = (from pa in db.PartAttributes
                     where pa.partID.Equals(partID)
                     select new APIAttribute {
                         key = pa.field,
                         value = pa.value
                     }).OrderBy(x => x.key).ToList<APIAttribute>();
            return JsonConvert.SerializeObject(attrs);
        }

        public static XDocument GetAttributesXML(int partID = 0) {
            XDocument xml = new XDocument();
            CurtDevDataContext db = new CurtDevDataContext();
            List<APIAttribute> attrs = new List<APIAttribute>();

            attrs = (from pa in db.PartAttributes
                     where pa.partID.Equals(partID)
                     select new APIAttribute {
                         key = pa.field,
                         value = pa.value
                     }).OrderBy(x => x.key).ToList<APIAttribute>();

            XElement attr_container = new XElement("Attributes",
                new XAttribute("PartID", partID));

            foreach (APIAttribute attr in attrs) {
                XElement attr_xml = new XElement(XmlConvert.EncodeName(attr.key), attr.value);
                attr_container.Add(attr_xml);
            }
            xml.Add(attr_container);
            return xml;
        }

        public static string GetReviewsByPartJSON(int partID = 0, int page = 1, int perPage = 10) {
            List<APIReview> reviews = new List<APIReview>();
            CurtDevDataContext db = new CurtDevDataContext();

            if (page < 1)
                page = 1;

            if (perPage < 1)
                perPage = 1;

            int skip = (page - 1) * perPage;

            reviews = (from r in db.Reviews
                       where r.approved.Equals(true) && r.active.Equals(true) && r.partID.Equals(partID)
                       orderby r.createdDate descending
                       select new APIReview {
                           reviewID = r.reviewID,
                           partID = (r.partID != null) ? (int)r.partID : 0,
                           rating = r.rating,
                           name = r.name,
                           email = r.email,
                           subject = r.subject,
                           review_text = r.review_text,
                           createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)
                       }).Skip(skip).Take(perPage).ToList<APIReview>();
            return JsonConvert.SerializeObject(reviews);
        }

        public static XDocument GetReviewsByPartXML(int partID = 0, int page = 1, int perPage = 10) {
            XDocument xml = new XDocument();
            XElement reviews = new XElement("Reviews");

            if (page < 1)
                page = 1;

            if (perPage < 1)
                perPage = 1;

            int skip = (page - 1) * perPage;
            CurtDevDataContext db = new CurtDevDataContext();

            reviews = new XElement("Reviews", (from r in db.Reviews
                                               where r.approved.Equals(true) && r.active.Equals(true) && r.partID.Equals(partID)
                                               orderby r.createdDate descending
                                               select new XElement("Review",
                                                   new XAttribute("reviewID", r.reviewID),
                                                   new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                   new XElement("rating", r.rating),
                                                   new XElement("name", r.name),
                                                   new XElement("email", r.email),
                                                   new XElement("subject", r.subject),
                                                   new XElement("review_text", r.review_text)
                                           )).Skip(skip).Take(perPage).ToList<XElement>());

            xml.Add(reviews);
            return xml;
        }

        public static void SubmitReview(int partID = 0, int customerID = 0, string name = "", string email = "", int rating = 0, string subject = "", string review_text = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            Review review = new Review() {
                partID = partID,
                cust_id = customerID,
                name = name,
                email = email,
                rating = rating,
                subject = subject,
                review_text = review_text,
                createdDate = DateTime.Now,
                active = true,
                approved = false
            };

            db.Reviews.InsertOnSubmit(review);
            db.SubmitChanges();
        }

        public static string GetParentCategoriesJSON() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<Categories> cats = new List<Categories>();

            cats = (from c in db.Categories
                    where c.parentID.Equals(0) && c.isLifestyle.Equals(false)
                    select c).OrderBy(x => x.sort).ToList<Categories>();
            return JsonConvert.SerializeObject(cats);
        }

        public static XDocument GetParentCategoriesXML() {
            CurtDevDataContext db = new CurtDevDataContext();
            XDocument xml = new XDocument();
            XElement cats = new XElement("Categories", (from c in db.Categories
                                                        where c.parentID.Equals(0) && c.isLifestyle.Equals(false)
                                                        orderby c.sort
                                                        select new XElement("Category",
                                                            new XAttribute("CatID", c.catID),
                                                            new XAttribute("DateAdded", c.dateAdded),
                                                            new XAttribute("ParentID", c.parentID),
                                                            new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                                                            new XAttribute("ShortDesc", (c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                                                            new XAttribute("LongDesc", (c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                                                            new XAttribute("image", (c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                                                            new XAttribute("isLifestyle", c.isLifestyle)
                                                        )));
            xml.Add(cats);
            return xml;
        }

        public static string GetCategoriesJSON(int parentID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<APICategory> cats = new List<APICategory>();
            if (parentID == 0) { // Get all categories
                cats = (from c in db.Categories
                        where c.isLifestyle.Equals(false)
                        orderby c.sort, c.catID
                        select new APICategory {
                            catID = c.catID,
                            dateAdded = c.dateAdded.ToString(),
                            parentID = c.parentID,
                            catTitle = c.catTitle,
                            shortDesc = c.shortDesc,
                            longDesc = c.longDesc,
                            image = c.image,
                            isLifestyle = c.isLifestyle,
                            sort = c.sort,
                            content = (from co in c.ContentBridges
                                       orderby co.Content.cTypeID
                                       select new APIContent {
                                           isHTML = co.Content.ContentType.allowHTML,
                                           type = co.Content.ContentType.type,
                                           content = co.Content.text
                                       }).ToList<APIContent>(),
                            partCount = (from cp in db.CatParts
                                         where cp.catID.Equals(c.catID)
                                         select cp.partID).Distinct().Count()
                        }).OrderBy(x => x.catID).ToList<APICategory>();
            } else {
                cats = (from c in db.Categories
                        where c.parentID.Equals(parentID) && c.isLifestyle.Equals(false)
                        orderby c.sort, c.catID
                        select new APICategory {
                            catID = c.catID,
                            dateAdded = c.dateAdded.ToString(),
                            parentID = c.parentID,
                            catTitle = c.catTitle,
                            shortDesc = c.shortDesc,
                            longDesc = c.longDesc,
                            image = c.image,
                            isLifestyle = c.isLifestyle,
                            sort = c.sort,
                            content = (from co in c.ContentBridges
                                       orderby co.Content.cTypeID
                                       select new APIContent {
                                           isHTML = co.Content.ContentType.allowHTML,
                                           type = co.Content.ContentType.type,
                                           content = co.Content.text
                                       }).ToList<APIContent>(),
                            partCount = (from cp in db.CatParts
                                         where cp.catID.Equals(c.catID)
                                         select cp.partID).Distinct().Count()
                        }).OrderBy(x => x.catID).ToList<APICategory>();
            }
            return JsonConvert.SerializeObject(cats);
        }

        public static XDocument GetCategoriesXML(int parentID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            XDocument xml = new XDocument();
            XElement cats = new XElement("Categories");
            if (parentID == 0) { // Get all categories
                cats = new XElement("Categories",
                                        (from c in db.Categories
                                         where c.isLifestyle.Equals(false)
                                         orderby c.sort, c.catID
                                         select new XElement("Category",
                                             new XAttribute("CatID", c.catID),
                                             new XAttribute("DateAdded", c.dateAdded),
                                             new XAttribute("ParentID", c.parentID),
                                             new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                                             new XAttribute("ShortDesc", (c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                                             new XAttribute("LongDesc", (c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                                             new XAttribute("image", (c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                                             new XAttribute("isLifestyle", c.isLifestyle),
                                             new XElement("content", (from co in c.ContentBridges
                                                                     orderby co.Content.cTypeID
                                                                     select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type),
                                                                         new XAttribute("isHTML", co.Content.ContentType.allowHTML),
                                                                         co.Content.text)).ToList<XElement>()),
                                             new XAttribute("PartCount", (from cp in db.CatParts
                                                                          where cp.catID.Equals(c.catID)
                                                                          select cp.partID).Distinct().Count())
                                         )));
            } else {
                cats = new XElement("Categories",
                                        (from c in db.Categories
                                         where c.parentID.Equals(parentID) && c.isLifestyle.Equals(false)
                                         orderby c.sort, c.catID
                                         select new XElement("Category",
                                             new XAttribute("CatID", c.catID),
                                             new XAttribute("DateAdded", c.dateAdded),
                                             new XAttribute("ParentID", c.parentID),
                                             new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                                             new XAttribute("ShortDesc", (c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                                             new XAttribute("LongDesc", (c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                                             new XAttribute("image", (c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                                             new XAttribute("isLifestyle", c.isLifestyle),
                                             new XElement("content", (from co in c.ContentBridges
                                                                      orderby co.Content.cTypeID
                                                                      select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type),
                                                                          new XAttribute("isHTML", co.Content.ContentType.allowHTML),
                                                                          co.Content.text)).ToList<XElement>()),
                                             new XAttribute("PartCount", (from cp in db.CatParts
                                                                          where cp.catID.Equals(c.catID)
                                                                          select cp.partID).Distinct().Count())
                                         )));
            }
            xml.Add(cats);
            return xml;
        }

        public static string GetPartCategoriesJSON(int partID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<APICategory> cats = new List<APICategory>();
            cats = (from c in db.Categories
                    join cp in db.CatParts on c.catID equals cp.catID
                    where c.isLifestyle.Equals(false) && cp.partID == partID
                    orderby c.catID
                    select new APICategory {
                        catID = c.catID,
                        dateAdded = c.dateAdded.ToString(),
                        parentID = c.parentID,
                        catTitle = c.catTitle,
                        shortDesc = c.shortDesc,
                        longDesc = c.longDesc,
                        image = c.image,
                        isLifestyle = c.isLifestyle,
                        content = (from co in c.ContentBridges
                                    orderby co.Content.cTypeID
                                    select new APIContent {
                                        isHTML = co.Content.ContentType.allowHTML,
                                        type = co.Content.ContentType.type,
                                        content = co.Content.text
                                    }).ToList<APIContent>(),
                        partCount = (from cps in db.CatParts
                                        where cps.catID.Equals(c.catID)
                                        select cps.partID).Distinct().Count()
                    }).OrderBy(x => x.catID).ToList<APICategory>();

            return JsonConvert.SerializeObject(cats);
        }

        public static XDocument GetPartCategoriesXML(int partID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            XDocument xml = new XDocument();
            XElement cats = new XElement("Categories");
            cats = new XElement("Categories",
                                    (from c in db.Categories
                                     join cp in db.CatParts on c.catID equals cp.catID
                                        where cp.partID.Equals(partID) && c.isLifestyle.Equals(false)
                                        orderby c.sort, c.catID
                                        select new XElement("Category",
                                            new XAttribute("CatID", c.catID),
                                            new XAttribute("DateAdded", c.dateAdded),
                                            new XAttribute("ParentID", c.parentID),
                                            new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                                            new XAttribute("ShortDesc", (c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                                            new XAttribute("LongDesc", (c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                                            new XAttribute("image", (c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                                            new XAttribute("isLifestyle", c.isLifestyle),
                                            new XElement("content", (from co in c.ContentBridges
                                                                    orderby co.Content.cTypeID
                                                                    select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type),
                                                                        new XAttribute("isHTML", co.Content.ContentType.allowHTML),
                                                                        co.Content.text)).ToList<XElement>()),
                                            new XAttribute("PartCount", (from cps in db.CatParts
                                                                        where cps.catID.Equals(c.catID)
                                                                        select cps.partID).Distinct().Count())
                                        )));
            xml.Add(cats);
            return xml;
        }
        
        public static string GetCategoryJSON(int catID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            FullCategory cat = new FullCategory();
            cat = (from c in db.Categories
                   where c.catID.Equals(catID)
                   select new FullCategory {
                       parent = c,
                       content = (from co in c.ContentBridges
                                    orderby co.Content.cTypeID
                                    select new APIContent {
                                        isHTML = co.Content.ContentType.allowHTML,
                                        type = co.Content.ContentType.type,
                                        content = co.Content.text
                                    }).ToList<APIContent>(),
                       sub_categories = (from c2 in db.Categories
                                         where c2.parentID.Equals(c.catID)
                                         orderby c2.sort, c2.catID
                                         select c2).ToList<Categories>()
                   }).FirstOrDefault<FullCategory>();
            return JsonConvert.SerializeObject(cat);
        }

        public static XDocument GetCategoryXML(int catID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            XElement cat = (from c in db.Categories
                            where c.catID.Equals(catID)
                            select new XElement("Category",
                                new XAttribute("CatID", c.catID),
                                new XAttribute("DateAdded", c.dateAdded),
                                new XAttribute("ParentID", c.parentID),
                                new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                                new XAttribute("ShortDesc", (c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                                new XAttribute("LongDesc", (c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                                new XAttribute("image", (c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                                new XAttribute("isLifestyle", c.isLifestyle),
                                new XAttribute("vehicleSpecific", c.vehicleSpecific),
                                new XElement("content", (from co in c.ContentBridges
                                                            orderby co.Content.cTypeID
                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type),
                                                                new XAttribute("isHTML", co.Content.ContentType.allowHTML),
                                                                co.Content.text)).ToList<XElement>()),
                                new XElement("SubCategories", new List<XElement>(from c2 in db.Categories
                                                                                 where c2.parentID.Equals(c.catID)
                                                                                 orderby c2.sort, c2.catID
                                                                                 select new XElement("SubCategory",
                                                                                    new XAttribute("CatID", c2.catID),
                                                                                    new XAttribute("DateAdded", c2.dateAdded),
                                                                                    new XAttribute("ParentID", c2.parentID),
                                                                                    new XAttribute("CategoryName", c2.catTitle.ToString().Trim()),
                                                                                    new XAttribute("ShortDesc", (c2.shortDesc.Length > 0) ? c2.shortDesc.ToString().Trim() : ""),
                                                                                    new XAttribute("LongDesc", (c2.longDesc.Length > 0) ? c2.longDesc.ToString().Trim() : ""),
                                                                                    new XAttribute("image", (c2.image.Length > 0) ? c2.image.ToString().Trim() : ""),
                                                                                    new XAttribute("isLifestyle", c2.isLifestyle))).ToList<XElement>())
                            )).FirstOrDefault<XElement>();
            XDocument xml = new XDocument();
            xml.Add(cat);
            return xml;
        }

        public static string GetCategoryPartsJSON(int catID = 0, List<int> statuses = null, int customerID = 0, bool integrated = false, int page = 1, int perpage = 0) {
            List<APIPart> part = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();
            int count = 0;

            if (page < 1)
                page = 1;

            if (perpage < 1)
                perpage = 1;


            if (catID == 0) {
                #region No CatID
                count = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                         from pc in PartJoin.DefaultIfEmpty()
                         where pc.catID.Equals(null) && statuslist.Contains(p.status)
                         select p).Count();
                if (perpage == 0) perpage = count;
                part = (from p in db.Parts
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                        from pc in PartJoin.DefaultIfEmpty()
                        where pc.catID.Equals(null) && statuslist.Contains(p.status)
                        select new APIPart {
                            partID = p.partID,
                            status = p.status,
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = p.RelatedParts.Count,
                            attributes = (from pa in p.PartAttributes
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in p.Prices
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in p.Reviews
                                       where r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in p.Reviews
                                             where r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in p.PartImages
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = pi.PartImageSize.size,
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();
                #endregion
            } else {
                if (!integrated) {
                    #region Not Integrated
                    count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                             select p).Count();
                    if (perpage == 0) perpage = count;
                    part = (from p in db.Parts
                            join c in db.Classes on p.classID equals c.classID into ClassTemp
                            from c in ClassTemp.DefaultIfEmpty()
                            join pc in db.CatParts on p.partID equals pc.partID
                            where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                            select new APIPart {
                                partID = p.partID,
                                status = p.status,
                                dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                oldPartNumber = p.oldPartNumber,
                                listPrice = GetCustomerPrice(customerID, p.partID),
                                pClass = (c != null) ? c.class1 : "",
                                priceCode = p.priceCode,
                                relatedCount = p.RelatedParts.Count,
                                attributes = (from pa in p.PartAttributes
                                              orderby pa.sort
                                              select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                content = (from co in p.ContentBridges
                                           orderby co.contentID
                                           select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                videos = (from pv in p.PartVideos
                                          orderby pv.vTypeID
                                          select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                packages = (from pp in p.PartPackages
                                            select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                pricing = (from pr in p.Prices
                                           select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                reviews = (from r in p.Reviews
                                           where r.active.Equals(true) && r.approved.Equals(true)
                                           orderby r.createdDate descending
                                           select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                                averageReview = (from r in p.Reviews
                                                 where r.active.Equals(true) && r.approved.Equals(true)
                                                 select (double?)r.rating).Average() ?? 0.0,
                                images = (from pi in p.PartImages
                                          select new APIImage {
                                              imageID = pi.imageID,
                                              sort = pi.sort,
                                              path = pi.path,
                                              height = pi.height,
                                              width = pi.width,
                                              size = pi.PartImageSize.size,
                                              partID = pi.partID
                                          }).OrderBy(x => x.sort).ToList<APIImage>()
                            }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();
                    #endregion
                } else {
                    #region Integrated
                    count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join ci in db.CartIntegrations on p.partID equals ci.partID
                             join cu in db.Customers on ci.custID equals cu.customerID
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                             select p).Count();
                    if (perpage == 0) perpage = count;
                    part = (from p in db.Parts
                            join c in db.Classes on p.classID equals c.classID into ClassTemp
                            from c in ClassTemp.DefaultIfEmpty()
                            join ci in db.CartIntegrations on p.partID equals ci.partID
                            join cu in db.Customers on ci.custID equals cu.customerID
                            join pc in db.CatParts on p.partID equals pc.partID
                            where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                            select new APIPart {
                                partID = p.partID,
                                custPartID = ci.custPartID,
                                status = p.status,
                                dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                oldPartNumber = p.oldPartNumber,
                                listPrice = GetCustomerPrice(customerID, p.partID),
                                pClass = (c != null) ? c.class1 : "",
                                priceCode = p.priceCode,
                                relatedCount = p.RelatedParts.Count,
                                attributes = (from pa in p.PartAttributes
                                              orderby pa.sort
                                              select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                content = (from co in p.ContentBridges
                                           orderby co.contentID
                                           select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                videos = (from pv in p.PartVideos
                                          orderby pv.vTypeID
                                          select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                packages = (from pp in p.PartPackages
                                            select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                pricing = (from pr in p.Prices
                                           select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                reviews = (from r in p.Reviews
                                           where r.active.Equals(true) && r.approved.Equals(true)
                                           orderby r.createdDate descending
                                           select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                                averageReview = (from r in p.Reviews
                                                 where r.active.Equals(true) && r.approved.Equals(true)
                                                 select (double?)r.rating).Average() ?? 0.0,
                                images = (from pi in p.PartImages
                                          where pi.partID.Equals(p.partID)
                                          select new APIImage {
                                              imageID = pi.imageID,
                                              sort = pi.sort,
                                              path = pi.path,
                                              height = pi.height,
                                              width = pi.width,
                                              size = pi.PartImageSize.size,
                                              partID = pi.partID
                                          }).OrderBy(x => x.sort).ToList<APIImage>()
                            }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();

                    #endregion
                }
            }
            return JsonConvert.SerializeObject(part);
        }

        public static string GetCategoryPartsWithVehicleIDJSON(int catID = 0, int vehicleID = 0, List<int> statuses = null, int customerID = 0, bool integrated = false, int page = 1, int perpage = 0) {
            List<APIPart> part = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();
            int count = 0;

            if (page < 1)
                page = 1;

            if (perpage < 1)
                perpage = 1;

            if (catID == 0) {
                #region No CatID
                count = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                         from pc in PartJoin.DefaultIfEmpty()
                         where pc.catID.Equals(null) && statuslist.Contains(p.status)
                         select p).Count();
                if (perpage == 0) perpage = count;
                part = (from p in db.Parts
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                        from pc in PartJoin.DefaultIfEmpty()
                        where pc.catID.Equals(null) && statuslist.Contains(p.status)
                        select new APIPart {
                            partID = p.partID,
                            status = p.status,
                            vehicleID = db.VehicleParts.Where(x => x.vehicleID == vehicleID).Where(x => x.partID == p.partID).Select(x => x.vehicleID).FirstOrDefault(),
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            relatedCount = p.RelatedParts.Count,
                            attributes = (from pa in p.PartAttributes
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                 join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                 where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                 orderby vpa.sort
                                                 select new APIAttribute { key = vpa.field, value = vpa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in p.Prices
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in p.Reviews
                                       where r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in p.Reviews
                                             where r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in p.PartImages
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = pi.PartImageSize.size,
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();
                #endregion
            } else {
                if (!integrated) {
                    #region Not Integrated
                    count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                             select p).Distinct().Count();
                    if (perpage == 0) perpage = count;
                    part = (from p in db.Parts
                            join c in db.Classes on p.classID equals c.classID into ClassTemp
                            from c in ClassTemp.DefaultIfEmpty()
                            join pc in db.CatParts on p.partID equals pc.partID
                            where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                            select new APIPart {
                                partID = p.partID,
                                status = p.status,
                                vehicleID = db.VehicleParts.Where(x => x.vehicleID == vehicleID).Where(x => x.partID == p.partID).Select(x => x.vehicleID).FirstOrDefault(),
                                dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                oldPartNumber = p.oldPartNumber,
                                listPrice = GetCustomerPrice(customerID, p.partID),
                                pClass = (c != null) ? c.class1 : "",
                                priceCode = p.priceCode,
                                relatedCount = p.RelatedParts.Count,
                                attributes = (from pa in p.PartAttributes
                                              orderby pa.sort
                                              select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                     join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                     where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                     orderby vpa.sort
                                                     select new APIAttribute { key = vpa.field, value = vpa.value }).Distinct().ToList<APIAttribute>(),
                                content = (from co in p.ContentBridges
                                           orderby co.contentID
                                           select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                videos = (from pv in p.PartVideos
                                          orderby pv.vTypeID
                                          select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                packages = (from pp in p.PartPackages
                                            select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                pricing = (from pr in p.Prices
                                           select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                reviews = (from r in p.Reviews
                                           where r.active.Equals(true) && r.approved.Equals(true)
                                           orderby r.createdDate descending
                                           select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                                averageReview = (from r in p.Reviews
                                                 where r.active.Equals(true) && r.approved.Equals(true)
                                                 select (double?)r.rating).Average() ?? 0.0,
                                images = (from pi in p.PartImages
                                          select new APIImage {
                                              imageID = pi.imageID,
                                              sort = pi.sort,
                                              path = pi.path,
                                              height = pi.height,
                                              width = pi.width,
                                              size = pi.PartImageSize.size,
                                              partID = pi.partID
                                          }).OrderBy(x => x.sort).ToList<APIImage>()
                            }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();
                    #endregion
                } else {
                    #region Integrated
                    count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join ci in db.CartIntegrations on p.partID equals ci.partID
                             join cu in db.Customers on ci.custID equals cu.customerID
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                             select p).Count();
                    if (perpage == 0) perpage = count;
                    part = (from p in db.Parts
                            join c in db.Classes on p.classID equals c.classID into ClassTemp
                            from c in ClassTemp.DefaultIfEmpty()
                            join ci in db.CartIntegrations on p.partID equals ci.partID
                            join cu in db.Customers on ci.custID equals cu.customerID
                            join pc in db.CatParts on p.partID equals pc.partID
                            where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                            select new APIPart {
                                partID = p.partID,
                                custPartID = ci.custPartID,
                                status = p.status,
                                vehicleID = db.VehicleParts.Where(x => x.vehicleID == vehicleID).Where(x => x.partID == p.partID).Select(x => x.vehicleID).FirstOrDefault(),
                                dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                oldPartNumber = p.oldPartNumber,
                                listPrice = GetCustomerPrice(customerID, p.partID),
                                pClass = (c != null) ? c.class1 : "",
                                priceCode = p.priceCode,
                                relatedCount = p.RelatedParts.Count,
                                attributes = (from pa in p.PartAttributes
                                              orderby pa.sort
                                              select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                     join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                     where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                     orderby vpa.sort
                                                     select new APIAttribute { key = vpa.field, value = vpa.value }).Distinct().ToList<APIAttribute>(),
                                content = (from co in p.ContentBridges
                                           orderby co.contentID
                                           select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                videos = (from pv in p.PartVideos
                                          orderby pv.vTypeID
                                          select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                packages = (from pp in p.PartPackages
                                            select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                pricing = (from pr in p.Prices
                                           select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                reviews = (from r in p.Reviews
                                           where r.active.Equals(true) && r.approved.Equals(true)
                                           orderby r.createdDate descending
                                           select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                                averageReview = (from r in p.Reviews
                                                 where r.active.Equals(true) && r.approved.Equals(true)
                                                 select (double?)r.rating).Average() ?? 0.0,
                                images = (from pi in p.PartImages
                                          where pi.partID.Equals(p.partID)
                                          select new APIImage {
                                              imageID = pi.imageID,
                                              sort = pi.sort,
                                              path = pi.path,
                                              height = pi.height,
                                              width = pi.width,
                                              size = pi.PartImageSize.size,
                                              partID = pi.partID
                                          }).OrderBy(x => x.sort).ToList<APIImage>()
                            }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();

                    #endregion
                }
            }
            return JsonConvert.SerializeObject(part);
        }

        public static string GetCategoryPartsCount(int catID = 0, List<int> statuses = null, int customerID = 0, bool integrated = false) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();
            int count = 0;

            if (catID == 0) {
                #region No CatID
                count = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                         from pc in PartJoin.DefaultIfEmpty()
                         where pc.catID.Equals(null) && statuslist.Contains(p.status)
                         select p).Distinct().Count();
                #endregion
            } else {
                if (!integrated) {
                    #region Not Integrated
                    count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                             select p).Count();
                    #endregion
                } else {
                    #region Integrated
                    count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join ci in db.CartIntegrations on p.partID equals ci.partID
                             join cu in db.Customers on ci.custID equals cu.customerID
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                             select p).Count();
                    #endregion
                }
            }
            return JsonConvert.SerializeObject(count);
        }

        public static string GetCategoryPartsWithFilterJSON(int catID = 0, double year = 0, string make = "", string model = "", string style = "", List<int> statuses = null, int customerID = 0, bool integrated = false, int page = 1, int perpage = 0) {
            List<APIPart> part = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();
            int count = 0;

            if (catID == 0) {
                #region No CatID
                count = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                         from pc in PartJoin.DefaultIfEmpty()
                         where pc.catID.Equals(null) && statuslist.Contains(p.status)
                         select p).Count();
                if (perpage == 0) perpage = count;
                part = (from p in db.Parts
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                        from pc in PartJoin.DefaultIfEmpty()
                        where pc.catID.Equals(null)
                        select new APIPart {
                            partID = p.partID,
                            status = p.status,
                            vehicleID = (from vep in db.VehicleParts
                                                 join v in db.Vehicles on vep.vehicleID equals v.vehicleID
                                                 join y in db.Years on v.yearID equals y.yearID
                                                 join ma in db.Makes on v.makeID equals ma.makeID
                                                 join mo in db.Models on v.modelID equals mo.modelID
                                                 join s in db.Styles on v.styleID equals s.styleID
                                                 where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vep.partID.Equals(p.partID)
                                                 select vep.vehicleID).FirstOrDefault(),
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = p.RelatedParts.Count,
                            attributes = (from pa in p.PartAttributes
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                 join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                 join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                 join y in db.Years on v.yearID equals y.yearID
                                                 join ma in db.Makes on v.makeID equals ma.makeID
                                                 join mo in db.Models on v.modelID equals mo.modelID
                                                 join s in db.Styles on v.styleID equals s.styleID
                                                 where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vp.partID.Equals(p.partID)
                                                 orderby vpa.sort
                                                 select new APIAttribute { key = vpa.field, value = vpa.value }).Distinct().ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in p.Prices
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in p.Reviews
                                       where r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in p.Reviews
                                             where r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in p.PartImages
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = pi.PartImageSize.size,
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();
                #endregion
            } else {
                if (!integrated) {
                    #region Not Integrated
                    count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                             select p).Count();
                    if (perpage == 0) perpage = count;
                    part = (from p in db.Parts
                            join c in db.Classes on p.classID equals c.classID into ClassTemp
                            from c in ClassTemp.DefaultIfEmpty()
                            join pc in db.CatParts on p.partID equals pc.partID
                            where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                            select new APIPart {
                                partID = p.partID,
                                status = p.status,
                                vehicleID = (from vep in db.VehicleParts
                                             join v in db.Vehicles on vep.vehicleID equals v.vehicleID
                                             join y in db.Years on v.yearID equals y.yearID
                                             join ma in db.Makes on v.makeID equals ma.makeID
                                             join mo in db.Models on v.modelID equals mo.modelID
                                             join s in db.Styles on v.styleID equals s.styleID
                                             where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vep.partID.Equals(p.partID)
                                             select vep.vehicleID).FirstOrDefault(),
                                dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                oldPartNumber = p.oldPartNumber,
                                listPrice = GetCustomerPrice(customerID, p.partID),
                                pClass = (c != null) ? c.class1 : "",
                                priceCode = p.priceCode,
                                relatedCount = p.RelatedParts.Count,
                                attributes = (from pa in p.PartAttributes
                                              orderby pa.sort
                                              select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                     join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                     join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                     join y in db.Years on v.yearID equals y.yearID
                                                     join ma in db.Makes on v.makeID equals ma.makeID
                                                     join mo in db.Models on v.modelID equals mo.modelID
                                                     join s in db.Styles on v.styleID equals s.styleID
                                                     where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vp.partID.Equals(p.partID)
                                                     orderby vpa.sort
                                                     select new APIAttribute { key = vpa.field, value = vpa.value }).Distinct().ToList<APIAttribute>(),
                                content = (from co in p.ContentBridges
                                           orderby co.contentID
                                           select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                videos = (from pv in p.PartVideos
                                          orderby pv.vTypeID
                                          select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                packages = (from pp in p.PartPackages
                                            select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                pricing = (from pr in p.Prices
                                           select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                reviews = (from r in p.Reviews
                                           where r.active.Equals(true) && r.approved.Equals(true)
                                           orderby r.createdDate descending
                                           select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                                averageReview = (from r in p.Reviews
                                                 where r.active.Equals(true) && r.approved.Equals(true)
                                                 select (double?)r.rating).Average() ?? 0.0,
                                images = (from pi in p.PartImages
                                          select new APIImage {
                                              imageID = pi.imageID,
                                              sort = pi.sort,
                                              path = pi.path,
                                              height = pi.height,
                                              width = pi.width,
                                              size = pi.PartImageSize.size,
                                              partID = pi.partID
                                          }).OrderBy(x => x.sort).ToList<APIImage>()
                            }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();
                    #endregion
                } else {
                    #region Integrated
                    count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join ci in db.CartIntegrations on p.partID equals ci.partID
                             join cu in db.Customers on ci.custID equals cu.customerID
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                             select p).Count();
                    if (perpage == 0) perpage = count;
                    part = (from p in db.Parts
                            join c in db.Classes on p.classID equals c.classID into ClassTemp
                            from c in ClassTemp.DefaultIfEmpty()
                            join ci in db.CartIntegrations on p.partID equals ci.partID
                            join cu in db.Customers on ci.custID equals cu.customerID
                            join pc in db.CatParts on p.partID equals pc.partID
                            where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                            select new APIPart {
                                partID = p.partID,
                                custPartID = ci.custPartID,
                                status = p.status,
                                vehicleID = (from vep in db.VehicleParts
                                             join v in db.Vehicles on vep.vehicleID equals v.vehicleID
                                             join y in db.Years on v.yearID equals y.yearID
                                             join ma in db.Makes on v.makeID equals ma.makeID
                                             join mo in db.Models on v.modelID equals mo.modelID
                                             join s in db.Styles on v.styleID equals s.styleID
                                                     where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vep.partID.Equals(p.partID)
                                             select vep.vehicleID).FirstOrDefault(),
                                dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                oldPartNumber = p.oldPartNumber,
                                listPrice = GetCustomerPrice(customerID, p.partID),
                                pClass = (c != null) ? c.class1 : "",
                                priceCode = p.priceCode,
                                relatedCount = p.RelatedParts.Count,
                                attributes = (from pa in p.PartAttributes
                                              orderby pa.sort
                                              select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                     join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                     join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                     join y in db.Years on v.yearID equals y.yearID
                                                     join ma in db.Makes on v.makeID equals ma.makeID
                                                     join mo in db.Models on v.modelID equals mo.modelID
                                                     join s in db.Styles on v.styleID equals s.styleID
                                                     where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vp.partID.Equals(p.partID)
                                                     orderby vpa.sort
                                                     select new APIAttribute { key = vpa.field, value = vpa.value }).Distinct().ToList<APIAttribute>(),
                                content = (from co in p.ContentBridges
                                           orderby co.contentID
                                           select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                videos = (from pv in p.PartVideos
                                          orderby pv.vTypeID
                                          select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                packages = (from pp in p.PartPackages
                                            select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                pricing = (from pr in p.Prices
                                           select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                reviews = (from r in p.Reviews
                                           where r.active.Equals(true) && r.approved.Equals(true)
                                           orderby r.createdDate descending
                                           select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                                averageReview = (from r in p.Reviews
                                                 where r.active.Equals(true) && r.approved.Equals(true)
                                                 select (double?)r.rating).Average() ?? 0.0,
                                images = (from pi in p.PartImages
                                          select new APIImage {
                                              imageID = pi.imageID,
                                              sort = pi.sort,
                                              path = pi.path,
                                              height = pi.height,
                                              width = pi.width,
                                              size = pi.PartImageSize.size,
                                              partID = pi.partID
                                          }).OrderBy(x => x.sort).ToList<APIImage>()
                            }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();

                    #endregion
                }
            }
            return JsonConvert.SerializeObject(part);
        }

        public static XDocument GetCategoryPartsXML(int catID = 0, List<int> statuses = null, int customerID = 0, bool integrated = false, int page = 1, int perpage = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (catID == 0) {
                #region no CatID
                int count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(null) && statuslist.Contains(p.status)
                             select p).Count();
                if (perpage == 0) {
                    perpage = count;
                }
                parts = new XElement("Parts", new XAttribute("Count", count),
                                              new XAttribute("Page", page),
                                              new XAttribute("PerPage", perpage),
                                             (from p in db.Parts
                                              join c in db.Classes on p.classID equals c.classID into ClassTemp
                                              from c in ClassTemp.DefaultIfEmpty()
                                              join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                                              from pc in PartJoin.DefaultIfEmpty()
                                              where pc.catID.Equals(null)
                                              select new XElement("Part",
                                                 new XAttribute("partID", p.partID),
                                                 new XAttribute("status", p.status),
                                                 new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                 new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                 new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                 new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                 new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                 new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                 new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                 new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                 new XElement("Attributes", (from pa in db.PartAttributes
                                                                             where pa.partID.Equals(p.partID)
                                                                             orderby pa.sort
                                                                             select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                new XElement("Content", (from co in p.ContentBridges
                                                                        orderby co.Content.ContentType.type, co.contentID
                                                                        select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                new XElement("Videos", (from vp in p.PartVideos
                                                                        orderby vp.vTypeID
                                                                        select new XElement("Video", vp.video,
                                                                            new XAttribute("videoID", vp.pVideoID),
                                                                            new XAttribute("isPrimary", vp.isPrimary),
                                                                            new XAttribute("type", vp.videoType.name),
                                                                            new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                            new XAttribute("typeID", vp.vTypeID)
                                                                        )).ToList<XElement>()),
                                                new XElement("Packages", (from pp in p.PartPackages
                                                                          select new XElement("Package",
                                                                              new XAttribute("height", pp.height),
                                                                              new XAttribute("length", pp.length),
                                                                              new XAttribute("width", pp.width),
                                                                              new XAttribute("weight", pp.weight),
                                                                              new XAttribute("quantity", pp.quantity),
                                                                              new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                              new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                              new XAttribute("weightUnit", pp.weightUnit.code),
                                                                              new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                              new XAttribute("packageUnit", pp.packageUnit.code),
                                                                              new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                          )).ToList<XElement>()),
                                                 new XElement("Pricing", (from pr in db.Prices
                                                                          where pr.partID.Equals(p.partID)
                                                                          orderby pr.priceType
                                                                          select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                 new XElement("Reviews",
                                                     new XAttribute("averageReview", (from r in db.Reviews
                                                                                      where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                      select (double?)r.rating).Average() ?? 0.0),
                                                                         (from r in db.Reviews
                                                                          where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                          orderby r.createdDate descending
                                                                          select new XElement("Review",
                                                                              new XAttribute("reviewID", r.reviewID),
                                                                              new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                              new XElement("rating", r.rating),
                                                                              new XElement("name", r.name),
                                                                              new XElement("email", r.email),
                                                                              new XElement("subject", r.subject),
                                                                              new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                new XElement("Images",
                                                                    (from pin in db.PartImages
                                                                        where pin.partID.Equals(p.partID)
                                                                        group pin by pin.sort into pi
                                                                        orderby pi.Key
                                                                        select new XElement("Index",
                                                                                            new XAttribute("name", pi.Key.ToString()),
                                                                                            (from pis in db.PartImages
                                                                                            join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                            where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                            orderby pis.sort, pis.sizeID
                                                                                            select new XElement(pig.size,
                                                                                                                new XAttribute("imageID", pis.imageID),
                                                                                                                new XAttribute("path", pis.path),
                                                                                                                new XAttribute("height", pis.height),
                                                                                                                new XAttribute("width", pis.width)
                                                                                            )).ToList<XElement>()
                                                                    )).ToList<XElement>())
                                              )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                #endregion
            } else {
                if (!integrated) {
                    #region Not Integrated
                    int count = (from p in db.Parts
                                 join c in db.Classes on p.classID equals c.classID into ClassTemp
                                 from c in ClassTemp.DefaultIfEmpty()
                                 join pc in db.CatParts on p.partID equals pc.partID
                                 where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                                 select p).Count();
                    if (perpage == 0) {
                        perpage = count;
                    }
                    parts = new XElement("Parts", new XAttribute("Count", count),
                                                  new XAttribute("Page", page),
                                                  new XAttribute("PerPage", perpage),
                                                  (from p in db.Parts
                                                   join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                   from c in ClassTemp.DefaultIfEmpty()
                                                   join pc in db.CatParts on p.partID equals pc.partID
                                                   where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                                                   select new XElement("Part",
                                                      new XAttribute("partID", p.partID),
                                                      new XAttribute("status", p.status),
                                                      new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                      new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                      new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                      new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                      new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                      new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                      new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                      new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                      new XElement("Attributes", (from pa in db.PartAttributes
                                                                                  where pa.partID.Equals(p.partID)
                                                                                  orderby pa.sort
                                                                                  select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                       new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                       new XElement("Videos", (from vp in p.PartVideos
                                                                               orderby vp.vTypeID
                                                                               select new XElement("Video", vp.video,
                                                                                   new XAttribute("videoID", vp.pVideoID),
                                                                                   new XAttribute("isPrimary", vp.isPrimary),
                                                                                   new XAttribute("type", vp.videoType.name),
                                                                                   new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                                   new XAttribute("typeID", vp.vTypeID)
                                                                               )).ToList<XElement>()),
                                                      new XElement("Packages", (from pp in p.PartPackages
                                                                                select new XElement("Package",
                                                                                    new XAttribute("height", pp.height),
                                                                                    new XAttribute("length", pp.length),
                                                                                    new XAttribute("width", pp.width),
                                                                                    new XAttribute("weight", pp.weight),
                                                                                    new XAttribute("quantity", pp.quantity),
                                                                                    new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                    new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                    new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                    new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                    new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                    new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                )).ToList<XElement>()),
                                                      new XElement("Pricing", (from pr in db.Prices
                                                                               where pr.partID.Equals(p.partID)
                                                                               orderby pr.priceType
                                                                               select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                      new XElement("Reviews",
                                                          new XAttribute("averageReview", (from r in db.Reviews
                                                                                           where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                           select (double?)r.rating).Average() ?? 0.0),
                                                                              (from r in db.Reviews
                                                                               where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                               orderby r.createdDate descending
                                                                               select new XElement("Review",
                                                                                   new XAttribute("reviewID", r.reviewID),
                                                                                   new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                   new XElement("rating", r.rating),
                                                                                   new XElement("name", r.name),
                                                                                   new XElement("email", r.email),
                                                                                   new XElement("subject", r.subject),
                                                                                   new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                                   )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                    #endregion
                } else {
                    #region Integrated
                    int count = (from p in db.Parts
                                 join c in db.Classes on p.classID equals c.classID into ClassTemp
                                 from c in ClassTemp.DefaultIfEmpty()
                                 join pc in db.CatParts on p.partID equals pc.partID
                                 join ci in db.CartIntegrations on p.partID equals ci.partID
                                 join cu in db.Customers on ci.custID equals cu.customerID
                                 where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                                 select p).Count();
                    if (perpage == 0) {
                        perpage = count;
                    }
                    parts = new XElement("Parts", new XAttribute("Count", count),
                                                  new XAttribute("Page", page),
                                                  new XAttribute("PerPage", perpage),
                                                  (from p in db.Parts
                                                   join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                   from c in ClassTemp.DefaultIfEmpty()
                                                   join pc in db.CatParts on p.partID equals pc.partID
                                                   join ci in db.CartIntegrations on p.partID equals ci.partID
                                                   join cu in db.Customers on ci.custID equals cu.customerID
                                                   where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                                                   select new XElement("Part",
                                                      new XAttribute("partID", p.partID),
                                                      new XAttribute("custPartID", ci.custPartID),
                                                      new XAttribute("status", p.status),
                                                      new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                      new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                      new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                      new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                      new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                      new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                      new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                      new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                      new XElement("Attributes", (from pa in db.PartAttributes
                                                                                  where pa.partID.Equals(p.partID)
                                                                                  orderby pa.sort
                                                                                  select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                       new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                       new XElement("Videos", (from vp in p.PartVideos
                                                                               orderby vp.vTypeID
                                                                               select new XElement("Video", vp.video,
                                                                                   new XAttribute("videoID", vp.pVideoID),
                                                                                   new XAttribute("isPrimary", vp.isPrimary),
                                                                                   new XAttribute("type", vp.videoType.name),
                                                                                   new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                                   new XAttribute("typeID", vp.vTypeID)
                                                                               )).ToList<XElement>()),
                                                      new XElement("Packages", (from pp in p.PartPackages
                                                                                select new XElement("Package",
                                                                                    new XAttribute("height", pp.height),
                                                                                    new XAttribute("length", pp.length),
                                                                                    new XAttribute("width", pp.width),
                                                                                    new XAttribute("weight", pp.weight),
                                                                                    new XAttribute("quantity", pp.quantity),
                                                                                    new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                    new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                    new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                    new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                    new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                    new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                )).ToList<XElement>()),
                                                      new XElement("Pricing", (from pr in db.Prices
                                                                               where pr.partID.Equals(p.partID)
                                                                               orderby pr.priceType
                                                                               select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                      new XElement("Reviews",
                                                          new XAttribute("averageReview", (from r in db.Reviews
                                                                                           where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                           select (double?)r.rating).Average() ?? 0.0),
                                                                              (from r in db.Reviews
                                                                               where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                               orderby r.createdDate descending
                                                                               select new XElement("Review",
                                                                                   new XAttribute("reviewID", r.reviewID),
                                                                                   new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                   new XElement("rating", r.rating),
                                                                                   new XElement("name", r.name),
                                                                                   new XElement("email", r.email),
                                                                                   new XElement("subject", r.subject),
                                                                                   new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                        new XElement("Images",
                                                                            (from pin in db.PartImages
                                                                             where pin.partID.Equals(p.partID)
                                                                             group pin by pin.sort into pi
                                                                             orderby pi.Key
                                                                             select new XElement("Index",
                                                                                                 new XAttribute("name", pi.Key.ToString()),
                                                                                                 (from pis in db.PartImages
                                                                                                  join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                                  where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                                  orderby pis.sort, pis.sizeID
                                                                                                  select new XElement(pig.size,
                                                                                                                      new XAttribute("imageID", pis.imageID),
                                                                                                                      new XAttribute("path", pis.path),
                                                                                                                      new XAttribute("height", pis.height),
                                                                                                                      new XAttribute("width", pis.width)
                                                                                                  )).ToList<XElement>()
                                                                         )).ToList<XElement>())
                                                   )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                    #endregion
                }
            }
            if (parts == null) {
                parts = new XElement("Part");
            }
            xml.Add(parts);
            return xml;
        }

        public static XDocument GetCategoryPartsWithVehicleIDXML(int catID = 0, int vehicleID = 0, List<int> statuses = null, int customerID = 0, bool integrated = false, int page = 1, int perpage = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (catID == 0) {
                #region no CatID
                int count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(null) && statuslist.Contains(p.status)
                             select p).Count();
                if (perpage == 0) {
                    perpage = count;
                }
                parts = new XElement("Parts", new XAttribute("Count", count),
                                              new XAttribute("Page", page),
                                              new XAttribute("PerPage", perpage),
                                             (from p in db.Parts
                                              join c in db.Classes on p.classID equals c.classID into ClassTemp
                                              from c in ClassTemp.DefaultIfEmpty()
                                              join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                                              from pc in PartJoin.DefaultIfEmpty()
                                              where pc.catID.Equals(null)
                                              select new XElement("Part",
                                                 new XAttribute("partID", p.partID),
                                                 new XAttribute("status", p.status),
                                                 new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                 new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                 new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                 new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                 new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                 new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                 new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                 new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                 new XAttribute("vehicleID", db.VehicleParts.Where(x => x.vehicleID == vehicleID).Where(x => x.partID == p.partID).Select(x => x.vehicleID.ToString()).FirstOrDefault() ?? "0"),
                                                 new XElement("Attributes", (from pa in db.PartAttributes
                                                                             where pa.partID.Equals(p.partID)
                                                                             orderby pa.sort
                                                                             select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                 new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                    join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                                                    where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                                                    orderby vpa.sort
                                                                                    select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                new XElement("Content", (from co in p.ContentBridges
                                                                        orderby co.Content.ContentType.type, co.contentID
                                                                        select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                new XElement("Videos", (from vp in p.PartVideos
                                                                        orderby vp.vTypeID
                                                                        select new XElement("Video", vp.video,
                                                                            new XAttribute("videoID", vp.pVideoID),
                                                                            new XAttribute("isPrimary", vp.isPrimary),
                                                                            new XAttribute("type", vp.videoType.name),
                                                                            new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                            new XAttribute("typeID", vp.vTypeID)
                                                                        )).ToList<XElement>()),
                                                new XElement("Packages", (from pp in p.PartPackages
                                                                        select new XElement("Package",
                                                                            new XAttribute("height", pp.height),
                                                                            new XAttribute("length", pp.length),
                                                                            new XAttribute("width", pp.width),
                                                                            new XAttribute("weight", pp.weight),
                                                                            new XAttribute("quantity", pp.quantity),
                                                                            new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                            new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                            new XAttribute("weightUnit", pp.weightUnit.code),
                                                                            new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                            new XAttribute("packageUnit", pp.packageUnit.code),
                                                                            new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                        )).ToList<XElement>()),
                                                 new XElement("Pricing", (from pr in db.Prices
                                                                          where pr.partID.Equals(p.partID)
                                                                          orderby pr.priceType
                                                                          select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                 new XElement("Reviews",
                                                     new XAttribute("averageReview", (from r in db.Reviews
                                                                                      where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                      select (double?)r.rating).Average() ?? 0.0),
                                                                         (from r in db.Reviews
                                                                          where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                          orderby r.createdDate descending
                                                                          select new XElement("Review",
                                                                              new XAttribute("reviewID", r.reviewID),
                                                                              new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                              new XElement("rating", r.rating),
                                                                              new XElement("name", r.name),
                                                                              new XElement("email", r.email),
                                                                              new XElement("subject", r.subject),
                                                                              new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                new XElement("Images",
                                                                    (from pin in db.PartImages
                                                                     where pin.partID.Equals(p.partID)
                                                                     group pin by pin.sort into pi
                                                                     orderby pi.Key
                                                                     select new XElement("Index",
                                                                                         new XAttribute("name", pi.Key.ToString()),
                                                                                         (from pis in db.PartImages
                                                                                          join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                          where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                          orderby pis.sort, pis.sizeID
                                                                                          select new XElement(pig.size,
                                                                                                              new XAttribute("imageID", pis.imageID),
                                                                                                              new XAttribute("path", pis.path),
                                                                                                              new XAttribute("height", pis.height),
                                                                                                              new XAttribute("width", pis.width)
                                                                                          )).ToList<XElement>()
                                                                 )).ToList<XElement>())
                                              )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                #endregion
            } else {
                if (!integrated) {
                    #region Not Integrated
                    int count = (from p in db.Parts
                                 join c in db.Classes on p.classID equals c.classID into ClassTemp
                                 from c in ClassTemp.DefaultIfEmpty()
                                 join pc in db.CatParts on p.partID equals pc.partID
                                 where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                                 select p).Count();
                    if (perpage == 0) {
                        perpage = count;
                    }
                    parts = new XElement("Parts", new XAttribute("Count", count),
                                                  new XAttribute("Page", page),
                                                  new XAttribute("PerPage", perpage),
                                                  (from p in db.Parts
                                                   join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                   from c in ClassTemp.DefaultIfEmpty()
                                                   join pc in db.CatParts on p.partID equals pc.partID
                                                   where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                                                   select new XElement("Part",
                                                      new XAttribute("partID", p.partID),
                                                      new XAttribute("status", p.status),
                                                      new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                      new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                      new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                      new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                      new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                      new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                      new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                      new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                      new XAttribute("vehicleID", db.VehicleParts.Where(x => x.vehicleID == vehicleID).Where(x => x.partID == p.partID).Select(x => x.vehicleID.ToString()).FirstOrDefault() ?? "0"),
                                                      new XElement("Attributes", (from pa in db.PartAttributes
                                                                                  where pa.partID.Equals(p.partID)
                                                                                  orderby pa.sort
                                                                                  select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                      new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                         join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                                                         where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                                                         orderby vpa.sort
                                                                                         select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                       new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                       new XElement("Videos", (from vp in p.PartVideos
                                                                               orderby vp.vTypeID
                                                                               select new XElement("Video", vp.video,
                                                                                   new XAttribute("videoID", vp.pVideoID),
                                                                                   new XAttribute("isPrimary", vp.isPrimary),
                                                                                   new XAttribute("type", vp.videoType.name),
                                                                                   new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                                   new XAttribute("typeID", vp.vTypeID)
                                                                               )).ToList<XElement>()),
                                                      new XElement("Packages", (from pp in p.PartPackages
                                                                                select new XElement("Package",
                                                                                    new XAttribute("height", pp.height),
                                                                                    new XAttribute("length", pp.length),
                                                                                    new XAttribute("width", pp.width),
                                                                                    new XAttribute("weight", pp.weight),
                                                                                    new XAttribute("quantity", pp.quantity),
                                                                                    new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                    new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                    new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                    new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                    new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                    new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                )).ToList<XElement>()),
                                                      new XElement("Pricing", (from pr in db.Prices
                                                                               where pr.partID.Equals(p.partID)
                                                                               orderby pr.priceType
                                                                               select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                      new XElement("Reviews",
                                                          new XAttribute("averageReview", (from r in db.Reviews
                                                                                           where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                           select (double?)r.rating).Average() ?? 0.0),
                                                                              (from r in db.Reviews
                                                                               where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                               orderby r.createdDate descending
                                                                               select new XElement("Review",
                                                                                   new XAttribute("reviewID", r.reviewID),
                                                                                   new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                   new XElement("rating", r.rating),
                                                                                   new XElement("name", r.name),
                                                                                   new XElement("email", r.email),
                                                                                   new XElement("subject", r.subject),
                                                                                   new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                                   )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                    #endregion
                } else {
                    #region Integrated
                    int count = (from p in db.Parts
                                 join c in db.Classes on p.classID equals c.classID into ClassTemp
                                 from c in ClassTemp.DefaultIfEmpty()
                                 join pc in db.CatParts on p.partID equals pc.partID
                                 join ci in db.CartIntegrations on p.partID equals ci.partID
                                 join cu in db.Customers on ci.custID equals cu.customerID
                                 where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                                 select p).Count();
                    if (perpage == 0) {
                        perpage = count;
                    }
                    parts = new XElement("Parts", new XAttribute("Count", count),
                                                  new XAttribute("Page", page),
                                                  new XAttribute("PerPage", perpage),
                                                  (from p in db.Parts
                                                   join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                   from c in ClassTemp.DefaultIfEmpty()
                                                   join pc in db.CatParts on p.partID equals pc.partID
                                                   join ci in db.CartIntegrations on p.partID equals ci.partID
                                                   join cu in db.Customers on ci.custID equals cu.customerID
                                                   where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                                                   select new XElement("Part",
                                                      new XAttribute("partID", p.partID),
                                                      new XAttribute("custPartID", ci.custPartID),
                                                      new XAttribute("status", p.status),
                                                      new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                      new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                      new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                      new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                      new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                      new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                      new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                      new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                      new XAttribute("vehicleID", db.VehicleParts.Where(x => x.vehicleID == vehicleID).Where(x => x.partID == p.partID).Select(x => x.vehicleID.ToString()).FirstOrDefault() ?? "0"),
                                                      new XElement("Attributes", (from pa in db.PartAttributes
                                                                                  where pa.partID.Equals(p.partID)
                                                                                  orderby pa.sort
                                                                                  select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                      new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                         join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                                                         where vp.vehicleID.Equals(vehicleID) && vp.partID.Equals(p.partID)
                                                                                         orderby vpa.sort
                                                                                         select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                       new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                       new XElement("Videos", (from vp in p.PartVideos
                                                                               orderby vp.vTypeID
                                                                               select new XElement("Video", vp.video,
                                                                                   new XAttribute("videoID", vp.pVideoID),
                                                                                   new XAttribute("isPrimary", vp.isPrimary),
                                                                                   new XAttribute("type", vp.videoType.name),
                                                                                   new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                                   new XAttribute("typeID", vp.vTypeID)
                                                                               )).ToList<XElement>()),
                                                      new XElement("Packages", (from pp in p.PartPackages
                                                                                select new XElement("Package",
                                                                                    new XAttribute("height", pp.height),
                                                                                    new XAttribute("length", pp.length),
                                                                                    new XAttribute("width", pp.width),
                                                                                    new XAttribute("weight", pp.weight),
                                                                                    new XAttribute("quantity", pp.quantity),
                                                                                    new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                    new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                    new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                    new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                    new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                    new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                )).ToList<XElement>()),
                                                      new XElement("Pricing", (from pr in db.Prices
                                                                               where pr.partID.Equals(p.partID)
                                                                               orderby pr.priceType
                                                                               select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                      new XElement("Reviews",
                                                          new XAttribute("averageReview", (from r in db.Reviews
                                                                                           where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                           select (double?)r.rating).Average() ?? 0.0),
                                                                              (from r in db.Reviews
                                                                               where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                               orderby r.createdDate descending
                                                                               select new XElement("Review",
                                                                                   new XAttribute("reviewID", r.reviewID),
                                                                                   new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                   new XElement("rating", r.rating),
                                                                                   new XElement("name", r.name),
                                                                                   new XElement("email", r.email),
                                                                                   new XElement("subject", r.subject),
                                                                                   new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                        new XElement("Images",
                                                                            (from pin in db.PartImages
                                                                             where pin.partID.Equals(p.partID)
                                                                             group pin by pin.sort into pi
                                                                             orderby pi.Key
                                                                             select new XElement("Index",
                                                                                                 new XAttribute("name", pi.Key.ToString()),
                                                                                                 (from pis in db.PartImages
                                                                                                  join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                                  where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                                  orderby pis.sort, pis.sizeID
                                                                                                  select new XElement(pig.size,
                                                                                                                      new XAttribute("imageID", pis.imageID),
                                                                                                                      new XAttribute("path", pis.path),
                                                                                                                      new XAttribute("height", pis.height),
                                                                                                                      new XAttribute("width", pis.width)
                                                                                                  )).ToList<XElement>()
                                                                         )).ToList<XElement>())
                                                   )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                    #endregion
                }
            }
            if (parts == null) {
                parts = new XElement("Part");
            }
            xml.Add(parts);
            return xml;
        }

        public static XDocument GetCategoryPartsWithFilterXML(int catID = 0, double year = 0, string make = "", string model = "", string style = "", List<int> statuses = null, int customerID = 0, bool integrated = false, int page = 1, int perpage = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (catID == 0) {
                #region no CatID
                int count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                             from pc in PartJoin.DefaultIfEmpty()
                             where pc.catID.Equals(null) && statuslist.Contains(p.status)
                             select p).Count();
                if (perpage == 0) {
                    perpage = count;
                }
                parts = new XElement("Parts", new XAttribute("Count", count),
                                              new XAttribute("Page", page),
                                              new XAttribute("PerPage", perpage),
                                             (from p in db.Parts
                                              join c in db.Classes on p.classID equals c.classID into ClassTemp
                                              from c in ClassTemp.DefaultIfEmpty()
                                              join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                                              from pc in PartJoin.DefaultIfEmpty()
                                              where pc.catID.Equals(null) && statuslist.Contains(p.status)
                                              select new XElement("Part",
                                                 new XAttribute("partID", p.partID),
                                                 new XAttribute("status", p.status),
                                                 new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                 new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                 new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                 new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                 new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                 new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                 new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                 new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                 new XAttribute("vehicleID", (from vep in db.VehicleParts
                                                                              join v in db.Vehicles on vep.vehicleID equals v.vehicleID
                                                                              join y in db.Years on v.yearID equals y.yearID
                                                                              join ma in db.Makes on v.makeID equals ma.makeID
                                                                              join mo in db.Models on v.modelID equals mo.modelID
                                                                              join s in db.Styles on v.styleID equals s.styleID
                                                                              where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style)
                                                                              select vep.vehicleID.ToString()).FirstOrDefault() ?? "0"),
                                                 new XElement("Attributes", (from pa in db.PartAttributes
                                                                             where pa.partID.Equals(p.partID)
                                                                             orderby pa.sort
                                                                             select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                 new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                    join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                                                    join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                                                    join y in db.Years on v.yearID equals y.yearID
                                                                                    join ma in db.Makes on v.makeID equals ma.makeID
                                                                                    join mo in db.Models on v.modelID equals mo.modelID
                                                                                    join s in db.Styles on v.styleID equals s.styleID
                                                                                    where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vp.partID.Equals(p.partID)
                                                                                    orderby vpa.sort
                                                                                    select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                new XElement("Content", (from co in p.ContentBridges
                                                                        orderby co.Content.ContentType.type, co.contentID
                                                                        select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                new XElement("Videos", (from vp in p.PartVideos
                                                                        orderby vp.vTypeID
                                                                        select new XElement("Video", vp.video,
                                                                            new XAttribute("videoID", vp.pVideoID),
                                                                            new XAttribute("isPrimary", vp.isPrimary),
                                                                            new XAttribute("type", vp.videoType.name),
                                                                            new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                            new XAttribute("typeID", vp.vTypeID)
                                                                        )).ToList<XElement>()),
                                                new XElement("Packages", (from pp in p.PartPackages
                                                                        select new XElement("Package",
                                                                            new XAttribute("height", pp.height),
                                                                            new XAttribute("length", pp.length),
                                                                            new XAttribute("width", pp.width),
                                                                            new XAttribute("weight", pp.weight),
                                                                            new XAttribute("quantity", pp.quantity),
                                                                            new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                            new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                            new XAttribute("weightUnit", pp.weightUnit.code),
                                                                            new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                            new XAttribute("packageUnit", pp.packageUnit.code),
                                                                            new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                        )).ToList<XElement>()),
                                                 new XElement("Pricing", (from pr in db.Prices
                                                                          where pr.partID.Equals(p.partID)
                                                                          orderby pr.priceType
                                                                          select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                 new XElement("Reviews",
                                                     new XAttribute("averageReview", (from r in db.Reviews
                                                                                      where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                      select (double?)r.rating).Average() ?? 0.0),
                                                                         (from r in db.Reviews
                                                                          where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                          orderby r.createdDate descending
                                                                          select new XElement("Review",
                                                                              new XAttribute("reviewID", r.reviewID),
                                                                              new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                              new XElement("rating", r.rating),
                                                                              new XElement("name", r.name),
                                                                              new XElement("email", r.email),
                                                                              new XElement("subject", r.subject),
                                                                              new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                new XElement("Images",
                                                                    (from pin in db.PartImages
                                                                     where pin.partID.Equals(p.partID)
                                                                     group pin by pin.sort into pi
                                                                     orderby pi.Key
                                                                     select new XElement("Index",
                                                                                         new XAttribute("name", pi.Key.ToString()),
                                                                                         (from pis in db.PartImages
                                                                                          join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                          where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                          orderby pis.sort, pis.sizeID
                                                                                          select new XElement(pig.size,
                                                                                                              new XAttribute("imageID", pis.imageID),
                                                                                                              new XAttribute("path", pis.path),
                                                                                                              new XAttribute("height", pis.height),
                                                                                                              new XAttribute("width", pis.width)
                                                                                          )).ToList<XElement>()
                                                                 )).ToList<XElement>())
                                              )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                #endregion
            } else {
                if (!integrated) {
                    #region Not Integrated
                    int count = (from p in db.Parts
                                 join c in db.Classes on p.classID equals c.classID into ClassTemp
                                 from c in ClassTemp.DefaultIfEmpty()
                                 join pc in db.CatParts on p.partID equals pc.partID
                                 where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                                 select p).Count();
                    if (perpage == 0) {
                        perpage = count;
                    }
                    parts = new XElement("Parts", new XAttribute("Count", count),
                                                  new XAttribute("Page", page),
                                                  new XAttribute("PerPage", perpage),
                                                  (from p in db.Parts
                                                   join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                   from c in ClassTemp.DefaultIfEmpty()
                                                   join pc in db.CatParts on p.partID equals pc.partID
                                                   where pc.catID.Equals(catID) && statuslist.Contains(p.status)
                                                   select new XElement("Part",
                                                      new XAttribute("partID", p.partID),
                                                      new XAttribute("status", p.status),
                                                      new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                      new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                      new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                      new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                      new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                      new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                      new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                      new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                      new XAttribute("vehicleID", (from vep in db.VehicleParts
                                                                                  join v in db.Vehicles on vep.vehicleID equals v.vehicleID
                                                                                  join y in db.Years on v.yearID equals y.yearID
                                                                                  join ma in db.Makes on v.makeID equals ma.makeID
                                                                                  join mo in db.Models on v.modelID equals mo.modelID
                                                                                  join s in db.Styles on v.styleID equals s.styleID
                                                                                  where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vep.partID.Equals(p.partID)
                                                                                  select vep.vehicleID.ToString()).FirstOrDefault() ?? "0"),
                                                      new XElement("Attributes", (from pa in db.PartAttributes
                                                                                  where pa.partID.Equals(p.partID)
                                                                                  orderby pa.sort
                                                                                  select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                      new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                         join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                                                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                                                         join y in db.Years on v.yearID equals y.yearID
                                                                                         join ma in db.Makes on v.makeID equals ma.makeID
                                                                                         join mo in db.Models on v.modelID equals mo.modelID
                                                                                         join s in db.Styles on v.styleID equals s.styleID
                                                                                         where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vp.partID.Equals(p.partID)
                                                                                         orderby vpa.sort
                                                                                         select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                       new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                       new XElement("Videos", (from vp in p.PartVideos
                                                                               orderby vp.vTypeID
                                                                               select new XElement("Video", vp.video,
                                                                                   new XAttribute("videoID", vp.pVideoID),
                                                                                   new XAttribute("isPrimary", vp.isPrimary),
                                                                                   new XAttribute("type", vp.videoType.name),
                                                                                   new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                                   new XAttribute("typeID", vp.vTypeID)
                                                                               )).ToList<XElement>()),
                                                      new XElement("Packages", (from pp in p.PartPackages
                                                                                select new XElement("Package",
                                                                                    new XAttribute("height", pp.height),
                                                                                    new XAttribute("length", pp.length),
                                                                                    new XAttribute("width", pp.width),
                                                                                    new XAttribute("weight", pp.weight),
                                                                                    new XAttribute("quantity", pp.quantity),
                                                                                    new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                    new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                    new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                    new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                    new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                    new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                )).ToList<XElement>()),
                                                      new XElement("Pricing", (from pr in db.Prices
                                                                               where pr.partID.Equals(p.partID)
                                                                               orderby pr.priceType
                                                                               select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                      new XElement("Reviews",
                                                          new XAttribute("averageReview", (from r in db.Reviews
                                                                                           where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                           select (double?)r.rating).Average() ?? 0.0),
                                                                              (from r in db.Reviews
                                                                               where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                               orderby r.createdDate descending
                                                                               select new XElement("Review",
                                                                                   new XAttribute("reviewID", r.reviewID),
                                                                                   new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                   new XElement("rating", r.rating),
                                                                                   new XElement("name", r.name),
                                                                                   new XElement("email", r.email),
                                                                                   new XElement("subject", r.subject),
                                                                                   new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                                   )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                    #endregion
                } else {
                    #region Integrated
                    int count = (from p in db.Parts
                                 join c in db.Classes on p.classID equals c.classID into ClassTemp
                                 from c in ClassTemp.DefaultIfEmpty()
                                 join pc in db.CatParts on p.partID equals pc.partID
                                 join ci in db.CartIntegrations on p.partID equals ci.partID
                                 join cu in db.Customers on ci.custID equals cu.customerID
                                 where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                                 select p).Count();
                    if (perpage == 0) {
                        perpage = count;
                    }
                    parts = new XElement("Parts", new XAttribute("Count", count),
                                                  new XAttribute("Page", page),
                                                  new XAttribute("PerPage", perpage),
                                                  (from p in db.Parts
                                                   join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                   from c in ClassTemp.DefaultIfEmpty()
                                                   join pc in db.CatParts on p.partID equals pc.partID
                                                   join ci in db.CartIntegrations on p.partID equals ci.partID
                                                   join cu in db.Customers on ci.custID equals cu.customerID
                                                   where pc.catID.Equals(catID) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status) && ci.custPartID > 0
                                                   select new XElement("Part",
                                                      new XAttribute("partID", p.partID),
                                                      new XAttribute("custPartID", ci.custPartID),
                                                      new XAttribute("status", p.status),
                                                      new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                      new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                      new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                      new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                      new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                      new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                      new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                      new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                      new XAttribute("vehicleID", (from vep in db.VehicleParts
                                                                                   join v in db.Vehicles on vep.vehicleID equals v.vehicleID
                                                                                   join y in db.Years on v.yearID equals y.yearID
                                                                                   join ma in db.Makes on v.makeID equals ma.makeID
                                                                                   join mo in db.Models on v.modelID equals mo.modelID
                                                                                   join s in db.Styles on v.styleID equals s.styleID
                                                                                   where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vep.partID.Equals(p.partID)
                                                                                   select vep.vehicleID.ToString()).FirstOrDefault() ?? "0"),
                                                      new XElement("Attributes", (from pa in db.PartAttributes
                                                                                  where pa.partID.Equals(p.partID)
                                                                                  orderby pa.sort
                                                                                  select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                      new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                         join vp in db.VehicleParts on vpa.vPartID equals vp.vPartID
                                                                                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                                                                                         join y in db.Years on v.yearID equals y.yearID
                                                                                         join ma in db.Makes on v.makeID equals ma.makeID
                                                                                         join mo in db.Models on v.modelID equals mo.modelID
                                                                                         join s in db.Styles on v.styleID equals s.styleID
                                                                                         where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && vp.partID.Equals(p.partID)
                                                                                         orderby vpa.sort
                                                                                         select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                       new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                       new XElement("Videos", (from vp in p.PartVideos
                                                                               orderby vp.vTypeID
                                                                               select new XElement("Video", vp.video,
                                                                                   new XAttribute("videoID", vp.pVideoID),
                                                                                   new XAttribute("isPrimary", vp.isPrimary),
                                                                                   new XAttribute("type", vp.videoType.name),
                                                                                   new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                                   new XAttribute("typeID", vp.vTypeID)
                                                                               )).ToList<XElement>()),
                                                      new XElement("Packages", (from pp in p.PartPackages
                                                                                select new XElement("Package",
                                                                                    new XAttribute("height", pp.height),
                                                                                    new XAttribute("length", pp.length),
                                                                                    new XAttribute("width", pp.width),
                                                                                    new XAttribute("weight", pp.weight),
                                                                                    new XAttribute("quantity", pp.quantity),
                                                                                    new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                    new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                    new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                    new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                    new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                    new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                )).ToList<XElement>()),
                                                      new XElement("Pricing", (from pr in db.Prices
                                                                               where pr.partID.Equals(p.partID)
                                                                               orderby pr.priceType
                                                                               select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                      new XElement("Reviews",
                                                          new XAttribute("averageReview", (from r in db.Reviews
                                                                                           where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                           select (double?)r.rating).Average() ?? 0.0),
                                                                              (from r in db.Reviews
                                                                               where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                               orderby r.createdDate descending
                                                                               select new XElement("Review",
                                                                                   new XAttribute("reviewID", r.reviewID),
                                                                                   new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                   new XElement("rating", r.rating),
                                                                                   new XElement("name", r.name),
                                                                                   new XElement("email", r.email),
                                                                                   new XElement("subject", r.subject),
                                                                                   new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                        new XElement("Images",
                                                                            (from pin in db.PartImages
                                                                             where pin.partID.Equals(p.partID)
                                                                             group pin by pin.sort into pi
                                                                             orderby pi.Key
                                                                             select new XElement("Index",
                                                                                                 new XAttribute("name", pi.Key.ToString()),
                                                                                                 (from pis in db.PartImages
                                                                                                  join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                                  where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                                  orderby pis.sort, pis.sizeID
                                                                                                  select new XElement(pig.size,
                                                                                                                      new XAttribute("imageID", pis.imageID),
                                                                                                                      new XAttribute("path", pis.path),
                                                                                                                      new XAttribute("height", pis.height),
                                                                                                                      new XAttribute("width", pis.width)
                                                                                                  )).ToList<XElement>()
                                                                         )).ToList<XElement>())
                                                   )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                    #endregion
                }
            }
            if (parts == null) {
                parts = new XElement("Part");
            }
            xml.Add(parts);
            return xml;
        }

        public static string GetCategoryPartsByNameJSON(string catName = "", List<int> statuses = null, int customerID = 0, bool integrated = false, int page = 1, int perpage = 0) {
            List<APIPart> part = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();
            int count = 0;

            if (!integrated) {
                #region Not Integrated
                count = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                         from pc in PartJoin.DefaultIfEmpty()
                         join cat in db.Categories on pc.catID equals cat.catID
                         where cat.catTitle.ToLower().Equals(catName.ToLower().Trim()) && statuslist.Contains(p.status)
                         select p).Count();
                if (perpage == 0) perpage = count;
                part = (from p in db.Parts
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        join pc in db.CatParts on p.partID equals pc.partID
                        join cat in db.Categories on pc.catID equals cat.catID
                        where cat.catTitle.ToLower().Equals(catName.ToLower().Trim()) && statuslist.Contains(p.status)
                        select new APIPart {
                            partID = p.partID,
                            status = p.status,
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = p.RelatedParts.Count,
                            attributes = (from pa in p.PartAttributes
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in p.Prices
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in p.Reviews
                                       where r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in p.Reviews
                                             where r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in p.PartImages
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = pi.PartImageSize.size,
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                count = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         join pc in db.CatParts on p.partID equals pc.partID into PartJoin
                         from pc in PartJoin.DefaultIfEmpty()
                         join cat in db.Categories on pc.catID equals cat.catID
                         where cat.catTitle.ToLower().Equals(catName.ToLower().Trim()) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status)
                         select p).Count();
                if (perpage == 0) perpage = count;
                part = (from p in db.Parts
                        join c in db.Classes on p.classID equals c.classID into ClassTemp
                        from c in ClassTemp.DefaultIfEmpty()
                        join ci in db.CartIntegrations on p.partID equals ci.partID
                        join cu in db.Customers on ci.custID equals cu.cust_id
                        join pc in db.CatParts on p.partID equals pc.partID
                        join cat in db.Categories on pc.catID equals cat.catID
                        where cat.catTitle.ToLower().Equals(catName.ToLower().Trim()) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status)
                        select new APIPart {
                            partID = p.partID,
                            custPartID = ci.custPartID,
                            status = p.status,
                            dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                            dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                            shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                            oldPartNumber = p.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, p.partID),
                            pClass = (c != null) ? c.class1 : "",
                            priceCode = p.priceCode,
                            relatedCount = p.RelatedParts.Count,
                            attributes = (from pa in p.PartAttributes
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            content = (from co in p.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in p.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in p.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in p.Prices
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in p.Reviews
                                       where r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in p.Reviews
                                             where r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in p.PartImages
                                      where pi.partID.Equals(p.partID)
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = pi.PartImageSize.size,
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        }).Skip((page - 1) * perpage).Take(perpage).ToList<APIPart>();

                #endregion
            }
            return JsonConvert.SerializeObject(part);
        }

        public static XDocument GetCategoryPartsByNameXML(string catName = "", List<int> statuses = null, int customerID = 0, bool integrated = false, int page = 1, int perpage = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                int count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join pc in db.CatParts on p.partID equals pc.partID
                             join cat in db.Categories on pc.catID equals cat.catID
                             where cat.catTitle.ToLower().Equals(catName.ToLower().Trim()) && statuslist.Contains(p.status)
                             select p).Count();
                if (perpage == 0) {
                    perpage = count;
                }
                parts = new XElement("Parts", new XAttribute("Count", count),
                                                new XAttribute("Page", page),
                                                new XAttribute("PerPage", perpage),
                                                (from p in db.Parts
                                                 join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                 from c in ClassTemp.DefaultIfEmpty()
                                                 join pc in db.CatParts on p.partID equals pc.partID
                                                 join cat in db.Categories on pc.catID equals cat.catID
                                                 where cat.catTitle.ToLower().Equals(catName.ToLower().Trim()) && statuslist.Contains(p.status)
                                                 select new XElement("Part",
                                                    new XAttribute("partID", p.partID),
                                                    new XAttribute("status", p.status),
                                                    new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                    new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                    new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                    new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                    new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                    new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                    new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                    new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                    new XElement("Attributes", (from pa in db.PartAttributes
                                                                                where pa.partID.Equals(p.partID)
                                                                                orderby pa.sort
                                                                                select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                    new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                    new XElement("Pricing", (from pr in db.Prices
                                                                             where pr.partID.Equals(p.partID)
                                                                             orderby pr.priceType
                                                                             select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                    new XElement("Reviews",
                                                        new XAttribute("averageReview", (from r in db.Reviews
                                                                                         where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                         select (double?)r.rating).Average() ?? 0.0),
                                                                            (from r in db.Reviews
                                                                             where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                             orderby r.createdDate descending
                                                                             select new XElement("Review",
                                                                                 new XAttribute("reviewID", r.reviewID),
                                                                                 new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                 new XElement("rating", r.rating),
                                                                                 new XElement("name", r.name),
                                                                                 new XElement("email", r.email),
                                                                                 new XElement("subject", r.subject),
                                                                                 new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                                )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                int count = (from p in db.Parts
                             join c in db.Classes on p.classID equals c.classID into ClassTemp
                             from c in ClassTemp.DefaultIfEmpty()
                             join pc in db.CatParts on p.partID equals pc.partID
                             join ci in db.CartIntegrations on p.partID equals ci.partID
                             join cu in db.Customers on ci.custID equals cu.customerID
                             join cat in db.Categories on pc.catID equals cat.catID
                             where cat.catTitle.ToLower().Equals(catName.ToLower().Trim()) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status)
                             select p).Count();
                if (perpage == 0) {
                    perpage = count;
                }
                parts = new XElement("Parts", new XAttribute("Count", count),
                                                new XAttribute("Page", page),
                                                new XAttribute("PerPage", perpage),
                                                (from p in db.Parts
                                                 join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                 from c in ClassTemp.DefaultIfEmpty()
                                                 join pc in db.CatParts on p.partID equals pc.partID
                                                 join ci in db.CartIntegrations on p.partID equals ci.partID
                                                 join cu in db.Customers on ci.custID equals cu.customerID
                                                 join cat in db.Categories on pc.catID equals cat.catID
                                                 where cat.catTitle.ToLower().Equals(catName.ToLower().Trim()) && cu.customerID.Equals(customerID) && statuslist.Contains(p.status)
                                                 select new XElement("Part",
                                                     new XAttribute("partID", p.partID),
                                                     new XAttribute("custPartID", ci.custPartID),
                                                     new XAttribute("status", p.status),
                                                     new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                     new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                     new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                     new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                     new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                     new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                     new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                     new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                     new XElement("Attributes", (from pa in db.PartAttributes
                                                                                 where pa.partID.Equals(p.partID)
                                                                                 orderby pa.sort
                                                                                 select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                   new XElement("Content", (from co in p.ContentBridges
                                                                            orderby co.Content.ContentType.type, co.contentID
                                                                            select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                   new XElement("Videos", (from vp in p.PartVideos
                                                                           orderby vp.vTypeID
                                                                           select new XElement("Video", vp.video,
                                                                               new XAttribute("videoID", vp.pVideoID),
                                                                               new XAttribute("isPrimary", vp.isPrimary),
                                                                               new XAttribute("type", vp.videoType.name),
                                                                               new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                               new XAttribute("typeID", vp.vTypeID)
                                                                           )).ToList<XElement>()),
                                                    new XElement("Packages", (from pp in p.PartPackages
                                                                            select new XElement("Package",
                                                                                new XAttribute("height", pp.height),
                                                                                new XAttribute("length", pp.length),
                                                                                new XAttribute("width", pp.width),
                                                                                new XAttribute("weight", pp.weight),
                                                                                new XAttribute("quantity", pp.quantity),
                                                                                new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                            )).ToList<XElement>()),
                                                     new XElement("Pricing", (from pr in db.Prices
                                                                              where pr.partID.Equals(p.partID)
                                                                              orderby pr.priceType
                                                                              select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                     new XElement("Reviews",
                                                         new XAttribute("averageReview", (from r in db.Reviews
                                                                                          where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                          select (double?)r.rating).Average() ?? 0.0),
                                                                             (from r in db.Reviews
                                                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                              orderby r.createdDate descending
                                                                              select new XElement("Review",
                                                                                  new XAttribute("reviewID", r.reviewID),
                                                                                  new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                  new XElement("rating", r.rating),
                                                                                  new XElement("name", r.name),
                                                                                  new XElement("email", r.email),
                                                                                  new XElement("subject", r.subject),
                                                                                  new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                    new XElement("Images",
                                                                        (from pin in db.PartImages
                                                                         where pin.partID.Equals(p.partID)
                                                                         group pin by pin.sort into pi
                                                                         orderby pi.Key
                                                                         select new XElement("Index",
                                                                                             new XAttribute("name", pi.Key.ToString()),
                                                                                             (from pis in db.PartImages
                                                                                              join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                              where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                              orderby pis.sort, pis.sizeID
                                                                                              select new XElement(pig.size,
                                                                                                                  new XAttribute("imageID", pis.imageID),
                                                                                                                  new XAttribute("path", pis.path),
                                                                                                                  new XAttribute("height", pis.height),
                                                                                                                  new XAttribute("width", pis.width)
                                                                                              )).ToList<XElement>()
                                                                     )).ToList<XElement>())
                                                 )).Skip((page - 1) * perpage).Take(perpage).ToList<XElement>());
                #endregion
            }
            if (parts == null) {
                parts = new XElement("Part");
            }
            xml.Add(parts);
            return xml;
        }

        public static string GetLifestylesJSON() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<APILifestyle> cats = new List<APILifestyle>();

            cats = (from c in db.Categories
                    where c.isLifestyle.Equals(true)
                    select new APILifestyle {
                        catID = c.catID,
                        catTitle = c.catTitle,
                        dateAdded = String.Format("{0:MM/dd/yyyy hh:mm tt}",c.dateAdded),
                        parentID = c.parentID,
                        shortDesc = c.shortDesc,
                        longDesc = c.longDesc,
                        image = c.image,
                        isLifestyle = c.isLifestyle,
                        sort = c.sort,
                        content = (from co in c.ContentBridges
                                   orderby co.Content.cTypeID
                                   select new APIContent {
                                       isHTML = co.Content.ContentType.allowHTML,
                                       type = co.Content.ContentType.type,
                                       content = co.Content.text
                                   }).ToList<APIContent>(),
                        towables = (from t in db.Trailers
                                    join lt in db.Lifestyle_Trailers on t.trailerID equals lt.trailerID
                                    where lt.catID == c.catID
                                    orderby t.TW
                                    select new APITowable {
                                        trailerID = t.trailerID,
                                        name = t.name,
                                        shortDesc = t.shortDesc,
                                        hitchClass = t.hitchClass,
                                        image = t.image,
                                        TW = t.TW,
                                        GTW = t.GTW,
                                        message = t.message
                                    }).ToList<APITowable>()
                    }).OrderBy(x => x.sort).ToList<APILifestyle>();
            return JsonConvert.SerializeObject(cats);
        }

        public static XDocument GetLifestylesXML() {
            CurtDevDataContext db = new CurtDevDataContext();
            XDocument xml = new XDocument();
            XElement cats = new XElement("Categories", (from c in db.Categories
                                                        where c.isLifestyle.Equals(true)
                                                        orderby c.sort
                                                        select new XElement("Category",
                                                            new XAttribute("CatID", c.catID),
                                                            new XAttribute("DateAdded", c.dateAdded),
                                                            new XAttribute("ParentID", c.parentID),
                                                            new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                                                            new XAttribute("ShortDesc", (c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                                                            new XAttribute("LongDesc", (c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                                                            new XAttribute("image", (c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                                                            new XAttribute("isLifestyle", c.isLifestyle),
                                                            new XElement("content", (from co in c.ContentBridges
                                                                                     orderby co.Content.cTypeID
                                                                                     select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type),
                                                                                            new XAttribute("isHTML", co.Content.ContentType.allowHTML),
                                                                                            co.Content.text)).ToList<XElement>()),
                                                            new XElement("towables", (from t in db.Trailers
                                                                                      join lt in db.Lifestyle_Trailers on t.trailerID equals lt.trailerID
                                                                                      where lt.catID == c.catID
                                                                                      orderby t.TW
                                                                                      select new XElement("towable",
                                                                                          new XAttribute("TrailerID", t.trailerID),
                                                                                          new XAttribute("name",t.name),
                                                                                          new XAttribute("image",t.image),
                                                                                          new XAttribute("shortDesc", t.shortDesc),
                                                                                          new XAttribute("hitchClass", t.hitchClass),
                                                                                          new XAttribute("TW",t.TW),
                                                                                          new XAttribute("GTW",t.GTW),
                                                                                          new XAttribute("message",t.message))).ToList<XElement>())
                                                        )));
            xml.Add(cats);
            return xml;
        }

        public static string GetLifestyleJSON(int id = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            APILifestyle cat = new APILifestyle();

            cat = (from c in db.Categories
                    where c.catID.Equals(id)
                    select new APILifestyle {
                        catID = c.catID,
                        catTitle = c.catTitle,
                        dateAdded = String.Format("{0:MM/dd/yyyy hh:mm tt}", c.dateAdded),
                        parentID = c.parentID,
                        shortDesc = c.shortDesc,
                        longDesc = c.longDesc,
                        image = c.image,
                        isLifestyle = c.isLifestyle,
                        sort = c.sort,
                        content = (from co in c.ContentBridges
                                   orderby co.Content.cTypeID
                                   select new APIContent {
                                       isHTML = co.Content.ContentType.allowHTML,
                                       type = co.Content.ContentType.type,
                                       content = co.Content.text
                                   }).ToList<APIContent>(),
                        towables = (from t in db.Trailers
                                    join lt in db.Lifestyle_Trailers on t.trailerID equals lt.trailerID
                                    where lt.catID == c.catID
                                    orderby t.TW
                                    select new APITowable {
                                        trailerID = t.trailerID,
                                        name = t.name,
                                        shortDesc = t.shortDesc,
                                        hitchClass = t.hitchClass,
                                        image = t.image,
                                        TW = t.TW,
                                        GTW = t.GTW,
                                        message = t.message
                                    }).ToList<APITowable>()
                    }).FirstOrDefault<APILifestyle>();
            return JsonConvert.SerializeObject(cat);
        }

        public static XDocument GetLifestyleXML(int id = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            XDocument xml = new XDocument();
            XElement cat = (from c in db.Categories
                                where c.catID.Equals(id)
                                orderby c.sort
                                select new XElement("Category",
                                    new XAttribute("CatID", c.catID),
                                    new XAttribute("DateAdded", c.dateAdded),
                                    new XAttribute("ParentID", c.parentID),
                                    new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                                    new XAttribute("ShortDesc", (c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                                    new XAttribute("LongDesc", (c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                                    new XAttribute("image", (c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                                    new XAttribute("isLifestyle", c.isLifestyle),
                                    new XElement("content", (from co in c.ContentBridges
                                                             orderby co.Content.cTypeID
                                                             select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), 
                                                                    new XAttribute("isHTML", co.Content.ContentType.allowHTML),
                                                                    co.Content.text)).ToList<XElement>()),
                                    new XElement("towables", (from t in db.Trailers
                                                                join lt in db.Lifestyle_Trailers on t.trailerID equals lt.trailerID
                                                                where lt.catID == c.catID
                                                                orderby t.TW
                                                                select new XElement("towable",
                                                                    new XAttribute("TrailerID", t.trailerID),
                                                                    new XAttribute("name", t.name),
                                                                    new XAttribute("image", t.image),
                                                                    new XAttribute("shortDesc", t.shortDesc),
                                                                    new XAttribute("hitchClass", t.hitchClass),
                                                                    new XAttribute("TW", t.TW),
                                                                    new XAttribute("GTW", t.GTW),
                                                                    new XAttribute("message", t.message))).ToList<XElement>())
                                )).FirstOrDefault<XElement>();
            xml.Add(cat);
            return xml;
        }

        public static string GetConnectorJSON(int vehicleID = 0, List<int> statuses = null, bool integrated = false, int customerID = 0) {
            List<APIPart> connectors = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                connectors = (from p in db.Parts
                              join c in db.Classes on p.classID equals c.classID into ClassTemp
                              from c in ClassTemp.DefaultIfEmpty()
                              join ct in db.CatParts on p.partID equals ct.partID
                              join cat in db.Categories on ct.catID equals cat.catID
                              join vp in db.VehicleParts on p.partID equals vp.partID
                              where vp.vehicleID.Equals(vehicleID) && c.class1.ToLower().Contains("wiring") && statuslist.Contains(p.status)
                              select new APIPart {
                                  partID = p.partID,
                                  status = p.status,
                                  dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                  dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                  shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                  oldPartNumber = p.oldPartNumber,
                                  listPrice = GetCustomerPrice(customerID, p.partID),
                                  pClass = (c != null) ? c.class1 : "",
                                  priceCode = p.priceCode,
                                  vehicleID = vp.vehicleID,
                                  installTime = (vp.installTime != null) ? (int)vp.installTime : 0,
                                  attributes = (from pa in db.PartAttributes
                                                where pa.partID.Equals(p.partID)
                                                orderby pa.sort
                                                select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                  vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                       where vpa.vPartID.Equals(vp.vPartID)
                                                       orderby vpa.sort
                                                       select new APIAttribute { key = vpa.field, value = vpa.value }).Distinct().ToList<APIAttribute>(),
                                  content = (from co in p.ContentBridges
                                             orderby co.contentID
                                             select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                  videos = (from pv in p.PartVideos
                                            orderby pv.vTypeID
                                            select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                  packages = (from pp in p.PartPackages
                                              select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                  pricing = (from pr in db.Prices
                                             where pr.partID.Equals(p.partID)
                                             select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                  images = (from pi in db.PartImages
                                            where pi.partID.Equals(p.partID)
                                            select new APIImage {
                                                imageID = pi.imageID,
                                                sort = pi.sort,
                                                path = pi.path,
                                                height = pi.height,
                                                width = pi.width,
                                                size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                                partID = pi.partID
                                            }).OrderBy(x => x.sort).ToList<APIImage>()
                              }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                connectors = (from p in db.Parts
                              join c in db.Classes on p.classID equals c.classID into ClassTemp
                              from c in ClassTemp.DefaultIfEmpty()
                              join ct in db.CatParts on p.partID equals ct.partID
                              join ci in db.CartIntegrations on p.partID equals ci.partID
                              join cu in db.Customers on ci.custID equals cu.customerID
                              join cat in db.Categories on ct.catID equals cat.catID
                              join vp in db.VehicleParts on p.partID equals vp.partID
                              where vp.vehicleID.Equals(vehicleID) && c.class1.ToLower().Contains("wiring") && cu.customerID.Equals(customerID) && statuslist.Contains(p.status)
                              select new APIPart {
                                  partID = p.partID,
                                  custPartID = ci.custPartID,
                                  status = p.status,
                                  dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                  dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                  shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                  oldPartNumber = p.oldPartNumber,
                                  listPrice = GetCustomerPrice(customerID, p.partID),
                                  pClass = (c != null) ? c.class1 : "",
                                  priceCode = p.priceCode,
                                  vehicleID = vp.vehicleID,
                                  installTime = (vp.installTime != null) ? (int)vp.installTime : 0,
                                  attributes = (from pa in db.PartAttributes
                                                where pa.partID.Equals(p.partID)
                                                orderby pa.sort
                                                select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                  vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                       where vpa.vPartID.Equals(vp.vPartID)
                                                       orderby vpa.sort
                                                       select new APIAttribute { key = vpa.field, value = vpa.value }).Distinct().ToList<APIAttribute>(),
                                  content = (from co in p.ContentBridges
                                             orderby co.contentID
                                             select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                  videos = (from pv in p.PartVideos
                                            orderby pv.vTypeID
                                            select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                  packages = (from pp in p.PartPackages
                                              select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                  pricing = (from pr in db.Prices
                                             where pr.partID.Equals(p.partID)
                                             select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                  images = (from pi in db.PartImages
                                            where pi.partID.Equals(p.partID)
                                            select new APIImage {
                                                imageID = pi.imageID,
                                                sort = pi.sort,
                                                path = pi.path,
                                                height = pi.height,
                                                width = pi.width,
                                                size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                                partID = pi.partID
                                            }).OrderBy(x => x.sort).ToList<APIImage>()
                              }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(connectors);
        }

        public static XDocument GetConnectorXML(int vehicleID = 0, List<int> statuses = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement connectors = new XElement("Connectors");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                connectors = new XElement("Connectors", (from p in db.Parts
                                                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                         from c in ClassTemp.DefaultIfEmpty()
                                                         join ct in db.CatParts on p.partID equals ct.partID
                                                         join cat in db.Categories on ct.catID equals cat.catID
                                                         join vp in db.VehicleParts on p.partID equals vp.partID
                                                         where vp.vehicleID.Equals(vehicleID) && c.class1.ToLower().Contains("wiring") && statuslist.Contains(p.status)
                                                         select new XElement("Connector",
                                                             new XAttribute("partID", p.partID),
                                                       new XAttribute("status", p.status),
                                                       new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                       new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                       new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                       new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                       new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                       new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                       new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                       new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                       new XAttribute("vehicleID", vp.vehicleID),
                                                       new XAttribute("installTime", (vp.installTime != null) ? vp.installTime.ToString() : ""),
                                                       new XElement("Attributes", (from pa in db.PartAttributes
                                                                                   where pa.partID.Equals(p.partID)
                                                                                   orderby pa.sort
                                                                                   select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                       new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                          where vpa.vPartID.Equals(vp.vPartID)
                                                                                          orderby vpa.sort
                                                                                          select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                       new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                       new XElement("Videos", (from pv in p.PartVideos
                                                                               orderby pv.vTypeID
                                                                               select new XElement("Video", pv.video,
                                                                                   new XAttribute("videoID", pv.pVideoID),
                                                                                   new XAttribute("isPrimary", pv.isPrimary),
                                                                                   new XAttribute("type", pv.videoType.name),
                                                                                   new XAttribute("icon", pv.videoType.icon),
                                                                                   new XAttribute("typeID", pv.vTypeID)
                                                                               )).ToList<XElement>()),
                                                      new XElement("Packages", (from pp in p.PartPackages
                                                                                select new XElement("Package",
                                                                                    new XAttribute("height", pp.height),
                                                                                    new XAttribute("length", pp.length),
                                                                                    new XAttribute("width", pp.width),
                                                                                    new XAttribute("weight", pp.weight),
                                                                                    new XAttribute("quantity", pp.quantity),
                                                                                    new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                    new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                    new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                    new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                    new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                    new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                )).ToList<XElement>()),
                                                       new XElement("Pricing", (from pr in db.Prices
                                                                                where pr.partID.Equals(p.partID)
                                                                                orderby pr.priceType
                                                                                select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                        new XElement("Images",
                                                                            (from pin in db.PartImages
                                                                             where pin.partID.Equals(p.partID)
                                                                             group pin by pin.sort into pi
                                                                             orderby pi.Key
                                                                             select new XElement("Index",
                                                                                                 new XAttribute("name", pi.Key.ToString()),
                                                                                                 (from pis in db.PartImages
                                                                                                  join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                                  where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                                  orderby pis.sort, pis.sizeID
                                                                                                  select new XElement(pig.size,
                                                                                                                      new XAttribute("imageID", pis.imageID),
                                                                                                                      new XAttribute("path", pis.path),
                                                                                                                      new XAttribute("height", pis.height),
                                                                                                                      new XAttribute("width", pis.width)
                                                                                                  )).ToList<XElement>()
                                                                         )).ToList<XElement>())
                                                         )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                connectors = new XElement("Connectors", (from p in db.Parts
                                                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                         from c in ClassTemp.DefaultIfEmpty()
                                                         join ci in db.CartIntegrations on p.partID equals ci.partID
                                                         join cu in db.Customers on ci.custID equals cu.customerID
                                                         join ct in db.CatParts on p.partID equals ct.partID
                                                         join cat in db.Categories on ct.catID equals cat.catID
                                                         join vp in db.VehicleParts on p.partID equals vp.partID
                                                         where vp.vehicleID.Equals(vehicleID) && c.class1.ToLower().Contains("wiring") && cu.customerID.Equals(customerID) && statuslist.Contains(p.status)
                                                         select new XElement("Connector",
                                                               new XAttribute("partID", p.partID),
                                                               new XAttribute("custPartID", ci.custPartID),
                                                               new XAttribute("status", p.status),
                                                               new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                               new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                               new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                               new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                               new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                                                               new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                               new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                               new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                               new XAttribute("vehicleID", vp.vehicleID),
                                                               new XAttribute("installTime", (vp.installTime != null) ? vp.installTime.ToString() : ""),
                                                               new XElement("Attributes", (from pa in db.PartAttributes
                                                                                           where pa.partID.Equals(p.partID)
                                                                                           orderby pa.sort
                                                                                           select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                               new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                                                                  where vpa.vPartID.Equals(vp.vPartID)
                                                                                                  orderby vpa.sort
                                                                                                  select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                                                               new XElement("Content", (from co in p.ContentBridges
                                                                                        orderby co.Content.ContentType.type, co.contentID
                                                                                        select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                               new XElement("Videos", (from pv in p.PartVideos
                                                                                       orderby pv.vTypeID
                                                                                       select new XElement("Video", pv.video,
                                                                                           new XAttribute("videoID", pv.pVideoID),
                                                                                           new XAttribute("isPrimary", pv.isPrimary),
                                                                                           new XAttribute("type", pv.videoType.name),
                                                                                           new XAttribute("icon", pv.videoType.icon),
                                                                                           new XAttribute("typeID", pv.vTypeID)
                                                                                       )).ToList<XElement>()),
                                                              new XElement("Packages", (from pp in p.PartPackages
                                                                                        select new XElement("Package",
                                                                                            new XAttribute("height", pp.height),
                                                                                            new XAttribute("length", pp.length),
                                                                                            new XAttribute("width", pp.width),
                                                                                            new XAttribute("weight", pp.weight),
                                                                                            new XAttribute("quantity", pp.quantity),
                                                                                            new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                                            new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                                            new XAttribute("weightUnit", pp.weightUnit.code),
                                                                                            new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                                            new XAttribute("packageUnit", pp.packageUnit.code),
                                                                                            new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                                        )).ToList<XElement>()),
                                                               new XElement("Pricing", (from pr in db.Prices
                                                                                        where pr.partID.Equals(p.partID)
                                                                                        orderby pr.priceType
                                                                                        select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                                new XElement("Images",
                                                                                    (from pin in db.PartImages
                                                                                     where pin.partID.Equals(p.partID)
                                                                                     group pin by pin.sort into pi
                                                                                     orderby pi.Key
                                                                                     select new XElement("Index",
                                                                                                         new XAttribute("name", pi.Key.ToString()),
                                                                                                         (from pis in db.PartImages
                                                                                                          join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                                          where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                                          orderby pis.sort, pis.sizeID
                                                                                                          select new XElement(pig.size,
                                                                                                                              new XAttribute("imageID", pis.imageID),
                                                                                                                              new XAttribute("path", pis.path),
                                                                                                                              new XAttribute("height", pis.height),
                                                                                                                              new XAttribute("width", pis.width)
                                                                                                          )).ToList<XElement>()
                                                                                 )).ToList<XElement>())
                                                         )).ToList<XElement>());
                #endregion
            }

            if (connectors == null) {
                connectors = new XElement("Connectors", "No connector found.");
            }
            xml.Add(connectors);
            return xml;
        }

        public static string GetConnectorBySpecJSON(double year, string make, string model, string style, List<int> statuses = null, bool integrated = false, int customerID = 0) {
            List<APIPart> connectors = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                connectors = (from p in db.Parts
                              join c in db.Classes on p.classID equals c.classID into ClassTemp
                              from c in ClassTemp.DefaultIfEmpty()
                              join ct in db.CatParts on p.partID equals ct.partID
                              join cat in db.Categories on ct.catID equals cat.catID
                              join vp in db.VehicleParts on p.partID equals vp.partID
                              join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                              join y in db.Years on v.yearID equals y.yearID
                              join ma in db.Makes on v.makeID equals ma.makeID
                              join mo in db.Models on v.modelID equals mo.modelID
                              join s in db.Styles on v.styleID equals s.styleID
                              where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && c.class1.ToLower().Contains("wiring") && statuslist.Contains(p.status)
                              select new APIPart {
                                  partID = p.partID,
                                  status = p.status,
                                  dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                  dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                  shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                  oldPartNumber = p.oldPartNumber,
                                  listPrice = GetCustomerPrice(customerID, p.partID),
                                  pClass = (c != null) ? c.class1 : "",
                                  priceCode = p.priceCode,
                                  vehicleID = vp.vehicleID,
                                  installTime = (vp.installTime != null) ? (int)vp.installTime : 0,
                                  attributes = (from pa in db.PartAttributes
                                                where pa.partID.Equals(p.partID)
                                                orderby pa.sort
                                                select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                  vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                       where vpa.vPartID.Equals(vp.vPartID)
                                                       orderby vpa.sort
                                                       select new APIAttribute { key = vpa.field, value = vpa.value }).Distinct().ToList<APIAttribute>(),
                                  content = (from co in p.ContentBridges
                                             orderby co.contentID
                                             select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                  videos = (from pv in p.PartVideos
                                            orderby pv.vTypeID
                                            select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                  packages = (from pp in p.PartPackages
                                              select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                  pricing = (from pr in db.Prices
                                             where pr.partID.Equals(p.partID)
                                             select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                  images = (from pi in db.PartImages
                                            where pi.partID.Equals(p.partID)
                                            select new APIImage {
                                                imageID = pi.imageID,
                                                sort = pi.sort,
                                                path = pi.path,
                                                height = pi.height,
                                                width = pi.width,
                                                size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                                partID = pi.partID
                                            }).OrderBy(x => x.sort).ToList<APIImage>()
                              }).ToList<APIPart>();
                #endregion
            } else {
                #region Integrated
                connectors = (from p in db.Parts
                              join c in db.Classes on p.classID equals c.classID into ClassTemp
                              from c in ClassTemp.DefaultIfEmpty()
                              join ct in db.CatParts on p.partID equals ct.partID
                              join ci in db.CartIntegrations on p.partID equals ci.partID
                              join cu in db.Customers on ci.custID equals cu.customerID
                              join cat in db.Categories on ct.catID equals cat.catID
                              join vp in db.VehicleParts on p.partID equals vp.partID
                              join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                              join y in db.Years on v.yearID equals y.yearID
                              join ma in db.Makes on v.makeID equals ma.makeID
                              join mo in db.Models on v.modelID equals mo.modelID
                              join s in db.Styles on v.styleID equals s.styleID
                              where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && c.class1.ToLower().Contains("wiring") && cu.customerID.Equals(customerID) && statuslist.Contains(p.status)
                              select new APIPart {
                                  partID = p.partID,
                                  custPartID = ci.custPartID,
                                  status = p.status,
                                  dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                                  dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                                  shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                                  oldPartNumber = p.oldPartNumber,
                                  listPrice = GetCustomerPrice(customerID, p.partID),
                                  pClass = (c != null) ? c.class1 : "",
                                  priceCode = p.priceCode,
                                  vehicleID = vp.vehicleID,
                                  installTime = (vp.installTime != null) ? (int)vp.installTime : 0,
                                  attributes = (from pa in db.PartAttributes
                                                where pa.partID.Equals(p.partID)
                                                orderby pa.sort
                                                select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                  vehicleAttributes = (from vpa in db.VehiclePartAttributes
                                                       where vpa.vPartID.Equals(vp.vPartID)
                                                       orderby vpa.sort
                                                       select new APIAttribute { key = vpa.field, value = vpa.value }).Distinct().ToList<APIAttribute>(),
                                  content = (from co in p.ContentBridges
                                             orderby co.contentID
                                             select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                  videos = (from pv in p.PartVideos
                                            orderby pv.vTypeID
                                            select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                  packages = (from pp in p.PartPackages
                                              select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                  pricing = (from pr in db.Prices
                                             where pr.partID.Equals(p.partID)
                                             select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                  images = (from pi in db.PartImages
                                            where pi.partID.Equals(p.partID)
                                            select new APIImage {
                                                imageID = pi.imageID,
                                                sort = pi.sort,
                                                path = pi.path,
                                                height = pi.height,
                                                width = pi.width,
                                                size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                                partID = pi.partID
                                            }).OrderBy(x => x.sort).ToList<APIImage>()
                              }).ToList<APIPart>();
                #endregion
            }
            return JsonConvert.SerializeObject(connectors);
        }

        public static XDocument GetConnectorBySpecXML(double year, string make, string model, string style, List<int> statuses = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            CurtDevDataContext db = new CurtDevDataContext();
            XElement connectors = new XElement("Connectors");
            List<int> statuslist = statuses ?? new List<int>();

            if (!integrated) {
                #region Not Integrated
                connectors = new XElement("Connectors", new List<XElement>
                    (from p in db.Parts
                     join c in db.Classes on p.classID equals c.classID into ClassTemp
                     from c in ClassTemp.DefaultIfEmpty()
                     join ct in db.CatParts on p.partID equals ct.partID
                     join cat in db.Categories on ct.catID equals cat.catID
                     join vp in db.VehicleParts on p.partID equals vp.partID
                     join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                     join y in db.Years on v.yearID equals y.yearID
                     join ma in db.Makes on v.makeID equals ma.makeID
                     join mo in db.Models on v.modelID equals mo.modelID
                     join s in db.Styles on v.styleID equals s.styleID
                     where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && c.class1.ToLower().Contains("wiring") && statuslist.Contains(p.status)
                     select new XElement("Connector",
                         new XAttribute("partID", p.partID),
                         new XAttribute("status", p.status),
                         new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                         new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                         new XAttribute("shortDesc", HttpUtility.HtmlEncode("CURT " + p.shortDesc + " #" + p.partID.ToString())),
                         new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber.ToString() : ""),
                         new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                         new XAttribute("pClass", (c != null) ? c.class1 : ""),
                         new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                         new XAttribute("vehicleID", vp.vehicleID),
                         new XAttribute("installTime", (vp.installTime != null) ? vp.installTime.ToString() : ""),
                         new XElement("Attributes", new List<XElement>(from pa in db.PartAttributes
                                                                       where pa.partID.Equals(p.partID)
                                                                       orderby pa.sort
                                                                       select new XElement(
                                                                           XmlConvert.EncodeName(pa.field),
                                                                           HttpUtility.HtmlEncode(pa.value)
                                                                       )).ToList<XElement>()),
                        new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                           where vpa.vPartID.Equals(vp.vPartID)
                                                           orderby vpa.sort
                                                           select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                        new XElement("Content", (from co in p.ContentBridges
                                                orderby co.Content.ContentType.type, co.contentID
                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                        new XElement("Videos", (from pv in p.PartVideos
                                                orderby pv.vTypeID
                                                select new XElement("Video", pv.video,
                                                    new XAttribute("videoID", pv.pVideoID),
                                                    new XAttribute("isPrimary", pv.isPrimary),
                                                    new XAttribute("type", pv.videoType.name),
                                                    new XAttribute("icon", pv.videoType.icon),
                                                    new XAttribute("typeID", pv.vTypeID)
                                                )).ToList<XElement>()),
                        new XElement("Packages", (from pp in p.PartPackages
                                                select new XElement("Package",
                                                    new XAttribute("height", pp.height),
                                                    new XAttribute("length", pp.length),
                                                    new XAttribute("width", pp.width),
                                                    new XAttribute("weight", pp.weight),
                                                    new XAttribute("quantity", pp.quantity),
                                                    new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                    new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                    new XAttribute("weightUnit", pp.weightUnit.code),
                                                    new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                    new XAttribute("packageUnit", pp.packageUnit.code),
                                                    new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                )).ToList<XElement>()),
                        new XElement("Pricing", (from pr in db.Prices
                                                where pr.partID.Equals(p.partID)
                                                orderby pr.priceType
                                                select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                        new XElement("Images",
                                            (from pin in db.PartImages
                                                where pin.partID.Equals(p.partID)
                                                group pin by pin.sort into pi
                                                orderby pi.Key
                                                select new XElement("Index",
                                                                    new XAttribute("name", pi.Key.ToString()),
                                                                    (from pis in db.PartImages
                                                                    join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                    where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                    orderby pis.sort, pis.sizeID
                                                                    select new XElement(pig.size,
                                                                                        new XAttribute("imageID", pis.imageID),
                                                                                        new XAttribute("path", pis.path),
                                                                                        new XAttribute("height", pis.height),
                                                                                        new XAttribute("width", pis.width)
                                                                    )).ToList<XElement>()
                                            )).ToList<XElement>())
                     )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                connectors = new XElement("Connectors", new List<XElement>
                    (from p in db.Parts
                     join c in db.Classes on p.classID equals c.classID into ClassTemp
                     from c in ClassTemp.DefaultIfEmpty()
                     join ct in db.CatParts on p.partID equals ct.partID
                     join ci in db.CartIntegrations on p.partID equals ci.partID
                     join cu in db.Customers on ci.custID equals cu.customerID
                     join cat in db.Categories on ct.catID equals cat.catID
                     join vp in db.VehicleParts on p.partID equals vp.partID
                     join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                     join y in db.Years on v.yearID equals y.yearID
                     join ma in db.Makes on v.makeID equals ma.makeID
                     join mo in db.Models on v.modelID equals mo.modelID
                     join s in db.Styles on v.styleID equals s.styleID
                     where y.year1.Equals(year) && ma.make1.Equals(make) && mo.model1.Equals(model) && s.style1.Equals(style) && c.class1.ToLower().Contains("wiring") && cu.customerID.Equals(customerID) && statuslist.Contains(p.status)
                     select new XElement("Connector",
                         new XAttribute("partID", p.partID),
                         new XAttribute("custPartID", ci.custPartID),
                         new XAttribute("status", p.status),
                         new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                         new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                         new XAttribute("shortDesc", HttpUtility.HtmlEncode("CURT " + p.shortDesc + " #" + p.partID.ToString())),
                         new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber.ToString() : ""),
                         new XAttribute("listPrice", GetCustomerPrice(customerID, p.partID)),
                         new XAttribute("pClass", (c != null) ? c.class1 : ""),
                         new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                         new XAttribute("vehicleID", vp.vehicleID),
                         new XAttribute("installTime", (vp.installTime != null) ? vp.installTime.ToString() : ""),
                         new XElement("Attributes", new List<XElement>(from pa in db.PartAttributes
                                                                       where pa.partID.Equals(p.partID)
                                                                       orderby pa.sort
                                                                       select new XElement(
                                                                           XmlConvert.EncodeName(pa.field),
                                                                           HttpUtility.HtmlEncode(pa.value)
                                                                       )).ToList<XElement>()),
                        new XElement("VehicleAttributes", (from vpa in db.VehiclePartAttributes
                                                           where vpa.vPartID.Equals(vp.vPartID)
                                                           orderby vpa.sort
                                                           select new XElement(XmlConvert.EncodeName(vpa.field), vpa.value)).ToList<XElement>()),
                        new XElement("Content", (from co in p.ContentBridges
                                                orderby co.Content.ContentType.type, co.contentID
                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                        new XElement("Videos", (from pv in p.PartVideos
                                                orderby pv.vTypeID
                                                select new XElement("Video", pv.video,
                                                    new XAttribute("videoID", pv.pVideoID),
                                                    new XAttribute("isPrimary", pv.isPrimary),
                                                    new XAttribute("type", pv.videoType.name),
                                                    new XAttribute("icon", pv.videoType.icon),
                                                    new XAttribute("typeID", pv.vTypeID)
                                                )).ToList<XElement>()),
                        new XElement("Packages", (from pp in p.PartPackages
                                                  select new XElement("Package",
                                                      new XAttribute("height", pp.height),
                                                      new XAttribute("length", pp.length),
                                                      new XAttribute("width", pp.width),
                                                      new XAttribute("weight", pp.weight),
                                                      new XAttribute("quantity", pp.quantity),
                                                      new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                      new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                      new XAttribute("weightUnit", pp.weightUnit.code),
                                                      new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                      new XAttribute("packageUnit", pp.packageUnit.code),
                                                      new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                  )).ToList<XElement>()),
                       new XElement("Pricing", new List<XElement>(from pr in db.Prices
                                                                  where pr.partID.Equals(p.partID)
                                                                  orderby pr.priceType
                                                                  select new XElement(
                                                                      XmlConvert.EncodeName(pr.priceType),
                                                                      pr.price1.ToString()
                                                                  )).ToList<XElement>()),
                        new XElement("Images",
                                            (from pin in db.PartImages
                                             where pin.partID.Equals(p.partID)
                                             group pin by pin.sort into pi
                                             orderby pi.Key
                                             select new XElement("Index",
                                                                 new XAttribute("name", pi.Key.ToString()),
                                                                 (from pis in db.PartImages
                                                                  join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                  where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                  orderby pis.sort, pis.sizeID
                                                                  select new XElement(pig.size,
                                                                                      new XAttribute("imageID", pis.imageID),
                                                                                      new XAttribute("path", pis.path),
                                                                                      new XAttribute("height", pis.height),
                                                                                      new XAttribute("width", pis.width)
                                                                  )).ToList<XElement>()
                                         )).ToList<XElement>())
                     )).ToList<XElement>());
                #endregion
            }
            if (connectors == null) {
                connectors = new XElement("Connectors", "No Connectors Found");
            }
            xml.Add(connectors);
            return xml;

        }

        public static string GetCategoryByNameJSON(string catName) {
            CurtDevDataContext db = new CurtDevDataContext();
            FullCategory cat = new FullCategory();
            cat = (from c in db.Categories
                   where c.catTitle.Equals(catName)
                   select new FullCategory {
                       parent = c,
                       content = (from co in c.ContentBridges
                                  orderby co.Content.cTypeID
                                  select new APIContent {
                                      isHTML = co.Content.ContentType.allowHTML,
                                      type = co.Content.ContentType.type,
                                      content = co.Content.text
                                  }).ToList<APIContent>(),
                       sub_categories = (from c2 in db.Categories
                                         where c2.parentID.Equals(c.catID)
                                         orderby c2.sort, c2.catID
                                         select c2).ToList<Categories>()
                   }).FirstOrDefault<FullCategory>();
            return JsonConvert.SerializeObject(cat);
        }

        public static XDocument GetCategoryByNameXML(string catName) {
            CurtDevDataContext db = new CurtDevDataContext();
            XElement cat = null;
            try {
                cat = (from c in db.Categories
                       where c.catTitle.Equals(catName)
                       select new XElement("Category",
                           new XAttribute("CatID", c.catID),
                           new XAttribute("DateAdded", c.dateAdded),
                           new XAttribute("ParentID", c.parentID),
                           new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                           new XAttribute("ShortDesc", (c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                           new XAttribute("LongDesc", (c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                           new XAttribute("image", (c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                           new XAttribute("isLifestyle", c.isLifestyle),
                           new XAttribute("vehicleSpecific", c.vehicleSpecific),
                           new XElement("Content", (from co in c.ContentBridges
                                                    orderby co.Content.ContentType.type, co.contentID
                                                    select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                           new XElement("SubCategories", new List<XElement>(from c2 in db.Categories
                                                                            where c2.parentID.Equals(c.catID)
                                                                            orderby c2.sort, c2.catID
                                                                            select new XElement("SubCategory",
                                                                               new XAttribute("CatID", c2.catID),
                                                                               new XAttribute("DateAdded", c2.dateAdded),
                                                                               new XAttribute("ParentID", c2.parentID),
                                                                               new XAttribute("CategoryName", c2.catTitle.ToString().Trim()),
                                                                               new XAttribute("ShortDesc", (c2.shortDesc.Length > 0) ? c2.shortDesc.ToString().Trim() : ""),
                                                                               new XAttribute("LongDesc", (c2.longDesc.Length > 0) ? c2.longDesc.ToString().Trim() : ""),
                                                                               new XAttribute("image", (c2.image.Length > 0) ? c2.image.ToString().Trim() : ""),
                                                                               new XAttribute("isLifestyle", c2.isLifestyle))).ToList<XElement>())
                       )).FirstOrDefault<XElement>();
            } catch { };
            if (cat == null) {
                cat = new XElement("Category");
            }
            XDocument xml = new XDocument();
            xml.Add(cat);
            return xml;
        }

        public static string GetCategoryAttributesJSON(int catID) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<string> attributes = new List<string>();
            attributes = (from pa in db.PartAttributes
                          join p in db.Parts on pa.partID equals p.partID
                          join cp in db.CatParts on p.partID equals cp.partID
                          where cp.catID == catID && (p.status.Equals(800) || p.status.Equals(900))
                          select pa.field).Distinct().ToList<string>();

            return JsonConvert.SerializeObject(attributes);
        }

        public static XDocument GetCategoryAttributesXML(int catID) {
            CurtDevDataContext db = new CurtDevDataContext();
            XElement attributes = new XElement("Attributes", (from pa in db.PartAttributes
                                                              join p in db.Parts on pa.partID equals p.partID
                                                              join cp in db.CatParts on p.partID equals cp.partID
                                                              where cp.catID == catID && (p.status.Equals(800) || p.status.Equals(900))
                                                              select new XElement("Attribute", pa.field)).Distinct().ToList<XElement>());
            if (attributes == null) {
                attributes = new XElement("Attributes");
            }
            XDocument xml = new XDocument();
            xml.Add(attributes);
            return xml;
        }

        public static string GetCategoryBreadCrumbsJSON(int catID) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<Categories> categories = new List<Categories>();
            try {
                categories.Add(db.Categories.Where(x => x.catID == catID).FirstOrDefault<Categories>());
                bool more = (categories[0].parentID == 0) ? false : true;
                while (more) {
                    categories.Add(db.Categories.Where(x => x.catID == categories[categories.Count() - 1].parentID).FirstOrDefault<Categories>());
                    if (categories[categories.Count() - 1].parentID == 0) {
                        more = false;
                    }
                }
            } catch { }
            return JsonConvert.SerializeObject(categories);
        }

        public static XDocument GetCategoryBreadCrumbsXML(int catID) {
            CurtDevDataContext db = new CurtDevDataContext();
            XDocument xml = new XDocument();
            List<Categories> catlist = new List<Categories>();
            try {
                catlist.Add(db.Categories.Where(x => x.catID == catID).FirstOrDefault<Categories>());
                bool more = (catlist[0].parentID == 0) ? false : true;
                while (more) {
                    catlist.Add(db.Categories.Where(x => x.catID == catlist[catlist.Count() - 1].parentID).FirstOrDefault<Categories>());
                    if (catlist[catlist.Count() - 1].parentID == 0) {
                        more = false;
                    }
                }
            } catch { }
            XElement cats = new XElement("Categories", new List<XElement>(from c in catlist.AsQueryable()
                                                                          select new XElement("Category",
                                                                             new XAttribute("CatID", c.catID),
                                                                             new XAttribute("DateAdded", c.dateAdded),
                                                                             new XAttribute("ParentID", c.parentID),
                                                                             new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                                                                             new XAttribute("ShortDesc", (c.shortDesc != null && c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                                                                             new XAttribute("LongDesc", (c.longDesc != null && c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                                                                             new XAttribute("image", (c.image != null && c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                                                                             new XAttribute("isLifestyle", c.isLifestyle),
                                                                             new XAttribute("vehicleSpecific", c.vehicleSpecific))).ToList<XElement>());
            xml.Add(cats);
            return xml;
        }

        public static string GetPartBreadCrumbsJSON(int partID, int catID) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<Categories> categories = new List<Categories>();
            try {
                List<int> catids = db.CatParts.Where(x => x.partID == partID).Select(x => x.catID).ToList();
                if (!catids.Contains(catID)) {
                    catID = catids.First();
                }
                categories.Add(db.Categories.Where(x => x.catID == catID).FirstOrDefault<Categories>());
                bool more = (categories[0].parentID == 0) ? false : true;
                while (more) {
                    categories.Add(db.Categories.Where(x => x.catID == categories[categories.Count() - 1].parentID).FirstOrDefault<Categories>());
                    if (categories[categories.Count() - 1].parentID == 0) {
                        more = false;
                    }
                }
            } catch { }
            return JsonConvert.SerializeObject(categories);
        }

        public static XDocument GetPartBreadCrumbsXML(int partID, int catID) {
            CurtDevDataContext db = new CurtDevDataContext();
            XDocument xml = new XDocument();
            List<Categories> catlist = new List<Categories>();
            try {
                List<int> catids = db.CatParts.Where(x => x.partID == partID).Select(x => x.catID).ToList();
                if (!catids.Contains(catID)) {
                    catID = catids.First();
                }
                catlist.Add(db.Categories.Where(x => x.catID == catID).FirstOrDefault<Categories>());
                bool more = (catlist[0].parentID == 0) ? false : true;
                while (more) {
                    catlist.Add(db.Categories.Where(x => x.catID == catlist[catlist.Count() - 1].parentID).FirstOrDefault<Categories>());
                    if (catlist[catlist.Count() - 1].parentID == 0) {
                        more = false;
                    }
                }
            } catch { }
            XElement cats = new XElement("Categories", new List<XElement>(from c in catlist.AsQueryable()
                                                                          select new XElement("Category",
                                                                             new XAttribute("CatID", c.catID),
                                                                             new XAttribute("DateAdded", c.dateAdded),
                                                                             new XAttribute("ParentID", c.parentID),
                                                                             new XAttribute("CategoryName", c.catTitle.ToString().Trim()),
                                                                             new XAttribute("ShortDesc", (c.shortDesc != null && c.shortDesc.Length > 0) ? c.shortDesc.ToString().Trim() : ""),
                                                                             new XAttribute("LongDesc", (c.longDesc != null && c.longDesc.Length > 0) ? c.longDesc.ToString().Trim() : ""),
                                                                             new XAttribute("image", (c.image != null && c.image.Length > 0) ? c.image.ToString().Trim() : ""),
                                                                             new XAttribute("isLifestyle", c.isLifestyle),
                                                                             new XAttribute("vehicleSpecific", c.vehicleSpecific))).ToList<XElement>());
            xml.Add(cats);
            return xml;
        }
        
        public static string GetSearch_JSON(string term = "", List<int> statuses = null) {
            List<searchResult> parts = new List<searchResult>();
            List<searchResult> cats = new List<searchResult>();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            string[] terms = term.Split(' ');
            var partsPredicate = PredicateBuilder.False<Part>();
            var catsPredicate = PredicateBuilder.False<Categories>();
            foreach (string keyword in terms) {
                string temp = keyword;
                partsPredicate = partsPredicate.Or(p => p.partID.ToString().Contains(temp));
                partsPredicate = partsPredicate.Or(p => p.shortDesc.ToLower().Contains(temp.ToLower()));
                catsPredicate = catsPredicate.Or(c => c.catTitle.ToLower().Contains(temp.ToLower()));
                catsPredicate = catsPredicate.Or(c => c.shortDesc.ToLower().Contains(temp.ToLower()));
            }

            parts = (from p in db.Parts.AsQueryable().Where(partsPredicate)
                     where statuslist.Contains(p.status)
                     select new searchResult {
                         partID = p.partID,
                         isHitch = (p.classID != 0) ? true : false,
                         description = p.shortDesc,
                         long_description = p.ContentBridges.Select(x => x.Content.text).FirstOrDefault().ToString()
                     }).ToList<searchResult>();

            cats = (from c in db.Categories.AsQueryable().Where(catsPredicate)
                    select new searchResult {
                        categoryID = c.catID,
                        isHitch = false,
                        description = c.catTitle,
                        long_description = c.longDesc
                    }).ToList<searchResult>();
            if (cats.Count > 0) {
                foreach (searchResult s in cats) {
                    parts.Add(s);
                }
            }

            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetSearch_XML(string term = "", List<int> statuses = null) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Results");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            string[] terms = term.Split(' ');
            var partsPredicate = PredicateBuilder.False<Part>();
            var catsPredicate = PredicateBuilder.False<Categories>();
            foreach (string keyword in terms) {
                string temp = keyword;
                partsPredicate = partsPredicate.Or(p => p.partID.ToString().Contains(temp));
                partsPredicate = partsPredicate.Or(p => p.shortDesc.ToLower().Contains(temp.ToLower()));
                catsPredicate = catsPredicate.Or(c => c.catTitle.ToLower().Contains(temp.ToLower()));
                catsPredicate = catsPredicate.Or(c => c.shortDesc.ToLower().Contains(temp.ToLower()));
            }

            // Query the parts table for results
            List<XElement> parts_result = (from p in db.Parts.AsQueryable().Where(partsPredicate)
                                           where statuslist.Contains(p.status)
                                           select new XElement("Result",
                                               new XAttribute("partID", p.partID),
                                               new XAttribute("short_desc", p.shortDesc),
                                               new XAttribute("isHitch", (p.classID != 0) ? true : false),
                                               (from con in p.ContentBridges
                                                select new XElement("description", con.Content.text)).ToList<XElement>()
                                           )).ToList<XElement>();
            if (parts_result.Count > 0) {
                parts.Add(parts_result);
            }

            // Query the category table for results
            List<XElement> cats_result = (from c in db.Categories.AsQueryable().Where(catsPredicate)
                                          select new XElement("Result",
                                              new XAttribute("categoryID", c.catID),
                                              new XAttribute("short_desc", c.catTitle),
                                              new XAttribute("isHitch", false),
                                              new XElement("description", c.shortDesc),
                                              new XElement("description", c.longDesc)
                                              )).ToList<XElement>();
            if (cats_result.Count > 0) {
                parts.Add(cats_result);
            }
            xml.Add(parts);
            return xml;
        }

        public static string GetPowerSearch_JSON(string term = "", List<int> statuses = null, bool integrated = false, int customerID = 0) {
            List<APIPart> parts = new List<APIPart>();
            APIPart temppart = new APIPart();
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();

            List<Part> searchResult = db.searchIndex(term).ToList<Part>();

            foreach (Part part in searchResult) {
                if(statuslist.Contains(part.status)) {
                    if (!integrated) {
                        #region Not Integrated
                        temppart = new APIPart {
                                        partID = part.partID,
                                        status = part.status,
                                        dateModified = part.dateModified.ToString(),
                                        dateAdded = part.dateModified.ToString(),
                                        shortDesc = "CURT " + part.shortDesc + " #" + part.partID.ToString(),
                                        oldPartNumber = part.oldPartNumber,
                                        listPrice = GetCustomerPrice(customerID, part.partID),
                                        pClass = (part.classID != 0) ? db.Classes.Where(x => x.classID.Equals(part.classID)).Select(x => x.class1).FirstOrDefault() : "",
                                        priceCode = part.priceCode,
                                        relatedCount = part.RelatedParts.Count,
                                        attributes = (from pa in part.PartAttributes
                                                      orderby pa.sort
                                                      select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                                        content = (from co in part.ContentBridges
                                                   orderby co.contentID
                                                   select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                        videos = (from pv in part.PartVideos
                                                  orderby pv.vTypeID
                                                  select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                                        packages = (from pp in part.PartPackages
                                                    select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                                        pricing = (from pr in part.Prices
                                                   select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                                        reviews = (from r in part.Reviews
                                                   where r.active.Equals(true) && r.approved.Equals(true)
                                                   orderby r.createdDate descending
                                                   select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                                        averageReview = (from r in part.Reviews
                                                         where r.active.Equals(true) && r.approved.Equals(true)
                                                         select (double?)r.rating).Average() ?? 0.0,
                                        images = (from pi in part.PartImages
                                                  select new APIImage {
                                                      imageID = pi.imageID,
                                                      sort = pi.sort,
                                                      path = pi.path,
                                                      height = pi.height,
                                                      width = pi.width,
                                                      size = pi.PartImageSize.size,
                                                      partID = pi.partID
                                                  }).OrderBy(x => x.sort).ToList<APIImage>()
                                    };
                        if (temppart != null && temppart.partID != 0) {
                            if (temppart.partID.ToString() == term.Trim()) {
                                parts.Insert(0, temppart);
                            } else {
                                parts.Add(temppart);
                            }
                        }
                        #endregion
                    } else {
                        #region Integrated
                        temppart = new APIPart {
                            partID = part.partID,
                            custPartID = db.CartIntegrations.Where(x => x.partID.Equals(part.partID)).Where(x => x.custID.Equals(customerID)).Select(x => x.custPartID).FirstOrDefault(),
                            status = part.status,
                            dateModified = part.dateModified.ToString(),
                            dateAdded = part.dateAdded.ToString(),
                            shortDesc = "CURT " + part.shortDesc + " #" + part.partID.ToString(),
                            oldPartNumber = part.oldPartNumber,
                            listPrice = GetCustomerPrice(customerID, part.partID),
                            pClass = (part.classID != 0) ? db.Classes.Where(x => x.classID.Equals(part.classID)).Select(x => x.class1).FirstOrDefault() : "",
                            priceCode = part.priceCode,
                            relatedCount = part.RelatedParts.Count,
                            attributes = (from pa in part.PartAttributes
                                          orderby pa.sort
                                          select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                            content = (from co in part.ContentBridges
                                       orderby co.contentID
                                       select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            videos = (from pv in part.PartVideos
                                      orderby pv.vTypeID
                                      select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                            packages = (from pp in part.PartPackages
                                        select new APIPackage { height = pp.height, length = pp.length, width = pp.width, weight = pp.weight, quantity = pp.quantity, dimensionUnit = pp.dimensionUnit.code, dimensionUnitLabel = pp.dimensionUnit.name, weightUnit = pp.weightUnit.code, weightUnitLabel = pp.weightUnit.name, packageUnit = pp.packageUnit.code, packageUnitLabel = pp.packageUnit.name }).ToList<APIPackage>(),
                            pricing = (from pr in part.Prices
                                       select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                            reviews = (from r in part.Reviews
                                       where r.active.Equals(true) && r.approved.Equals(true)
                                       orderby r.createdDate descending
                                       select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                            averageReview = (from r in part.Reviews
                                             where r.active.Equals(true) && r.approved.Equals(true)
                                             select (double?)r.rating).Average() ?? 0.0,
                            images = (from pi in part.PartImages
                                      select new APIImage {
                                          imageID = pi.imageID,
                                          sort = pi.sort,
                                          path = pi.path,
                                          height = pi.height,
                                          width = pi.width,
                                          size = pi.PartImageSize.size,
                                          partID = pi.partID
                                      }).OrderBy(x => x.sort).ToList<APIImage>()
                        };
                        if (temppart != null && temppart.partID != 0 && temppart.custPartID != null && temppart.custPartID != 0) {
                            if (temppart.partID.ToString() == term.Trim()) {
                                parts.Insert(0, temppart);
                            } else {
                                parts.Add(temppart);
                            }
                        }
                    #endregion
                    }

                }
            }

            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetPowerSearch_XML(string term = "", List<int> statuses = null, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Results");
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = statuses ?? new List<int>();
            List<Part> searchResult = db.searchIndex(term).ToList<Part>();

            foreach (Part part in searchResult) {

                if (statuslist.Contains(part.status)) {
                    if (!integrated) {
                        #region Not Integrated
                        XElement xml_part = new XElement("Part",
                                                 new XAttribute("partID", part.partID),
                                                 new XAttribute("status", part.status),
                                                 new XAttribute("dateModified", part.dateModified.ToString()),
                                                 new XAttribute("dateAdded", part.dateAdded.ToString()),
                                                 new XAttribute("shortDesc", "CURT " + part.shortDesc + " #" + part.partID.ToString()),
                                                 new XAttribute("oldPartNumber", (part.oldPartNumber != null) ? part.oldPartNumber : ""),
                                                 new XAttribute("listPrice", GetCustomerPrice(customerID, part.partID)),
                                                 new XAttribute("pClass", (part.classID != 0) ? db.Classes.Where(x => x.classID.Equals(part.classID)).Select(x => x.class1).FirstOrDefault() : ""),
                                                 new XAttribute("priceCode", (part.priceCode != null) ? part.priceCode : 0),
                                                 new XAttribute("relatedCount", part.RelatedParts.Count),
                                                 new XElement("Attributes", (from pa in part.PartAttributes
                                                                             orderby pa.sort
                                                                             select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                new XElement("Content", (from co in part.ContentBridges
                                                                         orderby co.Content.ContentType.type, co.contentID
                                                                         select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                new XElement("Videos", (from pv in part.PartVideos
                                                                        orderby pv.vTypeID
                                                                        select new XElement("Video", pv.video,
                                                                            new XAttribute("videoID", pv.pVideoID),
                                                                            new XAttribute("isPrimary", pv.isPrimary),
                                                                            new XAttribute("type", pv.videoType.name),
                                                                            new XAttribute("icon", pv.videoType.icon),
                                                                            new XAttribute("typeID", pv.vTypeID)
                                                                        )).ToList<XElement>()),
                                                new XElement("Packages", (from pp in part.PartPackages
                                                                          select new XElement("Package",
                                                                              new XAttribute("height", pp.height),
                                                                              new XAttribute("length", pp.length),
                                                                              new XAttribute("width", pp.width),
                                                                              new XAttribute("weight", pp.weight),
                                                                              new XAttribute("quantity", pp.quantity),
                                                                              new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                              new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                              new XAttribute("weightUnit", pp.weightUnit.code),
                                                                              new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                              new XAttribute("packageUnit", pp.packageUnit.code),
                                                                              new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                          )).ToList<XElement>()),
                                                 new XElement("Pricing", (from pr in part.Prices
                                                                          orderby pr.priceType
                                                                          select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                 new XElement("Reviews",
                                                     new XAttribute("averageReview", (from r in part.Reviews
                                                                                      where r.active.Equals(true) && r.approved.Equals(true)
                                                                                      select (double?)r.rating).Average() ?? 0.0),
                                                                         (from r in part.Reviews
                                                                          where r.active.Equals(true) && r.approved.Equals(true)
                                                                          orderby r.createdDate descending
                                                                          select new XElement("Review",
                                                                              new XAttribute("reviewID", r.reviewID),
                                                                              new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                              new XElement("rating", r.rating),
                                                                              new XElement("name", r.name),
                                                                              new XElement("email", r.email),
                                                                              new XElement("subject", r.subject),
                                                                              new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                new XElement("Images",
                                                                    (from pin in part.PartImages
                                                                     group pin by pin.sort into pi
                                                                     orderby pi.Key
                                                                     select new XElement("Index",
                                                                                         new XAttribute("name", pi.Key.ToString()),
                                                                                         (from pis in part.PartImages
                                                                                          where pis.sort.Equals(pi.Key)
                                                                                          orderby pis.sort, pis.sizeID
                                                                                          select new XElement(pis.PartImageSize.size,
                                                                                                              new XAttribute("imageID", pis.imageID),
                                                                                                              new XAttribute("path", pis.path),
                                                                                                              new XAttribute("height", pis.height),
                                                                                                              new XAttribute("width", pis.width)
                                                                                          )).ToList<XElement>()
                                                                 )).ToList<XElement>())
                                             );
                        if (xml_part != null && xml_part.Attribute("partID").Value != "0") {
                            if (xml_part.Attribute("partID").Value.ToString() == term.Trim()) {
                                parts.AddFirst(xml_part);
                            } else {
                                parts.Add(xml_part);
                            }
                        }
                        #endregion
                    } else {
                        #region Integrated
                        XElement xml_part = new XElement("Part",
                                                 new XAttribute("partID", part.partID),
                                                 new XAttribute("custPartID", db.CartIntegrations.Where(x => x.partID.Equals(part.partID)).Where(x => x.custID.Equals(customerID)).Select(x => x.custPartID).FirstOrDefault()),
                                                 new XAttribute("status", part.status),
                                                 new XAttribute("dateModified", part.dateModified.ToString()),
                                                 new XAttribute("dateAdded", part.dateAdded.ToString()),
                                                 new XAttribute("shortDesc", "CURT " + part.shortDesc + " #" + part.partID.ToString()),
                                                 new XAttribute("oldPartNumber", (part.oldPartNumber != null) ? part.oldPartNumber : ""),
                                                 new XAttribute("listPrice", GetCustomerPrice(customerID, part.partID)),
                                                 new XAttribute("pClass", (part.classID != 0) ? db.Classes.Where(x => x.classID.Equals(part.classID)).Select(x => x.class1).FirstOrDefault() : ""),
                                                 new XAttribute("priceCode", (part.priceCode != null) ? part.priceCode : 0),
                                                 new XAttribute("relatedCount", part.RelatedParts.Count),
                                                 new XElement("Attributes", (from pa in part.PartAttributes
                                                                             orderby pa.sort
                                                                             select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                new XElement("Content", (from co in part.ContentBridges
                                                                         orderby co.Content.ContentType.type, co.contentID
                                                                         select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                new XElement("Videos", (from pv in part.PartVideos
                                                                        orderby pv.vTypeID
                                                                        select new XElement("Video", pv.video,
                                                                            new XAttribute("videoID", pv.pVideoID),
                                                                            new XAttribute("isPrimary", pv.isPrimary),
                                                                            new XAttribute("type", pv.videoType.name),
                                                                            new XAttribute("icon", pv.videoType.icon),
                                                                            new XAttribute("typeID", pv.vTypeID)
                                                                        )).ToList<XElement>()),
                                                new XElement("Packages", (from pp in part.PartPackages
                                                                          select new XElement("Package",
                                                                              new XAttribute("height", pp.height),
                                                                              new XAttribute("length", pp.length),
                                                                              new XAttribute("width", pp.width),
                                                                              new XAttribute("weight", pp.weight),
                                                                              new XAttribute("quantity", pp.quantity),
                                                                              new XAttribute("dimensionUnit", pp.dimensionUnit.code),
                                                                              new XAttribute("dimensionUnitLabel", pp.dimensionUnit.name),
                                                                              new XAttribute("weightUnit", pp.weightUnit.code),
                                                                              new XAttribute("weightUnitLabel", pp.weightUnit.name),
                                                                              new XAttribute("packageUnit", pp.packageUnit.code),
                                                                              new XAttribute("packageUnitLabel", pp.packageUnit.name)
                                                                          )).ToList<XElement>()),
                                                 new XElement("Pricing", (from pr in part.Prices
                                                                          orderby pr.priceType
                                                                          select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                 new XElement("Reviews",
                                                     new XAttribute("averageReview", (from r in part.Reviews
                                                                                      where r.active.Equals(true) && r.approved.Equals(true)
                                                                                      select (double?)r.rating).Average() ?? 0.0),
                                                                         (from r in part.Reviews
                                                                          where r.active.Equals(true) && r.approved.Equals(true)
                                                                          orderby r.createdDate descending
                                                                          select new XElement("Review",
                                                                              new XAttribute("reviewID", r.reviewID),
                                                                              new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                              new XElement("rating", r.rating),
                                                                              new XElement("name", r.name),
                                                                              new XElement("email", r.email),
                                                                              new XElement("subject", r.subject),
                                                                              new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                new XElement("Images",
                                                                    (from pin in part.PartImages
                                                                     group pin by pin.sort into pi
                                                                     orderby pi.Key
                                                                     select new XElement("Index",
                                                                                         new XAttribute("name", pi.Key.ToString()),
                                                                                         (from pis in part.PartImages
                                                                                          where pis.sort.Equals(pi.Key)
                                                                                          orderby pis.sort, pis.sizeID
                                                                                          select new XElement(pis.PartImageSize.size,
                                                                                                              new XAttribute("imageID", pis.imageID),
                                                                                                              new XAttribute("path", pis.path),
                                                                                                              new XAttribute("height", pis.height),
                                                                                                              new XAttribute("width", pis.width)
                                                                                          )).ToList<XElement>()
                                                                 )).ToList<XElement>())
                                             );
                        if (xml_part != null && xml_part.Attribute("partID").Value != "0" && xml_part.Attribute("custPartID").Value != null && xml_part.Attribute("custPartID").Value != "0") {
                            if (xml_part.Attribute("partID").Value.ToString() == term.Trim()) {
                                parts.AddFirst(xml_part);
                            } else {
                                parts.Add(xml_part);
                            }
                        }
                        #endregion

                    }
                }
            }
            xml.Add(parts);
            return xml;
        }

        public static XDocument GetPartDefaultImages_XML(bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            CurtDevDataContext db = new CurtDevDataContext();
            XElement parts = new XElement("Parts");

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Images", 
                                        (from p in db.Parts
                                         orderby p.partID
                                         select new XElement("Part",
                                                                new XAttribute("partID", p.partID),
                                                                (from pis in db.PartImages
                                                                 join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                 where pis.partID.Equals(p.partID) && pis.sort.Equals('a')
                                                                 orderby pis.sort, pis.sizeID
                                                                 select new XElement(pig.size,
                                                                                     new XAttribute("imageID", pis.imageID),
                                                                                     new XAttribute("path", pis.path),
                                                                                     new XAttribute("height", pis.height),
                                                                                     new XAttribute("width", pis.width)
                                                                 )).ToList<XElement>()
                                        )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Images", 
                                        (from p in db.Parts
                                         join ci in db.CartIntegrations on p.partID equals ci.partID
                                         join cu in db.Customers on ci.custID equals cu.customerID
                                         where cu.customerID.Equals(customerID)
                                         orderby p.partID
                                         select new XElement("Part",
                                                                new XAttribute("partID", p.partID),
                                                                new XAttribute("custPartID", ci.custPartID),
                                                                (from pis in db.PartImages
                                                                 join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                 where pis.partID.Equals(p.partID) && pis.sort.Equals('a')
                                                                 orderby pis.sort, pis.sizeID
                                                                 select new XElement(pig.size,
                                                                                     new XAttribute("imageID", pis.imageID),
                                                                                     new XAttribute("path", pis.path),
                                                                                     new XAttribute("height", pis.height),
                                                                                     new XAttribute("width", pis.width)
                                                                 )).ToList<XElement>()
                                        )).ToList<XElement>());
                #endregion

            }
            xml.Add(parts);
            return xml;
        }

        public static XDocument GetPartImagesByIndex_XML(char index = 'a', bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            CurtDevDataContext db = new CurtDevDataContext();
            XElement parts = new XElement("Parts");

            if (!integrated) {
                #region Not Integrated
                parts = new XElement("Images",
                                        (from p in db.Parts
                                         orderby p.partID
                                         select new XElement("Part",
                                                                new XAttribute("partID", p.partID),
                                                                (from pis in db.PartImages
                                                                 join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                 where pis.partID.Equals(p.partID) && pis.sort.Equals(index)
                                                                 orderby pis.sort, pis.sizeID
                                                                 select new XElement(pig.size,
                                                                                     new XAttribute("imageID", pis.imageID),
                                                                                     new XAttribute("path", pis.path),
                                                                                     new XAttribute("height", pis.height),
                                                                                     new XAttribute("width", pis.width)
                                                                 )).ToList<XElement>()
                                        )).ToList<XElement>());
                #endregion
            } else {
                #region Integrated
                parts = new XElement("Images",
                                        (from p in db.Parts
                                         join ci in db.CartIntegrations on p.partID equals ci.partID
                                         join cu in db.Customers on ci.custID equals cu.customerID
                                         where cu.customerID.Equals(customerID)
                                         orderby p.partID
                                         select new XElement("Part",
                                                                new XAttribute("partID", p.partID),
                                                                new XAttribute("custPartID", ci.custPartID),
                                                                (from pis in db.PartImages
                                                                 join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                 where pis.partID.Equals(p.partID) && pis.sort.Equals(index)
                                                                 orderby pis.sort, pis.sizeID
                                                                 select new XElement(pig.size,
                                                                                     new XAttribute("imageID", pis.imageID),
                                                                                     new XAttribute("path", pis.path),
                                                                                     new XAttribute("height", pis.height),
                                                                                     new XAttribute("width", pis.width)
                                                                 )).ToList<XElement>()
                                        )).ToList<XElement>());
                #endregion

            }
            xml.Add(parts);
            return xml;
        }

        public static string GetPartImage(int partID = 0, char index = 'a', string size = "Grande") {
            CurtDevDataContext db = new CurtDevDataContext();
            string url = "";
            if (partID > 0) {
                url = (from pis in db.PartImages
                       join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                       where pis.partID.Equals(partID) && pis.sort.Equals(index) && pig.size.ToLower().Equals(size.Trim().ToLower())
                       select pis.path).FirstOrDefault();
            }
            return url;
        }

        public static XDocument GetPartImages_XML(int partID = 0, bool integrated = false, int customerID = 0) {
            XDocument xml = new XDocument();
            CurtDevDataContext db = new CurtDevDataContext();
            XElement parts = new XElement("Part");

            if (!integrated) {
                #region Not Integrated
                parts = (from p in db.Parts
                         where p.partID.Equals(partID)
                         select new XElement("Part",
                                                new XAttribute("partID", p.partID),
                                                (from pin in db.PartImages
                                                 where pin.partID.Equals(p.partID)
                                                 group pin by pin.sort into pi
                                                 orderby pi.Key
                                                 select new XElement("Index",
                                                                     new XAttribute("name", pi.Key.ToString()),
                                                                     (from pis in db.PartImages
                                                                      join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                      where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                      orderby pis.sort, pis.sizeID
                                                                      select new XElement(pig.size,
                                                                                          new XAttribute("imageID", pis.imageID),
                                                                                          new XAttribute("path", pis.path),
                                                                                          new XAttribute("height", pis.height),
                                                                                          new XAttribute("width", pis.width)
                                                                      )).ToList<XElement>()
                                             )).ToList<XElement>()
                        )).FirstOrDefault<XElement>();
                #endregion
            } else {
                #region Integrated
                parts = (from p in db.Parts
                         join ci in db.CartIntegrations on p.partID equals ci.partID
                         join cu in db.Customers on ci.custID equals cu.customerID
                         where cu.customerID.Equals(customerID)
                         orderby p.partID
                         select new XElement("Part",
                                                new XAttribute("partID", p.partID),
                                                new XAttribute("custPartID", ci.custPartID),
                                                (from pin in db.PartImages
                                                 where pin.partID.Equals(p.partID)
                                                 group pin by pin.sort into pi
                                                 orderby pi.Key
                                                 select new XElement("Index",
                                                                     new XAttribute("name", pi.Key.ToString()),
                                                                     (from pis in db.PartImages
                                                                      join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                      where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                      orderby pis.sort, pis.sizeID
                                                                      select new XElement(pig.size,
                                                                                          new XAttribute("imageID", pis.imageID),
                                                                                          new XAttribute("path", pis.path),
                                                                                          new XAttribute("height", pis.height),
                                                                                          new XAttribute("width", pis.width)
                                                                      )).ToList<XElement>()
                                             )).ToList<XElement>()
                        )).FirstOrDefault<XElement>();
                #endregion

            }
            xml.Add(parts);
            return xml;
        }

        public static XDocument getFileInfo_XML(string path = "") {
            XDocument xml = new XDocument();
            XElement file = new XElement("File");
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                //File f = db.Files.Where(x => x.path == path.Trim()).First<File>();
                file = (from f in db.Files
                         where f.path.ToLower().Equals(path.ToLower().Trim())
                         select new XElement("File",
                                                new XAttribute("fileID", f.fileID),
                                                new XAttribute("height", f.height),
                                                new XAttribute("width", f.width),
                                                new XAttribute("size", f.size),
                                                new XAttribute("name",f.name),
                                                new XAttribute("createdDate",String.Format("{0:MM/dd/yyyy hh:mm tt}",f.createdDate)),
                                                new XAttribute("path",f.path),
                                                new XAttribute("extension",db.FileExts.Where(x => x.fileExtID == f.fileExtID).Select(x => x.fileExt1).First())
                                             )).FirstOrDefault<XElement>();
            } catch { };
            xml.Add(file);
            return xml;
        }

        /******** Start Kiosk Methods *******/

        /// <summary>
        /// This function runs on Magic, if you don't have the proper wand, DON'T touch it!!! It will return ONLY hitches for the given vehicle
        /// </summary>
        /// <param name="vehicleID">Vehicle to find hitches for.</param>
        /// <param name="acctID">Customer to retrieve pricing for</param>
        /// <returns>JSON Serialized result</returns>
        public static string GetKioskHitches(int vehicleID = 0, int acctID = 0) {

            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();
            if (acctID > 0) {
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join cp in db.CatParts on p.partID equals cp.partID
                         join cat in db.Categories on cp.catID equals cat.catID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where v.vehicleID.Equals(vehicleID) &&
                         (cat.catTitle.ToUpper().Contains("CLASS I RECEIVER") ||
                         cat.catTitle.ToUpper().Contains("CLASS II RECEIVER") ||
                         cat.catTitle.ToUpper().Contains("CLASS III RECEIVER") ||
                         cat.catTitle.Contains("CLASS IV RECEIVER") ||
                         cat.catTitle.Contains("CLASS V RECEIVER") ||
                         cat.catTitle.ToUpper().Contains("IDC RECEIVER") ||
                         cat.catTitle.ToUpper().Contains("XDC RECEIVER") ||
                         cat.catTitle.Contains("FRONT MOUNT"))
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = GetCustomerPrice(acctID, p.partID),
                             pClass = (c != null) ? c.class1 : "",
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute {
                                               key = pa.field,
                                               value = pa.value
                                           }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        select new APIAttribute {
                                            key = co.Content.ContentType.type,
                                            value = co.Content.text
                                        }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute {
                                            key = pr.priceType,
                                            value = pr.price1.ToString()
                                        }).OrderBy(x => x.key).ToList<APIAttribute>()
                         }).ToList<APIPart>();
            } else {
                parts = (from p in db.Parts
                         join vp in db.VehicleParts on p.partID equals vp.partID
                         join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                         join cp in db.CatParts on p.partID equals cp.partID
                         join cat in db.Categories on cp.catID equals cat.catID
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where v.vehicleID.Equals(vehicleID) &&
                         (cat.catTitle.ToUpper().Contains("CLASS I RECEIVER") ||
                         cat.catTitle.ToUpper().Contains("CLASS II RECEIVER") ||
                         cat.catTitle.ToUpper().Contains("CLASS III RECEIVER") ||
                         cat.catTitle.Contains("CLASS IV RECEIVER") ||
                         cat.catTitle.Contains("CLASS V RECEIVER") ||
                         cat.catTitle.ToUpper().Contains("IDC RECEIVER") ||
                         cat.catTitle.ToUpper().Contains("XDC RECEIVER") ||
                         cat.catTitle.ToUpper().Contains("FRONT MOUNT"))
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = String.Format("{0:C}", (from prices in db.Prices
                                                                 where prices.partID.Equals(p.partID) && prices.priceType.Equals("List")
                                                                 select prices.price1 != null ? prices.price1 : (decimal?)0).FirstOrDefault<decimal?>()),
                             pClass = (c != null) ? c.class1 : "",
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute {
                                               key = pa.field,
                                               value = pa.value
                                           }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        select new APIAttribute {
                                            key = co.Content.ContentType.type,
                                            value = co.Content.text
                                        }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute {
                                            key = pr.priceType,
                                            value = pr.price1.ToString()
                                        }).OrderBy(x => x.key).ToList<APIAttribute>()
                         }).ToList<APIPart>();
            }
            return JsonConvert.SerializeObject(parts);
        }

        private static string GetCustomerPrice(int acctID, int p) {
            CurtDevDataContext db = new CurtDevDataContext();
            try {
                if (acctID > 0) {
                    // try and get a sale price
                    decimal sale_price = (from sp in db.CustomerPricings
                                          join c in db.Customers on sp.cust_id equals c.customerID
                                          where sp.isSale.Equals(1) &&
                                          c.customerID.Equals(acctID) &&
                                          sp.partID.Equals(p) &&
                                          sp.sale_start <= DateTime.Now &&
                                          sp.sale_end >= DateTime.Now
                                          select sp.price).FirstOrDefault<decimal>();
                    if (sale_price > 0) { return String.Format("{0:C}", sale_price); }

                    // try and get a customer price
                    decimal cust_price = (from cp in db.CustomerPricings
                                          join c in db.Customers on cp.cust_id equals c.customerID
                                          where cp.isSale.Equals(0) &&
                                          c.customerID.Equals(acctID) &&
                                          cp.partID.Equals(p)
                                          select cp.price).FirstOrDefault<decimal>();
                    if (cust_price > 0) { return String.Format("{0:C}", cust_price); }
                }

                // try and get the default list price
                decimal list = (from pr in db.Prices
                                where pr.partID.Equals(p) && pr.priceType.ToUpper().Equals("LIST")
                                select pr.price1).FirstOrDefault<decimal>();
                if (list > 0) { return String.Format("{0:C}", list); }

                return String.Format("{0:C}", 0);
            } catch (Exception) {
                return String.Format("{0:C}", 0);
            }
        }

        public static string GetConnectorKiosk(int vehicleID = 0, int acctID = 0) {
            List<APIPart> connectors = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            connectors = (from p in db.Parts
                          join c in db.Classes on p.classID equals c.classID into ClassTemp
                          from c in ClassTemp.DefaultIfEmpty()
                          join ct in db.CatParts on p.partID equals ct.partID
                          join cat in db.Categories on ct.catID equals cat.catID
                          join vp in db.VehicleParts on p.partID equals vp.partID
                          where vp.vehicleID.Equals(vehicleID) && cat.catTitle.ToUpper().Contains("CONNECTOR")
                          select new APIPart {
                              partID = p.partID,
                              status = p.status,
                              dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                              dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                              shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                              oldPartNumber = p.oldPartNumber,
                              listPrice = GetCustomerPrice(acctID, p.partID),
                              pClass = (c != null) ? c.class1 : "",
                              attributes = (from pa in db.PartAttributes
                                            where pa.partID.Equals(p.partID)
                                            orderby pa.sort
                                            select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                              content = (from co in p.ContentBridges
                                         select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                              pricing = (from pr in db.Prices
                                         where pr.partID.Equals(p.partID)
                                         select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>()
                          }).ToList<APIPart>();
            return JsonConvert.SerializeObject(connectors);
        }

        public static string GetRelatedKiosk(int partID = 0, int acctID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            parts = (from p in db.Parts
                     join rp in db.RelatedParts on p.partID equals rp.relatedID
                     join c in db.Classes on p.classID equals c.classID into ClassTemp
                     from c in ClassTemp.DefaultIfEmpty()
                     where rp.partID.Equals(partID)
                     select new APIPart {
                         partID = p.partID,
                         status = p.status,
                         dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                         dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                         shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                         oldPartNumber = p.oldPartNumber,
                         listPrice = GetCustomerPrice(acctID, p.partID),
                         pClass = (c != null) ? c.class1 : "",
                         attributes = (from pa in db.PartAttributes
                                       where pa.partID.Equals(p.partID)
                                       orderby pa.sort
                                       select new APIAttribute { key = pa.field, value = pa.value }).OrderBy(x => x.key).ToList<APIAttribute>(),
                         content = (from co in p.ContentBridges
                                    select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                         pricing = (from pr in db.Prices
                                    where pr.partID.Equals(p.partID)
                                    select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>()
                     }).ToList<APIPart>();
            return JsonConvert.SerializeObject(parts);
        }

        public static string GetOpenAccessories(int vehicleID = 0, int acctID = 0) {
            List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            List<int> ids = (from p in db.Parts
                             join vp in db.VehicleParts on p.partID equals vp.partID
                             join rp in db.RelatedParts on p.partID equals rp.partID
                             join acc in db.Parts on rp.relatedID equals acc.partID
                             where vp.vehicleID.Equals(vehicleID)
                             select acc.partID).Distinct().ToList<int>();

            parts = (from acc in db.Parts
                     join c in db.Classes on acc.classID equals c.classID into ClassTemp
                     from c in ClassTemp.DefaultIfEmpty()
                     where ids.Contains(acc.partID)
                     select new APIPart {
                         partID = acc.partID,
                         status = acc.status,
                         dateModified = Convert.ToDateTime(acc.dateModified).ToString(),
                         dateAdded = Convert.ToDateTime(acc.dateAdded).ToString(),
                         shortDesc = acc.shortDesc,
                         oldPartNumber = acc.oldPartNumber,
                         listPrice = GetCustomerPrice(acctID, acc.partID),
                         pClass = (c != null) ? c.class1 : "",
                         attributes = (from pa in db.PartAttributes
                                       where pa.partID.Equals(acc.partID)
                                       orderby pa.sort
                                       select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                         content = (from co in acc.ContentBridges
                                    select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                         pricing = (from pr in db.Prices
                                    where pr.partID.Equals(acc.partID)
                                    select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>()
                     }).OrderBy(y => y.partID).ToList<APIPart>();
            return JsonConvert.SerializeObject(parts);

        }

        public static int CheckAccount(int acctID = 0) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                int id = (from c in db.Customers
                          where c.customerID.Equals(acctID)
                          select c.cust_id).FirstOrDefault<int>();
                if (id > 0) {
                    return 1;
                } else {
                    return 0;
                }
            } catch (Exception e) {
                return 0;
            }
        }

        public static string GetCategoryColor_Kiosk(int partID = 0) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                ColorCode color = (from cc in db.ColorCodes
                                   join c in db.Categories on cc.codeID equals c.codeID
                                   join cp in db.CatParts on c.catID equals cp.catID
                                   where cp.partID.Equals(partID)
                                   select cc).FirstOrDefault<ColorCode>();
                if (color == null) {
                    color = new ColorCode {
                        codeID = 0,
                        code = "052052052",
                        font = "ffffff"
                    };
                }
                return JsonConvert.SerializeObject(color);
            } catch (Exception e) {
                return "";
            }
        }

        /********** End Kiosk Methods ******/

        /****** eLocal Methods ******/

        public static string GetSearch_eLocalJSON(string term = "") {
            List<eLocalSearchResult> parts = new List<eLocalSearchResult>();
            CurtDevDataContext db = new CurtDevDataContext();
            parts = (from p in db.Parts
                     join cp in db.CatParts on p.partID equals cp.catID into CatPartTemp
                     from cp in CatPartTemp.DefaultIfEmpty()
                     join cat in db.Categories on cp.catID equals cat.catID into CatTemp
                     from cat in CatTemp.DefaultIfEmpty()
                     join c in db.Classes on p.classID equals c.classID into ClassTemp
                     from c in ClassTemp.DefaultIfEmpty()
                     where p.partID.ToString().Contains(term) || p.shortDesc.Contains(term) || cat.catTitle.Contains(term) && (cat.catTitle.Length > 0 || p.partID > 0)
                     select new eLocalSearchResult {
                         link = (p.partID == null) ? "/Category/Index/" + cat.catID : "/Part/" + p.partID,
                         description = (p.partID == null) ? cat.catTitle : p.shortDesc,
                         long_description = (p.partID == null) ? cat.longDesc : p.ContentBridges.Where(x => x.Content.cTypeID.Equals(3)).Select(x => x.Content.text).FirstOrDefault().ToString()
                     }).ToList<eLocalSearchResult>();

            return JsonConvert.SerializeObject(parts);
        }

        public static XDocument GetSearch_eLocalXML(string term = "") {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Results");
            CurtDevDataContext db = new CurtDevDataContext();

            // Query the parts table for results
            List<XElement> parts_result = (from p in db.Parts
                                           where p.partID.ToString().Contains(term) || p.shortDesc.Contains(term)
                                           select new XElement("Result",
                                               new XAttribute("link", "/Part/" + p.partID),
                                               new XAttribute("short_desc", p.shortDesc),
                                               (from con in p.ContentBridges
                                                select new XElement("description", con.Content.text)).ToList<XElement>()
                                           )).ToList<XElement>();
            if (parts_result.Count > 0) {
                parts.Add(parts_result);
            }

            // Query the category table for results
            List<XElement> cats_result = (from c in db.Categories
                                          where c.catTitle.Contains(term) || c.shortDesc.Contains(term)
                                          select new XElement("Result",
                                              new XAttribute("link", "/Category/Index/" + c.catID),
                                              new XAttribute("short_desc", c.catTitle),
                                              new XElement("description", c.shortDesc),
                                              new XElement("description", c.longDesc)
                                              )).ToList<XElement>();
            if (cats_result.Count > 0) {
                parts.Add(cats_result);
            }
            xml.Add(parts);
            return xml;
        }

        public static string CheckAuth_eLocal(int i, string s) {
            try {
                int cust_id = i;
                string site = s;
                CurtDevDataContext db = new CurtDevDataContext();
                eLocal_Customer customer = new eLocal_Customer();
                if (site.Contains("localhost")) {
                    customer = (from c in db.Customers
                                join st in db.PartStates on c.stateID equals st.stateID into StateTemp
                                from st in StateTemp.DefaultIfEmpty()
                                join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type into DealerTemp
                                from dt in DealerTemp.DefaultIfEmpty()
                                where c.customerID.Equals(cust_id) || c.parentID.Equals(cust_id)
                                select new eLocal_Customer {
                                    cust_id = (c.customerID != null) ? Convert.ToInt32(c.customerID) : (int)c.parentID,
                                    name = c.name,
                                    email = c.email,
                                    address = c.address,
                                    city = c.city,
                                    state = st.abbr,
                                    fax = c.fax,
                                    contact_person = c.contact_person,
                                    dealer_type = dt.type,
                                    latitude = c.latitude,
                                    longitude = c.longitude,
                                    password = c.password,
                                    website = c.website
                                }).FirstOrDefault<eLocal_Customer>();
                } else {
                    customer = (from c in db.Customers
                                join st in db.PartStates on c.stateID equals st.stateID into StateTemp
                                from st in StateTemp.DefaultIfEmpty()
                                join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type into DealerTemp
                                from dt in DealerTemp.DefaultIfEmpty()
                                where (c.customerID.Equals(cust_id) || c.parentID.Equals(cust_id)) && c.website.Contains(site)
                                select new eLocal_Customer {
                                    cust_id = (c.customerID != null) ? Convert.ToInt32(c.customerID) : (int)c.parentID,
                                    name = c.name,
                                    email = c.email,
                                    address = c.address,
                                    city = c.city,
                                    state = st.abbr,
                                    fax = c.fax,
                                    contact_person = c.contact_person,
                                    dealer_type = dt.type,
                                    latitude = c.latitude,
                                    longitude = c.longitude,
                                    password = c.password,
                                    website = c.website
                                }).FirstOrDefault<eLocal_Customer>();
                }
                if (customer.cust_id > 0) {
                    return JsonConvert.SerializeObject(customer);
                } else {
                    return "";
                }
            } catch (Exception e) {
                return "";
            }
        }

        public static string DoLogin_eLocal(string e, string p, string u) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                if (u.Contains("localhost")) {
                    eLocal_Customer customer = (from c in db.Customers
                                                join s in db.PartStates on c.stateID equals s.stateID into StateTemp
                                                from s in StateTemp.DefaultIfEmpty()
                                                join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type into DealerTemp
                                                from dt in DealerTemp.DefaultIfEmpty()
                                                where c.email.Equals(e) && c.password.Equals(p)
                                                select new eLocal_Customer {
                                                    cust_id = (c.customerID != null) ? Convert.ToInt32(c.customerID) : (int)c.parentID,
                                                    name = c.name,
                                                    email = c.email,
                                                    address = c.address,
                                                    city = c.city,
                                                    state = s.abbr,
                                                    fax = c.fax,
                                                    contact_person = c.contact_person,
                                                    dealer_type = dt.type,
                                                    latitude = c.latitude,
                                                    longitude = c.longitude,
                                                    password = c.password,
                                                    website = c.website
                                                }).FirstOrDefault<eLocal_Customer>();
                    return JsonConvert.SerializeObject(customer);
                } else {
                    eLocal_Customer customer = (from c in db.Customers
                                                join s in db.PartStates on c.stateID equals s.stateID into StateTemp
                                                from s in StateTemp.DefaultIfEmpty()
                                                join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type into DealerTemp
                                                from dt in DealerTemp.DefaultIfEmpty()
                                                where c.email.Equals(e) && c.password.Equals(p) && c.website.Contains(u)
                                                select new eLocal_Customer {
                                                    cust_id = (c.customerID != null) ? Convert.ToInt32(c.customerID) : (int)c.parentID,
                                                    name = c.name,
                                                    email = c.email,
                                                    address = c.address,
                                                    city = c.city,
                                                    state = s.abbr,
                                                    fax = c.fax,
                                                    contact_person = c.contact_person,
                                                    dealer_type = dt.type,
                                                    latitude = c.latitude,
                                                    longitude = c.longitude,
                                                    password = c.password,
                                                    website = c.website
                                                }).FirstOrDefault<eLocal_Customer>();
                    return JsonConvert.SerializeObject(customer);
                }
            } catch (Exception ex) {
                return "";
            }
        }

        public static string Setup_eLocal(string e, string p, string u) {

            try {
                CurtDevDataContext db = new CurtDevDataContext();
                Customer cust = new Customer();

                cust = (from c in db.Customers
                        where c.email.Equals(e) && c.website.Contains(u)
                        select c).FirstOrDefault<Customer>();
                if (cust.password == null || cust.password.Length == 0) {
                    cust.password = p;
                    db.SubmitChanges();
                    return "";
                } else {
                    return "PASSWORD_SET";
                }
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        public static string GetPricing_eLocal(int acctID = 0) {
            try {
                List<eLocalPricing> pricing = new List<eLocalPricing>();
                CurtDevDataContext db = new CurtDevDataContext();

                pricing = (from cp in db.CustomerPricings
                           join p in db.Parts on cp.partID equals p.partID
                           where cp.cust_id.Equals(acctID)
                           select new eLocalPricing {
                               acctID = acctID,
                               price = cp.price,
                               partID = cp.partID,
                               name = p.shortDesc,
                               isSale = cp.isSale,
                               saleStart = Convert.ToDateTime(cp.sale_start).ToString(),
                               saleEnd = Convert.ToDateTime(cp.sale_end).ToString()
                           }).OrderBy(x => x.partID).ToList<eLocalPricing>();
                List<int> pricing_ids = pricing.Select(x => x.partID).ToList<int>();
                List<eLocalPricing> unmatched = (from p in db.Parts
                                                 join pr in db.Prices on p.partID equals pr.partID
                                                 where pr.priceType.ToUpper().Equals("LIST") && !pricing_ids.Contains(p.partID)
                                                 select new eLocalPricing {
                                                     acctID = acctID,
                                                     price = pr.price1,
                                                     partID = p.partID,
                                                     name = p.shortDesc,
                                                     isSale = 0,
                                                     saleStart = "",
                                                     saleEnd = ""
                                                 }).OrderBy(x => x.partID).ToList<eLocalPricing>();
                pricing.AddRange(unmatched);
                return JsonConvert.SerializeObject(pricing);

            } catch (Exception e) {
                return e.Message;
            }
        }

        public static string GetCustomerID(string email) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                int? cust_id = db.Customers.Where(x => x.email == email).Select(x => x.customerID).FirstOrDefault<int?>();
                if (cust_id == null) {
                    return "0";
                }
                return cust_id.ToString();
            } catch (Exception) {
                return "0";
            }
        }

        /****** End eLocal ******/

        /****** Theisens Integration ********/

        public static XDocument GetTheisensByDate(string date = "") {
            try {
                DateTime date_mod = Convert.ToDateTime(date);
                CurtDevDataContext db = new CurtDevDataContext();
                XDocument xml = new XDocument();
                XElement parts = new XElement("Parts", (from p in db.Parts
                                                        join cp in db.CatParts on p.partID equals cp.partID
                                                        join cat in db.Categories on cp.catID equals cat.catID
                                                        where p.dateModified > date_mod
                                                        select new XElement("Product",
                                                            new XAttribute("Action", "Add"),
                                                            new XElement("Name", p.shortDesc),
                                                            new XElement("Summary", (from c in p.ContentBridges
                                                                                     where c.Content.cTypeID.Equals(11)
                                                                                     select c.Content.text).FirstOrDefault<string>()),
                                                           new XElement("Description", (from c in p.ContentBridges
                                                                                        where c.Content.cTypeID.Equals(3)
                                                                                        orderby c.contentID
                                                                                        select c.Content.text).FirstOrDefault<string>()),
                                                           new XElement("SpecTitle", ""),
                                                           new XElement("MiscText", (from c in p.ContentBridges
                                                                                     where c.Content.cTypeID.Equals(2)
                                                                                     select c.Content.text).FirstOrDefault<string>()),
                                                           new XElement("Notes", (from c in p.ContentBridges
                                                                                  where c.Content.cTypeID.Equals(1)
                                                                                  orderby c.contentID descending
                                                                                  select c.Content.text).FirstOrDefault<string>()),
                                                           new XElement("IsFeaturedTeaser", ""),
                                                           new XElement("FroogleDescription", ""),
                                                           new XElement("SKU", (from pa in p.PartAttributes
                                                                                where pa.partID.Equals(p.partID) && pa.field.ToUpper().Equals("UPC")
                                                                                select pa.value).FirstOrDefault<string>()),
                                                           new XElement("ManufacturerPartNumber", p.partID),
                                                           new XElement("SwatchImageMap", ""),
                                                           new XElement("ProductType", cat.catTitle),
                                                           new XElement("Images",
                                                               new XElement("Icon",
                                                                   new XAttribute("Extension", "jpg"),
                                                                   new XAttribute("Delete", "false"),
                                                                   "http://www.curtmfg.com/CURTLibrary/" + p.partID + "/images/" + p.partID + "_100x75_a.jpg"
                                                                   ),
                                                               new XElement("Medium",
                                                                   new XAttribute("Extension", "jpg"),
                                                                   new XAttribute("Delete", "false"),
                                                                   "http://www.curtmfg.com/CURTLibrary/" + p.partID + "/images/" + p.partID + "_300x225_a.jpg"
                                                                   ),
                                                               new XElement("Large",
                                                                   new XAttribute("Extension", "jpg"),
                                                                   new XAttribute("Delete", "false"),
                                                                   "http://www.curtmfg.com/CURTLibrary/" + p.partID + "/images/" + p.partID + "_1024x768_a.jpg"
                                                                   ))
                                                            )).ToList<XElement>());
                //XElement xml_parts = new XElement("Parts", parts);
                //xml.Add(xml_parts);
                xml.Add(parts);
                return xml;
            } catch (Exception e) {
                return new XDocument();
            }
        }

        /****** End Theisens *************/

        /***** Hitch Widget Tracker *******/

        public static string AddDeployment(string url = "") {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                if (url.Length == 0) { throw new Exception(); }

                // Make sure we don't already have a listing for this url
                int exists = db.WidgetDeployments.Where(x => x.url == url).Count();

                if (exists == 0) { // Add new record
                    WidgetDeployment deployment = new WidgetDeployment {
                        url = url,
                        date_added = DateTime.Now
                    };
                    db.WidgetDeployments.InsertOnSubmit(deployment);
                    db.SubmitChanges();
                }
                return "";
            } catch (Exception) {
                return "";
            }
        }

        /***** End Tracker **************/

        /**** Image Submission *********/
        public static void AddImage(int partID = 0, char sort = ' ', string path = "", int height = 0, int width = 0, string size = "") {
            // Validate info
            if (partID == 0) { throw new Exception("Invalid part ID"); }
            if (sort == ' ') { throw new Exception("No sort specified"); }
            if (path.Length == 0) { throw new Exception("No path specified"); }
            if (height == 0) { throw new Exception("Invalid height"); }
            if (width == 0) { throw new Exception("Invalid width"); }
            if (size.Length == 0) { throw new Exception("Invalid size"); }

            CurtDevDataContext db = new CurtDevDataContext();
            int sizeID = db.PartImageSizes.Where(x => x.size.Equals(size)).Select(x => x.sizeID).FirstOrDefault<int>();
            if (sizeID > 0) {

                // Make sure we don't already have this image
                PartImage img = (from pi in db.PartImages
                                 join pis in db.PartImageSizes on pi.sizeID equals pis.sizeID
                                 where pi.partID.Equals(partID) && pi.sort.Equals(sort) && pis.size.Equals(size)
                                 select pi).FirstOrDefault<PartImage>();

                if (img == null) { // Insert new record
                    PartImage newimg = new PartImage {
                        partID = partID,
                        sizeID = sizeID,
                        path = path,
                        height = height,
                        width = width,
                        sort = sort
                    };
                    db.PartImages.InsertOnSubmit(newimg);
                } else { // Update record
                    img.path = path;
                    img.height = height;
                    img.width = width;
                }
                db.SubmitChanges();
            } else {
                throw new Exception("Invalid size");
            }
        }
        /****** End Image ****************/

        private static bool isSecure() {
            return HttpContext.Current.Request.IsSecureConnection;
        }
    }

    public class SPStatic {
        public string upc { get; set; }
        public string weight { get; set; }
        public decimal jobber { get; set; }
        public decimal map { get; set; }
    }

    public class FullCategory {
        public Categories parent { get; set; }
        public List<APIContent> content { get; set; }
        public List<Categories> sub_categories { get; set; }
    }

    public class APIPrice {
        public int partID { get; set; }
        public List<APIAttribute> prices { get; set; }
    }

    public class APICategory {
        public int catID { get; set; }
        public string dateAdded { get; set; }
        public int parentID { get; set; }
        public string catTitle { get; set; }
        public string shortDesc { get; set; }
        public string longDesc { get; set; }
        public string image { get; set; }
        public int isLifestyle { get; set; }
        public int sort { get; set; }
        public int partCount { get; set; }
        public List<APIContent> content { get; set; }
    }

    public class APILifestyle {
        public int catID { get; set; }
        public string dateAdded { get; set; }
        public int parentID { get; set; }
        public string catTitle { get; set; }
        public string shortDesc { get; set; }
        public string longDesc { get; set; }
        public string image { get; set; }
        public int isLifestyle { get; set; }
        public int sort { get; set; }
        public int partCount { get; set; }
        public List<APIContent> content { get; set; }
        public List<APITowable> towables { get; set; }
    }

    public class APIContent {
        public bool isHTML { get; set; }
        public string type { get; set; }
        public string content { get; set; }
    }

    public class APITowable : Trailer { }

    public class APIImage {
        public int imageID { get; set; }
        public string size { get; set; }
        public string path { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int partID { get; set; }
        public char sort { get; set; }
    }

    public class APIVideo {
        public int videoID { get; set; }
        public string youTubeVideoID { get; set; }
        public bool isPrimary { get; set; }
        public int typeID { get; set; }
        public string type { get; set; }
        public string typeicon { get; set; }
    }

    public class eLocal_Customer {
        public int cust_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string fax { get; set; }
        public string contact_person { get; set; }
        public string dealer_type { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string password { get; set; }
        public string website { get; set; }
    }

    public class eLocalPricing {
        public decimal price { get; set; }
        public int partID { get; set; }
        public string name { get; set; }
        public int acctID { get; set; }
        public int isSale { get; set; }
        public string saleStart { get; set; }
        public string saleEnd { get; set; }
    }

    public class eLocalSearchResult {
        public string link { get; set; }
        public string description { get; set; }
        public string long_description { get; set; }
        public decimal relevance { get; set; }
    }

    public class searchResult {
        public int? categoryID { get; set; }
        public int? partID { get; set; }
        public string type { get; set; }
        public bool isHitch { get; set; }
        public string description { get; set; }
        public string long_description { get; set; }
        public decimal relevance { get; set; }
    }

    public class XElementValueEqualityComparer : IEqualityComparer<XElement> {
        public bool Equals(XElement x, XElement y) {
            return x.Value.Equals(y.Value);
        }

        public int GetHashCode(XElement x) {
            return x.Value.GetHashCode();
        }
    }
    
}
