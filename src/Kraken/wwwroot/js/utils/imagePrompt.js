define(['jquery'], function ($) {

    function parseDataUrl(url) {
        var dataUrlRegex = /^data:(.+\/.+);base64,(.*)$/;
        var matches = url.match(dataUrlRegex);
        return {
            content: matches[2],
            contentType: matches[1]
        };
    }

    return function () {
        var deferred = $.Deferred();

        var $body = $(document.body);
        var $input = $('<input type="file" accept="image/*" style="display: none" />');
        $body.append($input);

        $input.on('change', function (e) {
            try {
                var file = e.target.files[0];
                if (!file) {
                    deferred.reject('No file');
                    return;
                }
                var reader = new FileReader();
                reader.onload = function (e) {
                    var image;
                    try {
                        image = parseDataUrl(e.target.result);
                    } catch (error) {
                        deferred.reject(error);
                        return;
                    }
                    deferred.resolve(image);
                }
                reader.readAsDataURL(file);
            } catch (error) {
                deferred.reject(error);
            }          
        });

        var focusHandler = document.body.onfocus;
        document.body.onfocus = function () {
            if (focusHandler) {
                focusHandler.apply(this, arguments);
            }
            document.body.onfocus = focusHandler;
            setTimeout(function () {
                if (!$input.val()) {
                    deferred.reject('Cancelled');
                }
            }.bind(this), 500);
        };

        $input.click();

        deferred.always(function () {
            $input.remove();
        });

        return deferred;
    }
});