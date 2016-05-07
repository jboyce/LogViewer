var app = (function () {

    var template;

    function init() {
        Handlebars.registerHelper('getLogField', function (entry, field) {
            return entry[field];
        });

        template = Handlebars.compile($("#logsTemplate").html());
    }

    function getLogEntries() {
        return $.get("/log");
    }

    function getFieldNames() {
        return $.get("/log/fieldNames");
    }
    function getLog() {
        $.when(getLogEntries(), getFieldNames())
            .done(function (logResponse, fieldNamesResponse) {
                if (logResponse[1] != "success" || fieldNamesResponse[1] != "success")
                    return;

                var context = { fieldNames: fieldNamesResponse[0], entries: logResponse[0] };
                var html = template(context);
                var div = $("#logs");
                div.append(html);
            });
    }

    init();

    return { getLog: getLog };
})();
