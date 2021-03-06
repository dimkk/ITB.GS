﻿/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />

var renderCore;

(function (renderCore) {
    renderCore.init = function init(context) {
        this.ctx = context;
        return this;
    };

    renderCore.getInternalFieldName = function (displayName) {
        var res = '';

        var fields = $.grep(this.ctx.ListSchema.Field, function (item) {
            return item.Title == displayName;
        });
        if (fields.length != 0) {
            res = fields[0].Name;
        }

        return res;
    };

    renderCore.getField = function (fieldName) {
        var fields = $.grep(this.ctx.ListSchema.Field, function (item) { return item.Name == fieldName; });
        return fields.length == 1 ? fields[0] : null;
    };

    renderCore.getControlByFieldName = function (fieldName) {
        var field = renderCore.getField(fieldName);
        var fieldType = field.FieldType + 'Field';
        if (field.FieldType == 'Choice')
            fieldType = 'DropDownChoice';
        return renderCore.getControlById(field.Name + '_' + field.Id + '_$' + fieldType);
    };

    renderCore.getFieldTitle = function (fieldName) {
        var field = renderCore.getField(fieldName);
        return field ? field.Title : '';
    };

    renderCore.renderFieldByName = function (fieldName) {
        return this.ctx.RenderFieldByName(this.ctx, fieldName);
    };

    renderCore.renderFieldByDisplayName = function (displayName) {
        var fieldName = this.getInternalFieldName(displayName);
        if (!fieldName) return;

        return this.ctx.RenderFieldByName(this.ctx, fieldName);
    };

    renderCore.getControlFromRenderedHtml = function (html, controlType) {
        var htmldoc = $("<div></div>").append(html);
        return htmldoc.find(controlType);
    };

    renderCore.getLookupFromRenderedHtml = function (html) {
        return this.getControlFromRenderedHtml(html, 'select');
    };

    renderCore.getInputFromRenderedHtml = function (html) {
        return this.getControlFromRenderedHtml(html, 'input');
    };

    renderCore.getControlById = function (id) {
        return $('#' + id.replace(/[!"#$%&'()*+,.\/:;<=>?@[\\\]^`{|}~]/g, "\\$&"));
    };

    renderCore.formatDate = function (date) {
        if (date === undefined) return;

        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();

        return (String).format("{0}/{1}/{2}", day < 10 ? "0" + day : day, month < 10 ? "0" + month : month, year);
    };

    renderCore.ifget = function (elId, callback) {
        var element = $get(elId);
        if (element) {
            callback(element);
        }
    };

	renderCore.getParamsFromUrl = function (url) {
        var params = url.split('?')[1].split('&');
		var paramList = [];
        for (var i = 0; i < params.length; i++) {
            var param = params[i].split('=');
            paramList.push({ Key: param[0], Value: param[1]});
        }
		return paramList;
	}

    renderCore.getUrlParamValue = function (url, key) {
		var v = renderCore.getParamsFromUrl(url).filter(function(e) {
			return e.Key == key })[0];
		return v ? v.Value : null;
    };
	
	renderCore.getUrlId = function (url, parentUrlContainsList) {
        if (!url ||
            url.indexOf('?') < 0 ||
            !parentUrlContainsList.some(function (e) { return ~url.indexOf(e); }))
            return null;

		return renderCore.getUrlParamValue(url, 'ID');
	}
	
    renderCore.getParentListItemId = function (parentUrlContainsList) {
		return window.self !== window.top ?
			renderCore.getUrlId(window.top.document.URL, parentUrlContainsList) :
			renderCore.getUrlId(document.referrer, parentUrlContainsList);
    };

    renderCore.hasParentContext = function (parentListName) {
        var hasContext = document.referrer &&
        (~document.referrer.indexOf(parentListName + '/DispForm') ||
            ~document.referrer.indexOf(parentListName + '/EditForm'));
        return hasContext;
    };

    renderCore.getParentIdFromContext = function (parentListName) {
        var hasContext = document.referrer &&
        (~document.referrer.indexOf(parentListName + '/DispForm') ||
            ~document.referrer.indexOf(parentListName + '/EditForm'));
        if (!hasContext) return null;

        var params = document.referrer.split('?')[1].split('&');
        for (var i = 0; i < params.length; i++) {
            var param = params[i].split('=');
            if (param[0] !== 'ID') continue;

            return param[1];
        }

        return null;
    };

    // bootstrap
    renderCore.bs = {
        getLabel: function (value, span) {
            return '<label class="col-lg-' + span + '">' + value + '</label>';
        },

        applyCSS: function (html) {
            if (!html) return;

            var container = document.createElement("div");
            container.innerHTML = html;

            $.each(container.getElementsByTagName("textarea"), function () {
                this.className = 'form-control';
            });
            $.each(container.querySelectorAll('input:not([type="checkbox"])'), function () {
                this.className = 'form-control';
            });
            $.each(container.getElementsByTagName("select"), function () {
                this.className = 'form-control';
            });

            return container.innerHTML;
        },

        renderFieldByName: function (fieldName, noFormControlClass) {
            var html = renderCore.renderFieldByName(fieldName);
            var controlMode = renderCore.ctx.FieldControlModes[fieldName];

            if (controlMode == SPClientTemplates.ClientControlMode.DisplayForm) {
                return html;
            }

            if (!noFormControlClass) {
                return this.applyCSS(html);
            } else {
                return html;
            }
        },
        renderFieldBlock: function (label, labelSpan, inputSpan, fieldName, controlInvisible, noFormControlClass) {
            var resultHtml = '';
            resultHtml += this.getLabel(label, labelSpan);
            resultHtml += '<div' + (controlInvisible ? ' style="display:none"' : '') + ' class="col-lg-' + inputSpan + '">';
            resultHtml += this.renderFieldByName(fieldName, noFormControlClass);
            resultHtml += '</div>';

            return resultHtml;
        },
        renderModalLink: function (target, text, width, lookupControlId) {
            var elementId = Math.random().toString(36).substr(2);
            var resultHtml = '';

            resultHtml += (String).format(
                '<div class="col-lg-{0}">', width > 12 ? 12 : width);
            resultHtml += (String).format(
                    '<a data-toggle="modal" href="{0}" data-target="#{1}" data-lookup="#{2}" class="btn btn-default">{3}</a>', target, elementId, lookupControlId, text);
            resultHtml +=
                '</div>';
            resultHtml += (String).format(
                '<div class="modal fade" id="{0}" role="dialog" aria-hidden="true">', elementId);
            resultHtml +=
                '</div>';

            return { html: resultHtml, modalId: elementId };
        }
    };
})(renderCore || (renderCore = {}));
