define(['knockout'], function (ko) {

    ko.components.loaders.unshift({
        getConfig: function (name, callback) {
            callback({
                viewModel: { require: name },
                template: { require: 'text!html/' + name + '.html' }
            });
        },
        loadTemplate: function (name, templateConfig, callback) {
            if (templateConfig.require) {
                require([templateConfig.require], function(viewModel) {
                    ko.components.defaultLoader.loadViewModel(name, viewModel, callback);
                });
            } else {
                callback(null);
            }
        },
        loadViewModel: function(name, viewModelConfig, callback) {
            if (viewModelConfig.require) {
                require([viewModelConfig.require], function (template) {
                    ko.components.defaultLoader.loadTemplate(name, template, callback);
                });
            } else {
                callback(null);
            }
        }
    });

})