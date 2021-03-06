﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Videre.Core.Extensions;

namespace Videre.Core.Extensions.Bootstrap
{
    public abstract class BootstrapControlBase<TControl, TModel> 
        where TModel : BootstrapBaseControlModel, new()
        where TControl : class, IBootstrapBaseControl
    {
        protected TControl _control = null;
        protected HtmlHelper _html;
        protected TModel _model = new TModel();
        protected Videre.Core.Models.IClientControl _clientControl = null;

        public BootstrapControlBase(HtmlHelper html)
        {
            this._html = html;
            _clientControl = html.ViewData.Model as Videre.Core.Models.IClientControl;
            _control = this as TControl;
        }
        public BootstrapControlBase(HtmlHelper html, string id)
            : this(html)
        {
            if (!string.IsNullOrEmpty(id))
                this._model.id = GetId(id);
        }

        public string Id 
        {
            get { return _model.id; }
            set { _model.id = value; }
        }

        public TModel Model 
        { 
            get { return _model; } 
        }

        protected string GetText(string token, string defaultText)
        {
            if (_clientControl != null)
                return _clientControl.GetText(token, defaultText);
            return defaultText;
        }
        protected string GetPortalText(string token, string defaultText)
        {
            if (_clientControl != null)
                return _clientControl.GetPortalText(token, defaultText);
            return defaultText;
        }

        protected string GetId(string id)
        {
            if (_clientControl != null && !string.IsNullOrEmpty(id))
                return _clientControl.GetId(id);
            return id;
        }

        public TControl DataAttribute(string key, string value)
        {
            _model.htmlAttributes.AddSafe("data-" + key, value);
            return _control;
        }

        public TControl DataAttributes(object attributes)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(attributes))
                _model.htmlAttributes.AddSafe("data-" + property.Name, property.GetValue(attributes));
            return _control;
        }

        public TControl StyleAttribute(string key, string value)
        {
            _model.styleAttributes.AddSafe(key, value);
            return _control;
        }

        public TControl StyleAttributes(object attributes)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(attributes))
                _model.styleAttributes.AddSafe(property.Name, property.GetValue(attributes).ToString());
            return _control;
        }

        public TControl ToolTip(string token, string text)
        {
            _model.title = GetText(token, text);
            return _control;
        }

        public TControl Html(params IHtmlString[] html)
        {
            html.ToList().ForEach(x => this._model.html += x.ToHtmlString());
            return _control;
        }

        public void AddHtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            this._model.htmlAttributes = htmlAttributes;
        }

        public TControl HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            AddHtmlAttributes(htmlAttributes);
            return _control;
        }

        public void AddHtmlAttributes(object htmlAttributes)
        {
            this._model.htmlAttributes.Merge(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)); 
        }
        public TControl HtmlAttributes(object htmlAttributes)
        {
            AddHtmlAttributes(htmlAttributes);
            return _control;
        }

        public void AddCss(string css)
        {
            this._model.CssClasses.AddRange(css.Split(' '));
        }

        public TControl Css(string css)
        {
            AddCss(css);
            return _control;
        }

        public TControl GridSize(BootstrapUnits.GridSize size)
        {
            AddCss(BootstrapUnits.GetGridSizeCss(size));
            return _control;
        }

        public TControl DataColumn(string name)
        {
            AddHtmlAttributes(new { data_column = name });
            return _control;
        }

        public abstract string ToHtmlString();

        protected virtual void AddBaseMarkup(TagBuilder ctl)
        {
            if (!string.IsNullOrEmpty(_model.id))
                ctl.Attributes.AddSafe("id", _model.id);
            if (!string.IsNullOrEmpty(_model.title))
                ctl.Attributes.AddSafe("title", _model.title);
            ctl.MergeAttributes(_model.htmlAttributes);

            foreach (var style in _model.styleAttributes)
                ctl.AddCssStyle(style.Key, style.Value.ToString());

            foreach (var css in _model.CssClasses.Distinct())
                ctl.AddCssClass(css);
        }

    }

}
