"use strict";

var app = (function () {

    var logTemplate;
    var detailsTemplate;
    var useLambdaFilter = false;
    var sortOrder = "ascending";
    var logEntries;

    function init() {
        Handlebars.registerHelper('getLogField', function (entry, field) {
            return entry[field];
        });

        Handlebars.registerHelper('levelStyling', function (entry, levelField) {
            var level = entry[levelField];

            if (level == "Warning")
                return "alert alert-warning";
            else if (level == "Error" || level == "Fatal")
                return "alert alert-danger";
            else
                return "";
        });

        logTemplate = Handlebars.compile($("#logsTemplate").html());
        detailsTemplate = Handlebars.compile($("#detailsTemplate").html());

        $("#searchButton").click(getLog);
        $("#advancedSearching").click(setAdvancedSearching);
        setAdvancedSearching();
    }

    function setAdvancedSearching() {
        useLambdaFilter = $("#advancedSearching").prop("checked");        
        if (useLambdaFilter)
            $("#searchBox").attr("placeholder", "(LogEntry x) => //your boolean filter expression here");
        else
            $("#searchBox").attr("placeholder", "Leave blank to view all log entries");
    }

    function changeSortOrder() {
        var sortIndicator = $("#sortIndicator");

        if (sortOrder == "ascending") {
            sortOrder = "descending";
            sortIndicator.removeClass("ascendingSortIndicator")
            sortIndicator.addClass("descendingSortIndicator")
        } else {
            sortOrder = "ascending";
            sortIndicator.removeClass("descendingSortIndicator")
            sortIndicator.addClass("ascendingSortIndicator")
        }

        getLog();
    }

    function getLogEntryDetailFieldNames(logEntry) {
        var fieldNames = ["Message"];
        var allFields = Object.getOwnPropertyNames(logEntry);
        allFields.forEach(function (name) {
            if (name != "Message" && name != "Timestamp" && name != "Level")
                fieldNames.push(name);
        });
        return fieldNames;
    }

    function expandCollapseEntry(data) {
        var cell = $(data.target)
        var isExpanded = cell.data("isExpanded");
        if (isExpanded) {
            //switch to collapsed
            cell.data("isExpanded", false);
            cell.text("»");
            var detailsRow = cell.parent().next();
            detailsRow.remove();
        } else {
            //switch to expanded
            cell.data("isExpanded", true);
            cell.text("«");
            var tr = cell.parent();
            var logIndex = tr.attr("data-entry-index");
            var logEntry = logEntries[logIndex];
            var viewModel = { logEntry: logEntry, fieldNames: getLogEntryDetailFieldNames(logEntry) };
            var detailshtml = detailsTemplate(viewModel);
            $(detailshtml).insertAfter(tr);
        }
    }

    function hookupTemplateContentEventHandlers() {        
        $("#timestampHeader").click(changeSortOrder);
        $(".expandCollapseCell").click(expandCollapseEntry);
    }

    function getLogEntries() {
        return $.get("/log?sortOrder=" + sortOrder);
    }

    function getMetadata() {
        return $.get("/log/metadata");
    }

    function CreateViewModel(logEntries, metadata)
    {
        var sortOrderClass = sortOrder == "ascending" ? "ascendingSortIndicator" : "descendingSortIndicator";

        return { 
            fieldNames: metadata.FieldNames, 
            entries: logEntries,
            countDescription: "Showing " + logEntries.length + " of " + metadata.TotalLogEntries + " log entries",
            expandChar: "»",
            sortOrderClass: sortOrderClass
        };
    }

    function getLog() {
        $.when(getLogEntries(), getMetadata())
            .done(function (logResponse, metadataResponse) {
                if (logResponse[1] != "success" || metadataResponse[1] != "success")
                    return;

                logEntries = logResponse[0];
                var viewModel = CreateViewModel(logEntries, metadataResponse[0]);
                var html = logTemplate(viewModel);
                var div = $("#logs");
                div.empty();
                div.append(html);
                hookupTemplateContentEventHandlers();
            });
    }

    init();

    return { getLog: getLog };
})();
