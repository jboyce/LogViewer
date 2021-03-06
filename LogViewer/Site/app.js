﻿"use strict";

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

            if (level == "Warn")
                return "alert alert-warning";
            else if (level == "Error" || level == "Fatal")
                return "alert alert-danger";
            else
                return "";
        });

        Handlebars.registerHelper('getFormattedTimestamp', function (entry) {
            var timestamp = new Date(entry.Timestamp);
            return timestamp.toISOString();
        });

        logTemplate = Handlebars.compile($("#logsTemplate").html());
        detailsTemplate = Handlebars.compile($("#detailsTemplate").html());

        $("#searchButton").click(getLog);
        $("#clearButton").click(clearLog);
        $("#advancedSearching").click(setAdvancedSearching);
        $("#searchBox").keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                getLog();
                return false;
            }
        });
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
        var searchText = $("#searchBox").val();
        var url = "/log?sortOrder=" + sortOrder 
            + "&useLambdaSearch=" + useLambdaFilter 
            + "&searchText=" + encodeURIComponent(searchText);
        return $.get(url);
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
                var logTableDiv = $("#logs");
                logTableDiv.empty();
                var errorMessage = $("#errorMessage");
                errorMessage.text("");
                errorMessage.toggleClass("hidden", true);

                if (logResponse[1] != "success" || metadataResponse[1] != "success") {

                }
                else {
                    logEntries = logResponse[0];
                    var viewModel = CreateViewModel(logEntries, metadataResponse[0]);
                    var html = logTemplate(viewModel);
                    logTableDiv.append(html);
                    hookupTemplateContentEventHandlers();
                }
            })
            .fail(function (error) {
                var logTableDiv = $("#logs");
                logTableDiv.empty();

                var errorMessage = $("#errorMessage");
                errorMessage.text(error.responseText);
                errorMessage.toggleClass("hidden", false);
            });
    }

    function clearLog() {
        $.post("/log/clear", function () {
            getLog();
        });
    }

    $(document).ready(function () {
        init();
    });
    
    return { getLog: getLog };
})();
