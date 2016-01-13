(function (ko, $) {

    function resolveComponentName(name) {
        if (!ko.components.isRegistered(name)) {
            var indexName = name.replace(/\/$/, '') + '/index';
            if (ko.components.isRegistered(indexName)) {
                return indexName;
            }
        }
        return name;
    }

    ko.components.loaders.unshift({
        getConfig: function (name, callback) {
            ko.components.defaultLoader.getConfig(resolveComponentName(name), callback);
        },
        loadTemplate: function (name, templateConfig, callback) {
            if (templateConfig.html) {
                var url = templateConfig.html;
                if (typeof url !== 'string') {
                    url = app.basePath + 'html/' + resolveComponentName(name) + '.html';
                }
                $.get(url, function (markupString) {
                    ko.components.defaultLoader.loadTemplate(name, markupString, callback);
                });
            } else {
                callback(null);
            }
        }
    });

})(window.ko, window.jQuery);