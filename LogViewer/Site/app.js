"use strict";

var app = (function () {

    var template;
    var useLambdaFilter = false;
    var sortOrder = "ascending";

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

        template = Handlebars.compile($("#logsTemplate").html());

        $("#searchButton").click(getLog);
        $("#advancedSearching").click(setAdvancedSearching);
        setAdvancedSearching();
    }

    function setAdvancedSearching() {
        useLambdaFilter = $("#advancedSearching").prop("checked");        
        if (useLambdaFilter)
            $("#searchBox").attr("placeholder", "(LogEntry x) => //your boolean filter expression here");
        else
            $("#searchBox").attr("placeholder", "Leave blank to view all logs");
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
    }

    function expandCollapseEntry(data) {
        var cell = $(data.target)
        var isExpanded = cell.data("isExpanded");
        if (isExpanded) {
            //switch to collapsed
            cell.data("isExpanded", false);
            cell.text("»");
        } else {
            //switch to expanded
            cell.data("isExpanded", true);
            cell.text("«");
        }
    }

    function hookupTemplateContentEventHandlers() {        
        $("#timestampHeader").click(changeSortOrder);
        $(".expandCollapseCell").click(expandCollapseEntry);
    }

    function getLogEntries() {
        return $.get("/log");
    }

    function getMetadata() {
        return $.get("/log/metadata");
    }

    function CreateViewModel(logEntries, metadata)
    {
        return { 
            fieldNames: metadata.FieldNames, 
            entries: logEntries,
            countDescription: "Showing " + logEntries.length + " of " + metadata.TotalLogEntries + " log entries",
            expandChar: "»"
        };
    }

    function getLog() {
        $.when(getLogEntries(), getMetadata())
            .done(function (logResponse, metadataResponse) {
                if (logResponse[1] != "success" || metadataResponse[1] != "success")
                    return;

                var viewModel = CreateViewModel(logResponse[0], metadataResponse[0]);
                var html = template(viewModel);
                var div = $("#logs");
                div.empty();
                div.append(html);
                hookupTemplateContentEventHandlers();
            });
    }

    init();

    return { getLog: getLog };
})();
