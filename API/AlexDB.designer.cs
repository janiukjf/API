﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="alex_db")]
	public partial class AlexDBDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void Insertuser(user instance);
    partial void Updateuser(user instance);
    partial void Deleteuser(user instance);
    #endregion
		
		public AlexDBDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["alex_dbConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public AlexDBDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AlexDBDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AlexDBDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AlexDBDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<user> users
		{
			get
			{
				return this.GetTable<user>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.users")]
	public partial class user : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _userID;
		
		private string _username;
		
		private string _password;
		
		private string _email;
		
		private string _fname;
		
		private string _lname;
		
		private string _website;
		
		private string _phone;
		
		private string _fax;
		
		private string _comments;
		
		private System.DateTime _dateAdded;
		
		private int _isActive;
		
		private int _superUser;
		
		private int _stateID;
		
		private string _city;
		
		private string _address;
		
		private int _isDealer;
		
		private string _biography;
		
		private string _photo;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnuserIDChanging(int value);
    partial void OnuserIDChanged();
    partial void OnusernameChanging(string value);
    partial void OnusernameChanged();
    partial void OnpasswordChanging(string value);
    partial void OnpasswordChanged();
    partial void OnemailChanging(string value);
    partial void OnemailChanged();
    partial void OnfnameChanging(string value);
    partial void OnfnameChanged();
    partial void OnlnameChanging(string value);
    partial void OnlnameChanged();
    partial void OnwebsiteChanging(string value);
    partial void OnwebsiteChanged();
    partial void OnphoneChanging(string value);
    partial void OnphoneChanged();
    partial void OnfaxChanging(string value);
    partial void OnfaxChanged();
    partial void OncommentsChanging(string value);
    partial void OncommentsChanged();
    partial void OndateAddedChanging(System.DateTime value);
    partial void OndateAddedChanged();
    partial void OnisActiveChanging(int value);
    partial void OnisActiveChanged();
    partial void OnsuperUserChanging(int value);
    partial void OnsuperUserChanged();
    partial void OnstateIDChanging(int value);
    partial void OnstateIDChanged();
    partial void OncityChanging(string value);
    partial void OncityChanged();
    partial void OnaddressChanging(string value);
    partial void OnaddressChanged();
    partial void OnisDealerChanging(int value);
    partial void OnisDealerChanged();
    partial void OnbiographyChanging(string value);
    partial void OnbiographyChanged();
    partial void OnphotoChanging(string value);
    partial void OnphotoChanged();
    #endregion
		
		public user()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_userID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int userID
		{
			get
			{
				return this._userID;
			}
			set
			{
				if ((this._userID != value))
				{
					this.OnuserIDChanging(value);
					this.SendPropertyChanging();
					this._userID = value;
					this.SendPropertyChanged("userID");
					this.OnuserIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_username", DbType="VarChar(255) NOT NULL", CanBeNull=false)]
		public string username
		{
			get
			{
				return this._username;
			}
			set
			{
				if ((this._username != value))
				{
					this.OnusernameChanging(value);
					this.SendPropertyChanging();
					this._username = value;
					this.SendPropertyChanged("username");
					this.OnusernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_password", DbType="VarChar(255) NOT NULL", CanBeNull=false)]
		public string password
		{
			get
			{
				return this._password;
			}
			set
			{
				if ((this._password != value))
				{
					this.OnpasswordChanging(value);
					this.SendPropertyChanging();
					this._password = value;
					this.SendPropertyChanged("password");
					this.OnpasswordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_email", DbType="VarChar(255)")]
		public string email
		{
			get
			{
				return this._email;
			}
			set
			{
				if ((this._email != value))
				{
					this.OnemailChanging(value);
					this.SendPropertyChanging();
					this._email = value;
					this.SendPropertyChanged("email");
					this.OnemailChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_fname", DbType="VarChar(255)")]
		public string fname
		{
			get
			{
				return this._fname;
			}
			set
			{
				if ((this._fname != value))
				{
					this.OnfnameChanging(value);
					this.SendPropertyChanging();
					this._fname = value;
					this.SendPropertyChanged("fname");
					this.OnfnameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_lname", DbType="VarChar(255)")]
		public string lname
		{
			get
			{
				return this._lname;
			}
			set
			{
				if ((this._lname != value))
				{
					this.OnlnameChanging(value);
					this.SendPropertyChanging();
					this._lname = value;
					this.SendPropertyChanged("lname");
					this.OnlnameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_website", DbType="VarChar(255)")]
		public string website
		{
			get
			{
				return this._website;
			}
			set
			{
				if ((this._website != value))
				{
					this.OnwebsiteChanging(value);
					this.SendPropertyChanging();
					this._website = value;
					this.SendPropertyChanged("website");
					this.OnwebsiteChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_phone", DbType="VarChar(50)")]
		public string phone
		{
			get
			{
				return this._phone;
			}
			set
			{
				if ((this._phone != value))
				{
					this.OnphoneChanging(value);
					this.SendPropertyChanging();
					this._phone = value;
					this.SendPropertyChanged("phone");
					this.OnphoneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_fax", DbType="VarChar(50)")]
		public string fax
		{
			get
			{
				return this._fax;
			}
			set
			{
				if ((this._fax != value))
				{
					this.OnfaxChanging(value);
					this.SendPropertyChanging();
					this._fax = value;
					this.SendPropertyChanged("fax");
					this.OnfaxChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_comments", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string comments
		{
			get
			{
				return this._comments;
			}
			set
			{
				if ((this._comments != value))
				{
					this.OncommentsChanging(value);
					this.SendPropertyChanging();
					this._comments = value;
					this.SendPropertyChanged("comments");
					this.OncommentsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dateAdded", DbType="DateTime NOT NULL")]
		public System.DateTime dateAdded
		{
			get
			{
				return this._dateAdded;
			}
			set
			{
				if ((this._dateAdded != value))
				{
					this.OndateAddedChanging(value);
					this.SendPropertyChanging();
					this._dateAdded = value;
					this.SendPropertyChanged("dateAdded");
					this.OndateAddedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_isActive", DbType="Int NOT NULL")]
		public int isActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if ((this._isActive != value))
				{
					this.OnisActiveChanging(value);
					this.SendPropertyChanging();
					this._isActive = value;
					this.SendPropertyChanged("isActive");
					this.OnisActiveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_superUser", DbType="Int NOT NULL")]
		public int superUser
		{
			get
			{
				return this._superUser;
			}
			set
			{
				if ((this._superUser != value))
				{
					this.OnsuperUserChanging(value);
					this.SendPropertyChanging();
					this._superUser = value;
					this.SendPropertyChanged("superUser");
					this.OnsuperUserChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_stateID", DbType="Int NOT NULL")]
		public int stateID
		{
			get
			{
				return this._stateID;
			}
			set
			{
				if ((this._stateID != value))
				{
					this.OnstateIDChanging(value);
					this.SendPropertyChanging();
					this._stateID = value;
					this.SendPropertyChanged("stateID");
					this.OnstateIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_city", DbType="VarChar(250)")]
		public string city
		{
			get
			{
				return this._city;
			}
			set
			{
				if ((this._city != value))
				{
					this.OncityChanging(value);
					this.SendPropertyChanging();
					this._city = value;
					this.SendPropertyChanged("city");
					this.OncityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_address", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string address
		{
			get
			{
				return this._address;
			}
			set
			{
				if ((this._address != value))
				{
					this.OnaddressChanging(value);
					this.SendPropertyChanging();
					this._address = value;
					this.SendPropertyChanged("address");
					this.OnaddressChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_isDealer", DbType="Int NOT NULL")]
		public int isDealer
		{
			get
			{
				return this._isDealer;
			}
			set
			{
				if ((this._isDealer != value))
				{
					this.OnisDealerChanging(value);
					this.SendPropertyChanging();
					this._isDealer = value;
					this.SendPropertyChanged("isDealer");
					this.OnisDealerChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_biography", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string biography
		{
			get
			{
				return this._biography;
			}
			set
			{
				if ((this._biography != value))
				{
					this.OnbiographyChanging(value);
					this.SendPropertyChanging();
					this._biography = value;
					this.SendPropertyChanged("biography");
					this.OnbiographyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_photo", DbType="VarChar(500)")]
		public string photo
		{
			get
			{
				return this._photo;
			}
			set
			{
				if ((this._photo != value))
				{
					this.OnphotoChanging(value);
					this.SendPropertyChanging();
					this._photo = value;
					this.SendPropertyChanged("photo");
					this.OnphotoChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
