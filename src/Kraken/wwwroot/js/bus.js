define(function() {

    return {
        topics: {},

        subscribe: function (topic, listener) {
            if (!this.topics[topic]) this.topics[topic] = [];
            this.topics[topic].push(listener);
        },

        unsubscribe: function (topic, listener) {
            if (!this.topics[topic]) return;
            var index = this.topics[topic].indexOf(listener);
            if (index !== -1) {
                this.topics[topic].splice(index, 1);
            }
        },

        publish: function (topic, data) {
            if (!this.topics[topic] || this.topics[topic].length < 1) return;
            this.topics[topic].forEach(function (listener) {
                listener(data || {});
            });
        }
    };

});