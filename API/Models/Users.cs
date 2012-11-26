using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace API {
    partial class user {

        public string GetUser() {
            AlexDBDataContext db = new AlexDBDataContext();
            user u = new user();
            u = db.users.Where(x => (x.username.Equals(this.username) || x.email.Equals(username)) && x.password.Equals(password)).FirstOrDefault<API.user>();

            if (u == null || u.userID == 0) {
                throw new Exception("User not found");
            }
            return JsonConvert.SerializeObject(u);
        }
    }
}