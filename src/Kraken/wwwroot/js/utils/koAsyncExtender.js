define(['knockout', 'jquery'], function (ko, $) {

    ko.extenders.async = function (computedDeferred, initialValue) {
        var plainObservable = ko.observable(initialValue);
        var currentDeferred;

        plainObservable.working = ko.observable(false)

        ko.computed(function () {
            if (currentDeferred) {
                currentDeferred.reject();
                currentDeferred = null;
            }

            var newDeferred = computedDeferred();

            // chained promise, wait for fulfillment
            if (newDeferred && (typeof newDeferred.done === 'function')) {

                plainObservable.working(true);

                currentDeferred = $.Deferred().done(function (data) {
                    plainObservable.working(false);
                    plainObservable(data);
                })
                newDeferred.done(currentDeferred.resolve)
            } else {
                // real value, publish immediately
                plainObservable(newDeferred);
            }
        });

        return plainObservable;
    };

});