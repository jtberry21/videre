﻿using System.Collections.Generic;
//using System.Web.Script.Serialization;
using CodeEndeavors.Extensions.Serialization;
using Videre.Core.Extensions;
using CodeEndeavors.Extensions;
using System;
//using Newtonsoft.Json;

namespace Videre.Core.Models
{
    public class PageTemplate 
    {
        public PageTemplate()
        {
            Widgets = new List<Widget>();
            Urls = new List<string>();
            //Roles = new List<string>();
            WebReferences = new List<string>();
        }

        //[JsonIgnore()]
        public string Id { get; set; }
        //public string Name { get; set; }
        public string Title { get; set; }
        
        public string LayoutId { get; set; }
        [Obsolete("Use LayoutId")]
        public string LayoutName { get; set; }
        public string ThemeName { get; set; }
        public List<string> Urls { get; set; }
        public string PortalId { get; set; }
        private List<string> _roles = new List<string>();
        [System.Obsolete("Use RoleIds")]
        [SerializeIgnore(new string[] { "client" })]
        public List<string> Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }

        private List<string> _roleIds = new List<string>();
        public List<string> RoleIds
        {
            get
            {
                if (_roles != null && _roles.Count > 0)
                {
                    _roleIds.AddRange(_roles);
                    _roles.Clear();
                }
                return _roleIds;
            }
            set
            {
                _roleIds = value;
            }
        }
        public bool? Authenticated { get; set; }
        public List<string> WebReferences { get; set; }

        public List<Widget> Widgets { get; set; }

        [SerializeIgnore(new string[] { "db", "client" })]
        public Models.Theme Theme
        {
            get
            {
                return Services.UI.GetTheme(ThemeName);
            }
        }

        [SerializeIgnore(new string[] {"db", "client"})]         
        public bool IsDefault
        {
            get
            {
                return Urls.Count == 0 || Urls.Contains("");
            }
        }

        //[JsonIgnore]
        [SerializeIgnore("db")] //todo: we need this on client?
        public bool IsAuthorized
        {
            get
            {
                return Services.Account.RoleAuthorized(RoleIds) &&
                    (!Authenticated.HasValue || Authenticated == Services.Account.IsAuthenticated);
            }
        }

        [SerializeIgnore(new string[] { "db", "client" })]
        public Models.LayoutTemplate Layout
        {
            get
            {
                return Services.Portal.GetLayoutTemplateById(LayoutId);
            }
        }

        //[ScriptIgnore, JsonIgnore()]
        [SerializeIgnore(new string[] {"db", "client"})]
        public List<Widget> LayoutWidgets
        {
            get
            {
                var template = Layout;
                if (template != null && template.IsAuthorized)
                    return template.Widgets;
                return new List<Widget>();
            }
        }

        public Dictionary<string, string> GetWidgetContent()
        {
            var dict = new Dictionary<string, string>();
            string json;
            foreach (var widget in Widgets)
            {
                json = widget.GetContentJson();
                if (!string.IsNullOrEmpty(json))
                    dict[widget.Id] = json;
            }
            return dict;
        }

        public void SaveContent(Dictionary<string, string> content)
        {
            //todo: remove content from widgets no longer in existance
            foreach (var widget in Widgets)
                widget.SaveContentJson(widget.ContentJson);
        }

        //[SerializeIgnore(new string[] { "db" })]  
        //public string UrlId   //todo: kind of a hack as we don't want to "corrupt" global cached instance... so use context...  should we place this somewhere else... widget needs access
        //{
        //    get
        //    {
        //        return Services.Portal.GetRequestContextData<string>("UrlId", null);
        //    }
        //    set
        //    {
        //        Services.Portal.SetRequestContextData("UrlId", value);
        //    }
        //}

    }

}
