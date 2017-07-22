<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentImport.aspx.cs" Inherits="Specialized.Content.Import.UI.ContentImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Content Import</title>
    <script type="text/javascript" src="http://code.jquery.com/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" src="http://code.jquery.com/ui/1.11.4/jquery-ui.min.js"></script>
    <link type="text/css" rel="stylesheet" href="http://code.jquery.com/ui/1.11.4/themes/redmond/jquery-ui.css" />

    <style type="text/css">
        html, body {
            color: #303030;
            font-family: Helvetica,Arial,sans-serif;
            font-size: 14px;
            letter-spacing: .05em;
            min-height: 100%;
            overflow-x: hidden;
            width: 100%;
        }

        input[type=file] {
            padding: 3px 8px;
            width: 350px;
        }

        input[type=text], select {
            width: 200px;
        }

        option {
            width: auto;
        }

        input, select, .ui-widget input, .ui-widget select {
            border: 2px solid #79B7E7;
            box-sizing: border-box;
            font-family: Helvetica,Arial,sans-serif;
            font-size: 12px;
            padding: 6px 8px;
        }

            input[type="submit"], input[type="button"], button, .ui-widget button, .button-group button {
                background: #fff;
                border: 2px solid #73A80A;
                border-radius: 0;
                box-sizing: border-box;
                color: #303030;
                cursor: pointer;
                font-family: Helvetica,Arial,sans-serif;
                font-size: 13px;
                font-weight: bold;
                letter-spacing: .05em;
                padding: 5px 8px;
            }

                input[type="submit"]:hover, input[type="button"]:hover, button:hover {
                    background: #73A80A;
                    color: #fff;
                }

                input[type="submit"]:active, input[type="button"]:active, button:active {
                    text-decoration: underline;
                }

                input.go[type="submit"], input.go[type="button"], button.go {
                    background: #73A80A;
                    color: #fff;
                }

                    input.go[type="submit"]:hover, input.go[type="button"]:hover, button.go:hover {
                        background: #fff;
                        color: #73A80A;
                    }

                input.warning[type="submit"], input.warning[type="button"], button.warning {
                    background: #E52A00;
                    border: 2px solid #E52A00;
                    color: #fff;
                    font-weight: bold;
                }

                    input.warning[type="submit"]:hover, input.warning[type="button"]:hover, button.warning:hover {
                        background: #fff;
                        color: #E52A00;
                    }

                input.caution[type=submit], input.caution[type=button], button.caution {
                    background: #fff;
                    border-color: #EF8200;
                    color: #E57A00;
                }

                    input.caution[type=submit]:hover, input.caution[type=button]:hover, button.caution:hover {
                        background: #EF8200;
                        color: #fff;
                    }

                input.ui-button::-moz-focus-inner,
                button.ui-button::-moz-focus-inner {
                    border: 0;
                    padding: 1px;
                }

        .button-group .menu .ui-button-text {
            padding: 0;
        }

        .button-group .buttons {
            display: block;
            overflow-y: auto;
        }

            .button-group .buttons button,
            .button-group .buttons [type="button"] {
                float: left;
                margin: 0;
            }

            .button-group .buttons .menu.ui-state-focus {
                border-left: none;
                border-style: solid;
            }

            .button-group .buttons .ui-corner-left {
            }

            .button-group .buttons .ui-corner-right {
                border-left: none !important;
            }

        .button-group .ui-button .ui-icon {
            background-image: url("http://code.jquery.com/ui/1.11.4/themes/smoothness/images/ui-icons_454545_256x240.png");
        }

        .button-group .ui-button:hover .ui-icon {
            background-image: url("http://download.jqueryui.com/themeroller/images/ui-icons_ffffff_256x240.png");
        }

        .button-group .group-menu {
            background: none;
            border-color: #73A80A;
            border-style: solid;
            border-width: 1px 2px;
            list-style: none;
            margin: 0;
            padding: 0;
            position: absolute;
            z-index: 99;
        }

            .button-group .group-menu li {
                margin: 0;
            }

                .button-group .group-menu li button {
                    background: #fff;
                    border: none;
                    border-bottom: 1px solid #73A80A;
                    border-collapse: collapse;
                    color: #303030;
                    display: block;
                    font-size: 12px;
                    padding: 5px 10px;
                    text-align: left;
                    text-decoration: none;
                    width: 100%;
                }

                    .button-group .group-menu li button:hover {
                        background: #73A80A;
                        border-color: #73A80A;
                        color: #fff;
                    }

        p, ul {
            margin: 10px 0;
        }

        li {
            margin: 3px 0;
        }

        .title, .import-wrapper {
            border: 2px solid #148E9B;
            box-sizing: border-box;
            margin: 5px auto;
            width: 940px;
        }

        .title {
            border-bottom: 0;
            color: #FFFFFF;
            font-size: 18px;
            line-height: 40px;
            margin-bottom: 0;
            overflow-y: auto;
            padding: 0 0 0 15px;
        }

        .title.heading {
            background: #148E9B;
        }

            .title .version {
                background: #148E9B;
                float: right;
                font-size: 10px;
                line-height: normal;
                padding: 8px 15px;
            }

        .import-wrapper {
            border-top: 0;
            clear: both;
            margin-top: 0;
            min-height: 100%;            
            padding: 8px 15px;
        }


        .information {
            border: 1px solid #79B7E7;
            font-size: 14px;
            margin: 10px 0;
            overflow-y: auto;
            padding: 5px 15px;
        }

            .information .label {
                font-weight: bold;
            }

        .errors-row .information {
            background-color: #ffeeee;
            border-color: #ff0000; 
            color: #ff0000; 
        }

        .active-import-wrapper .input-row {
            margin-bottom: 0;
        }

        .active-import-wrapper .input-column:first-child {
            border: 1px solid #EF8200;
            padding: 0 10px;
        }

        .new-import-wrapper .input-column, .add-mapping-wrapper .input-column {
            width: auto;
        }

        .import-file, .worksheet-count {
            display: block;
            margin: 8px 0;
        }

        .worksheet-count {
        }

        .row {
            clear: both;
            margin: 20px 0;
            overflow-y: auto;
        }

        .input-row, check-row, .button-row {
            clear: both;
            margin: 12px 0;
            overflow-y: auto;
        }

        .button-row {
            margin: 20px 0;
        }

        .input-column .button-row {
            margin: 0;
        }

        .check-row .input-label, .check-row .input-field {
            width: auto;
        }

        .check-row.input-column {
            /* bit of an oxymoron in the selector */
            margin-left: 0;
        }

        .button-row input[type=button], .button-row button {
            margin-left: 30px;
        }

            .button-row input[type=button]:first-child, .button-row button:first-child {
                margin-left: 0;
            }

        .template-row .input-column {
            width: auto;
        }

        .template-row input[type="text"],
        .global-parent-row input {
            text-align: center;
            width: 275px;
        }

        .template-row a {
            color: #5E2244;
            cursor: pointer;
            font-size: 12px;
            line-height: 20px;
            margin-left: 8px;
            text-decoration: none;
            vertical-align: bottom;
        }

            .template-row a:hover {
                color: #73A80A;
            }

        .global-parent-row .input-label {
            width: 100px;
        }

        .input-column {
            float: left;
            margin-left: 15px;
            width: 400px;
        }

            .input-column:first-child {
                margin-left: 0;
            }

        .input-label, .input-field, .field-value {
            display: inline-block;
            line-height: 30px;
            vertical-align: top;
        }

        .input-label {
            padding-right: 10px;
            text-align: right;
            width: 150px;
        }

        .field-wrapper .information {
            border: 1px solid #EF8200;
        }

        .field-wrapper .input-row {
            margin: 15px 0;
        }

        .field-wrapper .input-column {
            width: 265px;
        }

        .sub-text {
            clear: both;
            display: block;
            font-size: 12px;
        }

            .sub-text .input-field {
                line-height: 25px;
                padding-left: 8px;
            }

        input.field-name-input {
            border: none;
            text-align: right;
            width: 180px;
        }
        .xml-mappings input.field-name-input {
            text-align: left;
        }

        input.column-number-input {
            width: 75px;
        }

        .hidden {
            display: none;
        }

        .invisible {
            visibility: hidden;
        }

        [readonly] {
            background: #c8d8e4;
            color: #535353;
        }

        .remove-link {
            font-size: 14px;
            line-height: 26px;
            margin-right: 5px;
        }

        .validation-text[style*=inline] {
            color: #EF0000;
            display: block !important;
        }

        .invalid-tab {
            color: #ff0000;
        }

        .left {
            text-align: left;
        }

        .right {
            text-align: right;
        }

        .pull-right {
            float: right;
        }

        .pull-left {
            float: left;
        }

        .spinner {
            background: url("http://spiffygif.com/gif?color=000&lines=9&trail=66&halo=false") no-repeat scroll 0 0 transparent;
            display: block;
            height: 30px;
            margin-top: 15px;
            width: 30px;
        }

        .toggle-block {
            color: #505050;
            cursor: pointer;
            text-decoration: none;
        }

        .new-import-wrapper h4 {
            background-color: #9C9C9C;
            color: #fff;
            font-size: 16px;
            line-height: 30px;
            margin: 6px 0px 0px;
            padding: 0px 0px 0px 12px;
        }

            .new-import-wrapper h4 .spinner {
                margin: 0;
            }

        .new-import-wrapper .job-type-selections {
            background-color: #f0f0f8;
            margin: 0px;
            padding: 12px 18px;
        }

        .job-type-headers {
            position: relative;
            z-index: 5;
        }

        .type-selection-header {
            background-color: #6986D3;
            border-color: #6986D3;
            border-style: solid;
            border-width: 2px;
            color: #FFF;
            cursor: pointer;
            font-weight: bold;
            line-height: 28px;
            text-align: right;
            text-transform: uppercase;
            padding: 0px 12px 0px 20px;
        }

            .type-selection-header.active, .type-selection-header.active:hover {
                background-color: #fff;
                border-color: #6986D3;
                border-right-width: 0;
                color: #6986D3;
            }

            .type-selection-header:hover {
                background-color: #9AA9CE;
                border-color: #9AA9CE;
                border-right-color: #6986D3;
            }

        [class*="type-wrapper"] {
            background-color: #fff;
            border: 2px solid #6986D3;
            padding: 6px 18px;
            margin-left: -2px;
        }

            [class*="type-wrapper"] .input-label {
                display: block;
                text-align: left;
                width: auto;
            }

        .xml-type-selector {
            margin: 0;
        }

            .xml-type-selector input[type="text"] {
                width: 350px;
            }

        .xls-type-heading {
            background-color: #6986D3;
            color: #fff;
            display: block;
            line-height: 24px;
            margin: 0px;
            padding: 2px 8px 0px;
            text-transform: uppercase;
        }

        .xml-type-choices {
            display: table;
            margin: 0;
            width: 100%;
        }

            .xml-type-choices .xml-choice {
                background-color: #fff;
                color: #6986D3;
                cursor: pointer;
                display: table-cell;
                line-height: 24px;
                padding: 2px 8px 0;
                text-align: center;
                text-transform: uppercase;
            }

                .xml-type-choices .xml-choice.active, .xml-type-choices .xml-choice.active:hover {
                    background-color: #6986D3;
                    color: #fff;
                }

                .xml-type-choices .xml-choice:hover {
                    background-color: #9AA9CE;
                    color: #fff;
                }

        .xml-input-wrapper {
            border-color: #6986D3;
            border-style: solid;
            border-width: 2px 2px 0;
            padding: 10px 18px;
            width: 385px;
        }

        .font-button {
            background-color: #79B7E7;
            color: #fff;
            cursor: pointer;
            display: inline-block;
            font-size: 20px;
            font-weight: normal;
            height: 20px;
            letter-spacing: normal;
            line-height: 20px;
            text-align: center;
            vertical-align: middle;
            width: 20px;
        }

            .font-button:hover, .add-field-button:hover .font-button {
                background-color: #0082E5;
            }

        .minus-btn:before {
            content: "−";
        }

        .add-btn:before {
            content: "+";
        }

        .add-field-button {
            cursor: pointer;
        }

        .progress {
            margin: 15px 0;
        }

        .progress-label {
            overflow-y: auto;
            padding: 0 20px;
            text-align: left;
            width: 460px;
        }

        .progress-current, .progress-total {
            display: block;
            margin: 5px 0;
        }

        .progress-current {
            float: left;
        }

        .progress-total {
            float: right;
        }

        .progress-bar-wrapper {
            border: 2px solid #155786;
            clear: both;
            height: 40px;
            margin: 5px 0;
            width: 500px;
        }

        .progress-bar {
            background: #73A80A;
            height: 50%;
            width: 0;
        }

            .progress-bar.total {
                background: #155786;
            }

        .progress-report {
            margin: 15px 0;
            max-height: 480px;
            overflow-y: scroll;
        }

            .progress-report table {
                border: medium none;
                border-collapse: collapse;
                width: 100%;
            }

            .progress-report th {
                background: none repeat scroll 0 0 #EFEFEF;
                border-bottom: 1px solid #EF8200;
                color: #148E9B;
                height: 30px;
                min-width: 110px;
                text-align: left;
                text-indent: 10px;
            }

            .progress-report td {
                border-bottom: 1px solid #EFEFEF;
                font-size: 13px;
                min-height: 100px;
                padding: 8px 0 8px 10px;
            }

                .progress-report td.row-num {
                    padding-left: 0;
                    text-align: center;
                }

        #import-modal {
            display: none;
        }

            #import-modal .button-row,
            #import-modal .button-row input[type="button"],
            #import-modal .button-row button {
                margin-bottom: 0;
                margin-left: 0;
            }

            #import-modal .spinner {
                margin: 0 3px;
            }

        /* UI overrides */
        .ui-widget-content {
            border: none;
        }

        .ui-widget-header {
            background-color: #155786;
            background-image: none;
            border: none;
            border-radius: 0 !important;
        }

        .ui-widget-overlay {
            background: #333333;
            opacity: .5;
        }

        .ui-state-active, .ui-widget-content .ui-state-active, .ui-widget-header .ui-state-active {
            background: #fcfdfd;
        }

        .ui-widget input, .ui-widget select, .ui-widget textarea, .ui-widget button {
            font-family: Helvetica,Arial,sans-serif;
        }

        .ui-tabs .ui-tabs-nav .ui-tabs-anchor {
            font-size: 12px;
        }

        .ui-tabs .ui-tabs-panel {
            border: 1px solid #155786;
            font-size: 14px;
        }

        .ui-tabs .ui-tabs-nav li {
            border-radius: 0;
        }
    </style>
    <script type="text/javascript">
        var globalVar = {
            pollInterval: 250,
            updateTry: 0,
            editRowMax: 10
        };

        function setButtonGroups() {
            $('.button-group')
                .find('.menu').button({
                    text: false,
                    icons: {
                        primary: 'ui-icon-triangle-1-s'
                    }
                }).click(function (e) {
                    var menu = $(this).closest('.button-group')
                        .find('.group-menu').show()
                        .position({
                            my: "right top",
                            at: "right bottom",
                            of: this
                        })
                        ;
                    $(document).one("click", function () {
                        menu.hide();
                    });
                    return false;
                }).end()
                .find('.button').button().end()
                .children('.buttons').buttonset().end()
                .children('.group-menu').hide()
                ;
        }

        function setTabs(idx) {
            $('.tab-view').tabs({ active: idx });
        }

        (function ($) {
            $(function () {
                var importModal = $('#import-modal').dialog({
                    autoOpen: false,
                    appendTo: 'body > form',
                    close: function () {
                        $('#import-modal .spinner').addClass('invisible');
                    },
                    modal: true,
                    resizable: false,
                    width: 390
                });

                importModal.find('.cancel').click(function (e) {
                    e.preventDefault();
                    importModal.dialog("close");
                }).end()
                    .find('#btnImportMappings').click(function () {
                        $('#import-modal .spinner').removeClass('invisible');
                    });

                var slideSpeed = 200;
                $(document)
                    .on('click', '.new-import-wrapper :button', function () {
                        $('.new-import-wrapper .spinner').removeClass('invisible');
                    })
                    .on('change', '.parent-row-wrapper .parent-row-sourcce select', function () {
                        var $t = $(this);
                        var opt = $t.find(':selected').val();
                        var $par = $t.closest('.parent-row-wrapper');
                        var $gpr = $par.find('.global-parent-row');
                        var $ipr = $par.find('.item-parent-row');
                        if (opt == 'global') {
                            $gpr.slideDown(slideSpeed);
                            $ipr.slideUp(slideSpeed);
                        }
                        else {
                            $ipr.slideDown(slideSpeed);
                            $gpr.slideUp(slideSpeed);
                        }
                    })
                    .on('click', '[data-toggle-selector]', function () {
                        var $t = $(this);
                        var group = $t.data('toggle-group');
                        var sel = $t.data('toggle-selector');
                        var target = $('[data-toggle-target="' + sel + '"]');
                        if (target.length > 0) {
                            var groupEle = $('[data-toggle-group="' + group + '"]');
                            groupEle.filter('[data-toggle-selector]').removeClass('active');
                            groupEle.filter('[data-toggle-target]').addClass('hidden');
                            target.removeClass('hidden');
                            $t.addClass('active');
                        }
                    })
                    .on('click', '.xml-remove-field', function () {
                        var parent = $(this).closest('.edit-row-parent');
                        var editRows = parent.find('.edit-row');

                        if (editRows.length <= 1) { return; }

                        $(this).closest('.edit-row').remove();

                        if (editRows.length <= globalVar.editRowMax) {
                            parent.find('.add-field-button').removeClass('hidden');
                        }
                    })
                    .on('click', '.add-field-button', function () {
                        var parent = $(this).closest('.edit-row-parent');
                        var editRows = parent.find('.edit-row');
                        if (editRows.length >= globalVar.editRowMax) {
                            return;
                        }

                        var template = parent.find('script[type="text/template"]').first();
                        if (template.length <= 0) { return; }
                        var $newRow = $(template.text());
                        $newRow.addClass('edit-row');
                        editRows.last().after($newRow);
                        if (editRows.length >= globalVar.editRowMax - 1) {
                            $(this).addClass('hidden');
                        }
                    })
                    .on('change', '.mappings-wrapper select', function () {
                        var $t = $(this);
                        var val = $t.find('option:selected').text();
                        $t.prop('title', val);
                    })
                    .on('change', '.parent-row-wrapper :text', function () {
                        if (typeof (Page_ClientValidate) == 'function') {
                            Page_ClientValidate('requiredParentField');
                        }
                    })
                    .on('click', '.tab-view .ui-tabs-anchor', function () {
                        $('.current-tab-index').val($('.mappings-wrapper .tab-view').tabs('option', 'active'));
                    })
                    .on('click', ':button', function () {
                        if (!$(this).is('.import-export-button,#import-modal :button')) {
                            $('.active-import-wrapper .spinner').removeClass('invisible');
                        }
                    })
                    .on('click', '.toggle-block', function (e) {
                        e.preventDefault();
                        var $t = $(this)
                        var $target = $(this.hash);
                        if ($t.hasClass('add-btn')) {
                            $t.removeClass('add-btn').addClass('minus-btn');
                            $target.slideDown(slideSpeed);
                            //$target.show();
                        }
                        else {
                            $t.addClass('add-btn').removeClass('minus-btn');
                            $target.slideUp(slideSpeed);
                            //$target.hide();
                        }
                    })
                    .on('click', '#btnMenuImportSettings', function (e) {
                        e.preventDefault();
                        importModal.dialog('open');
                    });

                $('[data-toggle-selector].active').click();

                if (!inIframe()) {
                    $('h3.title').addClass('heading');
                }

                if (typeof (Page_ClientValidate) == 'function') {
                    var originalValidation = Page_ClientValidate;
                    Page_ClientValidate = function (validationGroup) {

                        originalValidation(validationGroup);
                        if (!Page_IsValid) {
                            $('.mappings-wrapper .tab-view > ul .ui-tabs-anchor').each(function () {
                                var $t = $(this);
                                var $par = $t.parent().removeClass('invalid-tab').find('.error-mark').remove().end();
                                var id = this.hash;
                                if ($(id).find('.validation-text').length > 0) {
                                    $par.addClass('invalid-tab').append($('<span class="error-mark"/>').text('*'));
                                }
                            });
                        }
                    }
                }
                if (typeof (IsValidationGroupMatch) == 'function') {
                    IsValidationGroupMatch = function (ctrl, validationGroup) {
                        if (!validationGroup) return true;

                        var ctrlGroup = '';
                        if (typeof (ctrl.validationGroup) === 'string') ctrlGroup = ctrl.validationGroup;

                        var validatingGroups = validationGroup.split(' ');

                        for (var i = 0; i < validatingGroups.length; i++) {
                            if (validatingGroups[i] === ctrlGroup) return true;
                        }

                        return false;
                    };
                }
            });
        })(jQuery);
        // validation methods
        function validateParentField(src, args) {
            args.IsValid = false;
        }

        function validateRequireds() {
            return Page_ClientValidate('templateIDValidation requiredParentField');
        }
        // progress bar functionality
        function handleUpdate(data) {
            if (data != null && data.d != '' && data.d != 'success' && data.d != 'error') {
                var d = data.d;
                d = JSON.parse(d);
                var currentLbl = d.CurrentMapping + ' - ' + (d.CurrentRow > 0 ? d.CurrentRow : 0) + '/' + d.CurrentTotalRows;
                var totalLbl = (d.TotalComplete > 0 ? d.TotalComplete : 0) + '/' + d.TotalRows;
                var totalPct = (d.TotalPercent > 0 ? d.TotalPercent : 0);
                var currentPct = (d.CurrentPercent > 0 ? d.CurrentPercent : 0);
                if (d.Status != 'Idle') { globalVar.updateTry = 0; }
                switch (d.Status) {
                    case 'Running':
                        updateProgress('Current: ' + currentLbl, currentPct + '%', 'Total: ' + totalLbl, totalPct + '%');
                        window.setTimeout(pollUpdates, globalVar.pollInterval);
                        break;
                    case 'Completed':
                        updateProgress('Job completed', '100%', 'Total: ' + totalLbl, totalPct + '%');
                        $('.progress .complete-button').show();
                        showMessageList();
                        break;
                    case 'Idle':
                        if (globalVar.updateTry < 10) {
                            globalVar.updateTry++;  // try to poll updates up to 10 times if idle returned
                            window.setTimeout(pollUpdates, globalVar.pollInterval * 2); // after starting, give it some time if idle but onl
                        }
                        break;
                    default:
                        updateProgress(d.Status, currentPct + '%', 'Total: ' + totalLbl, totalPct + '%');
                        $('.progress .complete-button').show();
                        showMessageList();
                        break;
                }
            }
        }

        function handleError(message) {
            $('.progress .progress-report').text(message).show();
        }

        function showMessageList() {
            var pr = $('.progress .progress-report').empty();
            var tbl = $('<table/>');
            var thead = $('<thead/>');
            var thr = $('<tr/>');
            thr.append($('<th/>').text('Current Item')).append($('<th/>').text('Mapping')).append($('<th/>').text('Status Message'));
            tbl.append(thead.append(thr));
            var tbody = $('<tbody/>');
            pr.append(tbl.append(tbody)).show();
            //msgData.CurrentPage, msgData.TotalMessages, msgData.PageSize
            var loadMessages = function (i) {
                getImportMessages(i, function (data) {
                    var lastPage = parseInt((data.TotalMessages + data.PageSize - 1) / data.PageSize) - 1;
                    if (data.CurrentPage < lastPage) {
                        loadMessages(data.CurrentPage + 1);
                    }
                });
            };
            loadMessages(0);
        }

        function addMessagesToList(messages) {
            var listBody = $('.progress .progress-report tbody');
            if (listBody.length > 0) {
                var rows = $([]);
                for (var i = 0; i < messages.length; i++) {
                    var m = messages[i];
                    var tr = $('<tr/>');
                    var name = $('<td class="map-name"/>').text(m.MappingName);
                    var rowNum = $('<td class="row-num"/>').text(m.CurrentItem);
                    var message = $('<td class="status-message"/>').text(m.Message);
                    tr.append(rowNum).append(name).append(message);
                    rows = rows.add(tr);
                }
                listBody.append(rows);
            }
        }

        function setMessagePaging(currentPage, totalItems, perPage) {
            var totalPages = parseInt((totalItems + perPage - 1) / perPage);
            var pr = $('.progress .progress-report');
            pr.find('.paging').remove();	// clear existing paging ctrls

            if (totalPages > 1) {
                var pDiv = $('<div/>').addClass('paging');

                for (var i = 0; i < totalPages; i++) {
                    var link = makePageLink(i);
                    if (i == currentPage) { link.addClass('current').off('click'); }
                    pDiv.append(link);
                }
                if (currentPage > 0) { pDiv.prepend(makePageLink(currentPage - 1, 'Prev').addClass('prev')); }
                if (currentPage < totalPages - 1) { pDiv.append(makePageLink(currentPage + 1, 'Next').addClass('next')); }
                pr.find('table').before(pDiv).after(pDiv.clone(true));
            }
        }

        function makePageLink(page, text) {
            text = text || (page + 1).toString();
            return $('<a/>').on('click', function (e) {
                e.preventDefault();
                getImportMessages(page);
            }).text(text);
        }

        function startImport() {
            globalVar.updateTry = 0;
            updateProgress('', '0%', '', '0%');
            $('.progress').find('.complete-button, .progress-report').hide();
            pollUpdates();
        }

        function getImportMessages(page, callBack) {
            sendRequest('<%= PagePath %>/GetImportMessages', { page: page }, function (data) {
                if (data.d != '') {
                    var msgData = JSON.parse(data.d);
                    addMessagesToList(msgData.Messages);
                    //setMessagePaging(msgData.CurrentPage, msgData.TotalMessages, msgData.PageSize);
                    if (typeof (callBack) == 'function') {
                        callBack(msgData);
                    }
                }
            });
        }

        function pollUpdates() {
            sendRequest('<%= PagePath %>/GetImportProgress', null, function (data) {
                handleUpdate(data);
            });
        }

        function sendRequest(url, data, onComplete) {
            var reqData = (data ? (typeof (data) == 'string' ? data : JSON.stringify(data)) : {});
            $.ajax({
                url: url,
                type: 'POST',
                dataType: 'json',
                data: reqData,
                contentType: 'application/json; charset=utf-8',
                success: onComplete,
                error: function () { handleError(arguments[2]); }
            });
        }

        function updateProgress(curLabel, curPct, totalLabel, totalPct) {
            var progress = $('.progress');
            var label = progress.find('.progress-label');
            var pBar = progress.find('.progress-bar');
            label.find('.current-label').text(curLabel);
            label.find('.current-percent').text('(' + curPct + ')');
            label.find('.total-label').text(totalLabel);
            label.find('.total-percent').text('(' + totalPct + ')');

            pBar.filter('.current').width(curPct);
            pBar.filter('.total').width(totalPct);
        }

        function inIframe() {
            try {
                return window.self !== window.top;
            } catch (e) {
                return true;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="pageScriptManager" EnablePartialRendering="true" EnablePageMethods="true"></asp:ScriptManager>
        <h3 class="title">
            <script type="text/javascript">
                if (!inIframe()) { document.write('Content Import Utility'); }
		    </script>
			<span class="version">
                <asp:Literal runat="server" ID="litVersion" /></span>
        </h3>
        <div class="import-wrapper">
            <div class="information">
                <div class="label">
                    <span>Help Information</span>
                    <a href="#info-text" class="pull-right toggle-block font-button add-btn" title="Toggle Information section"></a>
                </div>
                <div id="info-text" class="hidden">
                    <p>
                        This utility is for importing content into Sitecore from an Excel file or group of XML files. Please use
								either a valid Excel (XLS or XLSX) file, or folders containing XML for the source. 
                    </p>
                    <p>A few notes about operation:</p>
                    <h4>For Excel Files<a href="#excel-text" class="pull-right toggle-block font-button minus-btn" title="Toggle section"></a></h4>
                    <div id="excel-text">
                        <ul>
                            <li>A single file can contain multiple Worksheets of data to import.</li>
                            <li>Each Worksheet may only specify a single Sitecore Template ID. Each item created from that worksheet will use the same Template ID.</li>
                            <li>Every row is considered a new Sitecore "Item" and columns represent data that can be imported.</li>
                            <li>Only column numbers mapped to a Field name will be imported. Others will be ignored.</li>
                            <li>Verify the Sitecore ID of the Item you are using for the Template and Parent Items.</li>
                            <li>Add the mappings in priority order. Parent items should be created before children items.</li>
                        </ul>
                    </div>
                    <h4>For XML<a href="#xml-text" class="pull-right toggle-block font-button minus-btn" title="Toggle section"></a></h4>
                    <div id="xml-text">
                        <ul>
                            <li>You can upload a ZIP file containing XML files or sub-folders of XML files.</li>
                            <li>A ZIP file may contain multiple directories containing XML files. Only top level directories are searched. No files in nested directories or subfolders will be found.</li>
                            <li>You can manually specify up to 10 folders containing XML files.</li>
                            <li>Any specified folders containing XML must be located on the server.</li>
                            <li>Each folder may relate to only a single Sitecore Template ID. Each item created from files in a folder will use the same Template ID.</li>
                            <li>Each XML file is considered a new Sitecore "Item".</li>
                            <li>Only fields mapped to a data source will be imported. Any other data or fields will be ignored.</li>
                            <li>Files within a directory should all share the same XML structure. No pre-verification process to confirm schema is performed.</li>
                            <li>Add the mappings in priority order. Parent items should be created before children items.</li>
                        </ul>
                    </div>
                </div>
            </div>
            <asp:UpdatePanel runat="server" ID="updImportWrapper" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row new-import-wrapper" runat="server" id="newJobWrapper">
                        <div class="input-row">
                            <h4>Start a new Import Job<span class="pull-right invisible spinner"></span></h4>
                            <div class="input-row job-type-selections">
                                <div class="input-column job-type-headers">
                                    <div class="type-selection-header active" data-toggle-group="job-types" data-toggle-selector="xls-wrapper">Use a Spreadsheet</div>
                                    <div class="type-selection-header" data-toggle-group="job-types" data-toggle-selector="xml-wrapper">Use XML files</div>
                                </div>
                                <div class="input-column xls-type-wrapper" data-toggle-group="job-types" data-toggle-target="xls-wrapper">
                                    <div class="input-row xls-type-heading">Upload a file</div>
                                    <div class="input-row">
                                        <span class="input-label">Source File (xls or xlsx)</span>
                                        <span class="input-field">
                                            <input type="file" runat="server" id="inFileSource" class="source-file" accept=".xls, .xlsx" />
                                            <asp:RequiredFieldValidator runat="server" ID="rfvFileSource" ControlToValidate="inFileSource" Display="Dynamic" CssClass="validation-text"
                                                ErrorMessage="Please select a valid Excel file" ValidationGroup="newXlsImportValidation"></asp:RequiredFieldValidator>
                                        </span>
                                        <div class="input-row right">
                                            <asp:Button runat="server" ID="btnNewXlsImport" OnClick="btnNewXlsImport_ServerClick" ValidationGroup="newXlsImportValidation" Text="Create New Import Job" UseSubmitBehavior="false" />
                                        </div>
                                    </div>
                                </div>
                                <div class="input-column xml-type-wrapper" data-toggle-group="job-types" data-toggle-target="xml-wrapper">
                                    <div class="input-row xml-type-selector">
                                        <div class="input-row heading-row xml-type-choices">
                                            <span class="xml-choice active" data-toggle-group="xml-types" data-toggle-selector="zip">Upload a ZIP file</span>
                                            <span class="xml-choice" data-toggle-group="xml-types" data-toggle-selector="folders">Specify folders</span>
                                        </div>
                                        <div class="xml-input-wrapper" data-toggle-group="xml-types" data-toggle-target="zip">
                                            <span class="input-label">ZIP File</span>
                                            <span class="input-field">
                                                <input type="file" runat="server" id="inFileZip" class="source-file" accept=".zip" />
                                                <asp:RequiredFieldValidator runat="server" ID="rfvFileZip" ControlToValidate="inFileZip" Display="Dynamic" CssClass="validation-text"
                                                    ErrorMessage="Please provide a valid ZIP file" ValidationGroup="newXmlZipImportValidation"></asp:RequiredFieldValidator>
                                            </span>
                                            <div class="input-row right">
                                                <asp:Button runat="server" ID="btnNewXmlZipImport" OnClick="btnNewXmlZipImport_Click" ValidationGroup="newXmlZipImportValidation" Text="Create New Import Job" UseSubmitBehavior="false" />
                                            </div>
                                        </div>
                                        <div class="xml-input-wrapper edit-row-parent" data-toggle-group="xml-types" data-toggle-target="folders">
                                            <script type="text/template" id="xml-folder-input">
                                                <div class="input-row">
                                                    <input type="text" class="xml-folder-path" name="<%= XmlPathFieldName %>" />
                                                    <span class="xml-remove-field font-button minus-btn" title="Remove folder"></span>
                                                </div>
                                            </script>
                                            <div class="input-row edit-row">
                                                <input type="text" class="xml-folder-path" name="<%= XmlPathFieldName %>" />
                                                <span class="xml-remove-field font-button minus-btn" title="Remove folder"></span>
                                            </div>
                                            <div class="input-row">
                                                <div class="add-field-button" title="Add folder">
                                                    <span class="xml-add-field font-button add-btn"></span>
                                                    Add folder
                                                </div>
                                            </div>
                                            <div class="input-row right">
                                                <asp:Button runat="server" ID="btnNewXmlFoldersImport" OnClick="btnNewXmlFoldersImport_Click" ValidationGroup="newXmlFolderImportValidation" Text="Create New Import Job" UseSubmitBehavior="false" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                     <div class="row errors-row" runat="server" id="errorsWrapper" visible="false">
                        <div class="information">
                            <div class="label">Errors encountered:</div>
                            <div class="info-text">
                                <ul>
                                <asp:Repeater runat="server" ID="rptErrorList" ItemType="System.string">
                                    <ItemTemplate>
                                        <li><%# Item %></li>
                                    </ItemTemplate>
                                </asp:Repeater>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="row active-import-wrapper" runat="server" id="activeJobWrapper">
                        <div class="input-row">
                            <div class="input-column" runat="server" id="xlsActiveJobWrapper">
                                <span class="import-file">Importing from:
								<asp:Literal runat="server" ID="litImportXlsFile"></asp:Literal>
                                </span>
                                <span class="worksheet-count">
                                    <asp:Literal runat="server" ID="litWorksheetCount"></asp:Literal>
                                    Worksheets found
                                </span>
                            </div>
                            <div class="input-column" runat="server" id="xmlActiveJobWrapper">
                                <span class="import-file">Importing from:
								<asp:Literal runat="server" ID="litImportXmlFolder"></asp:Literal>
                                </span>
                                <span class="worksheet-count">
                                    <asp:Literal runat="server" ID="litDirectoriesCount"></asp:Literal>
                                    Directories available
                                </span>
                            </div>
                            <div class="input-column pull-right">
                                <div class="button-row right">
                                    <asp:Button runat="server" ID="btnClearImportJob" CssClass="warning" OnClick="clearImportJob_ServerClick" Text="Cancel Current Job"
                                        UseSubmitBehavior="false" />
                                </div>
                                <div class="pull-right invisible spinner"></div>
                            </div>
                        </div>
                    </div>
                    <div runat="server" id="configurationWrapper">
                        <div class="row add-mapping-wrapper" runat="server" id="addMappingWrapper">
                            <div class="input-row">
                                <div class="input-column" runat="server" id="addMappingColumn">
                                    <span class="input-label">
                                        <asp:Literal runat="server" ID="litAddMappingLabel"></asp:Literal></span>
                                    <span class="input-field">
                                        <asp:DropDownList runat="server" ID="ddlSourceOptions"></asp:DropDownList>
                                    </span>
                                </div>
                                <div class="input-column">
                                    <div class="input-field button-group">
                                        <span class="buttons">
                                            <%--<button runat="server" id="btnAddMappingOld" onserverclick="btnAddMapping_ServerClick">Add Mapping</button>--%>
                                            <asp:Button runat="server" ID="btnXlsAddMapping" CssClass="button" OnClick="btnXlsAddMapping_ServerClick" Text="Add Mapping" UseSubmitBehavior="false" />
                                            <asp:Button runat="server" ID="btnXmlAddMapping" CssClass="button" OnClick="btnXmlAddMapping_ServerClick" Text="Add Mapping" UseSubmitBehavior="false" />
                                            <asp:Button runat="server" ID="btnExportSettings" CssClass="button import-export-button" OnClick="btnExportSettings_ServerClick" Text="Export mappings" UseSubmitBehavior="false" />
                                            <button class="menu">Additional Options</button>
                                        </span>
                                        <ul class="group-menu">
                                            <li>
                                                <button runat="server" id="btnXlsMenuAddMapping" onserverclick="btnXlsAddMapping_ServerClick">Add Mapping</button></li>
                                            <li>
                                                <button runat="server" id="btnXmlMenuAddMapping" onserverclick="btnXmlAddMapping_ServerClick">Add Mapping</button></li>
                                            <li>
                                                <button runat="server" id="btnMenuExportSettings" onserverclick="btnExportSettings_ServerClick" class="import-export-button">Export Mappings</button></li>
                                            <li>
                                                <button runat="server" id="btnMenuImportSettings" class="import-export-button">Import Mappings</button></li>
                                            <%--<li><a href="#" runat="server" id="linkAddMapping" onserverclick="btnAddMapping_ServerClick">Add Mapping</a></li>
											<li><a href="#" runat="server" id="linkExportSettings" onserverclick="btnExportSettings_ServerClick">Export Mappings</a></li>
											<li><a href="#" id="linkImportSettings">Import Mappings</a></li>--%>
                                        </ul>
                                    </div>
                                </div>
                                <%--<div class="input-column pull-right import-export">
									<button runat="server" id="btnExportSettings" class="caution" onserverclick="btnExportSettings_ServerClick">Export Mappings</button>
									<button id="btnOpenImportSettings" class="caution">Import Mappings</button>
								</div>--%>
                            </div>
                            <div class="input-row" runat="server" id="xmlIncludeAttributeRow">
                                <div class="input-column">
                                    <span class="input-label">&nbsp;</span>
                                </div>
                                <div class="input-column check-row">
                                    <span class="input-field">
                                        <input type="checkbox" runat="server" id="chkXmlIncludeAttributes" checked="checked" />
                                    </span>
                                    <span class="input-label">
                                        <asp:Label runat="server" ID="lblXmlIncludeAttributes" AssociatedControlID="chkXmlIncludeAttributes">Include XML attributes as data sources</asp:Label>
                                    </span>
                                </div>
                            </div>
                        </div>
                        <div class="row mappings-wrapper" runat="server" id="mappingsWrapper" visible="false">
                            <input type="hidden" runat="server" id="hdnTabIndex" value="0" class="current-tab-index" />
                            <div class="tab-view">
                                <asp:ListView runat="server" ID="lvMappingTabs" ItemPlaceholderID="tabsPlaceholder" ItemType="Specialized.Content.Import.Mapping.MappingSourceBase">
                                    <LayoutTemplate>
                                        <ul>
                                            <asp:PlaceHolder runat="server" ID="tabsPlaceholder"></asp:PlaceHolder>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <a href="#tab-<%# Container.DataItemIndex %>"><%# getSourceName(Item.Name)%></a>
                                            <asp:LinkButton runat="server" ID="lbRemoveMapping" OnClick="removeMapping_Click" CssClass="remove-link">X</asp:LinkButton>
                                        </li>
                                    </ItemTemplate>
                                </asp:ListView>
                                <asp:ListView runat="server" ID="lvXlsMappingFields" ItemPlaceholderID="mainFieldsPlaceholder"
                                    ItemType="Specialized.Content.Import.Mapping.Xls.XlsMapping" OnItemDataBound="lvXlsMappingFields_ItemDataBound">
                                    <LayoutTemplate>
                                        <asp:PlaceHolder runat="server" ID="mainFieldsPlaceholder"></asp:PlaceHolder>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <div id="tab-<%# Container.DataItemIndex %>" class="tab-content xls-mappings">
                                            <div class="parent-row-wrapper">
                                                <div class="input-row">
                                                    <div class="parent-row-sourcce input-column">
                                                        <span class="input-label">Source of Parent ID</span>
                                                        <span class="input-field">
                                                            <asp:DropDownList runat="server" ID="ddlParentSource">
                                                                <asp:ListItem Value="global">Global for Mapping</asp:ListItem>
                                                                <asp:ListItem Value="item">Per Item</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </span>
                                                    </div>
                                                    <div class="input-column parent-field-column">
                                                        <div class="global-parent-row" runat="server" id="globalParentRow">
                                                            <span class="input-label">Parent ID</span>
                                                            <span class="input-field">
                                                                <input type="text" runat="server" id="globalParentID" />
                                                                <%--<asp:RegularExpressionValidator runat="server" ID="rfvParentID" ControlToValidate="globalParentID" Display="Dynamic"
																	CssClass="validation-text" ValidationGroup="globalParentValidation-<%# Container.DataItemIndex %>"
																	ErrorMessage="Please enter the ID as a GUID" ValidationExpression="{?\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b}?">
																</asp:RegularExpressionValidator>--%>
                                                            </span>
                                                        </div>
                                                        <div class="item-parent-row" runat="server" id="itemParentRow">
                                                            <span class="input-label">Parent Column</span>
                                                            <span class="input-field">
                                                                <input type="text" runat="server" id="parentColNumber" class="column-number-input" />
                                                                <%--<asp:CompareValidator runat="server" ID="cvParentColNumber" ControlToValidate="parentColNumber" Display="Dynamic"
																	CssClass="validation-text" ValidationGroup="parentColValidation-<%# Container.DataItemIndex %>" Type="Integer" Operator="DataTypeCheck"
																	ErrorMessage="Please enter a valid number."></asp:CompareValidator>--%>
                                                            </span>
                                                        </div>
                                                        <%--<asp:CustomValidator runat="server" ID="cvParentField" ClientValidationFunction="validateParentField" Display="None" ValidationGroup="requiredParentField"></asp:CustomValidator>--%>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="input-row">
                                                <span class="input-label">Name Column</span>
                                                <input type="text" runat="server" id="nameColNumber" class="column-number-input" />
                                            </div>
                                            <div class="check-row">
                                                <span class="input-field">
                                                    <input type="checkbox" runat="server" id="chkCleanNames" checked="checked" />
                                                </span>
                                                <span class="input-label">
                                                    <asp:Label runat="server" ID="lblCleanNames" AssociatedControlID="chkCleanNames">Clean Item names according to Sitecore settings</asp:Label>
                                                </span>
                                            </div>
                                            <div class="check-row">
                                                <span class="input-field">
                                                    <input type="checkbox" runat="server" id="chkFirstRowLabels" />
                                                </span>
                                                <span class="input-label">
                                                    <asp:Label runat="server" ID="Label1" AssociatedControlID="chkFirstRowLabels">First row is labels</asp:Label>
                                                </span>
                                            </div>
                                            <div class="input-row template-row">
                                                <div class="input-column">
                                                    <span class="input-label">Template ID</span>
                                                    <span class="input-field">
                                                        <input type="text" runat="server" id="templateID" />
                                                        <asp:LinkButton runat="server" ID="lbClearTemplate" OnClick="lbClearTemplate_Click" Visible="false">Clear</asp:LinkButton>
                                                        <span class="sub-text">
                                                            <span class="input-field">
                                                                <asp:Literal runat="server" ID="litTemplateName" />
                                                            </span>
                                                        </span>
                                                        <%--<asp:RequiredFieldValidator runat="server" ID="rfvTemplateID" ControlToValidate="templateID" Display="Dynamic" CssClass="validation-text"
															ValidationGroup="templateIDValidation" ErrorMessage="A Template ID is required for mapping fields."></asp:RequiredFieldValidator>--%>
                                                        <%--<asp:RegularExpressionValidator runat="server" ID="revTemplateID" ControlToValidate="templateID" Display="Dynamic" CssClass="validation-text"
															ValidationGroup="templateIDValidation" ErrorMessage="Please enter the ID as a GUID"
															ValidationExpression="{?\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b}?"></asp:RegularExpressionValidator>--%>

                                                    </span>
                                                </div>
                                                <div class="input-column">
                                                    <asp:Button runat="server" ID="btnAddFields" OnClick="btnAddFields_ServerClick" Text="Add Field Mappings"
                                                        ValidationGroup="templateIDValidation" UseSubmitBehavior="false" />
                                                </div>
                                            </div>
                                            <div class="field-wrapper" runat="server" id="addFieldsWrapper">
                                                <asp:ListView runat="server" ID="lvFields" ItemPlaceholderID="fieldPlaceholder" GroupItemCount="3" GroupPlaceholderID="groupPlaceholder">
                                                    <LayoutTemplate>
                                                        <div class="information">
                                                            <p>Please enter the number of the Column containing the data for the corresponding Field Name.</p>
                                                        </div>
                                                        <asp:PlaceHolder runat="server" ID="groupPlaceholder"></asp:PlaceHolder>
                                                    </LayoutTemplate>
                                                    <GroupTemplate>
                                                        <div class="input-row">
                                                            <asp:PlaceHolder runat="server" ID="fieldPlaceholder"></asp:PlaceHolder>
                                                        </div>
                                                    </GroupTemplate>
                                                    <ItemTemplate>
                                                        <div class="input-column">
                                                            <%--This should house the name of the field from the template --%>
                                                            <input type="text" runat="server" id="fieldName" class="field-name-input" value='<%# Eval("Name") %>' tabindex="-1" />
                                                            <%--The column number should be blank by default --%>
                                                            <input type="text" runat="server" id="columnNumber" class="column-number-input" value='<%# ((int)Eval("Column") > -1?Eval("Column"):"") %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:ListView>
                                <asp:ListView runat="server" ID="lvXmlMappingFields" ItemPlaceholderID="mainFieldsPlaceholder"
                                    ItemType="Specialized.Content.Import.Mapping.Xml.XmlMapping" OnItemDataBound="lvXmlMappingFields_ItemDataBound">
                                    <LayoutTemplate>
                                        <asp:PlaceHolder runat="server" ID="mainFieldsPlaceholder"></asp:PlaceHolder>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <div id="tab-<%# Container.DataItemIndex %>" class="tab-content xml-mappings">
                                            <div class="parent-row-wrapper">
                                                <div class="input-row">
                                                    <div class="parent-row-sourcce input-column">
                                                        <span class="input-label">Source of Parent ID</span>
                                                        <span class="input-field">
                                                            <asp:DropDownList runat="server" ID="ddlParentSource">
                                                                <asp:ListItem Value="global">Global for Mapping</asp:ListItem>
                                                                <asp:ListItem Value="item">Per Item</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </span>
                                                    </div>
                                                    <div class="input-column parent-field-column">
                                                        <div class="global-parent-row" runat="server" id="globalParentRow">
                                                            <span class="input-label">Parent ID</span>
                                                            <span class="input-field">
                                                                <input type="text" runat="server" id="globalParentID" />
                                                                <%--<asp:RegularExpressionValidator runat="server" ID="rfvParentID" ControlToValidate="globalParentID" Display="Dynamic"
																	CssClass="validation-text" ValidationGroup="globalParentValidation-<%# Container.DataItemIndex %>"
																	ErrorMessage="Please enter the ID as a GUID" ValidationExpression="{?\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b}?">
																</asp:RegularExpressionValidator>--%>
                                                            </span>
                                                        </div>
                                                        <div class="item-parent-row" runat="server" id="itemParentRow">
                                                            <span class="input-label">Parent Field</span>
                                                            <span class="input-field">
                                                                <asp:DropDownList runat="server" ID="ddlParentField" DataSource="<%# Item.XPaths %>" ToolTip='<%# Item.ParentNodePath??"" %>'></asp:DropDownList>
                                                                <%--<input type="text" runat="server" id="parentFieldPath" class="column-number-input" />--%>
                                                                <%--<asp:CompareValidator runat="server" ID="cvParentColNumber" ControlToValidate="parentColNumber" Display="Dynamic"
																	CssClass="validation-text" ValidationGroup="parentColValidation-<%# Container.DataItemIndex %>" Type="Integer" Operator="DataTypeCheck"
																	ErrorMessage="Please enter a valid number."></asp:CompareValidator>--%>
                                                            </span>
                                                        </div>
                                                        <%--<asp:CustomValidator runat="server" ID="cvParentField" ClientValidationFunction="validateParentField" Display="None" ValidationGroup="requiredParentField"></asp:CustomValidator>--%>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="input-row">
                                                <span class="input-label">Name Field</span>
                                                <asp:DropDownList runat="server" ID="ddlNameField" DataSource="<%# Item.XPaths %>" ToolTip='<%# Item.NameNodePath??"" %>'></asp:DropDownList>
                                                <%--<input type="text" runat="server" id="nameFieldPath" class="column-number-input" />--%>
                                            </div>
                                            <div class="check-row">
                                                <span class="input-field">
                                                    <input type="checkbox" runat="server" id="chkCleanNames" checked="checked" />
                                                </span>
                                                <span class="input-label">
                                                    <asp:Label runat="server" ID="lblCleanNames" AssociatedControlID="chkCleanNames">Clean Item names according to Sitecore settings</asp:Label>
                                                </span>
                                            </div>
                                            <div class="input-row template-row">
                                                <div class="input-column">
                                                    <span class="input-label">Template ID</span>
                                                    <span class="input-field">
                                                        <input type="text" runat="server" id="templateID" />
                                                        <asp:LinkButton runat="server" ID="lbClearTemplate" OnClick="lbClearTemplate_Click" Visible="false">Clear</asp:LinkButton>
                                                        <span class="sub-text">
                                                            <span class="input-field">
                                                                <asp:Literal runat="server" ID="litTemplateName" />
                                                            </span>
                                                        </span>
                                                        <%--<asp:RequiredFieldValidator runat="server" ID="rfvTemplateID" ControlToValidate="templateID" Display="Dynamic" CssClass="validation-text"
															ValidationGroup="templateIDValidation" ErrorMessage="A Template ID is required for mapping fields."></asp:RequiredFieldValidator>--%>
                                                        <%--<asp:RegularExpressionValidator runat="server" ID="revTemplateID" ControlToValidate="templateID" Display="Dynamic" CssClass="validation-text"
															ValidationGroup="templateIDValidation" ErrorMessage="Please enter the ID as a GUID"
															ValidationExpression="{?\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b}?"></asp:RegularExpressionValidator>--%>

                                                    </span>
                                                </div>
                                                <div class="input-column">
                                                    <asp:Button runat="server" ID="btnAddFields" OnClick="btnAddFields_ServerClick" Text="Add Field Mappings"
                                                        ValidationGroup="templateIDValidation" UseSubmitBehavior="false" />
                                                </div>
                                            </div>
                                            <div class="field-wrapper" runat="server" id="addFieldsWrapper">
                                                <asp:ListView runat="server" ID="lvFields" ItemPlaceholderID="fieldPlaceholder" GroupItemCount="3" GroupPlaceholderID="groupPlaceholder">
                                                    <LayoutTemplate>
                                                        <div class="information">
                                                            <p>Please select the Xml Node Path containing the data for the corresponding Field Name.</p>
                                                        </div>
                                                        <asp:PlaceHolder runat="server" ID="groupPlaceholder"></asp:PlaceHolder>
                                                    </LayoutTemplate>
                                                    <GroupTemplate>
                                                        <div class="input-row">
                                                            <asp:PlaceHolder runat="server" ID="fieldPlaceholder"></asp:PlaceHolder>
                                                        </div>
                                                    </GroupTemplate>
                                                    <ItemTemplate>
                                                        <div class="input-column">
                                                            <%--This should house the name of the field from the template --%>
                                                            <input type="text" runat="server" id="fieldName" class="field-name-input" value='<%# Eval("Name") %>' tabindex="-1" />
                                                            <%--The column number should be blank by default --%>
                                                            <%--<input type="text" runat="server" id="columnNumber" class="column-number-input" value='<%# (!string.IsNullOrWhiteSpace((string)Eval("XPath"))?Eval("XPath"):"") %>' />--%>
                                                            <asp:DropDownList runat="server" ID="nodePath" DataSource='<%# Eval("XPathList") %>' ToolTip='<%# (!string.IsNullOrWhiteSpace((string)Eval("XPath"))?Eval("XPath"):"") %>' OnDataBound="nodePath_DataBound"></asp:DropDownList>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:ListView>
                            </div>
                            <div class="button-row" runat="server" id="saveAndRunRow" visible="false">
                                <%--do we really need a save button when every post action saves state? if you want to save the name or parent without adding mappings, yes.
								additionally, though, every postback from a control or form update calls the save function so it might be pretty limited in missed saves--%>
                                <button runat="server" id="btnSaveMappings" class="pull-left caution" onserverclick="btnSaveMappings_ServerClick">Save</button>
                                <%--<button runat="server" id="btnXlsDoImportOld" class="pull-right" validationgroup="import-validation" onserverclick="btnXlsDoImport_ServerClick">Start Import</button>--%>
                                <asp:Button runat="server" ID="btnXlsDoImport" CssClass="pull-right go" OnClick="btnXlsDoImport_ServerClick" UseSubmitBehavior="false" Text="Start Import" />
                            </div>
                        </div>
                    </div>
                    <div class="progress" runat="server" id="importProgressWrapper" visible="true">
                        <div class="progress-label">
                            <span class="progress-current">
                                <span class="current-label"></span>
                                <span class="current-percent"></span>
                            </span>
                            <span class="progress-total">
                                <span class="total-label"></span>
                                <span class="total-percent"></span>
                            </span>
                        </div>
                        <div class="progress-bar-wrapper">
                            <div class="progress-bar current"></div>
                            <div class="progress-bar total"></div>
                        </div>
                        <div class="progress-report error"></div>
                        <div class="button-row right">
                            <asp:Button runat="server" ID="btnCompleteJob" Text="Done" CssClass="complete-button" OnClick="btnCompleteJob_Click" UseSubmitBehavior="false" />
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnNewXmlZipImport" />
                    <asp:PostBackTrigger ControlID="btnNewXmlFoldersImport" />
                    <asp:PostBackTrigger ControlID="btnNewXlsImport" />
                    <asp:PostBackTrigger ControlID="btnClearImportJob" />
                    <asp:PostBackTrigger ControlID="btnMenuExportSettings" />
                    <asp:PostBackTrigger ControlID="btnExportSettings" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="import-modal" title="Import Settings">
            <p>
                Import settings from a file
            </p>
            <div class="input-row">
                <input type="file" runat="server" id="inFileSettings" accept=".ims" />
            </div>
            <div class="button-row">
                <button class="cancel caution">Cancel</button>
                <asp:Button runat="server" ID="btnImportMappings" class="pull-right go" OnClick="btnImportMappings_ServerClick" UseSubmitBehavior="false" Text="Import" />
                <div class="pull-right invisible spinner"></div>
            </div>
        </div>
    </form>
</body>
</html>
