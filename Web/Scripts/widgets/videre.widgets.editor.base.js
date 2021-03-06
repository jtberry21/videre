﻿videre.registerNamespace('videre.widgets');
videre.registerNamespace('videre.widgets.editor');

videre.widgets.editor.base = videre.widgets.base.extend(
{
    //constructor
    init: function()
    {
        this._base();  //call base method
        this._widgetData = null;
        this._manifestData = null;
        this._dialog = null;
        this._dirty = false;
        this._contentAdmin = false;

        this._baseEditorDelegates = {
            onItemBind: videre.createDelegate(this, this._onItemBind),
            onDependencyControlChange: videre.createDelegate(this, this._onDependencyControlChange)
        }
    },

    _onLoad: function(src, args)
    {
        this._base(); //call base
        this._dialog = this.getControl('Dialog').modal('hide').hide();
        this.getControl('btnOk').click(videre.createDelegate(this, this._onOkClicked));
        this._widget.find('[data-column]').change(videre.createDelegate(this, this._onDataChange));
    },

    show: function(widget, manifest, contentAdmin)
    {
        if (contentAdmin == null)
            contentAdmin = true;
        this._widgetData = widget;
        this._manifestData = manifest;
        this._contentAdmin = contentAdmin;
        this.bindData(this._widgetData, this.getControl('CommonProperties'));
        this.bindAttributes();
        this._dialog.modal('show');
        this.clearMsgs(this._dialog);
        this._dirty = false;
    },

    bindAttributes: function()
    {
        var ctr = this.getControl('AttributeList');
        ctr.html(this.getControl('AttributeListTemplate').render(this._manifestData.AttributeDefinitions.orderBy(function(d) { return d.Label; }), { attributes: this._widgetData.Attributes }));
        ctr.toggle(this._manifestData.AttributeDefinitions.length > 0);
        this._handleDependencies(ctr);

        //todo: should have global registry for actions to take after binding
        //ctr.find('[data-datatype="date"]').datepicker();
        //ctr.find('[data-controltype="bootstrap-select"]').selectpicker();
        ctr.find('[title]').tooltip();
        videre.UI.initializeControlTypes(ctr);

    },

    validate: function()
    {
        return this.validControls(this.getControl('CommonProperties'), this._widget);
    },

    save: function()
    {
        if (this.validControls(this._dialog, this._dialog))
        {
            this.persistData(this._widgetData, false, this.getControl('CommonProperties'));
            this.persistData(this._widgetData.Attributes, false, this.getControl('AttributeList'));
            this.raiseCustomEvent('Save', this._widgetData);
            //this._dialog.modal('hide');   //we don't know save was successful.. if errored we need to see the error, so cannot just assume we close.
        }
    },

    hide: function()
    {
        this._dialog.modal('hide');
    },

    _onOkClicked: function(e)
    {
        if (this.validate())
            this.save();
    },

    _onDataChange: function (e)
    {
        this._dirty = true;
    },

    _onDependencyControlChange: function(e)
    {
        this._handleDependencies(this.getControl('AttributeList'));
    },

    _handleDependencies: function(ctr)
    {
        var self = this;
        $.each(this._getDependencyCtrls(ctr), function(idx, item)
        {
            var match = self._getDependencyMatch(item);
            if (!match)
                item.ctl.val('');
            item.ctl.data('dependencymatch', match);
            item.controlGroup.toggle(match);
        });
    },

    _getDependencyMatch: function(d)
    {
        var ret = true;
        for (var i = 0; i < d.dependencies.length; i++)
        {
            var val = d.dependencies[i].ctl.val();
            ret = ret && d.dependencies[i].HasValue ? !String.isNullOrEmpty(val) : d.dependencies[i].Values.contains(val);
        }
        return ret;
    },

    _getDependencyCtrls: function(ctr)
    {
        var ret = [];
        var ctls = ctr.find('[data-dependencies]');
        
        for (var i = 0; i < ctls.length; i++)
        {
            var o = {};
            o.ctl = $(ctls[i]);
            o.dependencies = o.ctl.data('dependencies'); 
            for (var i = 0; i < o.dependencies.length; i++)
                o.dependencies[i].ctl = ctr.find('[data-column="' + o.dependencies[i].DependencyName + '"]').change(this._baseEditorDelegates.onDependencyControlChange);
            o.controlGroup = o.ctl.closest('.form-group');
            ret.push(o);
        }
        return ret;
    }

});

